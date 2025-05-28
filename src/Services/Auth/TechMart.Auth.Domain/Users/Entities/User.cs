using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Constants;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.Enums;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.Events;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Domain.Users.Entities;

/// <summary>
/// User aggregate root representing a system user
/// </summary>
public sealed class User : AggregateRoot<UserId>
{
    private readonly List<UserRole> _userRoles = [];

    // EF Core constructor
    private User()
        : base() { }

    /// <summary>
    /// Creates a new user
    /// </summary>
    private User(
        UserId id,
        Email email,
        string firstName,
        string lastName,
        string passwordHash,
        Guid? createdBy = null
    )
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        Status = UserStatus.PendingEmailConfirmation;
        IsEmailConfirmed = false;

        if (createdBy.HasValue)
            SetCreatedBy(createdBy.Value);

        RaiseDomainEvent(new UserCreatedEvent(Id, Email.Value, FirstName, LastName));
    }

    #region Properties

    /// <summary>
    /// User's email address (unique)
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Current status of the user account
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Whether the user's email has been confirmed
    /// </summary>
    public bool IsEmailConfirmed { get; private set; }

    /// <summary>
    /// When the user last logged in
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// User's roles
    /// </summary>
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    #endregion


    #region Factory Methods

    /// <summary>
    /// Creates a new user with validation
    /// </summary>
    public static Result<User> Create(
        Email email,
        string firstName,
        string lastName,
        string passwordHash,
        Guid? createdBy = null
    )
    {
        // Validate first name
        if (string.IsNullOrWhiteSpace(firstName))
            return UserErrors.FirstNameRequired();

        firstName = firstName.Trim();
        if (firstName.Length > 100)
            return UserErrors.FirstNameTooLong(100);

        // Validate last name
        if (string.IsNullOrWhiteSpace(lastName))
            return UserErrors.LastNameRequired();

        lastName = lastName.Trim();
        if (lastName.Length > 100)
            return UserErrors.LastNameTooLong(100);

        // Validate password hash
        if (string.IsNullOrWhiteSpace(passwordHash))
            return UserErrors.PasswordHashRequired();

        if (passwordHash.Length > 500)
            return UserErrors.PasswordHashTooLong(500);

        var userId = UserId.New();
        return new User(userId, email, firstName, lastName, passwordHash, createdBy);
    }

    #endregion

    #region Role Management Methods

    /// <summary>
    /// Checks if user has a specific role
    /// </summary>
    public bool HasRole(RoleId roleId)
    {
        return _userRoles.Any(ur => ur.RoleId == roleId);
    }

    /// <summary>
    /// Checks if user has a specific role by name
    /// </summary>
    public bool HasRole(string roleName)
    {
        return _userRoles.Any(ur => ur.Role?.Name == roleName);
    }

    /// <summary>
    /// Checks if user is an administrator
    /// </summary>
    public bool IsAdministrator()
    {
        return HasRole(RoleConstants.Administrator);
    }

    /// <summary>
    /// Gets all role names for the user
    /// </summary>
    public IEnumerable<string> GetRoleNames()
    {
        return _userRoles
            .Select(ur =>
                ur.Role?.Name
                ?? RoleConstants.RoleIdToName.GetValueOrDefault(ur.RoleId.Value, "Unknown")
            )
            .Distinct();
    }

    #endregion

    #region Business Logic Methods

    /// <summary>
    /// Updates the user's profile information
    /// </summary>
    public Result UpdateProfile(string firstName, string lastName, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return UserErrors.FirstNameRequired();

        if (string.IsNullOrWhiteSpace(lastName))
            return UserErrors.LastNameRequired();

        firstName = firstName.Trim();
        lastName = lastName.Trim();

        if (firstName.Length > 100)
            return UserErrors.FirstNameTooLong(100);

        if (lastName.Length > 100)
            return UserErrors.LastNameTooLong(100);

        FirstName = firstName;
        LastName = lastName;
        UpdateAudit(updatedBy);

        return Result.Success();
    }

    /// <summary>
    /// Changes the user's password
    /// </summary>
    public Result ChangePassword(
        string newPasswordHash,
        bool wasResetRequest = false,
        Guid? updatedBy = null
    )
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            return UserErrors.PasswordHashRequired();

        if (newPasswordHash.Length > 500)
            return UserErrors.PasswordHashTooLong(500);

        PasswordHash = newPasswordHash;
        UpdateAudit(updatedBy);

        RaiseDomainEvent(new UserPasswordChangedEvent(Id, Email.Value, wasResetRequest));

        return Result.Success();
    }

    /// <summary>
    /// Confirms the user's email address
    /// </summary>
    public Result ConfirmEmail(Guid? updatedBy = null)
    {
        if (IsEmailConfirmed)
            return UserErrors.AlreadyConfirmed();

        IsEmailConfirmed = true;

        // Activate user if they were pending email confirmation
        if (Status == UserStatus.PendingEmailConfirmation)
        {
            var previousStatus = Status;
            Status = UserStatus.Active;
            RaiseDomainEvent(
                new UserStatusChangedEvent(Id, Email.Value, previousStatus, Status, updatedBy)
            );
        }

        UpdateAudit(updatedBy);
        RaiseDomainEvent(new UserEmailConfirmedEvent(Id, Email.Value));

        return Result.Success();
    }

    /// <summary>
    /// Records a successful login
    /// </summary>
    public Result RecordLogin(string? ipAddress = null)
    {
        if (!CanLogin())
            return UserErrors.CannotLogin(Status);

        LastLoginAt = DateTime.UtcNow;
        UpdateAudit();

        RaiseDomainEvent(new UserLoggedInEvent(Id, Email.Value, ipAddress));

        return Result.Success();
    }

    /// <summary>
    /// Changes user status
    /// </summary>
    public Result ChangeStatus(UserStatus newStatus, Guid? updatedBy = null)
    {
        if (Status == newStatus)
            return UserErrors.AlreadyInStatus(newStatus);

        // Business rule: Cannot activate deleted users
        if (Status == UserStatus.Deleted && newStatus == UserStatus.Active)
            return UserErrors.CannotActivateDeletedUser();

        var previousStatus = Status;
        Status = newStatus;
        UpdateAudit(updatedBy);

        RaiseDomainEvent(
            new UserStatusChangedEvent(Id, Email.Value, previousStatus, newStatus, updatedBy)
        );

        return Result.Success();
    }

    /// <summary>
    /// Activates the user account
    /// </summary>
    public Result Activate(Guid? updatedBy = null) => ChangeStatus(UserStatus.Active, updatedBy);

    /// <summary>
    /// Deactivates the user account
    /// </summary>
    public Result Deactivate(Guid? updatedBy = null) =>
        ChangeStatus(UserStatus.Inactive, updatedBy);

    /// <summary>
    /// Suspends the user account
    /// </summary>
    public Result Suspend(Guid? updatedBy = null) => ChangeStatus(UserStatus.Suspended, updatedBy);

    /// <summary>
    /// Soft deletes the user account
    /// </summary>
    public Result Delete(Guid? updatedBy = null) => ChangeStatus(UserStatus.Deleted, updatedBy);

    #endregion

    #region Business Rules

    /// <summary>
    /// Determines if the user can login
    /// </summary>
    public bool CanLogin()
    {
        return Status == UserStatus.Active && IsEmailConfirmed;
    }

    /// <summary>
    /// Gets the user's full name
    /// </summary>
    public string GetFullName()
    {
        return $"{FirstName} {LastName}".Trim();
    }

    /// <summary>
    /// Gets the user's display name (first name or email if no first name)
    /// </summary>
    public string GetDisplayName()
    {
        return string.IsNullOrWhiteSpace(FirstName) ? Email.Value : FirstName;
    }

    /// <summary>
    /// Checks if the user is in a specific status
    /// </summary>
    public bool IsInStatus(UserStatus status)
    {
        return Status == status;
    }

    /// <summary>
    /// Checks if the user account is active
    /// </summary>
    public bool IsActive()
    {
        return Status == UserStatus.Active;
    }

    /// <summary>
    /// Checks if the user account is deleted
    /// </summary>
    public bool IsDeleted()
    {
        return Status == UserStatus.Deleted;
    }

    /// <summary>
    /// Checks if the user needs email confirmation
    /// </summary>
    public bool NeedsEmailConfirmation()
    {
        return !IsEmailConfirmed || Status == UserStatus.PendingEmailConfirmation;
    }

    #endregion
}

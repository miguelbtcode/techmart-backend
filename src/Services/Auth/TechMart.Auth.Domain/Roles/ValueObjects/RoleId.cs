using TechMart.Auth.Domain.Roles.Constants;

namespace TechMart.Auth.Domain.Roles.ValueObjects;

public sealed class RoleId : IEquatable<RoleId>
{
    public Guid Value { get; }

    private RoleId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty.", nameof(value));

        Value = value;
    }

    public static RoleId From(Guid value) => new(value);

    public static RoleId New() => new(Guid.NewGuid());

    // Para los roles predefinidos del sistema
    public static RoleId Administrator => new(RoleConstants.AdministratorId);
    public static RoleId Customer => new(RoleConstants.CustomerId);
    public static RoleId Moderator => new(RoleConstants.ModeratorId);
    public static RoleId Support => new(RoleConstants.SupportId);

    #region Equality

    public bool Equals(RoleId? other)
    {
        if (other is null)
            return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as RoleId);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(RoleId? left, RoleId? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(RoleId? left, RoleId? right) => !(left == right);

    #endregion

    public override string ToString() => Value.ToString();

    // Conversiones implícitas pueden ser peligrosas, mejor explícitas
    public static implicit operator Guid(RoleId id) => id.Value;

    public static explicit operator RoleId(Guid id) => From(id);
}

namespace TechMart.Auth.Domain.Roles.ValueObjects;

public sealed class UserRoleId : IEquatable<UserRoleId>
{
    public Guid Value { get; }

    private UserRoleId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("UserRole ID cannot be empty.", nameof(value));

        Value = value;
    }

    public static UserRoleId From(Guid value) => new(value);

    public static UserRoleId New() => new(Guid.NewGuid());

    #region Equality
    public bool Equals(UserRoleId? other)
    {
        if (other is null)
            return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as UserRoleId);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(UserRoleId? left, UserRoleId? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(UserRoleId? left, UserRoleId? right) => !(left == right);
    #endregion

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(UserRoleId id) => id.Value;

    public static explicit operator UserRoleId(Guid id) => From(id);
}

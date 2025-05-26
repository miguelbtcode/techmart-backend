namespace TechMart.Auth.Domain.Users.ValueObjects;

public sealed class UserId : IEquatable<UserId>
{
    public Guid Value { get; }

    private UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(value));

        Value = value;
    }

    public static UserId From(Guid value) => new(value);

    public static UserId New() => new(Guid.NewGuid());

    #region Equality
    public bool Equals(UserId? other)
    {
        if (other is null)
            return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as UserId);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(UserId? left, UserId? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(UserId? left, UserId? right) => !(left == right);
    #endregion

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(UserId id) => id.Value;

    public static explicit operator UserId(Guid id) => From(id);
}

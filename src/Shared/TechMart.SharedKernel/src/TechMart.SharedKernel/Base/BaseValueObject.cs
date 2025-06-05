namespace TechMart.SharedKernel.Base;

/// <summary>
/// Base class for value objects in Domain-Driven Design.
/// Value objects are immutable and are defined by their attributes rather than their identity.
/// </summary>
public abstract class BaseValueObject : IEquatable<BaseValueObject>
{
    /// <summary>
    /// Gets the components that define the equality of this value object.
    /// </summary>
    /// <returns>An enumerable of objects that define equality.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (BaseValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public bool Equals(BaseValueObject? other)
    {
        return Equals((object?)other);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(BaseValueObject? left, BaseValueObject? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(BaseValueObject? left, BaseValueObject? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Creates a copy of the value object.
    /// Since value objects are immutable, this returns the same instance.
    /// </summary>
    /// <returns>The same instance of the value object.</returns>
    public virtual BaseValueObject Copy()
    {
        return this;
    }

    public override string ToString()
    {
        return $"{{{string.Join(", ", GetEqualityComponents())}}}";
    }
}
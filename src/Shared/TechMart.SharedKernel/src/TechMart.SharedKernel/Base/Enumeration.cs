using System.Reflection;

namespace TechMart.SharedKernel.Base;

/// <summary>
/// Base class for implementing type-safe enumerations.
/// Provides a way to create enumerations with more functionality than standard enums.
/// </summary>
public abstract class Enumeration : IComparable
{
    /// <summary>
    /// Gets the unique identifier of the enumeration.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the name of the enumeration.
    /// </summary>
    public string Name { get; }

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;

    /// <summary>
    /// Gets all enumeration values of the specified type.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <returns>All enumeration values of the specified type.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public |
                                        BindingFlags.Static |
                                        BindingFlags.DeclaredOnly);

        return fields
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    /// <summary>
    /// Gets an enumeration value by its identifier.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="id">The identifier of the enumeration value.</param>
    /// <returns>The enumeration value with the specified identifier.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no enumeration value with the specified identifier is found.</exception>
    public static T FromId<T>(int id) where T : Enumeration
    {
        var matchingItem = Parse<T, int>(id, "id", item => item.Id == id);
        return matchingItem;
    }

    /// <summary>
    /// Gets an enumeration value by its name.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="name">The name of the enumeration value.</param>
    /// <returns>The enumeration value with the specified name.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no enumeration value with the specified name is found.</exception>
    public static T FromName<T>(string name) where T : Enumeration
    {
        var matchingItem = Parse<T, string>(name, "name", item => item.Name == name);
        return matchingItem;
    }

    /// <summary>
    /// Tries to get an enumeration value by its identifier.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="id">The identifier of the enumeration value.</param>
    /// <param name="result">The enumeration value if found; otherwise, null.</param>
    /// <returns>True if the enumeration value was found; otherwise, false.</returns>
    public static bool TryFromId<T>(int id, out T? result) where T : Enumeration
    {
        result = GetAll<T>().FirstOrDefault(item => item.Id == id);
        return result != null;
    }

    /// <summary>
    /// Tries to get an enumeration value by its name.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="name">The name of the enumeration value.</param>
    /// <param name="result">The enumeration value if found; otherwise, null.</param>
    /// <returns>True if the enumeration value was found; otherwise, false.</returns>
    public static bool TryFromName<T>(string name, out T? result) where T : Enumeration
    {
        result = GetAll<T>().FirstOrDefault(item => item.Name == name);
        return result != null;
    }

    private static T Parse<T, TValue>(TValue value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public int CompareTo(object? other)
    {
        if (other is not Enumeration otherEnumeration)
            return 1;

        return Id.CompareTo(otherEnumeration.Id);
    }

    public static bool operator ==(Enumeration? left, Enumeration? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Enumeration? left, Enumeration? right)
    {
        return !(left == right);
    }

    public static bool operator <(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) >= 0;
    }
}
namespace TechMart.Auth.Application.Common.Models;

/// <summary>
/// Represents a void return value for commands that don't return data
/// Similar to System.Reactive.Unit but for application layer
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// Default instance of Unit
    /// </summary>
    public static readonly Unit Value = new();

    /// <summary>
    /// Task that returns Unit.Value
    /// </summary>
    public static Task<Unit> Task => System.Threading.Tasks.Task.FromResult(Value);

    public bool Equals(Unit other) => true;

    public override bool Equals(object? obj) => obj is Unit;

    public override int GetHashCode() => 0;

    public static bool operator ==(Unit left, Unit right) => true;

    public static bool operator !=(Unit left, Unit right) => false;

    public override string ToString() => "()";
}

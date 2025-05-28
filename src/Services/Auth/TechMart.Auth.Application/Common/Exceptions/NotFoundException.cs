namespace TechMart.Auth.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public sealed class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key)
        : base($"{name} with key '{key}' was not found")
    {
        Name = name;
        Key = key;
    }

    public NotFoundException(string message)
        : base(message) { }

    public string Name { get; } = string.Empty;
    public object Key { get; } = string.Empty;
}

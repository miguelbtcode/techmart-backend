using TechMart.SharedKernel.Exceptions;

namespace TechMart.Product.Domain.Exceptions;

/// <summary>
/// Base exception for product domain errors.
/// </summary>
public class ProductDomainException : DomainException
{
    public ProductDomainException(string message) : base("Product.Domain", message)
    {
    }

    public ProductDomainException(string message, Exception innerException) 
        : base("Product.Domain", message, innerException)
    {
    }

    public ProductDomainException(string errorCode, string message) 
        : base(errorCode, message)
    {
    }

    public ProductDomainException(string errorCode, string message, IDictionary<string, object> details) 
        : base(errorCode, message, details)
    {
    }
}
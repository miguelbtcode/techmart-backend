using MediatR;
using TechMart.Product.Application.Common.DTOs;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Queries.GetProduct;

public record GetProductQuery(Guid Id) : IRequest<Result<ProductDto>>;
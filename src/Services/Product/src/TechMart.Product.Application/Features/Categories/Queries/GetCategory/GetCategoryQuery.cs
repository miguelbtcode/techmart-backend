using MediatR;
using TechMart.Product.Application.Features.Categories.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Queries.GetCategory;

public record GetCategoryQuery(Guid Id) : IRequest<Result<CategoryVm>>;
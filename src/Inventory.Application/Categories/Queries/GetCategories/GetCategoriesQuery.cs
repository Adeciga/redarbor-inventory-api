using MediatR;
namespace Inventory.Application.Categories.Queries.GetCategories;
public sealed record GetCategoriesQuery() : IRequest<IReadOnlyList<CategoryDto>>;

using MediatR;

namespace Inventory.Application.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryReadRepository _readRepository;

    public GetCategoriesQueryHandler(ICategoryReadRepository readRepository)
        => _readRepository = readRepository;

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
        => await _readRepository.GetAllAsync(ct);
}

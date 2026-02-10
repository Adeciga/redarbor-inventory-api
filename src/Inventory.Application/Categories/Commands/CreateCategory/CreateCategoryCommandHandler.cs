using MediatR;
namespace Inventory.Application.Categories.Commands.CreateCategory;
public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly ICategoryWriteRepository _writeRepository;
    public CreateCategoryCommandHandler(ICategoryWriteRepository writeRepository)
        => _writeRepository = writeRepository;
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var createRequest = new CreateCategoryRequest(request.Name, request.IsActive);
        return await _writeRepository.CreateAsync(createRequest, ct);
    }
}

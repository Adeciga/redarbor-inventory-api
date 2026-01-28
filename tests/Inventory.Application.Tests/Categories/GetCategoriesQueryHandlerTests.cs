using Inventory.Application.Categories;
using Inventory.Application.Categories.Queries.GetCategories;
using Moq;
using Xunit;

namespace Inventory.Application.Tests.Categories;

public sealed class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_Returns_Categories_From_ReadRepository()
    {
        // Arrange
        var expected = new List<CategoryDto>
        {
            new CategoryDto(1, "Bebidas", true),
            new CategoryDto(2, "Snacks", true)
        };

        var repo = new Mock<ICategoryReadRepository>(MockBehavior.Strict);

        repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetCategoriesQueryHandler(repo.Object);

        // Act
        var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Bebidas", result[0].Name);
        repo.VerifyAll();
    }
}

using Inventory.Application.Categories;
using Inventory.Application.Categories.Commands.CreateCategory;
using Moq;
using Xunit;

namespace Inventory.Application.Tests.Categories;

public sealed class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_Calls_Repository_And_Returns_Id()
    {
        // Arrange
        var repo = new Mock<ICategoryWriteRepository>(MockBehavior.Strict);

        repo.Setup(x => x.CreateAsync(
                It.Is<CreateCategoryRequest>(r => r.Name == "Bebidas" && r.IsActive),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(123);

        var handler = new CreateCategoryCommandHandler(repo.Object);
        var command = new CreateCategoryCommand("Bebidas", true);

        // Act
        var id = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(123, id);
        repo.VerifyAll();
    }
}

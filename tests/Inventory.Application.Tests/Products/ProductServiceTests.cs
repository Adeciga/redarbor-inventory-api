using FluentAssertions;
using Inventory.Application.Products;
using Moq;
using Xunit;

namespace Inventory.Application.Tests.Products;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnRepositoryResult()
    {
        var expected = new ProductDto(1, "Laptop", 2, 10, true);

        var readRepo = new Mock<IProductReadRepository>();
        readRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var writeRepo = new Mock<IProductWriteRepository>();

        var service = new ProductService(readRepo.Object, writeRepo.Object);

        var result = await service.GetByIdAsync(1, CancellationToken.None);

        result.Should().Be(expected);
    }
}

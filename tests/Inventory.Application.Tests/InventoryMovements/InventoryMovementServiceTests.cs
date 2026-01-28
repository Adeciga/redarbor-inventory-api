using FluentAssertions;
using Inventory.Application.InventoryMovements;
using Inventory.Domain;
using Moq;
using Xunit;

namespace Inventory.Application.Tests.InventoryMovements;

public sealed class InventoryMovementServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnRepositoryResult()
    {
        var request = new CreateInventoryMovementRequest(1, 5, InventoryMovementType.In);
        var expected = new InventoryMovementResult(1, 15);

        var repo = new Mock<IInventoryMovementWriteRepository>();
        repo.Setup(x => x.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = new InventoryMovementService(repo.Object);

        var result = await service.CreateAsync(request, CancellationToken.None);

        result.Should().Be(expected);
    }
}

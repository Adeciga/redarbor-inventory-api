using Inventory.Application.InventoryMovements;
using Inventory.Application.InventoryMovements.Commands.CreateInventoryMovement;
using Inventory.Domain;
using Moq;
using Xunit;

namespace Inventory.Application.Tests.InventoryMovements;

public sealed class CreateInventoryMovementCommandHandlerTests
{
    [Fact]
    public async Task Handle_InMovement_IncreasesStock_And_ReturnsResult()
    {
        var writeRepo = new Mock<IInventoryMovementWriteRepository>(MockBehavior.Strict);
        var readRepo = new Mock<IInventoryMovementReadRepository>(MockBehavior.Strict);

        readRepo.Setup(x => x.GetCurrentStockAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        writeRepo.Setup(x => x.CreateAsync(
                It.Is<CreateInventoryMovementRequest>(r =>
                    r.ProductId == 1 && r.Quantity == 5 && r.Type == InventoryMovementType.In),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InventoryMovementResult(1, 15));

        var handler = new CreateInventoryMovementCommandHandler(writeRepo.Object, readRepo.Object);

        var result = await handler.Handle(
            new CreateInventoryMovementCommand(1, 5, InventoryMovementType.In),
            CancellationToken.None);

        Assert.Equal(1, result.ProductId);
        Assert.Equal(15, result.NewStock);

        readRepo.VerifyAll();
        writeRepo.VerifyAll();
    }

    [Fact]
    public async Task Handle_OutMovement_DecreasesStock()
    {
        var writeRepo = new Mock<IInventoryMovementWriteRepository>(MockBehavior.Strict);
        var readRepo = new Mock<IInventoryMovementReadRepository>(MockBehavior.Strict);

        readRepo.Setup(x => x.GetCurrentStockAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        writeRepo.Setup(x => x.CreateAsync(It.IsAny<CreateInventoryMovementRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InventoryMovementResult(1, 7));

        var handler = new CreateInventoryMovementCommandHandler(writeRepo.Object, readRepo.Object);

        var result = await handler.Handle(
            new CreateInventoryMovementCommand(1, 3, InventoryMovementType.Out),
            CancellationToken.None);

        Assert.Equal(7, result.NewStock);

        readRepo.VerifyAll();
        writeRepo.VerifyAll();
    }

    [Fact]
    public async Task Handle_OutMovement_WithInsufficientStock_ThrowsDomainException()
    {
        var writeRepo = new Mock<IInventoryMovementWriteRepository>(MockBehavior.Strict);
        var readRepo = new Mock<IInventoryMovementReadRepository>(MockBehavior.Strict);

        readRepo.Setup(x => x.GetCurrentStockAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var handler = new CreateInventoryMovementCommandHandler(writeRepo.Object, readRepo.Object);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(new CreateInventoryMovementCommand(1, 5, InventoryMovementType.Out), CancellationToken.None));

        // writeRepo no debe ser llamado si falla la regla de negocio
        writeRepo.Verify(x => x.CreateAsync(It.IsAny<CreateInventoryMovementRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        readRepo.VerifyAll();
    }
}

using MediatR;
using Inventory.Domain;
namespace Inventory.Application.InventoryMovements.Commands.CreateInventoryMovement;
public sealed record CreateInventoryMovementCommand(
    int ProductId,
    int Quantity,
    InventoryMovementType Type
) : IRequest<InventoryMovementResult>;


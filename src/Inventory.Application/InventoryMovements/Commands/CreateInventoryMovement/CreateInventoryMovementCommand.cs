using MediatR;
using Inventory.Domain;
using Inventory.Application.InventoryMovements;

namespace Inventory.Application.InventoryMovements.Commands.CreateInventoryMovement;

public sealed record CreateInventoryMovementCommand(
    int ProductId,
    int Quantity,
    InventoryMovementType Type
) : IRequest<InventoryMovementResult>;


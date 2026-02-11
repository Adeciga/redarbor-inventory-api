using FluentValidation;
using Inventory.Domain;
namespace Inventory.Application.InventoryMovements.Commands.CreateInventoryMovement;
public sealed class CreateInventoryMovementCommandValidator : AbstractValidator<CreateInventoryMovementCommand>
{
    public CreateInventoryMovementCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
        RuleFor(x => x.Type)
            .IsInEnum()
            .Must(t => t == InventoryMovementType.In || t == InventoryMovementType.Out);
    }
}

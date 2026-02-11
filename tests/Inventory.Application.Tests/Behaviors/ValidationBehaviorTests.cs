using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Inventory.Application.Behaviors;
using Inventory.Application.Tests.TestDoubles;
using MediatR;
using Moq;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Should_throw_when_any_validator_fails()
    {
        var validator = new Mock<IValidator<TestRequest>>();

        validator.As<IValidator>()
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Name", "Required") }));

        var behavior = new ValidationBehavior<TestRequest, Unit>(new[] { validator.Object });

        Func<Task> act = () => behavior.Handle(
            new TestRequest(),
            () => Task.FromResult(Unit.Value),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Should_call_next_when_all_validators_pass()
    {
        var validator = new Mock<IValidator<TestRequest>>();

        validator.As<IValidator>()
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, Unit>(new[] { validator.Object });

        var result = await behavior.Handle(
            new TestRequest(),
            () => Task.FromResult(Unit.Value),
            CancellationToken.None);

        result.Should().Be(Unit.Value);
    }
}

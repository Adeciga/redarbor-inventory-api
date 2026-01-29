using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Inventory.Application.Behaviors;
using MediatR;
using Moq;

namespace Inventory.Application.Tests.Behaviors {
    public class ValidationBehaviorTests
    {
        [Fact]
        public async Task Should_throw_validation_exception_when_validation_fails()
        {
            var validator = new Mock<IValidator<TestRequest>>();
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[]
                {
                new ValidationFailure("Field", "Error")
                }));

            var behavior = new ValidationBehavior<TestRequest, Unit>(
                new[] { validator.Object }
            );

            await FluentActions
                .Invoking(() => behavior.Handle(
                    new TestRequest(),
                    () => Task.FromResult(Unit.Value),
                    CancellationToken.None))
                .Should()
                .ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Should_call_next_when_validation_succeeds()
        {
            var validator = new Mock<IValidator<TestRequest>>();
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var behavior = new ValidationBehavior<TestRequest, Unit>(
                new[] { validator.Object }
            );

            var result = await behavior.Handle(
                new TestRequest(),
                () => Task.FromResult(Unit.Value),
                CancellationToken.None);

            result.Should().Be(Unit.Value);
        }

        private class TestRequest : IRequest<Unit> { }
    }
}



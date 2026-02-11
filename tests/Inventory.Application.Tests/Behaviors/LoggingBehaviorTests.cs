using FluentAssertions;
using Inventory.Application.Behaviors;
using Inventory.Application.Tests.TestDoubles;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task Should_call_next_and_return_response_when_success()
    {
        // Arrange
        var logger = new Mock<ILogger<LoggingBehavior<TestRequest, Unit>>>();
        var behavior = new LoggingBehavior<TestRequest, Unit>(logger.Object);

        var nextCalled = false;

        RequestHandlerDelegate<Unit> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(Unit.Value);
        };

        // Act
        var result = await behavior.Handle(new TestRequest(), next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Should_rethrow_exception_when_next_throws()
    {
        // Arrange
        var logger = new Mock<ILogger<LoggingBehavior<TestRequest, Unit>>>();
        var behavior = new LoggingBehavior<TestRequest, Unit>(logger.Object);

        RequestHandlerDelegate<Unit> next = () => throw new InvalidOperationException("boom");

        // Act
        Func<Task> act = () => behavior.Handle(new TestRequest(), next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("boom");
    }
}

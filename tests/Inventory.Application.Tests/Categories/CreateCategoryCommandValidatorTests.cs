using FluentAssertions;
using Inventory.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var result = _validator.Validate(new CreateCategoryCommand("", true));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_pass_when_name_is_valid()
    {
        var result = _validator.Validate(new CreateCategoryCommand("Electronics", true));

        result.IsValid.Should().BeTrue();
    }
}

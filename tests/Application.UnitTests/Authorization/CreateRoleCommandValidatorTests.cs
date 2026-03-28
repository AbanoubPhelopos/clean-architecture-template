using Application.Authorization.Roles.Create;
using FluentAssertions;
using FluentValidation;

namespace Application.UnitTests.Authorization;

public class CreateRoleCommandValidatorTests : BaseTest
{
    private readonly CreateRoleCommandValidator _validator;

    public CreateRoleCommandValidatorTests()
    {
        _validator = new CreateRoleCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        var command = new CreateRoleCommand(
            "Admin",
            "Administrator role",
            new[] { "users:read", "users:write" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        var command = new CreateRoleCommand(
            "",
            "Administrator role",
            new[] { "users:read" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_ShouldHaveError()
    {
        var longName = new string('A', 101);
        var command = new CreateRoleCommand(
            longName,
            "Description",
            new[] { "users:read" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Name" && 
            e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_WithDescriptionExceeding500Characters_ShouldHaveError()
    {
        var longDescription = new string('D', 501);
        var command = new CreateRoleCommand(
            "ValidName",
            longDescription,
            new[] { "users:read" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Description" && 
            e.ErrorMessage.Contains("500"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithInvalidNames_ShouldHaveError(string? name)
    {
        var command = new CreateRoleCommand(
            name!,
            "Description",
            new[] { "users:read" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
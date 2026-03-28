using Application.Users.Register;
using FluentAssertions;
using FluentValidation;

namespace Application.UnitTests.Users;

public class RegisterUserCommandValidatorTests : BaseTest
{
    private readonly RegisterUserCommandValidator _validator;

    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "John",
            "Doe",
            "Password123!");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyFirstName_ShouldHaveError()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "",
            "Doe",
            "Password123!");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validate_WithEmptyLastName_ShouldHaveError()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "John",
            "",
            "Password123!");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("")]
    [InlineData("missing@")]
    public void Validate_WithInvalidEmail_ShouldHaveError(string email)
    {
        var command = new RegisterUserCommand(
            email,
            "John",
            "Doe",
            "Password123!");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    public void Validate_WithPasswordTooShort_ShouldHaveError(string password)
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "John",
            "Doe",
            password);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            e.ErrorMessage.Contains("8"));
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldHaveError()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "John",
            "Doe",
            "");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
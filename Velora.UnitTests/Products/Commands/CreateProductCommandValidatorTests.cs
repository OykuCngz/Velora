using FluentValidation.TestHelper;
using Velora.Application.Products.Commands;
using Xunit;

namespace Velora.UnitTests.Products.Commands;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateProductCommand("", "Desc", 100, 10, 1, "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(v => v.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Zero_Or_Negative()
    {
        var command = new CreateProductCommand("Product", "Desc", 0, 10, 1, "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(v => v.Price);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new CreateProductCommand("Product", "Desc", 100, 10, 1, "img.jpg");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

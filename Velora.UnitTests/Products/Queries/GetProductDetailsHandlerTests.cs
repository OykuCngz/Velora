using Moq;
using Velora.Application.Common.Interfaces;
using Velora.Application.Products.Queries;
using Velora.Core.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;

namespace Velora.UnitTests.Products.Queries;

public class GetProductDetailsHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetProductDetailsHandler _handler;

    public GetProductDetailsHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetProductDetailsHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = 1;
        var products = new List<Product>
        {
            new Product { Id = productId, Name = "Test Product", Category = new Category { Name = "Test" } }
        }.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Products).Returns(products.Object);

        // Act
        var result = await _handler.Handle(new GetProductDetailsQuery(productId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var products = new List<Product>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Products).Returns(products.Object);

        // Act
        var result = await _handler.Handle(new GetProductDetailsQuery(99), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

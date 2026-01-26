using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Bogus;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Application.Sales;

public sealed class CancelSaleHandlerTests
{
    private readonly Faker _faker = new();

    [Fact(DisplayName = "CancelSaleHandler should cancel sale and publish message")]
    public async Task Given_ValidCommand_When_Handled_Then_ShouldCancelSale()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleCancelledPublisher>();

        var handler = new CancelSaleHandler(saleRepository, cache, publisher);

        var sale = new Sale(
            _faker.Random.AlphaNumeric(10),
            DateTime.UtcNow,
            Guid.NewGuid(),
            _faker.Person.FullName,
            Guid.NewGuid(),
            _faker.Company.CompanyName());
        sale.Id = Guid.NewGuid();

        var command = new CancelSaleCommand(sale.Id);

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await saleRepository.Received(1)
            .UpdateAsync(sale, Arg.Any<CancellationToken>());

        await cache.Received(1)
            .RemoveAsync(SalesCacheKeys.All, Arg.Any<CancellationToken>());

        await cache.Received(1)
            .RemoveAsync(SalesCacheKeys.GetById(sale.Id), Arg.Any<CancellationToken>());

        await publisher.Received(1)
            .PublishAsync(sale.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "CancelSaleHandler should throw when sale is already canceled")]
    public async Task Given_CancelledSale_When_Handled_Then_ShouldThrow()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleCancelledPublisher>();

        var handler = new CancelSaleHandler(saleRepository, cache, publisher);

        var sale = new Sale(
            _faker.Random.AlphaNumeric(10),
            DateTime.UtcNow,
            Guid.NewGuid(),
            _faker.Person.FullName,
            Guid.NewGuid(),
            _faker.Company.CompanyName());
        sale.Id = Guid.NewGuid();
        sale.CancelStatus();

        var command = new CancelSaleCommand(sale.Id);

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact(DisplayName = "CancelSaleHandler should throw on invalid command")]
    public async Task Given_InvalidCommand_When_Handled_Then_ShouldThrow()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleCancelledPublisher>();

        var handler = new CancelSaleHandler(saleRepository, cache, publisher);

        var command = new CancelSaleCommand(Guid.Empty);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.IsType<FluentValidation.ValidationException>(exception);
    }
}
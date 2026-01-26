using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Application.Sales;

public sealed class UpdateSaleHandlerTests
{
    private readonly Faker _faker = new();

    [Fact(DisplayName = "UpdateSaleHandler should replace items and publish message")]
    public async Task Given_ValidCommand_When_Handled_Then_ShouldUpdateSale()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleUpdatedPublisher>();
        var mapper = BuildMapper();

        var handler = new UpdateSaleHandler
        (
            saleRepository,
            productRepository,
            mapper,
            cache,
            publisher
        );

        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var sale = new Sale
        (
            _faker.Random.AlphaNumeric(10),
            DateTime.UtcNow,
            Guid.NewGuid(),
            _faker.Person.FullName,
            Guid.NewGuid(),
            _faker.Company.CompanyName()
        );
        sale.Id = saleId;
        sale.AddItem(Guid.NewGuid(), "Old item", 1, 10m);

        var command = new UpdateSaleCommand
        {
            Id = saleId,
            Items = new[]
            {
                new UpdateSaleItemCommand(productId, 4, 10m)
            }
        };

        saleRepository.GetByIdWithItemsAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(new Product("New item", 10m));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(saleId, result.Id);
        Assert.Equal("NotCanceled", result.Status);
        Assert.Single(sale.Items);
        Assert.Equal(4, sale.Items.First().Quantity);

        await saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await cache.Received(1).RemoveAsync(SalesCacheKeys.All, Arg.Any<CancellationToken>());
        await cache.Received(1).RemoveAsync(SalesCacheKeys.GetById(sale.Id), Arg.Any<CancellationToken>());
        await publisher.Received(1).PublishAsync(sale.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateSaleHandler should throw when sale is missing")]
    public async Task Given_MissingSale_When_Handled_Then_ShouldThrow()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleUpdatedPublisher>();
        var mapper = BuildMapper();

        var handler = new UpdateSaleHandler
        (
            saleRepository,
            productRepository,
            mapper,
            cache,
            publisher
        );

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            Items = new[]
            {
                new UpdateSaleItemCommand(Guid.NewGuid(), 1, 10m)
            }
        };

        saleRepository.GetByIdWithItemsAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.IsType<FluentValidation.ValidationException>(exception);
    }

    [Fact(DisplayName = "UpdateSaleHandler should throw when sale is canceled")]
    public async Task Given_CanceledSale_When_Handled_Then_ShouldThrow()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleUpdatedPublisher>();
        var mapper = BuildMapper();

        var handler = new UpdateSaleHandler
        (
            saleRepository,
            productRepository,
            mapper,
            cache,
            publisher
        );

        var saleId = Guid.NewGuid();
        var sale = new Sale
        (
            _faker.Random.AlphaNumeric(10),
            DateTime.UtcNow,
            Guid.NewGuid(),
            _faker.Person.FullName,
            Guid.NewGuid(),
            _faker.Company.CompanyName()
        );
        sale.Id = saleId;
        sale.CancelStatus();

        var command = new UpdateSaleCommand
        {
            Id = saleId,
            Items = new[]
            {
                new UpdateSaleItemCommand(Guid.NewGuid(), 1, 10m)
            }
        };

        saleRepository.GetByIdWithItemsAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.IsType<FluentValidation.ValidationException>(exception);
    }

    [Fact(DisplayName = "UpdateSaleHandler should throw on invalid command")]
    public async Task Given_InvalidCommand_When_Handled_Then_ShouldThrow()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleUpdatedPublisher>();
        var mapper = BuildMapper();

        var handler = new UpdateSaleHandler
        (
            saleRepository,
            productRepository,
            mapper,
            cache,
            publisher
        );

        var command = new UpdateSaleCommand
        {
            Id = Guid.Empty,
            Items = Array.Empty<UpdateSaleItemCommand>()
        };

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.IsType<FluentValidation.ValidationException>(exception);
    }

    private static IMapper BuildMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UpdateSaleProfile>();
        });

        return config.CreateMapper();
    }
}
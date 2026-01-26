using System.Text;
using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.EnqueueSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Application.Sales;

public sealed class CreateSaleHandlerTests
{
    private readonly Faker _faker = new();

    [Fact(DisplayName = "CreateSaleHandler should create sale and publish message")]
    public async Task Given_ValidCommand_When_Handled_Then_ShouldCreateSale()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var branchRepository = Substitute.For<IBranchRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleCreatedPublisher>();

        var mapper = BuildMapper();
        var handler = new CreateSaleHandler(
            saleRepository,
            userRepository,
            branchRepository,
            productRepository,
            mapper,
            cache,
            publisher);

        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var saleNumber = _faker.Random.AlphaNumeric(10);
        var expectedSaleId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            SaleNumber = saleNumber,
            CustomerId = customerId,
            BranchId = branchId,
            Items = new[]
            {
                new CreateSaleItemCommand(productId, 4, 10m)
            }
        };

        userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(new User { Username = _faker.Name.FirstName() });

        branchRepository.GetByIdAsync(branchId, Arg.Any<CancellationToken>())
            .Returns(new Branch(_faker.Company.CompanyName()));

        productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(new Product("Guitar", 10m));

        saleRepository.CreateAsync(
                Arg.Any<Sale>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var sale = callInfo.Arg<Sale>();
                sale.Id = expectedSaleId;
                return Task.FromResult(sale);
            });

        cache.GetAsync(SalesCacheKeys.Idempotency(saleNumber), Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedSaleId, result.Id);
        await saleRepository.Received(1)
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await cache.Received(1)
            .SetAsync(
                SalesCacheKeys.Idempotency(saleNumber),
                Arg.Is<byte[]>(value => Encoding.UTF8.GetString(value) == "1"),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        await publisher.Received(1)
            .PublishAsync(expectedSaleId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "CreateSaleHandler should reject duplicate request")]
    public async Task Given_DuplicateRequest_When_Handled_Then_ShouldThrowValidationException()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var branchRepository = Substitute.For<IBranchRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var cache = Substitute.For<IDistributedCache>();
        var publisher = Substitute.For<ISaleCreatedPublisher>();

        var mapper = BuildMapper();
        var handler = new CreateSaleHandler(
            saleRepository,
            userRepository,
            branchRepository,
            productRepository,
            mapper,
            cache,
            publisher);

        var saleNumber = _faker.Random.AlphaNumeric(10);
        var command = new CreateSaleCommand
        {
            SaleNumber = saleNumber,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new[]
            {
                new CreateSaleItemCommand(Guid.NewGuid(), 1, 10m)
            }
        };

        cache.GetAsync(SalesCacheKeys.Idempotency(saleNumber), Arg.Any<CancellationToken>())
            .Returns(Encoding.UTF8.GetBytes("1"));

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.IsType<FluentValidation.ValidationException>(exception);
    }

    private static IMapper BuildMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateSaleProfile>();
        });

        return config.CreateMapper();
    }
}
using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public sealed class CreateSaleCommand : IRequest<CreateSaleResult>
{
    public string SaleNumber { get; init; }
    public Guid CustomerId { get; init; }
    public Guid BranchId { get; init; }

    public IReadOnlyCollection<CreateSaleItemCommand> Items { get; init; }
        = Array.Empty<CreateSaleItemCommand>();

    public ValidationResultDetail Validate()
    {
        var validator = new CreateSaleCommandValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e)
        };
    }
}

public sealed record CreateSaleItemCommand
(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);
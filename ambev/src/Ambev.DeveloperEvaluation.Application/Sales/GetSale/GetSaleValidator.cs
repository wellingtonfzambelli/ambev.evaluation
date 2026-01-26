using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public sealed class GetSaleValidator : AbstractValidator<GetSaleQuery>
{
    public GetSaleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");
    }
}
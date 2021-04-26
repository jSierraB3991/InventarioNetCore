using FluentValidation;
using WebApi.Infrastructure.Dto.Request;

namespace WebApi.Infrastructure.Validation
{
    public class SaleBuyValidation: AbstractValidator<SaleBuy>
    {
        public SaleBuyValidation()
        {
            RuleFor(saleBuy => saleBuy.Product)
                .NotNull()
                .NotEmpty();

            RuleFor(saleBuy => saleBuy.Store)
                .NotNull()
                .NotEmpty();

            RuleFor(saleBuy => saleBuy.Stock)
                .GreaterThan(1)
                .LessThan(1000);
        }
    }
}

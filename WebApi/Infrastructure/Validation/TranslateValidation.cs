using FluentValidation;
using WebApi.Infrastructure.Dto.Request;

namespace WebApi.Infrastructure.Validation
{
    public class TranslateValidation : AbstractValidator<TranslateRequest>
    {
        public TranslateValidation()
        {
            RuleFor(translate => translate.Destino)
                .NotNull();

            RuleFor(translate => translate.Origin)
                .NotNull();

            RuleFor(translate => translate.Product)
                .NotNull();

            RuleFor(translate => translate.Stock)
                .GreaterThan(1)
                .LessThan(10000)
                .NotNull();
        }
    }
}

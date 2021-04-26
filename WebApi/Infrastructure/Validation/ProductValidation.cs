using FluentValidation;
using WebApi.Infrastructure.Dto;

namespace WebApi.Infrastructure.Validation
{
    public class ProductValidation : AbstractValidator<ProductDto>
    {
        public ProductValidation()
        {
            RuleFor(product => product.Description)
                .NotNull();

            RuleFor(product => product.Name)
                .NotEmpty()
                .NotNull()
                .Length(3, 50);

            RuleFor(product => product.Sku)
                .NotNull()
                .NotEmpty();
        }
    }
}

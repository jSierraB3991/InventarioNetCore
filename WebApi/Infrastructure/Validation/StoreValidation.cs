using FluentValidation;
using WebApi.Infrastructure.Dto;

namespace WebApi.Infrastructure.Validation
{
    public class StoreValidation: AbstractValidator<StoreDto>
    {
        public StoreValidation()
        {
            RuleFor(store => store.Direction)
                .NotNull();

            RuleFor(store => store.Description)
                .NotNull();

            RuleFor(store => store.Name)
                .NotNull()
                .NotEmpty()
                .Length(6, 30).WithMessage("The Name of Store, minimun length is 6 and maximun length is 30");

            RuleFor(store => store.MaxCapacity)
                .NotNull()
                .GreaterThan(1)
                .LessThanOrEqualTo(10000);
        }
    }
}

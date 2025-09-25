using FluentValidation;
using RealEstate.Application.DTOs;

namespace RealEstate.Application.Validators
{
    public class PropertyCreateDtoValidator : AbstractValidator<PropertyCreateDto>
    {
        public PropertyCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.IdOwner).NotEmpty();
            RuleFor(x => x.Bedrooms).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Bathrooms).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SquareMeters).GreaterThanOrEqualTo(0);
        }
    }
}

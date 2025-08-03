using FluentValidation;

namespace Application.Features.Houses.Commands.AddHouseMember
{
    public class AddHouseMemberValidator : AbstractValidator<AddHouseMemberCommand>
    {
        public AddHouseMemberValidator()
        {
            RuleFor(x => x.HouseId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}

using FluentValidation;

namespace SlotMachineAPI.Application.Players.Commands.UpdatePlayerCommand
{
    public class UpdateBalanceCommandValidator : AbstractValidator<UpdateBalanceCommand>
    {
        public UpdateBalanceCommandValidator()
        {
            RuleFor(x => x.PlayerId)
                .NotEmpty().WithMessage("The player ID cannot be empty!");

            RuleFor(x => x.Amount)
                .NotEqual(0).WithMessage("The balance to be updated cannot be 0!");
        }
    }
}

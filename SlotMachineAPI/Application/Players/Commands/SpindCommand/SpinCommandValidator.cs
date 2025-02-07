namespace SlotMachineAPI.Application.Players.Commands.SpindCommand
{
    using FluentValidation;

    public class SpinCommandValidator : AbstractValidator<SpinCommand>
    {
        public SpinCommandValidator()
        {
            RuleFor(x => x.PlayerId)
                .NotEmpty().WithMessage("The player ID cannot be empty!");

            RuleFor(x => x.BetAmount)
                .GreaterThan(0).WithMessage("The bet amount must be greater than 0!")
                .LessThanOrEqualTo(100000).WithMessage("The bet amount cannot be greater than 100000!");
        }
    }

}

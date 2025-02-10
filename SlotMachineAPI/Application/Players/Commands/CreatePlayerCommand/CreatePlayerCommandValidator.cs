using FluentValidation;

namespace SlotMachineAPI.Application.Players.Commands.CreatePlayerCommand
{
    public class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
    {
        public CreatePlayerCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The player name cannot be empty!")
                .Length(0, 15).WithMessage("Name length should be between 0 and 15 characters");
        }
    }
}

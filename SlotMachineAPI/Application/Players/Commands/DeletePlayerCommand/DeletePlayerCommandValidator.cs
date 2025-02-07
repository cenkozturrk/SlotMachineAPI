using FluentValidation;

namespace SlotMachineAPI.Application.Players.Commands.DeletePlayerCommand
{
    public class DeletePlayerCommandValidator : AbstractValidator<DeletePlayerCommand>
    {
        public DeletePlayerCommandValidator()
        {
            RuleFor(x => x.PlayerId)
                .NotEmpty().WithMessage("The player ID cannot be empty!")
                .Matches("^[0-9a-fA-F]{24}$").WithMessage("Invalid Player ID format! (MongoDB must be ObjectId)");
        }
    }
}

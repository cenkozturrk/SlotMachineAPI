using FluentValidation;
using SlotMachineAPI.Application.Players.Queries.GetPlayerQuery;

public class GetPlayerQueryValidator : AbstractValidator<GetPlayerQuery>
{
    public GetPlayerQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Player ID cannot be empty!")
            .Matches("^[0-9a-fA-F]{24}$").WithMessage("Invalid Player ID format! (MongoDB must be ObjectId)");
    }
}

using FluentValidation;
using Mirai.Application.Tags.Commands.UpdateTag;

namespace Mirai.Application.Tags.Commands.CreateTag;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.TagId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(50);
    }
}
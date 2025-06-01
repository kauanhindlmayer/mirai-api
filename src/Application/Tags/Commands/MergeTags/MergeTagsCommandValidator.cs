using FluentValidation;

namespace Application.Tags.Commands.MergeTags;

public sealed class MergeTagsCommandValidator : AbstractValidator<MergeTagsCommand>
{
    public MergeTagsCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.TargetTagId)
            .NotEmpty();

        RuleFor(x => x.SourceTagIds)
            .NotEmpty()
            .Must(sourceTagIds => sourceTagIds.Distinct().Count() == sourceTagIds.Count)
            .WithMessage("Source tag IDs must be unique.");
    }
}
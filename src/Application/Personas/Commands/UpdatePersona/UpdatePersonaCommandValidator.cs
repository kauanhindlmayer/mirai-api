using FluentValidation;

namespace Application.Personas.Commands.UpdatePersona;

internal sealed class UpdatePersonaCommandValidator : AbstractValidator<UpdatePersonaCommand>
{
    public UpdatePersonaCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.File)
            .Must(file => file is null || file.Length <= 5 * 1024 * 1024)
            .WithMessage("File size must not exceed 5 MB.")
            .Must(file => file is null || file.ContentType.StartsWith("image/"))
            .WithMessage("File must be an image.");
    }
}
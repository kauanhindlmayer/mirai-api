using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands.UpdateUserProfilePicture;

public sealed class UpdateUserProfilePictureCommandValidator : AbstractValidator<UpdateUserProfilePictureCommand>
{
    public UpdateUserProfilePictureCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("Profile picture is required")
            .Must(BeAValidImage).WithMessage("Invalid file type. Only JPEG and PNG are allowed")
            .Must(HaveValidSize).WithMessage("File size cannot exceed 2MB");
    }

    private bool BeAValidImage(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        return allowedExtensions.Contains(fileExtension);
    }

    private bool HaveValidSize(IFormFile file)
    {
        const long maxSizeInBytes = 2 * 1024 * 1024; // 2MB
        return file.Length <= maxSizeInBytes;
    }
}
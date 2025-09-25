using FluentValidation;
using Microsoft.AspNetCore.Http;
namespace RealEstate.Application.Validators
{
    public class FileUploadValidator : AbstractValidator<IFormFile>
    {
        public FileUploadValidator()
        {
            RuleFor(f => f).NotNull().WithMessage("File is required");
            RuleFor(f => f.Length).LessThanOrEqualTo(5 * 1024 * 1024).WithMessage("Max file size is 5MB");
            RuleFor(f => f.ContentType).Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp").WithMessage("Only jpeg/png/webp allowed");
        }
    }
}

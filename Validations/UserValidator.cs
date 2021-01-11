using System.Linq;
using dotnet.boilerplate.Dto;
using dotnet.boilerplate.Persistance;
using FluentValidation;

namespace dotnet.boilerplate.Validations
{
    public class UserValidator : AbstractValidator<AddUserDto>
    {
        private readonly AppDbContext context;

        public UserValidator(AppDbContext context)
        {
            this.context = context;

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required");

            RuleFor(u => u)
                .Must(u => !IsEmailExist(u)).WithName("Email").WithMessage("Email already exist");
        }

        private bool IsEmailExist(AddUserDto resource)
        {
            if (!string.IsNullOrEmpty(resource.Email))
            {
                return context.User.Any(u => u.Email == resource.Email);
            }
            return false;
        }

    }
}
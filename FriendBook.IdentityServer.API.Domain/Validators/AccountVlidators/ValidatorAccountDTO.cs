using FluentValidation;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;

namespace FriendBook.IdentityServer.API.Domain.Validators.AccountVlidators
{
    public class ValidatorAccountDTO : AbstractValidator<AccountDTO>
    {
        public ValidatorAccountDTO()
        {
            RuleFor(dto => dto.Login)
                .NotEmpty().WithMessage("Login is required")
                .Matches(@"^[a-zA-ZА-Яа-я0-9]+$").WithMessage("Login must contain only letters and numbers")
                .Length(2, 50).WithMessage("Login must not exceed 50 characters and least 2 characters.");

            RuleFor(dto => dto.Password)
                .NotEmpty().WithMessage("Password is required")
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{10,}$").WithMessage("Password must be at least 10 characters long and contain at least one uppercase letter, one lowercase letter, and one digit")
                .Length(10, 50).WithMessage("Login must not exceed 50 characters and least 10 characters.");
        }
    }
}

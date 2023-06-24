using FluentValidation;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using PhoneNumbers;

namespace FriendBook.IdentityServer.API.Domain.Validators.AccountVlidators
{
    public class ValidatorUserContactDTO : AbstractValidator<UserContactDTO>
    {
        private static readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
        private bool IsValidPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return true;

            var phoneNumberObject = phoneNumberUtil.Parse(phoneNumber, null);

            return phoneNumberUtil.IsValidNumber(phoneNumberObject);
        }
        public ValidatorUserContactDTO()
        {
            RuleFor(dto => dto.FullName)
                .Length(2, 50).WithMessage("Full name must not exceed 50 characters and at least 2 characters.")
                .Matches(@"^(?:[a-zA-ZА-Яа-я]+ ?)+$").WithMessage("Full name must contain only letters and numbers")
                .When(dto => !string.IsNullOrEmpty(dto.FullName));

            RuleFor(dto => dto.Email)
                .Length(50).WithMessage("Email must not exceed 100 characters.")
                .EmailAddress().WithMessage("Email is not in a valid format.")
                .When(dto => !string.IsNullOrEmpty(dto.Email));

            RuleFor(dto => dto.Login)
                .NotEmpty().WithMessage("Login is required")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Login must contain only letters and numbers")
                .Length(2, 50).WithMessage("Login must not exceed 50 characters and at least 2 characters.");

            RuleFor(dto => dto.Info)
                .Length(0,100).WithMessage("Info must not exceed 500 characters and at least 2 characters.");

            RuleFor(dto => dto.Profession)
                .Length(0,50).WithMessage("Profession must not exceed 50 characters and at least 2 characters.");

            RuleFor(dto => dto.Telephone)
                .Length(10, 25).WithMessage("Telephone must not exceed 25 characters and at least 10 characters.")
                .Must(IsValidPhoneNumber).WithMessage("Telephone is not in a valid format.")
                .When(dto => !string.IsNullOrEmpty(dto.Telephone));
        }
    }
}

using FluentValidation;
using FluentValidation.Results;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.InnerResponse;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class ValidationService<T> : IValidationService<T>
    {
        private readonly IValidator<T> Validator;

        public ValidationService(IValidator<T> validator)
        {
            Validator = validator;
        }

        public async Task<BaseResponse<List<Tuple<string, string>>?>> ValidateAsync(T dto)
        {
            var validationResult = await Validator.ValidateAsync(dto);
            return GetErrors(validationResult);
        }
        public BaseResponse<List<Tuple<string, string>>?> Validate(T dto)
        {
            var validationResult = Validator.Validate(dto);
            return GetErrors(validationResult);
        }

        private static BaseResponse<List<Tuple<string, string>>?> GetErrors(ValidationResult validationResult)
        {
            var isValid = validationResult.IsValid;

            var reponse = new StandartResponse<List<Tuple<string, string>>?>();
            if (!isValid)
            {
                reponse.StatusCode = StatusCode.ErrorValidation;
                reponse.Message = $"Error validation: {validationResult.Errors.First().ErrorMessage}";
                reponse.Data = validationResult.Errors.Select(x => new Tuple<string, string>(x.PropertyName, x.ErrorMessage)).ToList();
                return reponse;
            }
            return reponse;
        }
    }
}

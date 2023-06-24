using FriendBook.IdentityServer.API.Domain.InnerResponse;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IValidationService<T>
    {
        public Task<BaseResponse<List<Tuple<string, string>>>?> ValidateAsync(T dto);
        public BaseResponse<List<Tuple<string, string>>>? Validate(T dto);
    }
}

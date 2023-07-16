using FriendBook.IdentityServer.API.Domain.InnerResponse;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IRedisLockService
    {
        public Task SetSecretNumber(string secretNumber, string key);
        public Task<BaseResponse<string?>> GetSecretNumber(string key);
    }
}

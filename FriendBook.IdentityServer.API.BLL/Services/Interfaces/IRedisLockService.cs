using FriendBook.IdentityServer.API.Domain.Response;

namespace FriendBook.IdentityServer.API.BLL.Services.Interfaces
{
    public interface IRedisLockService
    {
        public Task SetSecretNumber(string secretNumber, string key);
        public Task<BaseResponse<string?>> GetSecretNumber(string key);
    }
}

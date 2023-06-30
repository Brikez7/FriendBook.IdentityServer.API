using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class RedisLockService : IRedisLockService
    {
        private readonly IDistributedCache _dispributedCache;
        private readonly RedisSettings _redisSetting;

        public RedisLockService(IDistributedCache dispributedCache, IOptions<RedisSettings> redisSetting)
        {
            _dispributedCache = dispributedCache;
            _redisSetting = redisSetting.Value;
        }

        public async Task<BaseResponse<string?>> GetSecretNumber(string key)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(_redisSetting.Host);
            List<RedLockMultiplexer> multiplexers = new List<RedLockMultiplexer> { connection };

            RedLockFactory redLockFactory = RedLockFactory.Create(multiplexers);

            await using var redLock = await redLockFactory.CreateLockAsync(_redisSetting.Resource, _redisSetting.Expiry, _redisSetting.Wait, _redisSetting.Retry);
            if (redLock.IsAcquired)
            {
                var secretNumber = await _dispributedCache.GetStringAsync(key);

                if (secretNumber != null)
                {
                    return new StandartResponse<string?> { Data = secretNumber, StatusCode = Domain.StatusCode.RedisReceive };
                }
                return new StandartResponse<string?> { Data = null, StatusCode = Domain.StatusCode.RedisEmpty, Message = "Object not found" };
            }
            return new StandartResponse<string?> { Data = null, StatusCode = Domain.StatusCode.RedisLock, Message = "Redis was locked" };
        }

        public async Task SetSecretNumber(string value, string key)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(_redisSetting.Host);
            List<RedLockMultiplexer> multiplexers = new List<RedLockMultiplexer> { connection };

            RedLockFactory redLockFactory = RedLockFactory.Create(multiplexers);

            await using var redLock = await redLockFactory.CreateLockAsync(_redisSetting.Resource, _redisSetting.Expiry, _redisSetting.Wait, _redisSetting.Retry);
            if (redLock.IsAcquired)
            {
                await _dispributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _redisSetting.StoreDuration
                });
            }
        }
    }
}

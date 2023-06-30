using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.UserToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.IdentityServer.API.Controllers
{
    [Route("Home/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private Lazy<TokenAuth> UserToken { get; set; } 
        IRedisLockService RedisLockService { get; set; }
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, IRedisLockService redisLockService, IAccessTokenService accessTokenService)
        {
            _logger = logger;
            UserToken =  accessTokenService.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
            RedisLockService = redisLockService;
        }
        [HttpGet("GetClaims")]
        [Authorize]
        public IActionResult GetClaims()
        {
            var claims = UserToken.Value;
            return Ok(claims);
        }

        [HttpPost("SetRedis")]
        public async Task<IActionResult> SetRedis([FromBody] string Param, [FromQuery] string key)
        {
            await RedisLockService.SetSecretNumber(Param, key);
            return Ok("Set");
        }
        [HttpGet("GetRedis")]
        public async Task<IActionResult> GetRedis([FromQuery] string key) 
        {
            var value = await RedisLockService.GetSecretNumber(key);
            return Ok(value);
        }
    }
}

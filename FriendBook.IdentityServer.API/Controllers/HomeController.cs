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
        public Lazy<UserAccsessToken> UserToken { get; set; } 
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            UserToken = new Lazy<UserAccsessToken>(() => UserAccsessToken.CreateUserToken(User.Claims));
        }
        [HttpGet("GetClaims")]
        [Authorize]
        public IActionResult GetClaims()
        {
            var claims = UserToken.Value;
            return Ok(claims);
        }
    }
}

using FriendBook.IdentityServer.API.BLL.Helpers;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.UserToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.IdentityServer.API.Controllers
{
    [Route("DeveloperHome/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private Lazy<TokenAuth> UserToken { get; set; } 
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            UserToken =  AccessTokenHelper.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
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

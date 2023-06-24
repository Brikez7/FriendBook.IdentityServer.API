using FriendBook.IdentityServer.API.Domain.UserToken;
using FriendBook.IdentityServer.API.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.IdentityServer.API.Controllers
{
    [Route("Home/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        public Lazy<UserTokenAuth> UserToken { get; set; } 
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            UserToken = new Lazy<UserTokenAuth>(() => UserTokenAuth.CreateUserToken(User.Claims));
        }
        [HttpGet("GetClaims")]
        [Authorize]
        public IActionResult GetClaims()
        {
            var claims = UserToken.Value;
            return Ok(claims);
        }
        [HttpGet("GetJSONValidatorsRule")]
        public IActionResult GetJSONValidatorsRule()
        {
            FluentValidationRulesProvider fluentValidationRulesProvider = new FluentValidationRulesProvider();
            var JSONRules = fluentValidationRulesProvider.GetAllValidationRules();
            return Ok(JSONRules);
        }
    }
}

using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.Controllers
{
    [Route("Home/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IdentityServerContext _db;

        public HomeController(ILogger<HomeController> logger, IRegistrationService registrationService, IdentityServerContext appDB)
        {
            _logger = logger;
            _registrationService = registrationService;
            _db = appDB;
        }
        [HttpGet("TestClaims")]
        public IResult GetClaims()
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;
            return Results.Json(claims);
        }
    }
}

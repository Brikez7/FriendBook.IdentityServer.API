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
        private readonly AppDBContext _db;

        public HomeController(ILogger<HomeController> logger, IRegistrationService registrationService, AppDBContext appDB)
        {
            _logger = logger;
            _registrationService = registrationService;
            _db = appDB;
        }

        [HttpGet("Test/CreateDatabase")]
        public async Task<IResult> CreateDatabase()
        {
            using (var db = _db)
            {
                db.Database.EnsureCreated();
            }
            return Results.Json(new { unswer = "ok" });
        }
        [HttpGet("Test/UpdateDatabase")]
        public async Task<IResult> UpdateDatabase()
        {
            using (var db = _db)
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            return Results.Json(new { unswer = "ok" });
        }
        [HttpGet("TestClaims")]
        public IResult GetClaims()
        {
            var identity = (HttpContext.User.Identity as ClaimsIdentity).Claims;
            var claims = _registrationService.ReadToken(Request.Cookies[CookieNames.JWTToken]);
            return Results.Json(claims);
        }
    }
}

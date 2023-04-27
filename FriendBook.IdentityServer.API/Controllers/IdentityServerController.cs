using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace FriendBook.IdentityServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityServerController : ControllerBase
    {
        private readonly ILogger<IdentityServerController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IAccountService _accountService;
        private readonly AppDBContext _db;

        public IdentityServerController(ILogger<IdentityServerController> logger, IRegistrationService registrationService, IAccountService accountService, AppDBContext appDB)
        {
            _logger = logger;
            _registrationService = registrationService;
            _accountService = accountService;
            _db = appDB;
        }

        [HttpPost("Authenticate/")]
        public async Task<IResult> Authenticate(AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return Results.NotFound();
            }
            var resourse = await _registrationService.Authenticate(accountDTO);
            Response.Cookies.SetJwtCookie((resourse.Data.Item1, "1", resourse.Data.Item2));
            if (resourse.Data.Item1 == null)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.Json(new { token = resourse.Data.Item1, id = resourse.Data.Item2 });
            }
        }

        [HttpPost("Registration/")]
        public async Task<IResult> Registration(AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return Results.NotFound();
            }
            var resourse = await _registrationService.Registration(accountDTO);
            Response.Cookies.SetJwtCookie((resourse.Data.Item1,"1", resourse.Data.Item2));
            if (resourse.Data.Item1 == null)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.Json(new { token = resourse.Data.Item1, id = resourse.Data.Item2 });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IResult> Delete(Guid id)
        {
            var resourse = await _accountService.DeleteAccount(x => x.Id == id);
            if (resourse.Data)
            {
                return Results.Ok(resourse.Data);
            }
            else
            {
                return Results.StatusCode(500);
            }
        }
    }
}
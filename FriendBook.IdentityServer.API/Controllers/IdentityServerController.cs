using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.IdentityServer.API.Controllers
{
    [ApiController]
    [Route("api/Identity[controller]")]
    public class IdentityServerController : ControllerBase
    {
        private readonly ILogger<IdentityServerController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IAccountService _accountService;

        public IdentityServerController(ILogger<IdentityServerController> logger, IRegistrationService registrationService, IAccountService accountService)
        {
            _logger = logger;
            _registrationService = registrationService;
            _accountService = accountService;
        }

        [HttpPost("Authenticate/")]
        public async Task<IResult> Authenticate(AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return Results.NotFound();
            }
            var response = await _registrationService.Authenticate(accountDTO);
            Response.Cookies.SetJwtCookie((response.Data.Item1, "1", response.Data.Item2));
            if (response.Data.Item1 == null)
            {
                response.Message = "Error Authenticate";
                response.StatusCode = Domain.InnerResponse.StatusCode.ErrorAuthenticate;
                return Results.Json(response);
            }
            else
            {
                return Results.Json(new { token = response.Data.Item1, id = response.Data.Item2 });
            }
        }

        [HttpPost("Registration/")]
        public async Task<IResult> Registration(AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return Results.NoContent();
            }
            var response = await _registrationService.Registration(accountDTO);
            Response.Cookies.SetJwtCookie((response.Data.Item1,"1", response.Data.Item2));
            if (response.Data.Item1 == null)
            {
                response.Message = "Error registration";
                response.StatusCode = Domain.InnerResponse.StatusCode.ErrorRegistration;
                return Results.Json(response);
            }
            else
            {
                return Results.Json(new { token = response.Data.Item1, id = response.Data.Item2 });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IResult> Delete(Guid id)
        {
            var response = await _accountService.DeleteAccount(x => x.Id == id);
            if (response.Data)
            {
                return Results.Json(response);
            }
            else
            {
                response.Message = "Error. Entity not delete.";
                response.StatusCode = Domain.InnerResponse.StatusCode.EntityNotFound;
                return Results.Json(response);
            }
        }
    }
}
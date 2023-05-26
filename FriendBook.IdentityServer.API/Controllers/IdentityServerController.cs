using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("authenticate/")]
        public async Task<IResult> Authenticate(AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return Results.Json(new StandartResponse<string>()
                {
                    Message = "Data null",
                    StatusCode = Domain.InnerResponse.StatusCode.OKNoContent
                });
            }
            var response = await _registrationService.Authenticate(accountDTO);

            return Results.Json(response);
        }

        [HttpPost("registration/")]
        public async Task<IActionResult> Registration([FromQuery] AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return Ok(new StandartResponse<string>()
                {
                    Message = "Data null",
                    StatusCode = Domain.InnerResponse.StatusCode.OKNoContent
                });
            }

            var response = await _registrationService.Registration(accountDTO);

            return Ok(response);
        }

        [HttpGet("checkToken/")]
        [Authorize]
        public async Task<IActionResult> CheckToken()
        {
            string? login = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            string? password = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var AccountDTO = new AccountDTO{ Login = login, Password = password };

            BaseResponse<string> response = await _registrationService.Authenticate(AccountDTO);
            
            return Ok(response);
        }

        [HttpGet("checkToken2/")]
        public async Task<IActionResult> CheckToken2()
        {
            
            return Ok("ok");
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
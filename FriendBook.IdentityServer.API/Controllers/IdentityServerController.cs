using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using HCL.IdentityServer.API.Domain.Enums;
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

        public IdentityServerController(ILogger<IdentityServerController> logger, IRegistrationService registrationService, IAccountService accountService)
        {
            _logger = logger;
            _registrationService = registrationService;
            _accountService = accountService;
        }

        [HttpPost("authenticate/")]
        public async Task<IResult> Authenticate( AccountDTO accountDTO)
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
        public async Task<IActionResult> Registration( AccountDTO accountDTO)
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

        [HttpGet("checkUserExists")]
        public async Task<IActionResult> CheckUserExists(string Id)
        {
            var userId = Guid.Parse(Id);

            var response = await _accountService.GetAccount(x => x.Id == userId);

            return Ok(new StandartResponse<bool>()
            {
                Data = response.Data is not null
            });
        }

        [HttpGet("checkToken/")]
        [Authorize]
        public async Task<IActionResult> CheckToken()
        {
            StandartResponse<string> response = new StandartResponse<string>()
            {
                StatusCode = Domain.InnerResponse.StatusCode.OK,
            };
            return Ok(response);
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
using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO.AcouuntsDTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using HCL.IdentityServer.API.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Authenticate([FromBody] AccountDTO accountDTO)
        {
            var response = await _registrationService.Authenticate(accountDTO);

            return Ok(response);
        }

        [HttpPost("registration/")]
        public async Task<IActionResult> Registration([FromBody] AccountDTO accountDTO)
        {
            var response = await _registrationService.Registration(accountDTO);

            return Ok(response);
        }

        [HttpGet("checkUserExists")]
        public async Task<IActionResult> CheckUserExists([FromQuery] Guid userId)
        {
            var response = await _accountService.GetAccount(x => x.Id == userId);

            return Ok(new StandartResponse<bool>()
            {
                Data = response.Data is not null
            });
        }

        [HttpPost("getLoginsUsers")]
        public async Task<IActionResult> GetLoginsUsers([FromBody] Guid[] usersIds)//Возможности для оптимизации
        {
            var response = await _accountService.GenLogins(usersIds);

            return Ok(response);
        }


        [HttpGet("checkToken/")]
        [Authorize]
        public IActionResult CheckToken()
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid idUser))
            {
                var response = _
                StandartResponse<bool> response = new StandartResponse<bool>()
                {
                    Data = true,
                    StatusCode = Domain.InnerResponse.StatusCode.OK
                };
                return Ok(response);
            }
            else 
            {
                return Ok(new StandartResponse<UserContactDTO>
                {
                    Message = "Not valid token",
                    StatusCode = Domain.InnerResponse.StatusCode.InternalServerError
                });
            }
        }
    }
}
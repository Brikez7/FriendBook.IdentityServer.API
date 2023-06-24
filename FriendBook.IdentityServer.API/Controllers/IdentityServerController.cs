using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.UserToken;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
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
        private readonly IValidationService<AccountDTO> _accountValidationService;
        public Lazy<UserTokenAuth> UserToken { get; set; }

        public IdentityServerController(ILogger<IdentityServerController> logger, IRegistrationService registrationService, IAccountService accountService, 
            IValidationService<AccountDTO> accountValidationService)
        {
            _logger = logger;
            _registrationService = registrationService;
            _accountService = accountService;
            _accountValidationService = accountValidationService;
            UserToken = new Lazy<UserTokenAuth>(() => UserTokenAuth.CreateUserToken(User.Claims));
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AccountDTO accountDTO)
        {
            var responseValidation = await _accountValidationService.ValidateAsync(accountDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _registrationService.Authenticate(accountDTO);
            return Ok(response);
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] AccountDTO accountDTO)
        {
            var responseValidation = await _accountValidationService.ValidateAsync(accountDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _registrationService.Registration(accountDTO);
            return Ok(response);
        }

        [HttpGet("CheckUserExists")]
        public async Task<IActionResult> CheckUserExists([FromQuery] Guid userId)
        {
            var response = await _accountService.GetAccount(x => x.Id == userId);

            return Ok(new StandartResponse<bool>()
            {
                Data = response.Data is not null
            });
        }

        [HttpPost("GetLoginsUsers")]
        public async Task<IActionResult> GetLoginsUsers([FromBody] Guid[] usersIds)//����������� ��� �����������
        {
            var response = await _accountService.GetLogins(usersIds);
            return Ok(response);
        }


        [HttpGet("CheckToken")]
        [Authorize]
        public IActionResult CheckToken()
        {
            return Ok(new StandartResponse<bool>
            {
                Data = true,
                StatusCode = Domain.InnerResponse.StatusCode.OK
            });
        }
    }
}
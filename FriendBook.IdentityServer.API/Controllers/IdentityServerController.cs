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
        private readonly IValidationService<AccountDTO> _accountValidationService;
        private readonly IUserAccessTokenService _userAccessTokenService;
        public IdentityServerController(ILogger<IdentityServerController> logger, IRegistrationService registrationService,
            IValidationService<AccountDTO> accountValidationService, IUserAccessTokenService userAccessTokenService)
        {
            _logger = logger;
            _registrationService = registrationService;
            _accountValidationService = accountValidationService;
            _userAccessTokenService = userAccessTokenService;
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

        [HttpPost("AuthenticateByRefreshToken")]
        public async Task<IActionResult> AuthenticateByRefreshToken([FromQuery] string refreshToken = null!)
        {
            var responseUserToken = _userAccessTokenService.CreateUserTokenTryEmpty(User.Claims);
            if (responseUserToken.Data is null)
                return Ok(responseUserToken);

            var responseAuthenicated = await _registrationService.AuthenticateByRefreshToken(responseUserToken.Data,refreshToken);
            return Ok(responseAuthenicated);
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


        [HttpGet("CheckToken")]
        [Authorize]
        public IActionResult CheckToken()
        {
            return Ok(new StandartResponse<bool>
            {
                Data = true,
                StatusCode = Domain.StatusCode.OK
            });
        }
    }
}
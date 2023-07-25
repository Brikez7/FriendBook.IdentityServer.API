using FriendBook.IdentityServer.API.BLL.Services.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.IdentityServer.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IdentityServerController : ControllerBase
    {
        private readonly ILogger<IdentityServerController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IValidationService<RequestAccount> _accountValidationService;
        public IdentityServerController(ILogger<IdentityServerController> logger, IRegistrationService registrationService,
            IValidationService<RequestAccount> accountValidationService)
        {
            _logger = logger;
            _registrationService = registrationService;
            _accountValidationService = accountValidationService;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] RequestAccount accountDTO)
        {
            var responseValidation = await _accountValidationService.ValidateAsync(accountDTO);
            if (responseValidation.StatusCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _registrationService.Authenticate(accountDTO);
            return Ok(response);
        }

        [HttpPost("AuthenticateByRefreshToken")]
        public async Task<IActionResult> AuthenticateByRefreshToken([FromBody] DataAccessToken accessTokenData,[FromQuery] string refreshToken)
        {
            var responseAuthenticated = await _registrationService.AuthenticateByRefreshToken(accessTokenData, refreshToken);
            return Ok(responseAuthenticated);
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] RequestAccount accountDTO)
        {
            var responseValidation = await _accountValidationService.ValidateAsync(accountDTO);
            if (responseValidation.StatusCode != ServiceCode.EntityIsValidated)
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
                StatusCode = ServiceCode.TokenValidated
            });
        }
    }
}
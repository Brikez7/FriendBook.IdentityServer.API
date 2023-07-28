using FriendBook.IdentityServer.API.BLL.Helpers;
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
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IContactService _contactService;
        private readonly IValidationService<UserContactDTO> _contactValidationService;
        public Lazy<DataAccessToken> UserToken { get; set; }
        public ContactController(ILogger<ContactController> logger, IContactService contactService, IValidationService<UserContactDTO> validationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _contactService = contactService;
            _contactValidationService = validationService;
            UserToken = AccessTokenHelper.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetContact([FromRoute] Guid id)
        {
            var response = await _contactService.GetContact(x => x.Id == id);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateContact([FromBody] UserContactDTO userContactDTO)
        {
            var responseValidation = await _contactValidationService.ValidateAsync(userContactDTO);
            if (responseValidation.StatusCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _contactService.UpdateContact(userContactDTO, UserToken.Value.Login,UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("GetProfiles/{login?}")]
        public async Task<IActionResult> GetProfiles([FromRoute] string login = "")
        {
            var response = await _contactService.GetProfiles(login, UserToken.Value.Id);
            return Ok(response);
        }
    }
}

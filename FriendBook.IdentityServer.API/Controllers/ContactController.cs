using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.UserToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.IdentityServer.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IContactService _contactService;
        private readonly IValidationService<UserContactDTO> _contactValidationService;
        public Lazy<TokenAuth> UserToken { get; set; }
        public ContactController(ILogger<ContactController> logger, IContactService contactService, IValidationService<UserContactDTO> validationService,
            IAccessTokenService userAccessTokenService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _contactService = contactService;
            _contactValidationService = validationService;
            UserToken = userAccessTokenService.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetContactInformation([FromRoute] Guid id)
        {
            var response = await _contactService.GetContact(x => x.Id == id);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateMyContactInformation([FromBody] UserContactDTO userContactDTO)
        {
            var responseValidation = await _contactValidationService.ValidateAsync(userContactDTO);
            if (responseValidation.StatusCode != Domain.StatusCode.EntityIsValid)
                return Ok(responseValidation);

            var response = await _contactService.UpdateContact(userContactDTO, UserToken.Value.Login,UserToken.Value.Id);
            return Ok(response);
        }
        // Add in AppUI after this method need remove
        [HttpGet("GetProfiles/{login?}")]
        public async Task<IActionResult> GetProfiles([FromRoute] string login = "")
        {
            var response = await _contactService.GetAllProphile(login, UserToken.Value.Id);
            return Ok(response);
        }
    }
}

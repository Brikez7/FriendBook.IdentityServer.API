using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.CustomClaims;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
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

        public ContactController(ILogger<ContactController> logger, IContactService contactService)
        {
            _logger = logger;
            _contactService = contactService;
        }

        [HttpGet("getMyContact")]
        public async Task<IActionResult> GetContactInformation()
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid idUser))
            {
                var response = await _contactService.GetContact(x => x.Id == idUser);

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

        [HttpGet("getContact/{id}")]
        public async Task<IActionResult> GetContactInformation([FromRoute] Guid id)
        {
            var response = await _contactService.GetContact(x => x.Id == id);

            return Ok(response);
        }

        [HttpPut("updateMyContactInformation")]
        public async Task<IActionResult> UpdateMyContactInformation([FromBody] UserContactDTO userContactDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid idUser))
            {
                string login = User.Claims.First(x => x.Type == CustomClaimType.Login).Value;

                var response = await _contactService.UpdateContact(userContactDTO, login);

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

        [HttpGet("GetProfiles/{login?}")]
        public async Task<IActionResult> GetProfiles([FromRoute] string login = "")
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var response = await _contactService.GetAllProphile(login, userId);

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

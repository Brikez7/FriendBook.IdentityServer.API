using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
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
            string? id = User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value;
            var idGuid = Guid.Parse(id);
            var response = await _contactService.GetContact(x => x.Id == idGuid);

            return Ok(response);
        }

        [HttpGet("getContact{Id}")]
        public async Task<IActionResult> GetContactInformation(string Id)
        {
            Guid idGuid = Guid.Parse(Id);
            var response = await _contactService.GetContact(x => x.Id == idGuid);

            return Ok(response);
        }

        [HttpPut("updateMyContactInformation")]
        public async Task<IActionResult> UpdateMyContactInformation([FromBody] UserContactDTO userContactDTO)
        {
            string? id = User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value;
            var idGuid = Guid.Parse(id);

            Account account = new Account(userContactDTO, idGuid);
            var response = await _contactService.UpdateContact(account);

            return Ok(response);
        }
        [HttpDelete("Clear")]
        public async Task<IResult> ClearContactInformation(Guid Id)
        {
            var response = await _contactService.ClearContact(x => x.Id == Id);
            if (!response.Data)
            {
                response.Message = "entity not faund";
                response.StatusCode = Domain.InnerResponse.StatusCode.EntityNotFound;
                return Results.Json(response);
            }
            return Results.Json(response);
        }

        [HttpGet("GetProfiles/{login?}")]
        public async Task<IActionResult> GetProfiles(string login = "")
        {
            string? id = User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value;
            var userId = Guid.Parse(id);

            var response = await _contactService.GetAllProphile(login, userId);
            if (response.Data == null)
            {
                response.Message = "entity not faund";
                response.StatusCode = Domain.InnerResponse.StatusCode.EntityNotFound;
                return Ok(response);
            }
            return Ok(response);
        }
    }
}

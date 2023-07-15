using FriendBook.IdentityServer.API.BLL.gRPCClients.ContactService;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class GrpcPublicContactService : PublicContact.PublicContactBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger<GrpcPublicContactService> _logger;
        private readonly IAccessTokenService _accessTokenService;
        public GrpcPublicContactService(IContactService contactService, ILogger<GrpcPublicContactService> logger, IAccessTokenService accessTokenService)
        {
            _contactService = contactService;
            _logger = logger;
            _accessTokenService = accessTokenService;
        }
        [Authorize]
        public override async Task<ResponseProfiles> GetProfiles(RequestUserLogin request, ServerCallContext context)
        {
            var User = _accessTokenService.CreateUser((context.GetHttpContext().User.Identity as ClaimsIdentity).Claims).Value;

            var responseLocalPropfile = await _contactService.GetProfiles(request.Login, User.Id);

            if (responseLocalPropfile.Message is not null) 
            {
                return null;
            }
            var r = responseLocalPropfile.Data.ToArray();
            var profiles = r.Select(x => new Profile() { FullName = x.FullName ?? "", Login = x.Login.ToString(), Id = x.Id.ToString() });
            
            var responseProfiles = new ResponseProfiles() {};
            responseProfiles.Profiles.AddRange(profiles);
            return responseProfiles;
        }
    }
}

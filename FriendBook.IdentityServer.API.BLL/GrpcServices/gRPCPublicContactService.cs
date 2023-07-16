using FriendBook.IdentityServer.API.BLL.gRPCClients.ContactService;
using FriendBook.IdentityServer.API.BLL.Helpers;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.GrpcServices
{
    public class GrpcPublicContactService : PublicContact.PublicContactBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger<GrpcPublicContactService> _logger;
        public GrpcPublicContactService(IContactService contactService, ILogger<GrpcPublicContactService> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }
        [Authorize]
        public override async Task<ResponseProfiles> GetProfiles(RequestUserLogin request, ServerCallContext context)
        {
            var User = AccessTokenHelper.CreateUserToken((context.GetHttpContext().User.Identity as ClaimsIdentity).Claims);

            var responseLocalPropfile = await _contactService.GetProfiles(request.Login, User.Id);

            ResponseProfiles responseProfiles = new ResponseProfiles();
            if (responseLocalPropfile.Message is not null)
            {
                return responseProfiles;
            }
            var r = responseLocalPropfile.Data.ToArray();
            var profiles = r.Select(x => new Profile() { FullName = x.FullName ?? "", Login = x.Login.ToString(), Id = x.Id.ToString() });

            responseProfiles.Profiles.AddRange(profiles);
            return responseProfiles;
        }
    }
}

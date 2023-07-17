using FriendBook.IdentityServer.API.BLL.gRPCServices.AccountService;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class GrpcPublicAccountService : PublicAccount.PublicAccountBase
    {
        private readonly IUserAccountService _userAccountService;
        private readonly ILogger<GrpcPublicAccountService> _logger;

        public GrpcPublicAccountService(IUserAccountService accountService, ILogger<GrpcPublicAccountService> logger)
        {
            _userAccountService = accountService;
            _logger = logger;
        }

        public override async Task<ResponseUserExists> CheckUserExists(RequestUserId request, ServerCallContext context)
        {
            string accountIdString = request.AccountId;
            ResponseUserExists userExists = new ResponseUserExists();
            if (Guid.TryParse(accountIdString, out Guid accountId))
            {
                var response = await _userAccountService.AccountExists(x => x.Id == accountId);
                userExists.Exists = response.Data;
            }
            return userExists;
        }
        public override async Task<ResponseUsers> GetUsersLoginById(RequestUsersId request, ServerCallContext context)
        {
            var responseLoginsWithId = await _userAccountService.GetLogins(request.UserId.Select(Guid.Parse).ToArray());
            ResponseUsers responseUsers = new ResponseUsers();

            responseUsers.Users.AddRange(responseLoginsWithId!.Data!.Select(x => new User() { Id = x.Item1.ToString(), Login = x.Item2 }));
            return responseUsers;
        }
    }
}

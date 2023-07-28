using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources;
using System.Net;
using FriendBook.IdentityServer.API.BLL.gRPCServices.AccountService;
using FriendBook.IdentityServer.Tests.TestHelpers;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    public class IntegrationTestsGrpcPublicAccountService : BaseIntegrationTests
    {
        internal const string UrlController = $"{UrlAPI}/GrpcAccountService";

        public IntegrationTestsGrpcPublicAccountService(RequestNewAccount requestAccount) : base(requestAccount){}

        [Test]
        public async Task CheckUserExists() 
        {
            HttpResponseMessage httpResponseUserExists = await _httpClient.GetAsync($"{UrlController}/CheckUserExists?AccountId={_userData.Id}");
            var responseUserExists = await DeserializeHelper.TryDeserialize<ResponseUserExists>(httpResponseUserExists);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUserExists.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUserExists.Exists, Is.EqualTo(true));
            });
        }

        [Test]
        public async Task GetUsersLoginById()
        {
            HttpResponseMessage httpResponseUsersLogin = await _httpClient.GetAsync($"{UrlController}/GetUsersLoginById?UserId={_userData.Id}&UserId={_userData.Id}");
            var responseUsersLogin = await DeserializeHelper.TryDeserialize<ResponseUsers>(httpResponseUsersLogin);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUsersLogin.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUsersLogin.Users.Count, Is.EqualTo(1));
                Assert.That(responseUsersLogin.Users[0].Login, Is.EqualTo(_userData.Login));
                Assert.That(responseUsersLogin.Users[0].Id, Is.EqualTo(_userData.Id.ToString()));
            });
        }
    }
}

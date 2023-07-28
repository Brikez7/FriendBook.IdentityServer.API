using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources;
using System.Net.Http.Json;
using System.Net;
using FriendBook.IdentityServer.API.BLL.gRPCServices.ContactService;
using FriendBook.IdentityServer.Tests.TestHelpers;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsGrpcPublicContactService : BaseIntegrationTests
    {
        internal const string UrlController = $"{UrlAPI}/GrpcContactService";

        public IntegrationTestsGrpcPublicContactService(RequestNewAccount requestAccount) : base(requestAccount){}

        [Test]
        public async Task GetProfiles()
        {
            RequestNewAccount requestNewAccount = new() { Login = "NewTestUser", Password = "TestPassword12345!" };
            HttpContent requestNewAccountContent = JsonContent.Create(requestNewAccount);

            await _httpClient.PostAsync($"{IntegrationTestsIdentityServerController.UrlController}/Registration", requestNewAccountContent);

            HttpResponseMessage httpResponseProfiles = await _httpClient.GetAsync($"{UrlController}/GetProfiles?Login={""}");
            ResponseProfiles responseProfiles = await DeserializeHelper.TryDeserialize<ResponseProfiles>(httpResponseProfiles);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseProfiles.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseProfiles.Profiles, Has.Count.EqualTo(1));
                Assert.That(responseProfiles.Profiles[0].Login, Is.EqualTo(requestNewAccount.Login));
                Assert.That(responseProfiles.Profiles[0].FullName, Is.EqualTo(""));
            });
        }
    }
}

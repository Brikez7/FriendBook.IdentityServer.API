using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using System.Net.Http.Json;
using FriendBook.IdentityServer.API.Domain.DTO;
using System.Net;
using FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.IdentityServer.API.Domain.Response;
using FriendBook.IdentityServer.Tests.TestHelpers;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    public class IntegrationTestsIdentityServerController : BaseIntegrationTests
    {
        internal const string UrlController = $"{UrlAPI}/IdentityServer";

        public IntegrationTestsIdentityServerController(RequestNewAccount requestAccount) : base(requestAccount){}

        [Test]
        public async Task Registration()
        {
            var requestNewAccount =  new RequestNewAccount() { Login = $"{_requestAccount.Login}Test", Password = _requestAccount.Password};
            HttpContent requestNewAccountContent = JsonContent.Create(requestNewAccount);

            HttpResponseMessage httpResponseAuth = await _httpClient.PostAsync($"{UrlController}/Registration", requestNewAccountContent);
            var responseAuth = await DeserializeHelper.TryDeserializeStandartResponse<ResponseAuthenticate>(httpResponseAuth);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseAuth.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(responseAuth.StatusCode, Is.EqualTo(ServiceCode.UserRegistered));
                Assert.That(responseAuth?.Data, Is.Not.Null);
            });
        }

        [Test]
        public async Task Authenticate()
        {
            HttpContent requestAccountContent = JsonContent.Create(_requestAccount);

            HttpResponseMessage httpResponseAuth = await _httpClient.PostAsync($"{UrlController}/Authenticate", requestAccountContent);
            var responseAuth = await DeserializeHelper.TryDeserializeStandartResponse<ResponseAuthenticate>(httpResponseAuth);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseAuth.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(responseAuth.StatusCode, Is.EqualTo(ServiceCode.UserAuthenticated));
                Assert.That(responseAuth?.Data?.RefreshToken, Is.Not.Null);
                Assert.That(responseAuth?.Data?.AccessToken, Is.Not.Null);
            });
        }

        [Test]
        public async Task AuthenticateByRefreshToken()
        {
            HttpContent dataAccessTokenContent = JsonContent.Create(_userData);

            HttpResponseMessage httpResponseAccessToken = await _httpClient.PostAsync($"{UrlController}/AuthenticateByRefreshToken?refreshToken={_responseRegistries.RefreshToken}", dataAccessTokenContent);

            var responseAccessToken = await DeserializeHelper.TryDeserializeStandartResponse<string>(httpResponseAccessToken);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseAccessToken.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseAccessToken.StatusCode, Is.EqualTo(ServiceCode.UserAuthenticatedByRT));
                Assert.That(responseAccessToken?.Data, Is.Not.Null);
            });
        }

        [Test]
        public async Task CheckToken()
        {
            HttpResponseMessage httpResponseTokenValid = await _httpClient.GetAsync($"{UrlController}/CheckToken");
            BaseResponse<bool> responseTokenValid = await DeserializeHelper.TryDeserializeStandartResponse<bool>(httpResponseTokenValid);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseTokenValid.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseTokenValid.StatusCode, Is.EqualTo(ServiceCode.TokenValidated));
                Assert.That(responseTokenValid?.Data, Is.EqualTo(true));
            });
        }
    }
}
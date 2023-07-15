using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.Tests.WebAppFactories;
using System.Net.Http.Json;
using FriendBook.IdentityServer.API;
using Newtonsoft.Json;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.DTO;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace FriendBook.IdentityServer.Tests
{
    [TestFixture]
    public class IntegrationTestsIdentityServerController
    {
        private WebHostFactory<Program, IdentityContext> _webHost;
        private HttpClient _httpClient;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _webHost = new WebHostFactory<Program, IdentityContext>();
            await _webHost.InitializeAsync();

            _httpClient = _webHost.CreateClient();
        }
        [TearDown]
        public async Task Clear()
        {
            await _webHost.ClearData();
        }

        [OneTimeTearDown]
        public async Task Dispose() 
        {
            await _webHost.DisposeAsync();
        }
        [Test]
        public async Task TestRegistration()
        {
            var accountForRegistration = new RequestAccount()
            {
                Login = "Ilia",
                Password = "TestPassword12345!",
            };
            HttpContent requestAccountContent = JsonContent.Create(accountForRegistration);

            HttpResponseMessage responseAuth = await _httpClient.PostAsync($"api/v1/IdentityServer/Registration", requestAccountContent);
            BaseResponse<ResponseAuthenticated> custResponseAuth = JsonConvert.DeserializeObject<StandartResponse<ResponseAuthenticated>>(await responseAuth.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: response auth");

            Assert.Multiple(() =>
            {
                Assert.That(responseAuth.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(custResponseAuth.StatusCode, Is.EqualTo(API.Domain.StatusCode.AccountRegistered));
                Assert.IsNotNull(custResponseAuth?.Data?.RefreshToken, custResponseAuth?.Data?.AccessToken);
            });
        }

        [Test]
        public async Task TestAuthenticate()
        {
            var accountForRegistration = new RequestAccount()
            {
                Login = "Ilia",
                Password = "TestPassword12345!",
            };
            HttpContent requestAccountContent = JsonContent.Create(accountForRegistration);

            HttpResponseMessage responseAuth1 = await _httpClient.PostAsync($"api/v1/IdentityServer/Registration", requestAccountContent);

            HttpResponseMessage responseAuth2 = await _httpClient.PostAsync($"api/v1/IdentityServer/Authenticate", requestAccountContent);
            BaseResponse<ResponseAuthenticated> custResponseAuth2 = JsonConvert.DeserializeObject<StandartResponse<ResponseAuthenticated>>(await responseAuth2.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: response auth");

            Assert.Multiple(() =>
            {
                Assert.That(responseAuth2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(custResponseAuth2.StatusCode, Is.EqualTo(API.Domain.StatusCode.AccountAuthenticated));
                Assert.IsNotNull(custResponseAuth2?.Data?.RefreshToken, custResponseAuth2?.Data?.AccessToken);
            });
        }
        [Test]
        public async Task TestAuthenticateByRefreshToken()
        {
            var accountForRegistration = new RequestAccount()
            {
                Login = "Ilia",
                Password = "TestPassword12345!",
            };
            HttpContent requestAccountContent = JsonContent.Create(accountForRegistration);

            HttpResponseMessage responseAuth = await _httpClient.PostAsync($"api/v1/IdentityServer/Registration", requestAccountContent);
            BaseResponse<ResponseAuthenticated> custResponseAuth = JsonConvert.DeserializeObject<StandartResponse<ResponseAuthenticated>>(await responseAuth.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: response auth");

            var jwtSettings = _webHost.Services.GetService<IOptions<JWTSettings>>()?.Value ??
                throw new InvalidOperationException($"{JWTSettings.Name} not found");

            var tokenAuth = TokenHelpers.CreateTokenAuth(custResponseAuth?.Data?.AccessToken, jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.AccessTokenSecretKey);
            HttpContent requestTokenAuth = JsonContent.Create(tokenAuth);

            HttpResponseMessage responseAT = await _httpClient.PostAsync($"api/v1/IdentityServer/AuthenticateByRefreshToken?refreshToken={custResponseAuth?.Data?.RefreshToken}", requestTokenAuth);

            BaseResponse<string> custResponseAT = JsonConvert.DeserializeObject<StandartResponse<string>>(await responseAT.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: response auth");

            Assert.Multiple(() =>
            {
                Assert.That(responseAT.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(custResponseAT.StatusCode, Is.EqualTo(API.Domain.StatusCode.AccountAuthenticatedByRT));
                Assert.IsNotNull(custResponseAT?.Data);
            });
        }
        [Test]
        public async Task TestCheckToken()
        {
            var accountForRegistration = new RequestAccount()
            {
                Login = "Ilia",
                Password = "TestPassword12345!",
            };
            HttpContent requestAccountContent = JsonContent.Create(accountForRegistration);

            HttpResponseMessage responseAuth = await _httpClient.PostAsync($"api/v1/IdentityServer/Registration", requestAccountContent);
            BaseResponse<ResponseAuthenticated> custResponseAuth = JsonConvert.DeserializeObject<StandartResponse<ResponseAuthenticated>>(await responseAuth.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: response auth");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", custResponseAuth?.Data?.AccessToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/v1/IdentityServer/CheckToken");

            HttpResponseMessage responseCheckToken = await _httpClient.SendAsync(request);
            BaseResponse<bool> custResponseCheckToken = JsonConvert.DeserializeObject<StandartResponse<bool>>(await responseCheckToken.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: response auth");

            Assert.Multiple(() =>
            {
                Assert.That(responseCheckToken.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(custResponseCheckToken.StatusCode, Is.EqualTo(API.Domain.StatusCode.AccessTokenValid));
                Assert.That(custResponseCheckToken?.Data, Is.EqualTo(true));
            });
        }
    }
}
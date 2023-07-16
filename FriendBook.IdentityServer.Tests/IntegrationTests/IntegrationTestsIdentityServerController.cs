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
using FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.IdentityServer.API.Domain.JWT;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    public class IntegrationTestsIdentityServerController
    {
        private WebHostFactory<Program, IdentityContext> _webHost;
        private HttpClient _httpClient;

        private readonly RequestAccount _requestAccount;
        private ResponseAuthenticated _responseRegistries;
        private DataAccessToken _userData;

        internal const string UrlController = "api/v1/IdentityServer";
        public IntegrationTestsIdentityServerController(RequestAccount requestAccount)
        {
            _requestAccount = requestAccount;
        }

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
        [SetUp]
        public async Task RegistrationTestAccount()
        {
            HttpContent requestAccountContent = JsonContent.Create(_requestAccount);
            HttpResponseMessage responseAuth = await _httpClient.PostAsync($"{UrlController}/Registration", requestAccountContent);
            _responseRegistries = JsonConvert.DeserializeObject<StandartResponse<ResponseAuthenticated>>(await responseAuth.Content.ReadAsStringAsync())?.Data
                ?? throw new JsonException("Error when parsing JSON: response auth");

            var jwtSettings = _webHost.Services.GetService<IOptions<JWTSettings>>()?.Value
                ?? throw new InvalidOperationException($"{JWTSettings.Name} not found");
            _userData = TokenHelpers.GetDataTokenAuth(_responseRegistries.AccessToken, jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.AccessTokenSecretKey)
                ?? throw new InvalidOperationException("The access token cannot be read");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _responseRegistries.AccessToken);
        }

        [OneTimeTearDown]
        public async Task Dispose()
        {
            await _webHost.DisposeAsync();
        }
        [Test]
        public async Task TestRegistration()
        {
            HttpContent requestAccountContent = JsonContent.Create(_requestAccount);

            HttpResponseMessage responseAuth = await _httpClient.PostAsync($"{UrlController}/Registration", requestAccountContent);
            BaseResponse<ResponseAuthenticated> customResponseAuth = JsonConvert.DeserializeObject<StandartResponse<ResponseAuthenticated>>(await responseAuth.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: responseAuth");

            Assert.Multiple(() =>
            {
                Assert.That(responseAuth.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(customResponseAuth.StatusCode, Is.EqualTo(API.Domain.StatusCode.UserAlreadyExists));
                Assert.IsNull(customResponseAuth?.Data?.RefreshToken);
                Assert.IsNull(customResponseAuth?.Data?.AccessToken);
            });
        }

        [Test]
        public async Task TestAuthenticate()
        {
            HttpContent requestAccountContent = JsonContent.Create(_requestAccount);

            HttpResponseMessage responseAuth = await _httpClient.PostAsync($"{UrlController}/Authenticate", requestAccountContent);
            BaseResponse<ResponseAuthenticated> customResponseAuth = JsonConvert.DeserializeObject<StandartResponse<ResponseAuthenticated>>(await responseAuth.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: responseAuth");

            Assert.Multiple(() =>
            {
                Assert.That(responseAuth.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(customResponseAuth.StatusCode, Is.EqualTo(API.Domain.StatusCode.UserAuthenticated));
                Assert.That(customResponseAuth?.Data?.RefreshToken, Is.Not.Null);
                Assert.That(customResponseAuth?.Data?.AccessToken, Is.Not.Null);
            });
        }
        [Test]
        public async Task TestAuthenticateByRefreshToken()
        {
            HttpContent requestTokenAuth = JsonContent.Create(_userData);

            HttpResponseMessage responseAT = await _httpClient.PostAsync($"{UrlController}/AuthenticateByRefreshToken?refreshToken={_responseRegistries.RefreshToken}", requestTokenAuth);

            BaseResponse<string> customResponseAT = JsonConvert.DeserializeObject<StandartResponse<string>>(await responseAT.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: customResponseAT");

            Assert.Multiple(() =>
            {
                Assert.That(responseAT.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(customResponseAT.StatusCode, Is.EqualTo(API.Domain.StatusCode.UserAuthenticatedByRT));
                Assert.That(customResponseAT?.Data, Is.Not.Null);
            });
        }
        [Test]
        public async Task TestCheckToken()
        {
            HttpResponseMessage responseTokenValid = await _httpClient.GetAsync($"{UrlController}/CheckToken");
            BaseResponse<bool> customResponseTokenValid = JsonConvert.DeserializeObject<StandartResponse<bool>>(await responseTokenValid.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: customResponseTokenValid");

            Assert.Multiple(() =>
            {
                Assert.That(responseTokenValid.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(customResponseTokenValid.StatusCode, Is.EqualTo(API.Domain.StatusCode.TokenValid));
                Assert.That(customResponseTokenValid?.Data, Is.EqualTo(true));
            });
        }
    }
}
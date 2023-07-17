using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Settings;
using FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.IdentityServer.Tests.WebAppFactories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FriendBook.IdentityServer.API;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using FriendBook.IdentityServer.API.BLL.gRPCServices.ContactService;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsGrpcPublicContactService
    {
        private WebHostFactory<Program, IdentityContext> _webHost;
        private HttpClient _httpClient;

        private readonly RequestAccount _requestAccount;
        private ResponseAuthenticated _responseRegistries;
        private DataAccessToken _userData;

        internal const string UrlController = "api/v1/GrpcContactService";

        public IntegrationTestsGrpcPublicContactService(RequestAccount requestAccount)
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

        [SetUp]
        public async Task TestRegistrationTestAccount()
        {
            HttpContent requestAccountContent = JsonContent.Create(_requestAccount);
            HttpResponseMessage responseAuth = await _httpClient.PostAsync($"{IntegrationTestsIdentityServerController.UrlController}/Registration", requestAccountContent);
            _responseRegistries = JsonConvert.DeserializeObject<StandartResponse<ResponseAuthenticated>>(await responseAuth.Content.ReadAsStringAsync())?.Data
                ?? throw new JsonException("Error parsing JSON: response AUTH");

            var jwtSettings = _webHost.Services.GetService<IOptions<JWTSettings>>()?.Value
                ?? throw new InvalidOperationException($"{JWTSettings.Name} not found");
            _userData = TokenHelpers.GetDataTokenAuth(_responseRegistries.AccessToken, jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.AccessTokenSecretKey)
                ?? throw new InvalidOperationException("The access token cannot be read");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _responseRegistries.AccessToken);
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
        public async Task GetProfiles()
        {
            RequestAccount requestNewTestAccount = new RequestAccount() { Login = "NewTestUser", Password = "TestPassword12345!" };
            HttpContent requestAccountContent = JsonContent.Create(requestNewTestAccount);
            await _httpClient.PostAsync($"{IntegrationTestsIdentityServerController.UrlController}/Registration", requestAccountContent);

            HttpResponseMessage responseProfiles = await _httpClient.GetAsync($"{UrlController}/GetProfiles?Login={""}");
            ResponseProfiles objResponseProfiles = JsonConvert.DeserializeObject<ResponseProfiles>(await responseProfiles.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: response auth");

            Assert.Multiple(() =>
            {
                Assert.That(responseProfiles.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(objResponseProfiles.Profiles.Count, Is.EqualTo(1));
                Assert.That(objResponseProfiles.Profiles[0].Login, Is.EqualTo(requestNewTestAccount.Login));
                Assert.That(objResponseProfiles.Profiles[0].FullName, Is.EqualTo(""));
            });
        }
    }
}

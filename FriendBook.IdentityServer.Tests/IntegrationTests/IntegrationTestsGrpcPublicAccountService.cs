using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.DTO;
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
using FriendBook.IdentityServer.API.BLL.gRPCServices.AccountService;
using FriendBook.IdentityServer.API.Domain.Response;
using FriendBook.IdentityServer.Tests.TestHelpers;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    public class IntegrationTestsGrpcPublicAccountService
    {
        private WebHostFactory<Program, IdentityContext> _webHost;
        private HttpClient _httpClient;

        private readonly RequestNewAccount _requestAccount;
        private ResponseAuthenticate _responseRegistries;
        private DataAccessToken _userData;

        internal const string UrlController = "api/v1/GrpcAccountService";

        public IntegrationTestsGrpcPublicAccountService(RequestNewAccount requestAccount)
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
            _responseRegistries = (await DeserializeHelper.TryDeserializeStandartResponse<ResponseAuthenticate>(responseAuth)).Data 
                ?? throw new InvalidOperationException($"Data ResponseAuthenticated is null");

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
        public async Task CheckUserExists() 
        {
            HttpResponseMessage responseUserExists = await _httpClient.GetAsync($"{UrlController}/CheckUserExists?AccountId={_userData.Id}");
            ResponseUserExists objResponseUserExists = JsonConvert.DeserializeObject<ResponseUserExists>(await responseUserExists.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: Response user exists");

            Assert.Multiple(() =>
            {
                Assert.That(responseUserExists.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(objResponseUserExists.Exists, Is.EqualTo(true));
            });
        }
        [Test]
        public async Task GetUsersLoginById()
        {
            HttpResponseMessage responseUsersLogin = await _httpClient.GetAsync($"{UrlController}/GetUsersLoginById?UserId={_userData.Id}&UserId={_userData.Id}");
            ResponseUsers objResponseUsersLogin = JsonConvert.DeserializeObject<ResponseUsers>(await responseUsersLogin.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error when parsing JSON: Response users login");

            Assert.Multiple(() =>
            {
                Assert.That(responseUsersLogin.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(objResponseUsersLogin.Users.Count, Is.EqualTo(1));
                Assert.That(objResponseUsersLogin.Users[0].Login, Is.EqualTo(_userData.Login));
                Assert.That(objResponseUsersLogin.Users[0].Id, Is.EqualTo(_userData.Id.ToString()));
            });
        }
    }
}

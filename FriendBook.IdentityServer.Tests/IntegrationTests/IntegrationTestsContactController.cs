using FriendBook.IdentityServer.API;
using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.Tests.WebAppFactories;
using System.Net.Http.Json;
using System.Net;
using FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Response;
using FriendBook.IdentityServer.Tests.TestHelpers;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    public class IntegrationTestsContactController
    {
        private WebHostFactory<Program, IdentityContext> _webHost;
        private HttpClient _httpClient;

        private readonly RequestAccount _requestAccount;
        private ResponseAuthenticated _responseRegistries;
        private DataAccessToken _userData;

        internal const string UrlController = "api/v1/Contact";

        public IntegrationTestsContactController(RequestAccount requestAccount)
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
            HttpContent accountContent = JsonContent.Create(_requestAccount);
            HttpResponseMessage httpResponseAuth = await _httpClient.PostAsync($"{IntegrationTestsIdentityServerController.UrlController}/Registration", accountContent);
            _responseRegistries = (await DeserializeHelper.TryDeserializeStandartResponse<ResponseAuthenticated>(httpResponseAuth)).Data
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
        public async Task GetContactInformation() 
        {
            HttpResponseMessage httpResponseContact = await _httpClient.GetAsync($"{UrlController}/Get/{_userData.Id}");

            var responseContact = await DeserializeHelper.TryDeserializeStandartResponse<UserContactDTO>(httpResponseContact);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseContact.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContact.StatusCode, Is.EqualTo(ServiceCode.ContactReadied));
                Assert.That(responseContact?.Data, Is.Not.Null);
            });
        }

        [Test]
        public async Task TestGetProfiles()
        {
            var newUser = new RequestAccount() { Login = "TestNewUser", Password = "TestPassword12345!" };
            HttpContent accountContent = JsonContent.Create(newUser);

            HttpResponseMessage httpResponseNewUser = await _httpClient.PostAsync($"{IntegrationTestsIdentityServerController.UrlController}/Registration", accountContent);

            HttpResponseMessage httpResponseProfiles = await _httpClient.GetAsync($"{UrlController}/GetProfiles");
            var responseProfiles = await DeserializeHelper.TryDeserializeStandartResponse<ResponseProfile[]>(httpResponseProfiles);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseProfiles.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseProfiles.StatusCode, Is.EqualTo(ServiceCode.ContactReadied));
                Assert.That(responseProfiles?.Data, Is.Not.Null);
                Assert.That(responseProfiles?.Data?[0].Login, Is.EqualTo(newUser.Login));
            });
        }

        [Test]
        public async Task UpdateMyContactInformation()
        {
            UserContactDTO userContactDTO = new() 
            {
                Email = "test@gmail.com",
                FullName = "Pasha Dubkovski",
                Info = "I Work in Microsoft",
                Profession = ".NET Developer",
                Telephone = "375 (29) 231 91 83" 
            };
            HttpContent userContactDTOContent = JsonContent.Create(userContactDTO);

            HttpResponseMessage httpResponseUpdatedContact = await _httpClient.PutAsync($"{UrlController}/Update", userContactDTOContent);
            var responseUpdatedContact = await DeserializeHelper.TryDeserializeStandartResponse<UserContactDTO>(httpResponseUpdatedContact);

            HttpResponseMessage httpResponseContact = await _httpClient.GetAsync($"{UrlController}/Get/{_userData.Id}");
            var responseContact = await DeserializeHelper.TryDeserializeStandartResponse<UserContactDTO>(httpResponseContact);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedContact.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(httpResponseContact.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(responseUpdatedContact.StatusCode, Is.EqualTo(ServiceCode.ContactUpdated));
                Assert.That(responseUpdatedContact?.Data, Is.Not.Null);
                Assert.That(responseContact?.Data, Is.Not.Null);
                AssertEx.PropertyValuesAreEquals(responseContact!.Data!, responseUpdatedContact!.Data!);
            });
        }
    }
}

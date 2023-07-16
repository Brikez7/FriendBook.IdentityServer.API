using FriendBook.IdentityServer.API;
using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.Tests.WebAppFactories;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;
using FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FriendBook.IdentityServer.API.Domain.UserToken;
using System.Net.Http.Headers;

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

        private const string UrlController = "api/v1/Contact";

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
        public async Task TestGetContactInformation() 
        {
            HttpResponseMessage responseContact = await _httpClient.GetAsync($"{UrlController}/Get/{_userData.Id}");
            BaseResponse<UserContactDTO> customResponseContact = JsonConvert.DeserializeObject<StandartResponse<UserContactDTO>>(await responseContact.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error parsing JSON: response UserContactDTO");

            Assert.Multiple(() =>
            {
                Assert.That(responseContact.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(customResponseContact.StatusCode, Is.EqualTo(API.Domain.StatusCode.ContactRead));
                Assert.IsNotNull(customResponseContact?.Data);
            });
        }

        [Test]
        public async Task TestGetProfiles()
        {
            var newUser = new RequestAccount() { Login = "TestNewUser", Password = "TestPassword12345!" };

            HttpContent requestAccountContent = JsonContent.Create(newUser);
            HttpResponseMessage responseAddNewUser = await _httpClient.PostAsync($"{IntegrationTestsIdentityServerController.UrlController}/Registration", requestAccountContent);

            HttpResponseMessage responseProfiles = await _httpClient.GetAsync($"{UrlController}/GetProfiles");
            BaseResponse<ResponseProfile[]> customResponseProfiles = JsonConvert.DeserializeObject<StandartResponse<ResponseProfile[]>>(await responseProfiles.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error parsing JSON: response ResponseProfile[]");

            Assert.Multiple(() =>
            {
                Assert.That(responseProfiles.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseAddNewUser.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(customResponseProfiles.StatusCode, Is.EqualTo(API.Domain.StatusCode.ContactRead));
                Assert.IsNotNull(customResponseProfiles?.Data);
                Assert.That(customResponseProfiles!.Data![0].Login, Is.EqualTo(newUser.Login));
            });
        }

        [Test]
        public async Task TestUpdateMyContactInformation()
        {
            UserContactDTO userContactDTO = new UserContactDTO() 
            {
                Email = "test@gmail.com",
                FullName = "Pasha Dubkovski",
                Info = "I Work in Microsoft",
                Profession = ".NET Developer",
                Telephone = "375 (29) 231 91 83" 
            };
            HttpContent requestUserContactDTO = JsonContent.Create(userContactDTO);

            HttpResponseMessage responseUpdatedContact = await _httpClient.PutAsync($"{UrlController}/Update", requestUserContactDTO);
            BaseResponse<UserContactDTO> customResponseUpdatedContact = JsonConvert.DeserializeObject<StandartResponse<UserContactDTO>>(await responseUpdatedContact.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error parsing JSON: response UserContactDTO");

            HttpResponseMessage responseContact = await _httpClient.GetAsync($"{UrlController}/Get/{_userData.Id}");
            BaseResponse<UserContactDTO> customResponseContact = JsonConvert.DeserializeObject<StandartResponse<UserContactDTO>>(await responseContact.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error parsing JSON: response UserContactDTO");

            Assert.Multiple(() =>
            {
                Assert.That(responseUpdatedContact.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContact.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(customResponseUpdatedContact.StatusCode, Is.EqualTo(API.Domain.StatusCode.ContactUpdate));
                Assert.That(customResponseUpdatedContact?.Data, Is.Not.Null);
                Assert.That(customResponseContact?.Data, Is.Not.Null);
                AssertEx.PropertyValuesAreEquals(customResponseContact!.Data!, customResponseUpdatedContact!.Data!);
            });
        }
    }
}

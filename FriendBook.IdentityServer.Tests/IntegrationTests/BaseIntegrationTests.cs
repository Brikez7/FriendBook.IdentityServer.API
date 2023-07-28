using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Settings;
using FriendBook.IdentityServer.Tests.TestHelpers;
using FriendBook.IdentityServer.Tests.WebAppFactories;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FriendBook.IdentityServer.API;
using Microsoft.Extensions.DependencyInjection;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    public abstract class BaseIntegrationTests
    {
        private protected readonly RequestNewAccount _requestAccount;

        private protected WebHostFactory<Program, IdentityContext> _webHost;
        private protected HttpClient _httpClient;

        private protected ResponseAuthenticate _responseRegistries;
        private protected DataAccessToken _userData;

        internal const string UrlAPI = "IdentityServer/v1";

        public BaseIntegrationTests(RequestNewAccount requestAccount)
        {
            _requestAccount = requestAccount;
        }

        [OneTimeSetUp]
        public virtual async Task Initialization()
        {
            _webHost = new WebHostFactory<Program, IdentityContext>();
            await _webHost.InitializeAsync();

            _httpClient = _webHost.CreateClient();
        }

        [SetUp]
        public virtual async Task SetUp()
        {
            HttpContent accountContent = JsonContent.Create(_requestAccount);
            HttpResponseMessage httpResponseAuth = await _httpClient.PostAsync($"{IntegrationTestsIdentityServerController.UrlController}/Registration", accountContent);
            _responseRegistries = (await DeserializeHelper.TryDeserializeStandartResponse<ResponseAuthenticate>(httpResponseAuth)).Data
                ?? throw new InvalidOperationException($"Data ResponseAuthenticate is null");

            var jwtSettings = _webHost.Services.GetService<IOptions<JWTSettings>>()?.Value
                ?? throw new InvalidOperationException($"{JWTSettings.Name} not found");
            _userData = TokenHelpers.GetDataTokenAuth(_responseRegistries.AccessToken, jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.AccessTokenSecretKey)
                ?? throw new InvalidOperationException("The access token cannot be read");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _responseRegistries.AccessToken);
        }
        [TearDown]
        public virtual async Task Clear()
        {
            await _webHost.ClearData();
        }

        [OneTimeTearDown]
        public virtual async Task Dispose()
        {
            await _webHost.DisposeAsync();
        }
    }
}

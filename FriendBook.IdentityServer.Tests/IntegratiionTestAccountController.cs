using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.Tests.WebAppFactories;
using System.Net.Http.Json;
using FriendBook.IdentityServer.API;

namespace FriendBook.IdentityServer.Tests
{
    [TestFixture]
    public class IntegratiionTestsIdentityServerController
    {
        private WebHostFactory<Program, IdentityContext> _webHost;
        private HttpClient _httpClient;

        [SetUp]
        public async Task Setup()
        {
            _webHost = new WebHostFactory<Program, IdentityContext>();
            await _webHost.InitializeAsync();

            _httpClient = _webHost.CreateClient();
        }
        [TearDown]
        public async Task Dispose() 
        {
            await _webHost.DisposeAsync();
        }
        [Test]
        public async Task Registration()
        {
            var accountForRegistration = new RequestAccount()
            {
                Login = "Ilia",
                Password = "TestPassword12345!",
            };

            HttpContent content = JsonContent.Create(accountForRegistration);
            HttpResponseMessage createdResult = await _httpClient.PostAsync($"api/IdentityServer/Registration", content);

            Assert.That(createdResult.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }
    }
}
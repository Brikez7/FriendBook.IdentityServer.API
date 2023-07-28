using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using System.Net.Http.Json;
using System.Net;
using FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.IdentityServer.API.Domain.Response;
using FriendBook.IdentityServer.Tests.TestHelpers;

namespace FriendBook.IdentityServer.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsContactController : BaseIntegrationTests
    {
        internal const string UrlController = $"{UrlAPI}/Contact";

        public IntegrationTestsContactController(RequestNewAccount requestAccount) : base(requestAccount){}

        [Test]
        public async Task GetContact() 
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
        public async Task GetProfiles()
        {
            var requestNewAccount = new RequestNewAccount() { Login = "TestNewUser", Password = "TestPassword12345!" };
            HttpContent requestNewAccountContent = JsonContent.Create(requestNewAccount);

            HttpResponseMessage httpResponseNewUser = await _httpClient.PostAsync($"{IntegrationTestsIdentityServerController.UrlController}/Registration", requestNewAccountContent);

            HttpResponseMessage httpResponseProfiles = await _httpClient.GetAsync($"{UrlController}/GetProfiles");
            var responseProfiles = await DeserializeHelper.TryDeserializeStandartResponse<ResponseProfile[]>(httpResponseProfiles);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseProfiles.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseProfiles.StatusCode, Is.EqualTo(ServiceCode.ContactReadied));
                Assert.That(responseProfiles?.Data, Is.Not.Null);
                Assert.That(responseProfiles?.Data?[0].Login, Is.EqualTo(requestNewAccount.Login));
            });
        }

        [Test]
        public async Task UpdateContact()
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
            var responseGetContact = await DeserializeHelper.TryDeserializeStandartResponse<UserContactDTO>(httpResponseContact);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedContact.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(httpResponseContact.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(responseUpdatedContact.StatusCode, Is.EqualTo(ServiceCode.ContactUpdated));
                Assert.That(responseUpdatedContact?.Data, Is.Not.Null);
                Assert.That(responseGetContact?.Data, Is.Not.Null);
                AssertEx.PropertyValuesAreEquals(responseGetContact!.Data!, responseUpdatedContact!.Data!);
            });
        }
    }
}

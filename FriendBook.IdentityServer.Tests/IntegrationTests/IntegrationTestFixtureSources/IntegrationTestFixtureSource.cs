using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using System.Collections;

namespace FriendBook.IdentityServer.Tests.IntegrationTests.IntegrationTestFixtureSources
{

    internal class IntegrationTestFixtureSource : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { new RequestAccount { Login = "Ilia", Password = "IliaPassword12345!" }};
            yield return new object[] { new RequestAccount { Login = "Dima", Password = "DimaPassword12345!" } };
        }
    }
}

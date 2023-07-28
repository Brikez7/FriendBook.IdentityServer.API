using FriendBook.IdentityServer.API.Domain.Response;
using Newtonsoft.Json;

namespace FriendBook.IdentityServer.Tests.TestHelpers
{
    internal static class DeserializeHelper
    {
        internal static async Task<StandartResponse<T>> TryDeserializeStandartResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<StandartResponse<T>>(await httpResponseMessage.Content.ReadAsStringAsync())
                ?? throw new JsonException($"Error deserialize JSON: StandartResponse<{typeof(T)}>");
        }
        internal static async Task<T> TryDeserialize<T>(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<T>(await httpResponseMessage.Content.ReadAsStringAsync())
                ?? throw new JsonException($"Error deserialize JSON: {typeof(T)}");
        }
    }
}

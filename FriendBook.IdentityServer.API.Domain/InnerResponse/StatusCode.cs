namespace FriendBook.IdentityServer.API.Domain.InnerResponse
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        AccountCreate = 1,
        AccountUpdate = 2,
        AccountDelete = 3,
        AccountRead = 4,
        AccountAuthenticate = 5,


        OK = 200,
        OKNoContent = 204,
        ErrorAuthenticate = 301,
        ErrorRegistration = 302,
        AccountNotAuthenticate = 401,
        InternalServerError = 500,
    }
}

namespace FriendBook.IdentityServer.API.Domain
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        AccountCreate = 201,
        AccountUpdate = 202,
        AccountDelete = 203,
        AccountRead = 204,
        AccountAuthenticate = 205,

        OK = 200,
        OKNoContent = 199,
        ErrorAuthenticate = 301,
        ErrorRegistration = 302,
        AccountNotAuthenticate = 401,
        AccountWithLoginExists = 409,
        InternalServerError = 500,
    }
}

namespace FriendBook.IdentityServer.API.Domain
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        AccountCreate = 201,
        AccountUpdate = 202,
        AccountDelete = 203,
        AccountRead = 204,

        AccountAuthenticated = 205,
        AccountAuthenticatedByRT = 206,
        AccountAlreadyExists = 207,
        AccountRegistered = 208,

        ContactClear = 211,

        OK = 200,

        TokenNotValid = 31,
        TokenRead = 32,
        TokenGenerate = 33,
        AccessTokenValid = 34,

        RedisLock = 41,
        RedisEmpty = 42,
        RedisReceive = 43,

        EntityIsValid = 54,
        ErrorValidation = 55,

        OKNoContent = 199,
        ErrorAuthenticate = 301,
        ErrorRegistration = 302,

        AccountNotAuthenticate = 401,
        InternalServerError = 500,
    }
}

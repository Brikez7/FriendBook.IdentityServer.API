namespace FriendBook.IdentityServer.API.Domain
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        AccountCreate = 201,
        AccountUpdate = 202,
        AccountDelete = 203,
        AccountRead = 204,

        ContactCreate = 211,
        ContactUpdate = 212,
        ContactDelete = 213,
        ContactRead = 214,
        ContactClear = 215,

        UserAuthenticated = 205,
        UserAuthenticatedByRT = 206,
        UserAlreadyExists = 207,
        UserRegistered = 208,
        UserNotAuthenticate = 209,

        OK = 200,

        TokenNotValid = 31,
        TokenRead = 32,
        TokenGenerate = 33,
        TokenValid = 34,

        RedisLock = 41,
        RedisEmpty = 42,
        RedisReceive = 43,

        EntityIsValid = 54,
        ErrorValidation = 55,

        OKNoContent = 199,
        ErrorAuthenticate = 301,
        ErrorRegistration = 302,

        InternalServerError = 500,
    }
}

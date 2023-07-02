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
        AccountAuthenticateByRT = 206,
        AccountAlraedyExists = 207,

        ContactClear = 211,

        OK = 200,


        
        TokenNotValid = 31,
        TokenRead = 32,
        TokenGenerate = 33,


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

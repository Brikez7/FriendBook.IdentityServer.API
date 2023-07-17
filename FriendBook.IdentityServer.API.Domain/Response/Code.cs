namespace FriendBook.IdentityServer.API.Domain.Response
{
    public enum Code
    {
        EntityNotFound = 200,

        AccountCreated = 1001,
        AccountUpdated = 1002,
        AccountDeleted = 1003,
        AccountReadied = 1004,
        AccountAlreadyExists = 1005,

        ContactUpdated = 1011,
        ContactReadied = 1012,
        ContactCleared = 1013,

        TokenGenerated = 1021,
        TokenReadied = 1022,
        TokenValidated = 1023,
        TokenNotValidated = 1024,

        RedisLocked = 1031,
        RedisEmpty = 1032,
        RedisReceived = 1033,

        UserRegistered = 1041,
        UserAuthenticated = 1042,
        ErrorAuthenticated = 1043,
        UserAuthenticatedByRT = 1044,
        ErrorAuthenticatedByRT = 1045,

        EntityIsValidated = 501,
        EntityIsNotValidated = 502,
    }
}

﻿namespace FriendBook.IdentityServer.API.Domain.Response
{
    public class StandartResponse<T> : BaseResponse<T>
    {
        public override string? Message { get; set; }
        public override Code StatusCode { get; set; }
        public override T? Data { get; set; }
    }
}

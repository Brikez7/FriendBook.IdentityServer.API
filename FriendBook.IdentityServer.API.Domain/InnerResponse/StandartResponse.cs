namespace FriendBook.IdentityServer.API.Domain.InnerResponse
{
    public class StandartResponse<T> : BaseResponse<T>
    {
        public override string? Message { get; set; } 
        public override StatusCode StatusCode { get; set; }
        public override T? Data { get; set; }
    }
}

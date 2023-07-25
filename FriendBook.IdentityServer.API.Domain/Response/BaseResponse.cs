namespace FriendBook.IdentityServer.API.Domain.Response
{
    public abstract class BaseResponse<T>
    {
        public virtual T? Data { get; set; }
        public virtual ServiceCode StatusCode { get; set; }
        public virtual string? Message { get; set; }
    }
}

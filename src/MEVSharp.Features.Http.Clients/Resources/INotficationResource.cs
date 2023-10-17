namespace MEVSharp.Features.Http.Clients.Resources
{
    public interface INotificationResource
    {
        Task<HttpResponseMessage> Notify(string message);
    }
}

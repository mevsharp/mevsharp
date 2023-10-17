using MEVSharp.Features.Http.Clients.Resources;

namespace MEVSharp.Features.Http.Clients.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<HttpResponseMessage>> Notify(string message);

    }

    public class NotificationService : INotificationService
    {
        private List<INotificationResource> resources = new List<INotificationResource>(); 
        
        public NotificationService Add(INotificationResource service)
        {
            resources.Add(service);
            return this;
        }

        public async Task<IEnumerable<HttpResponseMessage>> Notify(string message)
        {
            List<Task<HttpResponseMessage>> tasks = new();
            foreach (var service in resources)
            {
                tasks.Add(service.Notify(message));
            }
            await Task.WhenAll(tasks);

            return tasks.Select(x => x.Result);
            
        }
        
    }
}

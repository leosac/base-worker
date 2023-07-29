using Leosac.CredentialProvisioning.Core.Models;

namespace Leosac.CredentialProvisioning.Worker
{
    public class WorkerQueue
    {
        protected IDictionary<string, WorkerQueueItem> _items = new Dictionary<string, WorkerQueueItem>();

        public WorkerQueue()
        {
        }

        public string Add(string templateId, WorkerCredentialBase credential)
        {
            var itemId = Guid.NewGuid().ToString();
            lock (_items)
            {
                _items.Add(itemId, new WorkerQueueItem() { TemplateId = templateId, Credential = credential});
            }
            return itemId;
        }

        public WorkerQueueItem? Get(string itemId)
        {
            lock (_items)
            {
                if (_items.ContainsKey(itemId))
                {
                    return _items[itemId];
                }

                return null;
            }
        }

        public void ScheduleRemove(string itemId)
        {
            lock (_items)
            {
                var item = Get(itemId);
                if (item != null)
                {
                    item.Removing = true;
                    Task.Delay(30000).ContinueWith(task =>
                    {
                        Remove(itemId);
                    });
                }
            }
        }

        public void Remove(string itemId)
        {
            lock (_items)
            {
                if (_items.ContainsKey(itemId))
                {
                    _items.Remove(itemId);
                }
            }
        }
    }
}

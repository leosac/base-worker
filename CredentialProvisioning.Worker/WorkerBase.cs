using Leosac.CredentialProvisioning.Core.Contexts;
using Leosac.CredentialProvisioning.Core.Models;
using log4net;

namespace Leosac.CredentialProvisioning.Worker
{
    public abstract class WorkerBase<T>
    {
        static readonly ILog logger = LogManager.GetLogger(typeof(WorkerBase<T>));

        IList<TemplateContainer<T>> _templates = new List<TemplateContainer<T>>();

        public WorkerBase()
        {

        }

        public IDictionary<string, CredentialProcess<T>> Processes { get; private set; } = new Dictionary<string, CredentialProcess<T>>();

        public WorkerQueue Queue { get; private set; } = new WorkerQueue();

        public void LoadTemplate(string templateId, string template, long? revision = null)
        {
            var tpl = System.Text.Json.JsonSerializer.Deserialize<T>(template);
            if (tpl != null)
            {
                LoadTemplate(templateId, tpl, revision);
            }
        }

        public void LoadTemplate(string templateId, T template, long? revision = null)
        {
            lock (_templates)
            {
                var oldtpl = _templates.Where(t => t.Id == templateId).FirstOrDefault();
                if (oldtpl != null)
                {
                    _templates.Remove(oldtpl);
                }

                _templates.Add(new TemplateContainer<T>()
                {
                    Id = templateId,
                    Revision = revision ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Content = template
                });
            }
        }

        public T? GetTemplate(string templateId, long? revision = null)
        {
            var tpl = _templates.Where(t => t.Id == templateId).FirstOrDefault();
            if (tpl != null)
            {
                if (revision == null || tpl.Revision >= revision)
                    return tpl.Content;
            }

            return default(T);
        }

        public string[] GetTemplates()
        {
            return _templates.Select(t => t.Id).ToArray();
        }

        protected CredentialContext<T> CreateCredentialContext(string templateId, T template, WorkerCredentialBase credential)
        {
            return CreateCredentialContext(templateId, template, new[] { credential });
        }

        protected abstract CredentialContext<T> CreateCredentialContext(string templateId, T template, IList<WorkerCredentialBase> credentials);

        protected abstract CredentialProcess<T> CreateProcess();

        public CredentialProcess<T> InitializeProcess(string templateId, WorkerCredentialBase credential)
        {
            return InitializeProcess(templateId, new[] { credential });
        }

        public CredentialProcess<T> InitializeProcess(string templateId, IList<WorkerCredentialBase> credentials)
        {
            var process = CreateProcess();
            process.CredentialContext = CreateCredentialContext(templateId, GetTemplate(templateId), credentials);
            Processes.Add(process.Id, process);
            return process;
        }

        public CredentialProcess<T>? InitializeProcess(string itemId)
        {
            var item = Queue.Get(itemId);
            if (item == null)
                return null;
            
            var process = InitializeProcess(item.TemplateId, item.Credential);
            process.CredentialCompleted += (sender, e) =>
            {
                return Task.Run(() =>
                {
                    if (e != null)
                    {
                        // Be sure the data on item queue is up to date (worker implementation could have worked on copies)
                        var data = item.Credential.Data as IDictionary<string, object>;
                        if (data != null)
                        {
                            foreach (var key in e.Keys)
                            {
                                if (data.ContainsKey(key))
                                {
                                    data[key] = e[key];
                                }
                            }
                        }
                    }
                });
            };
            process.ProcessCompleted += (sender, e) =>
            {
                return Task.Run(() => {
                    Queue.ScheduleRemove(itemId);
                });
            };
            return process;
        }
    }
}

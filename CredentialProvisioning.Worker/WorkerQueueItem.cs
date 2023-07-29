using Leosac.CredentialProvisioning.Core.Models;

namespace Leosac.CredentialProvisioning.Worker
{
    public class WorkerQueueItem
    {
        public string TemplateId { get; set; }

        public WorkerCredentialBase Credential { get; set; }

        public object Context { get; set; }

        public bool Removing { get; set; }
    }
}

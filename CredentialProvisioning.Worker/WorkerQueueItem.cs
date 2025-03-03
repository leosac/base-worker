using Leosac.CredentialProvisioning.Core.Models;

namespace Leosac.CredentialProvisioning.Worker
{
    public class WorkerQueueItem
    {
        /// <summary>
        /// The associated Template Id.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// The credential details.
        /// </summary>
        public WorkerCredentialBase Credential { get; set; }

        /// <summary>
        /// An optional production context.
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// True if the iteam is scheduled for removing from the queue, false otherwise.
        /// </summary>
        public bool Removing { get; set; }

        /// <summary>
        /// Token populated on production success completion (if enabled).
        /// </summary>
        /// <remarks>
        /// This token can be used for delayed/remote validation.
        /// </remarks>
        public string? CompletionToken { get; set; }
    }
}

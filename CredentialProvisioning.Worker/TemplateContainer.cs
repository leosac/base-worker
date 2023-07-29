namespace Leosac.CredentialProvisioning.Worker
{
    public class TemplateContainer<T>
    {
        public string Id { get; set; }

        public long Revision { get; set; }

        public T Content { get; set; }
    }
}

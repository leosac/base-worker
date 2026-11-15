using Leosac.CredentialProvisioning.Core.Models;

namespace Leosac.CredentialProvisioning.Worker
{
    public class TemplateVariant
    {
        public string Code { get; set; }

        public CredentialFragmentTemplateParameter[]? Parameters { get; set; }
    }

    public class TemplateContainer<T>
    {
        public string Id { get; set; }

        public long Revision { get; set; }

        public T Content { get; set; }

        public IList<TemplateVariant> Variants { get; set; } = new List<TemplateVariant>();
    }
}

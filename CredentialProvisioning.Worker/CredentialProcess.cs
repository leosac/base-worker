using Leosac.CredentialProvisioning.Core.Contexts;
using Leosac.CredentialProvisioning.Core.Models;

namespace Leosac.CredentialProvisioning.Worker
{
    public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);

    public abstract class CredentialProcess<T>
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        public CredentialContext<T>? CredentialContext { get; set; }

        public abstract Task Run(DeviceContext deviceCtx);

        public event AsyncEventHandler<IDictionary<string, object>?>? CredentialCompleted;

        protected async Task OnCredentialCompleted(IDictionary<string, object>? changes)
        {
            if (CredentialCompleted != null)
            {
                await CredentialCompleted(this, changes);
            }
        }

        public event AsyncEventHandler<ProvisioningState>? ProcessCompleted;

        protected async Task OnProcessCompleted(ProvisioningState state)
        {
            if (ProcessCompleted != null)
            {
                await ProcessCompleted(this, state);
            }
        }
    }
}

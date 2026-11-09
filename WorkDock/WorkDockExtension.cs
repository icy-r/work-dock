using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CommandPalette.Extensions;

namespace WorkDock;

[Guid("77928981-7EA4-4819-A160-C9689DDAA1D6")]
public sealed partial class WorkDockExtension : IExtension, IDisposable
{
    private readonly ManualResetEvent _disposed;
    private readonly WorkDockCommandsProvider _provider = new();

    public WorkDockExtension(ManualResetEvent disposed) => _disposed = disposed;

    public object? GetProvider(ProviderType providerType) => providerType switch
    {
        ProviderType.Commands => _provider,
        _ => null,
    };

    public void Dispose() => _disposed.Set();
}

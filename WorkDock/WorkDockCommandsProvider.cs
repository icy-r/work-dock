using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace WorkDock;

public sealed partial class WorkDockCommandsProvider : CommandProvider
{
    public const string ProviderId = "com.workdock";

    private readonly WorkBand _band = new();
    private readonly ICommandItem[] _commands;

    public WorkDockCommandsProvider()
    {
        Id = ProviderId;
        DisplayName = "Work Dock";
        Icon = WorkIcons.Jira;
        _commands =
        [
            new CommandItem(new WorkListPage(WorkKind.Jira)) { Title = "Jira: assigned to me" },
            new CommandItem(new WorkListPage(WorkKind.MyPrs)) { Title = "Bitbucket: my pull requests" },
            new CommandItem(new WorkListPage(WorkKind.Review)) { Title = "Bitbucket: pull requests to review" },
        ];
        Notifier.Init();
    }

    public override ICommandItem[] TopLevelCommands() => _commands;

    public override ICommandItem[]? GetDockBands() => [_band];
}

# Work Dock

A [PowerToys Command Palette](https://learn.microsoft.com/windows/powertoys/command-palette/overview)
Dock widget for your day-to-day dev work: your assigned Jira issues, your open Bitbucket Cloud pull
requests, and the PRs waiting for your review, with build state, approvals and authors.

> Community project. Not affiliated with or endorsed by Microsoft or Atlassian.

## What it shows

| Item | Shows |
|---|---|
| `Jira N · k!` | assigned issues that are not done; `k!` = high priority, overdue, or blocked |
| `PRs N · k✓ · k✗` | your open PRs; ready to merge / needs attention (changes requested or build failed) |
| `Review N +k🤖` | PRs where you are a reviewer; `+k🤖` = bot PRs (Renovate etc.) |

- **Icon color** shows the worst state: red needs attention, green is ready, orange waits for your review.
- **Hover**: counts plus the top items, each with author, approvals, build and age.
- **Click**: a grouped list with tags (author, approvals, build, comments, tasks, age) and a details
  pane (description, reviewers, each build with a link, branches, linked Jira issue, sprint, due date).
  - Jira: *Blocked / In progress / To do*. Actions: open, copy key, copy a `feature/KEY-slug` branch name.
  - My PRs: *Needs attention / Ready to merge / Waiting for approval*.
  - Review: *To review / Approved by you / Bots*.
- **Toasts**: a Windows notification when a new PR asks for your review.

Build state uses only the PR's head commit and the newest run of each pipeline. A stopped run that a
newer run replaced does not count. "Ready to merge" is a heuristic (2 approvals, or all reviewers
approved, nothing blocking), not Bitbucket's merge checks.

Jira refreshes every 2 minutes. The review scan runs every 5 minutes.

## Setup

**Jira Cloud**: run this in your own terminal. The token goes to a hidden prompt and is stored in
Windows Credential Manager (`WorkDock:jira`). The site goes to `~/.work-dock/config.json`.

```powershell
.\set-jira-token.ps1 -Site https://your-company.atlassian.net -Email you@your-company.com
```

Token: <https://id.atlassian.com/manage-profile/security/api-tokens>. Classic, or scoped with
`read:jira-work` and `read:jira-user`. The environment variable `WORKDOCK_JIRA_SITE` overrides the site.

**Bitbucket Cloud**: the widget reads, in this order:

1. Windows Credential Manager, generic credential `WorkDock:bitbucket` (user = Atlassian email,
   password = API token with Bitbucket read scopes).
2. The `bitbucket` MCP server env in `~/.cursor/mcp.json` (`BITBUCKET_USER_EMAIL`, `BITBUCKET_API_TOKEN`).

The review list scans your working set (repos of your recent PRs plus recently updated repos),
not every repo in a large workspace.

## Requirements

- Windows 11, PowerToys with Command Palette and its Dock enabled.
- .NET 10 SDK to build.
- Developer Mode on (simplest), or a self-signed certificate (see below).

## Build and install

```powershell
.\pack.ps1          # build and register the package, restart Command Palette
.\add-to-dock.ps1   # pin the band to the Dock
```

- **Developer Mode on**: `pack.ps1` registers the build folder directly. No signing.
- **Developer Mode off**: run `.\trust-cert.ps1` once (self-signed `CN=WorkDock`, current user only).
  `pack.ps1` then downloads `makeappx`/`signtool` from the `Microsoft.Windows.SDK.BuildTools` NuGet
  package into `tools\`, and packs, signs and installs an MSIX.

`pack.ps1` and `add-to-dock.ps1` stop Command Palette **and all its extension processes**, then start
Command Palette again. Stopping only the UI leaves extensions linked to the dead instance, and their
bands stop updating.

`add-to-dock.ps1 -Section StartBands|CenterBands|EndBands` picks where the band goes. If it is
already on the Dock, it stays where you put it. It backs up `settings.json` first. The Dock shows
an item's subtitle as part of its tooltip, so the script hides subtitles on the bar.

## Diagnostics

```powershell
dotnet build WorkDock\WorkDock.csproj -c Debug -p:Platform=x64 -o out\cli
.\out\cli\WorkDock.exe --work | Out-String
```

Log: `~/.work-dock/workdock.log`.

## See also

[icy-r/claude-dock](https://github.com/icy-r/claude-dock): a Dock widget for Claude Code session status.

## License

[MIT](LICENSE)

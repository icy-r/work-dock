using System;
using System.Security;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace WorkDock;

/// <summary>Windows toast when a new pull request asks for your review.</summary>
internal static class Notifier
{
    private static bool _init;
    private static ToastNotifier? _notifier;

    public static void Init()
    {
        if (_init)
        {
            return;
        }

        _init = true;
        try
        {
            _notifier = ToastNotificationManager.CreateToastNotifier();
        }
        catch (Exception ex)
        {
            Log.Write($"Toasts unavailable: {ex.Message}");
        }

        WorkMonitor.NewReview += p =>
            Show($"New review: {p.RepoName} #{p.Id}", p.Title, $"by {p.Author}");
    }

    public static void Show(string title, string line1, string line2)
    {
        if (_notifier is null)
        {
            return;
        }

        try
        {
            var xml = new XmlDocument();
            xml.LoadXml(
                "<toast><visual><binding template=\"ToastGeneric\">" +
                $"<text>{Esc(title)}</text><text>{Esc(line1)}</text><text placement=\"attribution\">{Esc(line2)}</text>" +
                "</binding></visual></toast>");
            _notifier.Show(new ToastNotification(xml));
        }
        catch (Exception ex)
        {
            Log.Write($"Toast failed: {ex.Message}");
        }
    }

    private static string Esc(string s) => SecurityElement.Escape(s) ?? string.Empty;
}

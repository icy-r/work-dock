using System;
using System.IO;
using System.Linq;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace WorkDock;

internal static class Fmt
{
    public static string Ago(DateTime since) => Span(DateTime.Now - since);

    public static string Ago(DateTimeOffset since) => since == default ? "?" : Span(DateTimeOffset.Now - since);

    public static string Span(TimeSpan t) =>
        t.TotalSeconds < 60 ? $"{Math.Max(0, (int)t.TotalSeconds)}s"
        : t.TotalMinutes < 60 ? $"{(int)t.TotalMinutes}m"
        : t.TotalHours < 24 ? $"{(int)t.TotalHours}h {t.Minutes}m"
        : $"{(int)t.TotalDays}d {t.Hours}h";

    public static string OneLine(string text, int max)
    {
        var t = string.Join(' ', (text ?? string.Empty).Split(['\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries)).Trim();
        return t.Length <= max ? t : t[..(max - 1)].TrimEnd() + "…";
    }

    /// <summary>Quote text as a Markdown block quote, truncated.</summary>
    public static string Quote(string text, int max)
    {
        var t = (text ?? string.Empty).Trim();
        if (t.Length > max)
        {
            t = t[..max].TrimEnd() + " …";
        }

        return string.Join('\n', t.Split('\n').Select(l => "> " + l.TrimEnd()));
    }

    public static Tag Tag(string text, byte r, byte g, byte b, string tooltip = "") =>
        new(text) { Foreground = ColorHelpers.FromRgb(255, 255, 255), Background = ColorHelpers.FromRgb(r, g, b), ToolTip = tooltip };

    public static Tag Plain(string text, string tooltip = "") => new(text) { ToolTip = tooltip };
}

internal static class Log
{
    private static readonly string LogPath = Path.Combine(DataDir.Path, "workdock.log");
    private static readonly object Gate = new();

    public static void Write(string message)
    {
        try
        {
            lock (Gate)
            {
                var fi = new FileInfo(LogPath);
                if (fi.Exists && fi.Length > 1_000_000)
                {
                    fi.Delete();
                }

                File.AppendAllText(LogPath, $"{DateTime.Now:HH:mm:ss.fff} {message}{Environment.NewLine}");
            }
        }
        catch
        {
        }
    }
}

internal static class DataDir
{
    public static readonly string Path = Init();

    private static string Init()
    {
        var p = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".work-dock");
        Directory.CreateDirectory(p);
        return p;
    }
}

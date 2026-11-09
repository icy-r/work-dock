using System;
using System.Linq;
using System.Threading;
using Microsoft.CommandPalette.Extensions;
using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT;

namespace WorkDock;

public static class Program
{
    [MTAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "-RegisterProcessAsComServer")
        {
            global::Shmuelie.WinRTServer.ComServer server = new();
            ManualResetEvent disposed = new(false);
            WorkDockExtension instance = new(disposed);
            server.RegisterClass<WorkDockExtension, IExtension>(() => instance);
            server.Start();
            disposed.WaitOne();
            server.Stop();
            server.UnsafeDispose();
        }
        else if (args.Length > 0 && args[0] == "--work")
        {
            // Diagnostics: one poll of Jira and Bitbucket, without Command Palette.
            var done = new ManualResetEventSlim();
            WorkMonitor.Changed += _ => done.Set();
            WorkMonitor.Refresh();
            done.Wait(TimeSpan.FromSeconds(120));
            var w = WorkMonitor.Current;
            Console.WriteLine($"Jira: {w.JiraError ?? w.Issues.Count + " issues"}");
            foreach (var i in w.Issues.Take(5))
            {
                Console.WriteLine($"  {i.Key} [{i.Status}] {i.Summary}");
            }

            Console.WriteLine($"Bitbucket: {w.BitbucketError ?? $"{w.MyPrs.Count} mine, {w.ToReview.Count} to review"}");
            Console.WriteLine(WorkView.MyPrsHover(w));
            Console.WriteLine(WorkView.ReviewHover(w));
        }
        else if (args.Length > 0 && args[0] == "--toast-test")
        {
            // Needs package identity: Invoke-CommandInDesktopPackage ... -Args "--toast-test"
            Notifier.Init();
            Notifier.Show("Work Dock test", "Toasts work.", "You can dismiss this.");
            Thread.Sleep(1500);
        }
        else
        {
            Console.WriteLine("Not launched as an extension. Use --work.");
        }
    }
}

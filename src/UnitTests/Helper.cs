
using System;
using System.Collections.Generic;
using System.Threading;
using TreeDumpLibrary;
using Windows.System;
using Windows.UI.Xaml;

namespace UnitTests
{
    public static class Helper
    {
        internal static DispatcherQueue Queue { get => (App.Current as UnitTests.App).Queue; }
        internal static T RunOnUIThread<T>(Func<T> func)
        {
            T t = default(T);
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Queue.TryEnqueue(() =>
            {
                t = func();
                manualResetEvent.Set();
            });
            manualResetEvent.WaitOne();
            return t;
        }

        internal static ManualResetEvent RunOnUIThread(Action action)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Queue.TryEnqueue(() => {
                action();
                manualResetEvent.Set();
            });
            return manualResetEvent;
        }

        internal static string GetDump(Func<DependencyObject> creator, IList<string> extraProps = null, IList<AttachedProperty> attachedProperties = null)
        {
            if (extraProps == null)
            {
                extraProps = new List<string>();
            }
            if (attachedProperties == null)
            {
                attachedProperties = new List<AttachedProperty>();
            }
            return RunOnUIThread(() =>
            {
                return TreeDumpLibrary.VisualTreeDumper.DumpTree(creator(), null, extraProps, attachedProperties);
            });
        }

    }
}

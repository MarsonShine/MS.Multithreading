using System;
using System.Threading;
using static System.Console;

namespace MS.Multithreading.Chapter4
{
    public class EAPConvertTask
    {
        internal static int TaskMethod(string name, int seconds)
        {
            WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}", name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            return 42 * seconds;
        }
    }
}

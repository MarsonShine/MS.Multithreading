using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MS.Multithreading.Chapter4
{
    public class APMConvertTask
    {
        internal delegate string AsynchronousTask(string threadName);
        internal delegate string IncompatibleAsynchronousTask(out int threadId);

        public static void CallBack(IAsyncResult ar)
        {
            Console.WriteLine("Starting a callback...");
            Console.WriteLine("State passed to a callback:{0}", ar.AsyncState);
            Console.WriteLine("Is ThreadPool Thread:{0}", Thread.CurrentThread.IsThreadPoolThread);
        }
        public static string Test(string threadName)
        {
            Console.WriteLine("Starting...");
            Console.WriteLine("Is Thread pool thread:{0}", Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Thread.CurrentThread.Name = threadName;
            return string.Format("Thread name:{0}", Thread.CurrentThread.Name);
        } 

        public static string Test(out int threadId)
        {
            Console.WriteLine("Starting...");
            Console.WriteLine("Is ThreadPool Thread:{0}", Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            threadId = Thread.CurrentThread.ManagedThreadId;
            return string.Format("Thread pool worker thread id was:{0}", threadId);
        }
    }
}

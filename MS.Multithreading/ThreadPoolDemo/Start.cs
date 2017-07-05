using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MS.Multithreading.ThreadPoolDemo
{
    public class Start
    {
        public delegate string RunOnThreadPool(out int threadId);

        public static void CallBack(IAsyncResult ar)
        {
            Console.WriteLine("Starting a callback...");
            Console.WriteLine($"State Passed to a callback:{ar.AsyncState}");
            Console.WriteLine($"Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Console.WriteLine($"Thread pool worker thread is:{Thread.CurrentThread.ManagedThreadId}");
        }

        public static string Test(out int threadId)
        {
            Console.WriteLine("Starting...");
            Console.WriteLine($"Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            threadId = Thread.CurrentThread.ManagedThreadId;
            return $"Thread pool worker thread id was:{threadId}";
        }

        public static void UseThreads(int numberOfOperations)
        {
            using (var countDown = new CountdownEvent(numberOfOperations))
            {
                Console.WriteLine("Scheduling work by creating threads");
                for (int i = 0; i < numberOfOperations; i++)
                {
                    var thread = new Thread(() =>
                    {
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId},");
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        countDown.Signal();
                    });
                    thread.Start();
                }
                countDown.Wait();
                Console.WriteLine();
            }
        }

        public static void UseThreadPool(int numberOfOperations)
        {
            using (var countDown = new CountdownEvent(numberOfOperations))
            {
                Console.WriteLine("Starting work on threadpool");
                for (int i = 0; i < numberOfOperations; i++)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId},");
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        countDown.Signal();
                    });
                    countDown.Wait();
                    Console.WriteLine();
                }
            }
        }
    }
}

using MS.Multithreading.BarrierDemo;
using System;
using System.Diagnostics;
using System.Threading;

namespace MS.Multithreading
{
    class Program
    {
        static void Main(string[] args)
        {
            //StartBarrier();
            //StartReaderWriterLockSlim();
            //StartSpinWait();
            //StartThreadPool();
            StartThreadPoolByCountDownEvent();
            Console.ReadLine();
        }

        static void StartBarrier()
        {
            var t1 = new Thread(() =>
            {
                Start.PlayMusic("the guitarist", "play an amazing solo", 5);
            });
            var t2 = new Thread(() =>
            {
                Start.PlayMusic("the singer", "sing his song", 2);
            });
            t1.Start();
            t2.Start();
        }
        static void StartReaderWriterLockSlim()
        {
            new Thread(ReaderWriterLockSlimDemo.Start.Read) { IsBackground = true }.Start();
            new Thread(ReaderWriterLockSlimDemo.Start.Read) { IsBackground = true }.Start();
            new Thread(ReaderWriterLockSlimDemo.Start.Read) { IsBackground = true }.Start();

            new Thread(() => ReaderWriterLockSlimDemo.Start.Write("Thread 1")) { IsBackground = true }.Start();
            new Thread(() => ReaderWriterLockSlimDemo.Start.Write("Thread 2")) { IsBackground = true }.Start();

            Thread.Sleep(TimeSpan.FromSeconds(20));
        }
        static void StartSpinWait()
        {
            var t1 = new Thread(SpinWaitDemo.Start.UserModeWait);
            var t2 = new Thread(SpinWaitDemo.Start.HyBirdSpinWait);

            Console.WriteLine("Running user mode waiting");
            t1.Start();
            Thread.Sleep(20);
            SpinWaitDemo.Start._isCompleted = true;
            Thread.Sleep(TimeSpan.FromSeconds(1));
            SpinWaitDemo.Start._isCompleted = false;
            Console.WriteLine("Running hybird SpinWait construct waiting");
            t2.Start();
            Thread.Sleep(5);
            SpinWaitDemo.Start._isCompleted = true;
        }
        static void StartThreadPool()
        {
            int threadId = 0;
            ThreadPoolDemo.Start.RunOnThreadPool poolDelegate = ThreadPoolDemo.Start.Test;

            var t = new Thread(() => ThreadPoolDemo.Start.Test(out threadId));
            t.Start();
            t.Join();

            Console.WriteLine($"Thread id:{threadId}");

            IAsyncResult r = poolDelegate.BeginInvoke(out threadId, ThreadPoolDemo.Start.CallBack, "a delegate asynchronous call");
            r.AsyncWaitHandle.WaitOne();

            string result = poolDelegate.EndInvoke(out threadId, r);
            Console.WriteLine($"Thread pool worker thread id:{threadId}");
            Console.WriteLine(result);

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
        static void StartThreadPoolByCountDownEvent()
        {
            var numberOfOpertions = 500;
            var sw = new Stopwatch();
            sw.Start();
            ThreadPoolDemo.Start.UseThreads(numberOfOpertions);
            sw.Stop();
            Console.WriteLine($"Execution time using threads:{sw.ElapsedMilliseconds}");

            sw.Restart();
            ThreadPoolDemo.Start.UseThreadPool(numberOfOpertions);
            sw.Stop();
            Console.WriteLine($"Execution time using threads:{sw.ElapsedMilliseconds}");
        }
    }
}

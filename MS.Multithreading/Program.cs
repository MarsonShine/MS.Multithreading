using MS.Multithreading.AsyncAwait;
using MS.Multithreading.BarrierDemo;
using MS.Multithreading.Chapter4;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static MS.Multithreading.Chapter4.APMConvertTask;
using static System.Console;

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
            //StartThreadPoolByCountDownEvent();
            //StartThreadEnableCancellationOperation();
            //startThreadEnableCancellationTimeoutOperation();
            //StartChapter4();
            //StartEAPConvertTask();
            StartAsyncAwaitModel();
            ConCurrentCollection.ConcurrentQueueExample.Startup.RunProgram().Wait();
            ReadLine();
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
        static void StartThreadEnableCancellationOperation()
        {
            using (var cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => CancelOperation.Start.AsyncOperation1(token));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            using (var cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => CancelOperation.Start.AsyncOperation2(token));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            using (var cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => CancelOperation.Start.AsyncOperation3(token));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
        static void startThreadEnableCancellationTimeoutOperation()
        {
            CancelOperation.TimeoutOperation.RunOperation(TimeSpan.FromSeconds(5));
            CancelOperation.TimeoutOperation.RunOperation(TimeSpan.FromSeconds(7));
        }
        static void StartChapter4()
        {
            int threadId;
            AsynchronousTask d = Test;
            IncompatibleAsynchronousTask e = Test;
            Console.WriteLine("Option 1");
            Task<string> task = Task<string>.Factory.FromAsync(d.BeginInvoke("AsyncTaskThread", CallBack, "a delegate asynchronous call"), d.EndInvoke);
            task.ContinueWith(t => Console.WriteLine("Callback is finished,now running acontinuation! Result:{0}", t.Result));
            while (!task.IsCompleted)
            {
                Console.WriteLine(task.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task.Status);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("----------------------------------");
            Console.WriteLine();
            Console.WriteLine("Option 2");

            task = Task<string>.Factory.FromAsync(d.BeginInvoke, d.EndInvoke, "AsyncTaskThread", "a delegate asynchrounous call");
            while (!task.IsCompleted)
            {
                Console.WriteLine(task.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task.Status);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Console.WriteLine("-----------------------------------");
            Console.WriteLine();
            Console.WriteLine("Option 3");

            IAsyncResult ar = e.BeginInvoke(out threadId, CallBack, "a delegate asynchronous call");
            task = Task<string>.Factory.FromAsync(ar, _ => e.EndInvoke(out threadId, ar));
            task.ContinueWith(t => Console.WriteLine("Task is completed,now running a continuation:Result {0},Threadid:{1}", t.Result, threadId));
            while (!task.IsCompleted)
            {
                Console.WriteLine(task.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task.Status);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        static void StartEAPConvertTask() {
            var tcs = new TaskCompletionSource<int>();
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, eventArgs) =>
            {
                eventArgs.Result = EAPConvertTask.TaskMethod("Background worker", 5);
            };
            worker.RunWorkerCompleted += (sender, eventArgs) =>
            {
                if (eventArgs.Error != null)
                {
                    tcs.SetException(eventArgs.Error);
                }
                else if (eventArgs.Cancelled)
                {
                    tcs.SetCanceled();
                }
                else
                {
                    tcs.SetResult((int)eventArgs.Result);
                }
            };
            worker.RunWorkerAsync();
            int result = tcs.Task.Result;
            WriteLine("Result is: {0}", result);
        }
        static void StartAsyncAwaitModel()
        {
            Task t = AsyncAwaiter.AsynchronousProcessing();
            t.Wait();
        }
    }
}

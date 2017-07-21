using System;
using System.Threading;

namespace MS.Multithreading.CancelOperation
{
    public class TimeoutOperation
    {
        public static void RunOperation(TimeSpan workerOperationTimeout)
        {
            using (var evt = new ManualResetEvent(false))
            {
                using (var cts = new CancellationTokenSource())
                {
                    Console.WriteLine("Registing timeout operations...");
                    var worker = ThreadPool.RegisterWaitForSingleObject(evt, (state, isTimeOut) =>
                         WorkerOperationWait(cts, isTimeOut), null, workerOperationTimeout, true);
                    Console.WriteLine("Starting long running operation...");
                    ThreadPool.QueueUserWorkItem(_ => WorkerOperationWait(cts.Token, evt));
                    Thread.Sleep(workerOperationTimeout.Add(TimeSpan.FromSeconds(2)));
                    worker.Unregister(evt);
                }
            }
        }

        private static void WorkerOperationWait(CancellationTokenSource cts, bool isTimeOut)
        {
            if (isTimeOut)
            {
                cts.Cancel();
                Console.WriteLine("Worker operation timed out and was canceled");
            }
            else
            {
                Console.WriteLine("Worker operation succeded.");
            }
        }

        private static void WorkerOperationWait(CancellationToken token, ManualResetEvent evt)
        {
            for (int i = 0; i < 6; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            evt.Set();
        }
    }
}

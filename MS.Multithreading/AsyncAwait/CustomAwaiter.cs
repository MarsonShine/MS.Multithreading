using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MS.Multithreading.AsyncAwait
{
    public class CustomAwaiter : INotifyCompletion
    {
        public CustomAwaiter(bool completeSynchronously)
        {
            _completeSynchronously = completeSynchronously;
        }
        public void OnCompleted(Action continuation)
        {
            ThreadPool.QueueUserWorkItem(state =>
           {
               Thread.Sleep(TimeSpan.FromSeconds(1));
               _result = GetInfo();
               continuation?.Invoke();
           });
        }

        private string GetInfo()
        {
            return $"Task is running on a thread id {Thread.CurrentThread.ManagedThreadId}. Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
        }

        public bool IsCompleted
        {
            get
            {
                return _completeSynchronously;
            }
        }

        public string GetResult()
        {
            return _result;
        }
        private readonly bool _completeSynchronously;
        private string _result = "Completed synchronously";
    }
}
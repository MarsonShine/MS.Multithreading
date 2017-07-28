using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MS.Multithreading.AsyncAwait
{
    public class DynamicAwaitable
    {
        public async Task AsynchronousProcessing()
        {
            string result = await GetDynamicAwaitableObject(true);
            Console.WriteLine(result);

            result = await GetDynamicAwaitableObject(false);
            Console.WriteLine(result);
        }

        private dynamic GetDynamicAwaitableObject(bool completesSynchronously)
        {
            dynamic result = new ExpandoObject();
            dynamic awaiter = new ExpandoObject();

            awaiter.Message = "Completed synchronously";
            awaiter.IsCompleted = completesSynchronously;
            awaiter.GetResult = (Func<string>)(() => awaiter.Message);

            awaiter.OnCompleted = (Action<Action>)(callback =>
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                awaiter.Message = GetInfo();
                callback?.Invoke();
            }));
            IAwaiter<string> proxy = Impromptu.ActLike(awaiter);
            result.GetAwaiter = (Func<dynamic>)(() => proxy);
            return result;
        }

        private string GetInfo()
        {
            return $"Task is running on a thread id {Thread.CurrentThread.ManagedThreadId}. Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
        }
    }

    internal interface IAwaiter<T> : INotifyCompletion
    {
        bool IsCompleted { get; }
        T GetResult();
    }
}

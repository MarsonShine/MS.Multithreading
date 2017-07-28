using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Multithreading.AsyncAwait
{
    public class CustomAwaitable
    {
        public CustomAwaitable(bool completeSynchronously)
        {
            _completeSynchronously = completeSynchronously;
        }

        public CustomAwaiter GetAwaiter()
        {
            return new CustomAwaiter(_completeSynchronously);
        }
        private readonly bool _completeSynchronously;
    }

    public class AsyncAwaiter
    {
        public async static Task AsynchronousProcessing()
        {
            var sync = new CustomAwaitable(true);
            string result = await sync;
            Console.WriteLine(result);

            var async = new CustomAwaitable(false);
            result = await async;
            Console.WriteLine(result);
        }
    }
}

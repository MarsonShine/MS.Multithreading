using System;
using System.Threading;

namespace MS.Multithreading.CancelOperation
{
    public class Start
    {
        public static void AsyncOperation1(CancellationToken token)
        {
            Console.WriteLine("Starting the first task");
            for (int i = 0; i < 5; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("The firsr task has been canceled");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("The first task has completed successfully");
        }

        public static void AsyncOperation2(CancellationToken token)
        {
            try
            {
                Console.WriteLine("Starting the second task");
                for (int i = 0; i < 5; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                Console.WriteLine("The second task has been completed succesfully");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The second task has been canceled");
            }
        }

        public static void AsyncOperation3(CancellationToken token)
        {
            var cancellationFlag = false;
            token.Register(() => cancellationFlag = true);
            Console.WriteLine("Starting the third task");
            for (int i = 0; i < 5; i++)
            {
                if (cancellationFlag)
                {
                    Console.WriteLine("The third task has been canceled.");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("The third task has been completed successfully");
        }
    }
}

using System;
using System.Threading;

namespace MS.Multithreading.SpinWaitDemo
{
    class Start
    {
        public static volatile bool _isCompleted = false;
        public static void UserModeWait()
        {
            while (!_isCompleted)
            {
                Console.WriteLine(".");
            }
            Console.WriteLine();
            Console.WriteLine("waiting is complete");
        }

        public static void HyBirdSpinWait()
        {
            var w = new SpinWait();
            while (!_isCompleted)
            {
                w.SpinOnce();
                Console.WriteLine(w.NextSpinWillYield);
            }
            Console.WriteLine("Waiting is complete");
        }
    }
}

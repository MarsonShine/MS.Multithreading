using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MS.Multithreading.BarrierDemo
{
    public class Start
    {
        static Barrier _barrier = new Barrier(2, b =>
         {
             Console.WriteLine($"End of phase {b.CurrentPhaseNumber + 1}");
         });

        public static void PlayMusic(string name,string message,int seconds)
        {
            for (int i = 1; i < 3; i++)
            {
                Console.WriteLine("---------------------------------");
                Thread.Sleep(TimeSpan.FromSeconds(seconds));
                Console.WriteLine($"{name} start to {message}");
                Thread.Sleep(TimeSpan.FromSeconds(seconds));
                Console.WriteLine($"{name} finishes to {message}");
                _barrier.SignalAndWait();
            }
        }
    }
}

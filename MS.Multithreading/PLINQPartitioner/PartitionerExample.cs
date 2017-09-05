using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MS.Multithreading.PLINQPartitioner.PartitionerExample;
using static System.Console;

namespace MS.Multithreading.PLINQPartitioner
{
    public class PartitionerExample
    {
        public static void PrintInfo(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            WriteLine($"{typeName} type was printed on a thread id {Thread.CurrentThread.ManagedThreadId}");
        }

        public static string EmulateProcessing(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            WriteLine("{0} type was processed on a thread id {1}. Has {2} length.", typeName, Thread.CurrentThread.ManagedThreadId, typeName.Length % 2 == 0 ? "event" : "odd");
            return typeName;
        }

        public static IEnumerable<string> GetTypes()
        {
            var types = AppDomain.CurrentDomain.
                GetAssemblies()
                .SelectMany(a => a.GetExportedTypes());

            return from type in types
                   where type.Name.StartsWith("Web")
                   select type.Name;
        }

        internal class StringPartitioner : Partitioner<string>
        {
            private readonly IEnumerable<string> _data;

            public StringPartitioner(IEnumerable<string> data)
            {
                _data = data;
            }

            public override bool SupportsDynamicPartitions => false;

            public override IList<IEnumerator<string>> GetPartitions(int partitionCount)
            {
                var result = new List<IEnumerator<string>>(2);
                result.Add(CreateEnumerator(true));
                result.Add(CreateEnumerator(false));
                return result;
            }

            private IEnumerator<string> CreateEnumerator(bool isEvent)
            {
                foreach (var d in _data)
                {
                    if (!(d.Length % 2 == 0 ^ isEvent))
                        yield return d;
                }
            }
        }
    }

    public static class Startup
    {
        public static void Start()
        {
            var partitioner = new StringPartitioner(GetTypes());
            var parallelQuery = from t in partitioner.AsParallel()
                                select EmulateProcessing(t);
            parallelQuery.ForAll(PrintInfo);
        }
    }
}

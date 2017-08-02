using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using static System.Console;

namespace MS.Multithreading.ConCurrentCollection
{
    public class BlockingCollectionExample
    {
        public static async Task RunProgram(IProducerConsumerCollection<CustomTask> collection = null)
        {
            var taskCollection = new BlockingCollection<CustomTask>();
            if (null != collection)
                taskCollection = new BlockingCollection<CustomTask>(collection);
            var taskSource = Task.Run(() => TaskProducerAsync(taskCollection));

            Task[] processors = new Task[4];
            for (int i = 1; i <= 4; i++)
            {
                string processorId = "Processor" + i;
                processors[i - 1] = Task.Run(() =>
                {
                    TaskProcessor(taskCollection, processorId);
                });
                await taskSource;
                await Task.WhenAll(processors);
            }
        }

        private static void TaskProcessor(BlockingCollection<CustomTask> taskCollection, string processorId)
        {
            throw new NotImplementedException();
        }

        private static async Task TaskProducerAsync(BlockingCollection<CustomTask> collection)
        {
            for (int i = 1; i <= 20; i++)
            {
                await Task.Delay(20);
                var workItem = new CustomTask { Id = i };
                collection.Add(workItem);
                WriteLine($"Task {workItem} have been posted");
            }
            collection.CompleteAdding();
        }
    }
}

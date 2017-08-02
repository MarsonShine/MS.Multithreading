using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Console;

namespace MS.Multithreading.ConCurrentCollection
{
    public class ConcurrentBagExample
    {
        public static Dictionary<string, string[]> _contentEmulation = new Dictionary<string, string[]>();

        public static async Task RunProgram()
        {
            var bag = new ConcurrentBag<CrawlingTask>();
            string[] urls = new string[]
            {
                "http://microsoft.com",
                "http://google.com",
                "http://twitter.com",
                "http://facebook.com"
            };
            var crawlers = new Task[4];
            for (int i = 0; i < 4; i++)
            {
                string crawlerName = "Crawler " + i.ToString();
                bag.Add(new CrawlingTask(urls[i], "root"));
                crawlers[i] = Task.Run(() => CrawlAsync(bag, crawlerName));
            }
            await Task.WhenAll(crawlers);
        }

        private static async Task CrawlAsync(ConcurrentBag<CrawlingTask> bag, string crawlerName)
        {
            CrawlingTask task;
            while (bag.TryTake(out task))
            {
                IEnumerable<string> urls = await GetLinksFromContentAsync(task);
                if (urls != null)
                {
                    foreach (var url in urls)
                    {
                        var t = new CrawlingTask(url, crawlerName);
                        bag.Add(t);
                    }
                }
                WriteLine($"Indexing url {task.UrlToCrawl} posted by {task.ProducerName} is completed by {crawlerName}");
            }
        }

        public static void CreateLinks()
        {
            _contentEmulation["http://microsoft.com/"] = new[]
            {
                "http://microsoft.com/a.html",
                "http://microsoft.com/b.html"
            };
            _contentEmulation["http://microsoft.com/a.html"] = new[]
            {
                "http://microsoft.com/c.html",
                "http://microsoft.com/d.html"
            };
            _contentEmulation["http://microsoft.com/b.html"] = new[]
            {
                "http://microsoft.com/e.html"
            };
            _contentEmulation["http://google.com/a.html"] = new[]
            {
                "http://google.com/a.html",
                "http://google.com/b.html"
            };
        }

        private static async Task<IEnumerable<string>> GetLinksFromContentAsync(CrawlingTask task)
        {
            await GetRandomDelay();
            if (_contentEmulation.ContainsKey(task.UrlToCrawl))
                return _contentEmulation[task.UrlToCrawl];
            return null;
        }

        private static Task GetRandomDelay()
        {
            int delay = new Random(DateTime.Now.Millisecond).Next(150, 200);
            return Task.Delay(delay);
        }
        
        internal class Startup
        {
            public static void Run()
            {
                CreateLinks();
                Task t = RunProgram();
                t.Wait();
            }
        }
    }
}

namespace MS.Multithreading.ConCurrentCollection
{
    internal class CrawlingTask
    {
        internal CrawlingTask(string urlToCrawl,string producerName)
        {
            ProducerName = producerName;
            UrlToCrawl = urlToCrawl;
        }
        public string UrlToCrawl { get; set; }

        public string ProducerName { get; set; }
    }
}
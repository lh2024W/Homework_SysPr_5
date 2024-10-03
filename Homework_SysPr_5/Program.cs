using System.Net;

namespace Homework_SysPr_5
{
    public class Program
    {
        static void Main()
        {
            List<string> urls = new List<string>()
            {
                "https://learn.microsoft.com/",
                "https://dijix.com.ua"
            };
            int numTasks = Environment.ProcessorCount;
            CancellationTokenSource cts = new CancellationTokenSource();
            Task[] tasks = new Task[numTasks];
            for (int i = 0; i < numTasks; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    while (urls.Count > 0 && !cts.Token.IsCancellationRequested)
                    {
                        string url = null;
                        lock (urls)
                        {
                            if (urls.Count > 0)
                            {
                                url = urls[0];
                                urls.RemoveAt(0);
                            }
                        }
                        if (url != null)
                        {
                            try
                            {
                                Console.WriteLine("Downloading {0}", url);
                                WebClient client = new WebClient();
                                client.DownloadFile(url, $"{new DomianCutter().CleanUrl(url)}.txt");
                                Console.WriteLine("Successfully uploaded - {0}", url);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error downloading {0}: {1}", ex.Message);
                            }
                        }
                    }
                }, cts.Token);
            }

            Console.WriteLine("Press Enter to cancel the operation...");
            Console.ReadLine();

            cts.Cancel();

            Task.WaitAll(tasks);
        }
    }

    public class DomianCutter
    {
        public string CleanUrl(string url)
        {
            string Url = "";
            string CleanUrl = "";
            if (url.Contains("https://"))
            {
                CleanUrl = url.Substring(8);
                Url = "http://" + CleanUrl;
            }
            else if (url.Contains("http://") == false && url.Contains("https://") == false)
            {
                Url = "http://" + url;
            }
            else
            {
                Url = url;
            }

            string cleanurl = Url.Substring(7);
            if (cleanurl.Contains("/"))
            {
                string trimmed = cleanurl.Trim();
                cleanurl = "http://" + trimmed.Substring(0, trimmed.IndexOf('/'));
            }
            else
            {
                return "http://" + cleanurl;
            }
            return cleanurl.Substring(7);
        }
    }
}

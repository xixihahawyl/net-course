using System;
using System.Collections;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleCrawler
{
    class Crawler
    {
        private Hashtable urls = new Hashtable();
        private int count = 0;
        static void Main(string[] args)
        {
            Crawler myClawler = new Crawler();

            string startUrl = "http://www.cnblogs.com/dstang2000/";
            if (args.Length >= 1) startUrl = args[0];

            myClawler.urls.Add(startUrl,false);

            new Thread(myClawler.Crawl).Start(); ;

            
            Console.WriteLine("Hello World!");
        }
        private void Crawl()
        {
            Console.WriteLine("开始了");
            while (true)
            {
                string current = null;
                foreach (string url in urls.Keys)
                {
                    if ((bool)urls[url]) continue;
                    current = url;
                }
                if (current == null || count > 10) break;
                Console.WriteLine("爬行"+current+"页面");

                string html = DownLoad(current);
                urls[current] = true;
                count++;

                Parse(html);
            }
            Console.WriteLine("结束");
        }

        public string DownLoad(string url)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string html = webClient.DownloadString(url);

                string fileName = count.ToString();
                File.WriteAllText(fileName,html,Encoding.UTF8);
                return html;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public void Parse(string html)
        {
            string strRef = @"(href|HREF)[]*=[]*[""'][^""'#>]+[""']";
            //string strRef = @"(href|HREF)[]*=[]*[""'][www.cnblog.com]+[""']";
            MatchCollection matches = new Regex(strRef).Matches(html);
            foreach (Match match in matches)
            {
                strRef = match.Value.Substring(match.Value.IndexOf('=') + 1).Trim('"', '\'', '#', ' ', '>');
                    if (strRef.Length == 0) continue;

                if (urls[strRef] == null) urls[strRef] = false;
            }
        }
    }
}

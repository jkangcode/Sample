using PathToPhilosophy.Lib;
using System;
using System.Collections.Generic;

namespace PathToPhilosophy
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> results = new List<string>();
            WikiPage firstArticle = new WikiPage(args[0]);

            FindPathToPhilosphy(firstArticle, ref results);

            Print(results);
        }

        private static void FindPathToPhilosphy(WikiPage wikiPage, ref List<String> results, bool firstPage = true)
        {
            if (firstPage)
            {
                wikiPage.GoTo();
            }

            if (wikiPage.FindTitle().Equals("Philosophy"))
            {
                return;
            }

            try
            {
                string nextTopic = wikiPage.ClickFirstLink();
                WikiPage nextArticle = new WikiPage(nextTopic);

                results.Add(nextTopic);

                FindPathToPhilosphy(nextArticle, ref results, false);
            }
            catch (NoValidLinksException e)
            {
                results.Add(e.Message);
            }

        }

        private static void Print(List<string> results)
        {
            foreach(string result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}

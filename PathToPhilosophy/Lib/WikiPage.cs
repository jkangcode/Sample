using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PathToPhilosophy.Lib
{
    class WikiPage
    {
        //Variables & Properties
        private static string baseUrl = "https://en.wikipedia.org/wiki/";
        private static IWebDriver driver = new FirefoxDriver();

        private IList<IWebElement> mainContent;

        public string Title { get; private set; }
        public string Url { get; private set; }

        //Constructor
        public WikiPage(string topic)
        {
            this.Url = baseUrl + topic;
        }

        //Methods

        public string ClickFirstLink()
        {
            IWebElement link = FindFirstLink();

            if(link == null)
            {
                return null;
            }

            link.Click();

            return FindTitle();

        }

        private IWebElement FindFirstLink()
        {
            mainContent = driver.FindElements(By.CssSelector("div#mw-content-text p"));
            
            foreach(IWebElement section in mainContent)
            {
                IWebElement link = FindValidLink(section);

                if(link != null)
                {
                    return link;
                }
                else
                {
                    continue;
                }
            }

            throw new NoValidLinksException("No valid links were found");
        }

        private IWebElement FindValidLink(IWebElement section)
        {
            List<string> parentheses = GetParentheses(section);
            IList<IWebElement> links = section.FindElements(By.CssSelector("a"));

            if (!links.Count.Equals(0))
            {
                foreach(IWebElement link in links)
                {
                    if(IsLinkValid(parentheses, link))
                    {
                        return link;
                    }
                }
            }
            else
            {
                return null;
            }

            return null;

        }

        private List<string> GetParentheses(IWebElement section)
        {
            List<string> results = new List<string>();

            string sectionHtml = section.GetAttribute("outerHTML");
            string pattern = "\\(([^\\)]+)\\)";

            Regex r = new Regex(pattern);

            Match m = r.Match(sectionHtml);

            while(m.Success)
            {
                results.Add(m.Value);
                m = m.NextMatch();
            }

            return results;
        }

        private bool IsLinkValid(List<string> parentheses, IWebElement link)
        {
            if (IsCitationLink(link))
            {
                return false;
            }
            else if(parentheses.Count != 0)
            {
                foreach(string html in parentheses)
                {
                    string linkHtml = link.GetAttribute("outerHTML");

                    string pattern = Regex.Escape(linkHtml);

                    Regex r = new Regex(pattern);

                    Match result = r.Match(html);

                    return !result.Success;

                }
            }

            return true;
        }

        private bool IsCitationLink(IWebElement link)
        {
            string href = link.GetAttribute("href");
            
            string pattern = "\\#cite_note-.+";

            Regex r = new Regex(pattern);

            Match result = r.Match(href);

            return result.Success;
        }

        public void GoTo()
        {
            driver.Navigate().GoToUrl(this.Url);
            this.Title = FindTitle();
        }

        public void ClickLink(string linkName)
        {
            driver.FindElement(By.LinkText(linkName)).Click();
        }

        public string FindTitle()
        {
            return driver.FindElement(By.Id("firstHeading")).Text.ToString();
        }
    }

    public class NoValidLinksException : Exception
    {
        public NoValidLinksException(string message) : base(message) { }
    }
}

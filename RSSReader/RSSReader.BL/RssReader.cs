using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RSSReader.BL
{
    public class RssReader
    {
        public string Url { get; set; }

        public RssChannel channel;

        private List<RssArticle> articlesList;

        public RssReader()
        {
            Url = "";
        }

        public List<RssArticle> GetArticles()
        {
            articlesList = new List<RssArticle>();
            LoadData();
            return articlesList;
        }

        private void LoadData()
        {
            WebRequest webRequest = WebRequest.Create(Url);
            webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;

            // Class XmlTextReader provides direct access to Xml data streams (only for reading).
            XmlTextReader xmlReader = new XmlTextReader(webRequest.GetResponse().GetResponseStream());

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlReader);

            ParseXmlDoc(doc);
        }

        private void ParseXmlDoc(XmlDocument doc)
        {
            // XmlNode root contains the root Xml element for the loaded document.
            XmlNode root = doc.DocumentElement;

            // Obtaining a node containing a channel.
            XmlNode currentChannel;
            currentChannel = root.FirstChild;

            GetChannelInfo(currentChannel);
        }

        private void GetChannelInfo(XmlNode currentChannel)
        {
            channel = new RssChannel();

            // Cycle is used to pass through all the channel elements.
            foreach (XmlNode element in currentChannel)
            {
                switch (element.Name)
                {
                    case "title":
                        {
                            // Rss feed name.
                            channel.Title = element.InnerText;
                            break;
                        }
                    case "description":
                        {
                            // Rss feed description.
                            channel.Description = element.InnerText;
                            break;
                        }
                    case "link":
                        {
                            // Rss feed link.
                            channel.Link = element.InnerText;
                            break;
                        }
                    case "item":
                        {
                            XmlNodeList itemInfo = element.ChildNodes;
                            GetArticleInfo(itemInfo);
                            break;
                        }
                }
            }
        }

        private void GetArticleInfo(XmlNodeList itemInfo)
        {
            RssArticle article = new RssArticle();

            // Processing an channel item.
            foreach (XmlNode element in itemInfo)
            {
                switch (element.Name)
                {
                    case "title":
                        {
                            // Article name.
                            article.Title = element.InnerText;
                            break;
                        }
                    case "description":
                        {
                            // Article description (HTML code).
                            article.Description = element.InnerText;
                            break;
                        }
                    case "link":
                        {
                            // Article link.
                            article.Link = element.InnerText;
                            break;
                        }
                    case "pubDate":
                        {
                            // Date of publication of the article.
                            article.PubDate = element.InnerText;
                            break;
                        }
                }
            }
            articlesList.Add(article);
        }
    }
}

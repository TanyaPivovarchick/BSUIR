using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class Service1 : IService1
    {
        Channel channel;
        List<Article> articlesList = new List<Article>();

        public string[] GetFilenames(string url)
        {
            String[] filenames = new String[2];

            LoadData(url);

            filenames[0] = SerializeChannelInfo();
            filenames[1] = SerializeArticlesList();

            return filenames;
        }

        private void LoadData(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
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
            channel = new Channel();

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
            Article article = new Article();

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

        private string GetPathOfFile(string filename)
        {
            string location = AppDomain.CurrentDomain.BaseDirectory;
            string[] split = location.Split(new Char[] { '\\' });
            string path = split[0];
            for (int i = 1; i < split.Length - 2; i++)
            {
                path = path + "\\" + split[i];
            }
            return path + "\\" + filename;
        }

        private string SerializeArticlesList()
        {
            string filename = GetPathOfFile("articles.json");

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Article>));

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                jsonFormatter.WriteObject(fs, articlesList);
            }

            return filename;
        }

        private string SerializeChannelInfo()
        {
            string fileName = GetPathOfFile("channel.json");

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Channel));

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                jsonFormatter.WriteObject(fs, channel);
            }

            return fileName;
        }
    }
}
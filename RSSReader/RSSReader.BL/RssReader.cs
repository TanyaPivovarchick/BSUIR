using RSSReader.BL.ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RSSReader.BL
{
    public class RssReader
    {
        public string Url { get; set; }

        public RssChannel channel;

        Service1Client client;

        private List<RssArticle> articlesList;

        public RssReader()
        {
            Url = "";

            // Create an endpoint.
            Binding binding = new BasicHttpBinding();
            EndpointAddress endpoint = new EndpointAddress("http://localhost:41166/Service1.svc");

            // Create a client.
            client = new Service1Client(binding, endpoint);
        }

        public List<RssArticle> GetArticles()
        {
            articlesList = new List<RssArticle>();

            // filename[0] - name of the file containing information about the channel.
            // filename[1] - name of the file containing articles.
            String[] filenames = client.GetFilenames(Url);

            DeserializeChannelInfo(filenames[0]);
            DeserializeArticlesList(filenames[1]);

            return articlesList;
        }

        private void DeserializeChannelInfo(string filename)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(RssChannel));

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                channel = (RssChannel)jsonFormatter.ReadObject(fs);
            }
        }

        private void DeserializeArticlesList(string filename)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<RssArticle>));

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                articlesList = (List<RssArticle>)jsonFormatter.ReadObject(fs);
            }
        }
    }
}

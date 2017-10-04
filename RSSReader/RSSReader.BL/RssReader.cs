using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.BL
{
    public class RssChannel
    {
        // Rss feed header.
        public string Title { get; set; }

        // Rss feed description.
        public string Description { get; set; }

        // Rss feed link.
        public string Link { get; set; }

        public RssChannel()
        {
            Title = "";
            Description = "";
            Link = "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.BL
{
    public class RssArticle
    {
        // Article header.
        public string Title { get; set; }

        // Article link.
        public string Link { get; set; }

        // Short description of the article.
        public string Description { get; set; }

        // Date of publication of the article.
        public string PubDate { get; set; }

        public RssArticle()
        {
            Title = "";
            Link = "";
            Description = "";
            PubDate = "";
        }
    }
}

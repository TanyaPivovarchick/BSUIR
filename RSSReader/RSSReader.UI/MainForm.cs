using RSSReader.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSSReader.UI
{
    public partial class MainForm : Form
    {
        public RssReader rssReader = new RssReader();

        private List<RssArticle> articlesList;

        public MainForm()
        {
            InitializeComponent();
            webBrowser.ScriptErrorsSuppressed = true;
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            rssReader.Url = textBoxURL.Text;
            articlesList = rssReader.GetArticles();
            GenerateHtml();
            webBrowser.Navigate(Environment.CurrentDirectory + "\\articles.html");
        }

        private void GenerateHtml()
        {
            RssChannel channel = rssReader.channel;

            using (StreamWriter writer = new StreamWriter("articles.html"))
            {
                // Beginning of html-page formation.
                writer.WriteLine(@"<meta http-equiv=""content-type"" content=""text/html; charset=utf-8"">");

                writer.WriteLine("<title>");
                writer.WriteLine(channel.Title);
                writer.WriteLine("</title>");

                // Styles applied to the page.
                writer.WriteLine(@"<style type=""text/css"">");
                writer.WriteLine("a {color: #483D8B; text-decoration: none; font: Verdana;}");
                writer.WriteLine("background-color: #dfe2e5; padding-top: 5pt; padding-left: 5pt;");
                writer.WriteLine("padding-bottom: 5pt; padding-right: 5pt;");
                writer.WriteLine("</style>");

                // Output RSS feed header.
                writer.WriteLine(@"<h2 align=""center""><a href=""" + channel.Link + @""">" + channel.Title + "</a></h2>");

                // Output RSS feed description.
                writer.WriteLine(@"<h3 align=""center"">" + channel.Description + "</h3>");

                int countItems = 0;

                foreach (RssArticle article in articlesList)
                {
                    // Insert article headers into listBox.
                    countItems++;
                    listBox.Items.Add(countItems + ". " + article.Title);

                    // Output article header and date of publication of the article.
                    writer.WriteLine(@"<a href=""" + article.Link + @"""><b>" + countItems + ". " + article.Title + "</b></a>");
                    try
                    {
                        DateTime convertedDate = DateTime.Parse(article.PubDate);
                        writer.WriteLine(" (" + convertedDate.ToShortDateString() + " " + convertedDate.ToShortTimeString() + ")");
                    }
                    catch
                    {
                        writer.WriteLine(" (" + article.PubDate + ")");
                    }

                    // Output article description.
                    writer.WriteLine(@"<div align=""justify""><font color=""black"">");
                    writer.WriteLine(article.Description);
                    writer.WriteLine("</font><div>");

                    // Insert article link.
                    writer.WriteLine(@"<a href=""" + article.Link + @""">");
                    writer.WriteLine("Читать дальше >>> </a>");
                    writer.WriteLine("<br><br>");
                }
            }
        }
    }
}

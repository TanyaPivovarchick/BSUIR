using RSSReader.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSSReader.UI
{
    public partial class MainForm : Form
    {
        public RssReader rssReader = new RssReader();

        private List<RssArticle> articlesList;

        private Boolean isFirstTime = true, isMainPage = true;

        private const int refreshRate = 5000;

        public MainForm()
        {
            InitializeComponent();
            webBrowser.ScriptErrorsSuppressed = true;
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            // Get RSS feed link.
            rssReader.Url = textBoxURL.Text;

            isFirstTime = true;
            labelTime.Visible = true;

            UpdateData();
        }

        private async void UpdateData()
        {
            try
            {
                while (true)
                {
                    Func<List<RssArticle>> UpdateArticles = () =>
                    {
                        if (!isFirstTime)
                        {
                            Thread.Sleep(refreshRate);
                        }
                        isFirstTime = false;
                        return rssReader.GetArticles();
                    };

                    // Update articlesList.
                    articlesList = await Task<List<RssArticle>>.Factory.StartNew(UpdateArticles);

                    // Update listBox.
                    listBox.Items.Clear();
                    int countItems = 0;
                    foreach (RssArticle article in articlesList)
                    {
                        // Insert article headers into listBox.
                        countItems++;
                        listBox.Items.Add(countItems + ". " + article.Title);
                    }

                    // Update webBrowser.
                    if (isMainPage)
                    {
                        GenerateHtml(articlesList);
                        webBrowser.Navigate(Environment.CurrentDirectory + "\\articles.html");
                    }

                    // Display last update time.
                    labelTime.Text = "Последнее обновление: " + DateTime.Now.ToLongTimeString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GenerateHtml(List<RssArticle> articlesList)
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
                    countItems++;

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

        private void buttonMain_Click(object sender, EventArgs e)
        {
            // Output main page.
            GenerateHtml(articlesList);
            webBrowser.Navigate(Environment.CurrentDirectory + "\\articles.html");

            isMainPage = true;
            buttonMain.Visible = false;
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            // Download main page.
            if (e.Url.Scheme == "file")
            {
                isMainPage = true;
                buttonMain.Visible = false;
            }
            // Following a link.
            else
            {
                isMainPage = false;
                buttonMain.Visible = true;
            }
        }

        private async void buttonEmail_Click(object sender, EventArgs e)
        {
            string emailAddress = textBoxEmail.Text;

            // Verifies that a string is in valid email format.
            if (Regex.IsMatch(emailAddress, 
                @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" + 
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$"))
            {
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.ToEmail = emailAddress;
                await emailMessage.SendEmailAsync();
            }
            else
            {
                MessageBox.Show("Некорректный адрес электронной почты. Пожалуйста, проверьте введённый адрес.");
            }
        }
    }
}

using RSSReader.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSSReader.UI
{
    public partial class MainForm : Form
    {
        public RssReader rssReader = new RssReader();

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            rssReader.Url = textBoxURL.Text;
            rssReader.GetArticles();
        }
    }
}

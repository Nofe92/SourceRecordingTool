using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public partial class UpdateForm : Form
    {
        public UpdateForm(string changelogURL)
        {
            InitializeComponent();
            HttpWebRequest request = WebRequest.CreateHttp(changelogURL);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            richTextBox.LoadFile(responseStream, RichTextBoxStreamType.PlainText);
            responseStream.Close();
            response.Close();
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            FileSystem.Open(e.LinkText);
        }
    }
}

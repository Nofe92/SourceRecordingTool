using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public partial class DownloadFileForm : Form
    {
        private string url;
        private string fileName;

        public DownloadFileForm()
        {
            InitializeComponent();
        }

        public static void Start(string url, string fileName)
        {
            DownloadFileForm downloadFileForm = new DownloadFileForm();
            downloadFileForm.url = url;
            downloadFileForm.fileName = fileName;
            downloadFileForm.Text = "Downloading " + fileName;
            downloadFileForm.ShowDialog();
        }

        protected override void OnShown(EventArgs e)
        {
            Thread downloadThread = new Thread(DownloadFile);
            downloadThread.Start();
        }

        public void DownloadFile()
        {
            SetText("Sending HttpWebRequest...");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            SetText("Getting HttpWebResponse...");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            SetText("Creating File...");
            FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            SetText("Downloading...");

            int current = 0;
            int total = (int)response.ContentLength;
            SetMaxProgress(total);
            
            int size;
            byte[] buffer = new byte[ushort.MaxValue];

            while ((size = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, size);
                current += size;

                SetProgress(current);
                SetText(String.Format("Downloading {0:F0} KB/{1:F0} KB ({2:P2})", current / 1024d, total / 1024d, (double)current / (double)total));
            }

            fileStream.Close();
            responseStream.Close();
            response.Close();

            DialogResult = DialogResult.OK;
        }

        public void SetText(string text)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() =>
                {
                    infoLabel.Text = text;
                }));
        }

        public void SetProgress(int value)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() =>
                {
                    progressBar.Value = value;
                }));
        }

        public void SetMaxProgress(int value)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() =>
                {
                    progressBar.Maximum = value;
                }));
        }
    }
}

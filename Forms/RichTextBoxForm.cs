using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public partial class RichTextBoxForm : Form
    {
        public RichTextBoxForm(string title, string text, bool okOnly = false, bool keyValue = false)
        {
            InitializeComponent();
            this.Text = title;

            if (keyValue)
                this.textRichTextBox.SelectionTabs = new int[] { 50, 300 };

            this.textRichTextBox.Text = text;

            if (okOnly)
            {
                okButton.Location = cancelButton.Location;
                cancelButton.Visible = false;
            }

            okButton.Select();
        }

        private void textRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            FileSystem.Open(e.LinkText);
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public partial class SkyboxForm : Form
    {
        public SkyboxForm()
        {
            InitializeComponent();

            PictureBox defaultSkybox = new PictureBox();
            defaultSkybox.Size = new Size(600, 150);
            defaultSkybox.Image = Properties.Resources.defaultskybox;
            defaultSkybox.Margin = new Padding(0);
            defaultSkybox.Click += defaultSkybox_Click;

            FlowLayoutPanel.Controls.Add(defaultSkybox);
        }

        private void defaultSkybox_Click(object sender, EventArgs e)
        {
            MainForm.CurrentProfile.Skyname = "";
            Close();
        }

        public void PictureBox_Click(object sender, EventArgs e)
        {
            MainForm.CurrentProfile.Skyname = ((SRTSkybox)((PictureBox)sender).Tag).Name;
            Close();
        }
    }
}

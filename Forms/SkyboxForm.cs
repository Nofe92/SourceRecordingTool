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

            flowLayoutPanel.SuspendLayout();
            flowLayoutPanel.Controls.Add(defaultSkybox);
            
            foreach (SRTSkybox skybox in SRTSkybox.Skyboxes)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Size = new Size(600, 150);
                pictureBox.Image = skybox.PreviewImage;
                pictureBox.Margin = new Padding(0);
                pictureBox.Tag = skybox;
                pictureBox.Click += pictureBox_Click;
                flowLayoutPanel.Controls.Add(pictureBox);
            }

            flowLayoutPanel.ResumeLayout(false);
        }

        private void defaultSkybox_Click(object sender, EventArgs e)
        {
            MainForm.CurrentProfile.Skyname = "";
            Close();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            MainForm.CurrentProfile.Skyname = ((SRTSkybox)((PictureBox)sender).Tag).Name;
            Close();
        }
    }
}

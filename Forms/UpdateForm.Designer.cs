namespace SourceRecordingTool
{
    partial class UpdateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NoButton = new System.Windows.Forms.Button();
            this.YesButton = new System.Windows.Forms.Button();
            this.RichTextBox = new System.Windows.Forms.RichTextBox();
            this.HeadlineLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // NoButton
            // 
            this.NoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NoButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.NoButton.Location = new System.Drawing.Point(422, 519);
            this.NoButton.Name = "NoButton";
            this.NoButton.Size = new System.Drawing.Size(150, 30);
            this.NoButton.TabIndex = 3;
            this.NoButton.Text = "No";
            this.NoButton.UseVisualStyleBackColor = true;
            // 
            // YesButton
            // 
            this.YesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.YesButton.Location = new System.Drawing.Point(266, 519);
            this.YesButton.Name = "YesButton";
            this.YesButton.Size = new System.Drawing.Size(150, 30);
            this.YesButton.TabIndex = 2;
            this.YesButton.Text = "Yes";
            this.YesButton.UseVisualStyleBackColor = true;
            // 
            // RichTextBox
            // 
            this.RichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RichTextBox.Location = new System.Drawing.Point(12, 62);
            this.RichTextBox.Name = "RichTextBox";
            this.RichTextBox.ReadOnly = true;
            this.RichTextBox.Size = new System.Drawing.Size(560, 451);
            this.RichTextBox.TabIndex = 1;
            this.RichTextBox.Text = "";
            this.RichTextBox.WordWrap = false;
            this.RichTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox_LinkClicked);
            // 
            // HeadlineLabel
            // 
            this.HeadlineLabel.Location = new System.Drawing.Point(12, 9);
            this.HeadlineLabel.Name = "HeadlineLabel";
            this.HeadlineLabel.Size = new System.Drawing.Size(560, 50);
            this.HeadlineLabel.TabIndex = 0;
            this.HeadlineLabel.Text = "A new version is available! Would you like to download it?\r\n\r\nChangelog:";
            this.HeadlineLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.HeadlineLabel);
            this.Controls.Add(this.NoButton);
            this.Controls.Add(this.YesButton);
            this.Controls.Add(this.RichTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "UpdateForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label HeadlineLabel;
        public System.Windows.Forms.Button NoButton;
        public System.Windows.Forms.Button YesButton;
        public System.Windows.Forms.RichTextBox RichTextBox;
    }
}
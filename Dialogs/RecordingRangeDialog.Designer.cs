namespace SourceRecordingTool
{
    partial class RecordingRangeDialog
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
            this.demoNameLabel = new System.Windows.Forms.Label();
            this.demoNameTextBox = new System.Windows.Forms.TextBox();
            this.startTickLabel = new System.Windows.Forms.Label();
            this.endTickLabel = new System.Windows.Forms.Label();
            this.rangeLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.startNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.endNumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.startNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // demoNameLabel
            // 
            this.demoNameLabel.ForeColor = System.Drawing.Color.Navy;
            this.demoNameLabel.Location = new System.Drawing.Point(12, 49);
            this.demoNameLabel.Name = "demoNameLabel";
            this.demoNameLabel.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            this.demoNameLabel.Size = new System.Drawing.Size(150, 23);
            this.demoNameLabel.TabIndex = 1;
            this.demoNameLabel.Text = "Demo name:";
            this.demoNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // demoNameTextBox
            // 
            this.demoNameTextBox.Location = new System.Drawing.Point(168, 49);
            this.demoNameTextBox.Name = "demoNameTextBox";
            this.demoNameTextBox.ReadOnly = true;
            this.demoNameTextBox.Size = new System.Drawing.Size(304, 23);
            this.demoNameTextBox.TabIndex = 2;
            // 
            // startTickLabel
            // 
            this.startTickLabel.ForeColor = System.Drawing.Color.Navy;
            this.startTickLabel.Location = new System.Drawing.Point(12, 78);
            this.startTickLabel.Name = "startTickLabel";
            this.startTickLabel.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            this.startTickLabel.Size = new System.Drawing.Size(150, 23);
            this.startTickLabel.TabIndex = 3;
            this.startTickLabel.Text = "Start Tick:";
            this.startTickLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // endTickLabel
            // 
            this.endTickLabel.ForeColor = System.Drawing.Color.Navy;
            this.endTickLabel.Location = new System.Drawing.Point(12, 107);
            this.endTickLabel.Name = "endTickLabel";
            this.endTickLabel.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            this.endTickLabel.Size = new System.Drawing.Size(150, 23);
            this.endTickLabel.TabIndex = 5;
            this.endTickLabel.Text = "End Tick:";
            this.endTickLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rangeLabel
            // 
            this.rangeLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangeLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.rangeLabel.Location = new System.Drawing.Point(12, 9);
            this.rangeLabel.Name = "rangeLabel";
            this.rangeLabel.Size = new System.Drawing.Size(456, 40);
            this.rangeLabel.TabIndex = 0;
            this.rangeLabel.Text = "Range";
            this.rangeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(226, 136);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(120, 30);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(352, 136);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(120, 30);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // startNumericUpDown
            // 
            this.startNumericUpDown.Location = new System.Drawing.Point(168, 78);
            this.startNumericUpDown.Name = "startNumericUpDown";
            this.startNumericUpDown.Size = new System.Drawing.Size(304, 23);
            this.startNumericUpDown.TabIndex = 4;
            this.startNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDown_KeyDown);
            // 
            // endNumericUpDown
            // 
            this.endNumericUpDown.Location = new System.Drawing.Point(168, 107);
            this.endNumericUpDown.Name = "endNumericUpDown";
            this.endNumericUpDown.Size = new System.Drawing.Size(304, 23);
            this.endNumericUpDown.TabIndex = 6;
            this.endNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDown_KeyDown);
            // 
            // RecordingRangeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 171);
            this.Controls.Add(this.endNumericUpDown);
            this.Controls.Add(this.startNumericUpDown);
            this.Controls.Add(this.rangeLabel);
            this.Controls.Add(this.demoNameLabel);
            this.Controls.Add(this.demoNameTextBox);
            this.Controls.Add(this.startTickLabel);
            this.Controls.Add(this.endTickLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordingRangeDialog";
            this.ShowIcon = false;
            ((System.ComponentModel.ISupportInitialize)(this.startNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label demoNameLabel;
        private System.Windows.Forms.Label startTickLabel;
        private System.Windows.Forms.Label endTickLabel;
        private System.Windows.Forms.Label rangeLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        public System.Windows.Forms.TextBox demoNameTextBox;
        public System.Windows.Forms.NumericUpDown startNumericUpDown;
        public System.Windows.Forms.NumericUpDown endNumericUpDown;

    }
}
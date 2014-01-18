namespace SourceRecordingTool
{
    partial class VDMForm
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
            this.components = new System.ComponentModel.Container();
            this.directoriesLabel = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.filterLabel = new System.Windows.Forms.Label();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.demosLabel = new System.Windows.Forms.Label();
            this.demosListView = new System.Windows.Forms.ListView();
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.serverNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clientColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mapNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gameDirectoryColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.playbackTimeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ticksColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.demoProtocolColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.networkProtocolColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.demosContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewDemosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rangesLabel = new System.Windows.Forms.Label();
            this.rangesListView = new System.Windows.Forms.ListView();
            this.demoNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.startTickColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.endTickColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rangesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewRangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editRangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRangeButton = new System.Windows.Forms.Button();
            this.editRangeButton = new System.Windows.Forms.Button();
            this.deleteRangeButton = new System.Windows.Forms.Button();
            this.pathBrowseButton = new System.Windows.Forms.Button();
            this.startRecordingButton = new System.Windows.Forms.Button();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.demosContextMenuStrip.SuspendLayout();
            this.rangesContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // directoriesLabel
            // 
            this.directoriesLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directoriesLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.directoriesLabel.Location = new System.Drawing.Point(12, 9);
            this.directoriesLabel.Name = "directoriesLabel";
            this.directoriesLabel.Size = new System.Drawing.Size(960, 40);
            this.directoriesLabel.TabIndex = 0;
            this.directoriesLabel.Text = "Directories";
            this.directoriesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pathLabel
            // 
            this.pathLabel.ForeColor = System.Drawing.Color.Navy;
            this.pathLabel.Location = new System.Drawing.Point(12, 52);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            this.pathLabel.Size = new System.Drawing.Size(150, 23);
            this.pathLabel.TabIndex = 1;
            this.pathLabel.Text = "Path:";
            this.pathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // filterLabel
            // 
            this.filterLabel.ForeColor = System.Drawing.Color.Navy;
            this.filterLabel.Location = new System.Drawing.Point(12, 81);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            this.filterLabel.Size = new System.Drawing.Size(150, 23);
            this.filterLabel.TabIndex = 4;
            this.filterLabel.Text = "Filter:";
            this.filterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pathTextBox
            // 
            this.pathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathTextBox.Location = new System.Drawing.Point(168, 52);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.ReadOnly = true;
            this.pathTextBox.Size = new System.Drawing.Size(774, 23);
            this.pathTextBox.TabIndex = 2;
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTextBox.Location = new System.Drawing.Point(168, 81);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(804, 23);
            this.filterTextBox.TabIndex = 5;
            this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);
            // 
            // demosLabel
            // 
            this.demosLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.demosLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.demosLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.demosLabel.Location = new System.Drawing.Point(12, 107);
            this.demosLabel.Name = "demosLabel";
            this.demosLabel.Size = new System.Drawing.Size(960, 40);
            this.demosLabel.TabIndex = 6;
            this.demosLabel.Text = "Available Demos";
            this.demosLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // demosListView
            // 
            this.demosListView.AllowColumnReorder = true;
            this.demosListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.demosListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.serverNameColumnHeader,
            this.clientColumnHeader,
            this.mapNameColumnHeader,
            this.gameDirectoryColumnHeader,
            this.playbackTimeColumnHeader,
            this.ticksColumnHeader,
            this.demoProtocolColumnHeader,
            this.networkProtocolColumnHeader});
            this.demosListView.ContextMenuStrip = this.demosContextMenuStrip;
            this.demosListView.FullRowSelect = true;
            this.demosListView.HideSelection = false;
            this.demosListView.Location = new System.Drawing.Point(12, 150);
            this.demosListView.Name = "demosListView";
            this.demosListView.Size = new System.Drawing.Size(960, 200);
            this.demosListView.TabIndex = 7;
            this.demosListView.UseCompatibleStateImageBehavior = false;
            this.demosListView.View = System.Windows.Forms.View.Details;
            this.demosListView.VirtualMode = true;
            this.demosListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.demosListView_ColumnClick);
            this.demosListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.demosListView_RetrieveVirtualItem);
            this.demosListView.SelectedIndexChanged += new System.EventHandler(this.demosListView_SelectedIndexChanged);
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "Demo name";
            this.nameColumnHeader.Width = 300;
            // 
            // serverNameColumnHeader
            // 
            this.serverNameColumnHeader.Text = "Server name";
            this.serverNameColumnHeader.Width = 150;
            // 
            // clientColumnHeader
            // 
            this.clientColumnHeader.Text = "Client name";
            this.clientColumnHeader.Width = 100;
            // 
            // mapNameColumnHeader
            // 
            this.mapNameColumnHeader.Text = "Map name";
            this.mapNameColumnHeader.Width = 150;
            // 
            // gameDirectoryColumnHeader
            // 
            this.gameDirectoryColumnHeader.Text = "Game directory";
            this.gameDirectoryColumnHeader.Width = 100;
            // 
            // playbackTimeColumnHeader
            // 
            this.playbackTimeColumnHeader.Text = "Playback time";
            this.playbackTimeColumnHeader.Width = 100;
            // 
            // ticksColumnHeader
            // 
            this.ticksColumnHeader.Text = "Ticks";
            this.ticksColumnHeader.Width = 100;
            // 
            // demoProtocolColumnHeader
            // 
            this.demoProtocolColumnHeader.Text = "Demo Protocol";
            this.demoProtocolColumnHeader.Width = 100;
            // 
            // networkProtocolColumnHeader
            // 
            this.networkProtocolColumnHeader.Text = "Network Protocol";
            this.networkProtocolColumnHeader.Width = 100;
            // 
            // demosContextMenuStrip
            // 
            this.demosContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewDemosToolStripMenuItem});
            this.demosContextMenuStrip.Name = "demoContextMenuStrip";
            this.demosContextMenuStrip.Size = new System.Drawing.Size(100, 26);
            this.demosContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.demosContextMenuStrip_Opening);
            // 
            // viewDemosToolStripMenuItem
            // 
            this.viewDemosToolStripMenuItem.Name = "viewDemosToolStripMenuItem";
            this.viewDemosToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.viewDemosToolStripMenuItem.Text = "View";
            this.viewDemosToolStripMenuItem.Click += new System.EventHandler(this.viewDemosToolStripMenuItem_Click);
            // 
            // rangesLabel
            // 
            this.rangesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rangesLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangesLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.rangesLabel.Location = new System.Drawing.Point(12, 353);
            this.rangesLabel.Name = "rangesLabel";
            this.rangesLabel.Size = new System.Drawing.Size(960, 40);
            this.rangesLabel.TabIndex = 8;
            this.rangesLabel.Text = "Recording Ranges";
            this.rangesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rangesListView
            // 
            this.rangesListView.AllowColumnReorder = true;
            this.rangesListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rangesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.demoNameColumnHeader,
            this.startTickColumnHeader,
            this.endTickColumnHeader});
            this.rangesListView.ContextMenuStrip = this.rangesContextMenuStrip;
            this.rangesListView.FullRowSelect = true;
            this.rangesListView.Location = new System.Drawing.Point(12, 396);
            this.rangesListView.Name = "rangesListView";
            this.rangesListView.Size = new System.Drawing.Size(754, 200);
            this.rangesListView.TabIndex = 9;
            this.rangesListView.UseCompatibleStateImageBehavior = false;
            this.rangesListView.View = System.Windows.Forms.View.Details;
            this.rangesListView.VirtualMode = true;
            this.rangesListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.rangesListView_RetrieveVirtualItem);
            this.rangesListView.SelectedIndexChanged += new System.EventHandler(this.rangesListView_SelectedIndexChanged);
            // 
            // demoNameColumnHeader
            // 
            this.demoNameColumnHeader.Text = "Demo name";
            this.demoNameColumnHeader.Width = 300;
            // 
            // startTickColumnHeader
            // 
            this.startTickColumnHeader.Text = "Start Tick";
            this.startTickColumnHeader.Width = 100;
            // 
            // endTickColumnHeader
            // 
            this.endTickColumnHeader.Text = "End Tick";
            this.endTickColumnHeader.Width = 100;
            // 
            // rangesContextMenuStrip
            // 
            this.rangesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewRangesToolStripMenuItem,
            this.editRangesToolStripMenuItem,
            this.deleteRangesToolStripMenuItem});
            this.rangesContextMenuStrip.Name = "rangeContextMenuStrip";
            this.rangesContextMenuStrip.Size = new System.Drawing.Size(153, 92);
            this.rangesContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.rangesContextMenuStrip_Opening);
            // 
            // viewRangesToolStripMenuItem
            // 
            this.viewRangesToolStripMenuItem.Name = "viewRangesToolStripMenuItem";
            this.viewRangesToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.viewRangesToolStripMenuItem.Text = "View";
            this.viewRangesToolStripMenuItem.Click += new System.EventHandler(this.viewRangesToolStripMenuItem_Click);
            // 
            // editRangesToolStripMenuItem
            // 
            this.editRangesToolStripMenuItem.Name = "editRangesToolStripMenuItem";
            this.editRangesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editRangesToolStripMenuItem.Text = "Edit";
            this.editRangesToolStripMenuItem.Click += new System.EventHandler(this.editRangesToolStripMenuItem_Click);
            // 
            // deleteRangesToolStripMenuItem
            // 
            this.deleteRangesToolStripMenuItem.Name = "deleteRangesToolStripMenuItem";
            this.deleteRangesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteRangesToolStripMenuItem.Text = "Delete";
            this.deleteRangesToolStripMenuItem.Click += new System.EventHandler(this.deleteRangesToolStripMenuItem_Click);
            // 
            // addRangeButton
            // 
            this.addRangeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addRangeButton.Location = new System.Drawing.Point(772, 396);
            this.addRangeButton.Name = "addRangeButton";
            this.addRangeButton.Size = new System.Drawing.Size(200, 30);
            this.addRangeButton.TabIndex = 10;
            this.addRangeButton.Text = "Add Recording Range...";
            this.addRangeButton.UseVisualStyleBackColor = true;
            this.addRangeButton.Click += new System.EventHandler(this.addRangeButton_Click);
            // 
            // editRangeButton
            // 
            this.editRangeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.editRangeButton.Enabled = false;
            this.editRangeButton.Location = new System.Drawing.Point(772, 432);
            this.editRangeButton.Name = "editRangeButton";
            this.editRangeButton.Size = new System.Drawing.Size(200, 30);
            this.editRangeButton.TabIndex = 11;
            this.editRangeButton.Text = "Edit Recording Range...";
            this.editRangeButton.UseVisualStyleBackColor = true;
            this.editRangeButton.Click += new System.EventHandler(this.editRangeButton_Click);
            // 
            // deleteRangeButton
            // 
            this.deleteRangeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteRangeButton.Enabled = false;
            this.deleteRangeButton.Location = new System.Drawing.Point(772, 468);
            this.deleteRangeButton.Name = "deleteRangeButton";
            this.deleteRangeButton.Size = new System.Drawing.Size(200, 30);
            this.deleteRangeButton.TabIndex = 12;
            this.deleteRangeButton.Text = "Delete Recording Range";
            this.deleteRangeButton.UseVisualStyleBackColor = true;
            this.deleteRangeButton.Click += new System.EventHandler(this.deleteRangeButton_Click);
            // 
            // pathBrowseButton
            // 
            this.pathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pathBrowseButton.Location = new System.Drawing.Point(948, 52);
            this.pathBrowseButton.Name = "pathBrowseButton";
            this.pathBrowseButton.Size = new System.Drawing.Size(24, 23);
            this.pathBrowseButton.TabIndex = 3;
            this.pathBrowseButton.Text = "...";
            this.pathBrowseButton.UseVisualStyleBackColor = true;
            this.pathBrowseButton.Click += new System.EventHandler(this.pathBrowseButton_Click);
            // 
            // startRecordingButton
            // 
            this.startRecordingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startRecordingButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startRecordingButton.Location = new System.Drawing.Point(772, 548);
            this.startRecordingButton.Name = "startRecordingButton";
            this.startRecordingButton.Size = new System.Drawing.Size(200, 47);
            this.startRecordingButton.TabIndex = 13;
            this.startRecordingButton.Text = "Start Recording";
            this.startRecordingButton.Click += new System.EventHandler(this.startRecordingButton_Click);
            // 
            // modeComboBox
            // 
            this.modeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Items.AddRange(new object[] {
            "TGA-Sequence",
            "MP4-Video"});
            this.modeComboBox.Location = new System.Drawing.Point(772, 519);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(200, 23);
            this.modeComboBox.TabIndex = 14;
            // 
            // VDMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 607);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.startRecordingButton);
            this.Controls.Add(this.directoriesLabel);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.pathBrowseButton);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.filterTextBox);
            this.Controls.Add(this.demosLabel);
            this.Controls.Add(this.demosListView);
            this.Controls.Add(this.rangesLabel);
            this.Controls.Add(this.rangesListView);
            this.Controls.Add(this.editRangeButton);
            this.Controls.Add(this.addRangeButton);
            this.Controls.Add(this.deleteRangeButton);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.Name = "VDMForm";
            this.ShowIcon = false;
            this.Text = "Scheduled Recording";
            this.demosContextMenuStrip.ResumeLayout(false);
            this.rangesContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label directoriesLabel;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Label filterLabel;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Label demosLabel;
        private System.Windows.Forms.ListView demosListView;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ColumnHeader serverNameColumnHeader;
        private System.Windows.Forms.ColumnHeader clientColumnHeader;
        private System.Windows.Forms.ColumnHeader mapNameColumnHeader;
        private System.Windows.Forms.ColumnHeader gameDirectoryColumnHeader;
        private System.Windows.Forms.ColumnHeader playbackTimeColumnHeader;
        private System.Windows.Forms.ColumnHeader ticksColumnHeader;
        private System.Windows.Forms.Label rangesLabel;
        private System.Windows.Forms.ListView rangesListView;
        private System.Windows.Forms.ColumnHeader demoNameColumnHeader;
        private System.Windows.Forms.ColumnHeader startTickColumnHeader;
        private System.Windows.Forms.ColumnHeader endTickColumnHeader;
        private System.Windows.Forms.Button addRangeButton;
        private System.Windows.Forms.Button editRangeButton;
        private System.Windows.Forms.Button deleteRangeButton;
        private System.Windows.Forms.Button pathBrowseButton;
        private System.Windows.Forms.Button startRecordingButton;
        private System.Windows.Forms.ColumnHeader demoProtocolColumnHeader;
        private System.Windows.Forms.ColumnHeader networkProtocolColumnHeader;
        public System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.ContextMenuStrip demosContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewDemosToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip rangesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewRangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editRangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRangesToolStripMenuItem;
    }
}
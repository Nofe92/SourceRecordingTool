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
    public partial class RecordingRangeDialog : Form
    {
        //public int startTick;
        //public int endTick;
        //public int maxTick;

        public RecordingRangeDialog()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            startNumericUpDown.Select();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (startNumericUpDown.Value >= endNumericUpDown.Value)
            {
                Dialogs.Warning("Start tick must not be greater than end tick.");
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void numericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                okButton.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                cancelButton.PerformClick();
        }
    }
}

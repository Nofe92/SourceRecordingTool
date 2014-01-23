using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public static class Dialogs
    {
        public static bool Question(string text)
        {
            return MessageBox.Show(text, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static void Information(string text)
        {
            MessageBox.Show(text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Warning(string text)
        {
            MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Error(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

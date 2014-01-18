using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public class RecordingRange
    {
        public string path;
        public string name;
        public int startTick;
        public int endTick;
        public int maxTick;
        public ListViewItem item;

        public RecordingRange(string path, string name, int startTick, int endTick, int maxTick)
        {
            this.path = path;
            this.name = name;
            this.startTick = startTick;
            this.endTick = endTick;
            this.maxTick = maxTick;
            item = new ListViewItem(new string[] { name, startTick.ToString(), endTick.ToString() });
        }

        public string FullPath
        {
            get { return String.Concat(path, "\\", name); }
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}-{2}]", name, startTick, endTick);
        }
    }
}

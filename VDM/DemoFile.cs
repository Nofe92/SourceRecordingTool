using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRecordingTool
{
    public class DemoFile
    {
        public string Name;
        public string DirectoryName;
        public int DemoProtocol;
        public int NetworkProtocol;
        public string ServerName;
        public string ClientName;
        public string MapName;
        public string GameDirectory;
        public float PlaybackTime;
        public int Ticks;

        public string FullName
        {
            get { return DirectoryName+ "\\"+ Name; }
        }
    }
}

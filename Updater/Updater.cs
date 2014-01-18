using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SourceRecordingTool
{
    public static class Updater
    {
        private static string[] mirrors = new string[]
        {
            "http://hl3mukkel.url.ph/SourceRecordingTool",
            "http://hl3mukkel.bugs3.com/SourceRecordingTool",
        };

        public static void CheckForUpdatesASync()
        {
            Thread updateThread = new Thread(() => CheckForUpdates());
            updateThread.Start();
        }

        public static UpdateState CheckForUpdates()
        {
            System.Collections.IEnumerator mirrorsEnum = mirrors.GetEnumerator();

            while (mirrorsEnum.MoveNext())
            {
                try
                {
                    XDocument version = XDocument.Load((string)mirrorsEnum.Current + "/version.xml");
                    Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    Version latestVersion = Version.Parse(version.Root.Element("Version").Value);

                    if (currentVersion < latestVersion)
                    {
                        string changelog, downloadLink;

                        if (version.Root.Elements("ChangelogEx").Any())
                            changelog = version.Root.Element("ChangelogEx").Value.Replace("%MIRROR%", (string)mirrorsEnum.Current);
                        else
                            changelog = version.Root.Element("Changelog").Value;

                        if (version.Root.Elements("DownloadLinkEx").Any())
                            downloadLink = version.Root.Element("DownloadLinkEx").Value.Replace("%MIRROR%", (string)mirrorsEnum.Current);
                        else
                            downloadLink = version.Root.Element("DownloadLink").Value;

                        using (UpdateForm updateForm = new UpdateForm(changelog))
                        {
                            updateForm.Text = String.Concat("Version ", latestVersion.ToString(), " available!");

                            if (updateForm.ShowDialog() == DialogResult.Yes)
                                FileSystem.Open(downloadLink);
                        }

                        return UpdateState.Update;
                    }

                    return UpdateState.Latest;
                }
                catch
                {
                    continue;
                }
            }

            MessageBox.Show("No Connection to update servers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return UpdateState.NoConnection;
        }

        public enum UpdateState
        {
            Latest,
            Update,
            NoConnection,
        }
    }
}
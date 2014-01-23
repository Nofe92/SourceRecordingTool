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
        public static string[] Mirrors = new string[]
        {
            "http://hl3mukkel.url.ph/SourceRecordingTool",
            "http://hl3mukkel.bugs3.com/SourceRecordingTool",
        };
        public static string Changelog;
        public static string DownloadLink;
        public static Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static Version latestVersion;
        public static void CheckForUpdatesASync()
        {
            Thread updateThread = new Thread(() => CheckForUpdates());
            updateThread.Start();
        }

        public static UpdateState CheckForUpdates()
        {
            System.Collections.IEnumerator mirrorsEnum = Mirrors.GetEnumerator();

            while (mirrorsEnum.MoveNext())
            {
                try
                {
                    XDocument version = XDocument.Load((string)mirrorsEnum.Current + "/version.xml");
                    latestVersion = Version.Parse(version.Root.Element("Version").Value);

                    if (version.Root.Elements("ChangelogEx").Any())
                        Changelog = version.Root.Element("ChangelogEx").Value.Replace("%MIRROR%", (string)mirrorsEnum.Current);
                    else
                        Changelog = version.Root.Element("Changelog").Value;

                    if (version.Root.Elements("DownloadLinkEx").Any())
                        DownloadLink = version.Root.Element("DownloadLinkEx").Value.Replace("%MIRROR%", (string)mirrorsEnum.Current);
                    else
                        DownloadLink = version.Root.Element("DownloadLink").Value;

                    if (currentVersion < latestVersion)
                    {
                        ShowUpdateForm(true);

                        return UpdateState.Update;
                    }

                    return UpdateState.Latest;
                }
                catch
                {
                    continue;
                }
            }

            Dialogs.Error("No Connection to update servers.");
            return UpdateState.NoConnection;
        }

        public static void ShowUpdateForm(bool update)
        {
            if (String.IsNullOrEmpty(Changelog) || latestVersion == null)
            {
                Dialogs.Error("No Connection to update servers.");
                return;
            }

            using (UpdateForm updateForm = new UpdateForm(Changelog))
            {
                if (update)
                {
                    updateForm.Text = "Version " + latestVersion.ToString() + " available!";
                    updateForm.HeadlineLabel.Visible = true;
                    updateForm.RichTextBox.Dock = DockStyle.None;
                    updateForm.YesButton.Visible = true;
                    updateForm.NoButton.Visible = true;
                }
                else
                {
                    updateForm.Text = "Changelog";
                    updateForm.HeadlineLabel.Visible = false;
                    updateForm.RichTextBox.Dock = DockStyle.Fill;
                    updateForm.YesButton.Visible = false;
                    updateForm.NoButton.Visible = false;
                }

                if (updateForm.ShowDialog() == DialogResult.Yes)
                    FileSystem.Open(DownloadLink);
            }
        }

        public enum UpdateState
        {
            Latest,
            Update,
            NoConnection,
        }
    }
}
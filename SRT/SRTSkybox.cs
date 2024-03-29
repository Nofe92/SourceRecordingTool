﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public class SRTSkybox
    {
        public static string[] Sides = new string[] { "lf", "bk", "rt", "ft", "up", "dn" };
        public static List<SRTSkybox> Skyboxes = new List<SRTSkybox>();
        public static SkyboxForm SkyboxForm = new SkyboxForm();
        private static string vtfcmdPath;

        public string Name;
        public string FileName;
        public Image PreviewImage;

        static SRTSkybox()
        {
            vtfcmdPath = "moviefiles\\tools\\VTFLib\\" + (Environment.Is64BitProcess ? "x64" : "x86") + "\\VTFCmd.exe";
        }

        public static SRTSkybox FindSkyboxByName(string name)
        {
            if (name == "")
                return null;

            for (int i = 0; i < Skyboxes.Count; i++)
                if (name == Skyboxes[i].Name)
                    return Skyboxes[i];

            return null;
        }

        public static void AddSkyboxByDirectory(string dir)
        {
            SRTSkybox skybox = new SRTSkybox();
            skybox.Name = Path.GetFileName(dir);

            IEnumerator<string> fileNameEnumerator = Directory.EnumerateFiles(dir, "*up.vmt").GetEnumerator();
            fileNameEnumerator.MoveNext();
            skybox.FileName = fileNameEnumerator.Current.Substring(0, fileNameEnumerator.Current.Length - 6);
            fileNameEnumerator.Dispose();

            string previewFileName = dir + "\\preview.png";

            if (File.Exists(previewFileName))
                skybox.PreviewImage = Image.FromFile(previewFileName);
            else if (File.Exists(vtfcmdPath))
                skybox.PreviewImage = GenerateSkyboxPreview(skybox, previewFileName);
            else if (File.Exists(previewFileName + ".old"))
                skybox.PreviewImage = Image.FromFile(previewFileName + ".old");
            else
                skybox.PreviewImage = GenerateDefaultSkyboxPreview(skybox, previewFileName + ".old");

            Skyboxes.Add(skybox);

            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(600, 150);
            pictureBox.Image = skybox.PreviewImage;
            pictureBox.Margin = new Padding(0);
            pictureBox.Tag = skybox;
            pictureBox.Click += SkyboxForm.PictureBox_Click;

            SkyboxForm.FlowLayoutPanel.Controls.Add(pictureBox);
        }

        private static Image GenerateSkyboxPreview(SRTSkybox skybox, string previewFileName)
        {
            Process vtfcmd = new Process();
            vtfcmd.StartInfo.FileName = vtfcmdPath;
            vtfcmd.StartInfo.Arguments = "-exportformat bmp -file \"" + skybox.GetVTF(0) + "\" -file \"" + skybox.GetVTF(1) + "\" -file \"" + skybox.GetVTF(2) + "\" -file \"" + skybox.GetVTF(3) + "\"";
            vtfcmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            vtfcmd.Start();
            vtfcmd.WaitForExit();

            string lfBmp = skybox.FileName + Sides[0] + ".bmp";
            string bkBmp = skybox.FileName + Sides[1] + ".bmp";
            string rtBmp = skybox.FileName + Sides[2] + ".bmp";
            string ftBmp = skybox.FileName + Sides[3] + ".bmp";

            Image lf = Image.FromFile(lfBmp);
            Image bk = Image.FromFile(bkBmp);
            Image rt = Image.FromFile(rtBmp);
            Image ft = Image.FromFile(ftBmp);

            Image previewFull = new Bitmap(lf.Width + bk.Width + rt.Width + ft.Width, Math.Max(Math.Max(lf.Height, bk.Height), Math.Max(rt.Height, ft.Height)));
            Graphics previewFullGraphics = Graphics.FromImage(previewFull);
            previewFullGraphics.DrawImageUnscaled(lf, 0, -1);
            previewFullGraphics.DrawImageUnscaled(bk, lf.Width, -1);
            previewFullGraphics.DrawImageUnscaled(rt, lf.Width + bk.Width, -1);
            previewFullGraphics.DrawImageUnscaled(ft, lf.Width + bk.Width + rt.Width, -1);
            previewFullGraphics.DrawString(skybox.Name, new Font("Segoe UI Semibold", lf.Height / 8, System.Drawing.FontStyle.Bold), Brushes.White, lf.Width / 8, lf.Height - lf.Height / 16, new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far });
            previewFullGraphics.Dispose();

            lf.Dispose();
            bk.Dispose();
            rt.Dispose();
            ft.Dispose();

            File.Delete(lfBmp);
            File.Delete(bkBmp);
            File.Delete(rtBmp);
            File.Delete(ftBmp);

            Image preview = new Bitmap(600, 150);
            Graphics previewGraphics = Graphics.FromImage(preview);
            previewGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            previewGraphics.DrawImage(previewFull, 0, 0, preview.Width, preview.Height);
            previewGraphics.Dispose();
            previewFull.Dispose();
            preview.Save(previewFileName);

            return preview;
        }

        private static Image GenerateDefaultSkyboxPreview(SRTSkybox skybox, string previewFileName)
        {
            Image preview = new Bitmap(600, 150);
            Graphics previewGraphics = Graphics.FromImage(preview);
            previewGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            previewGraphics.Clear(Color.Black);
            previewGraphics.DrawString(skybox.Name, new Font("Segoe UI Semibold", 150 / 8, System.Drawing.FontStyle.Bold), Brushes.White, 150 / 8, 150 - 150 / 16, new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far });
            previewGraphics.Dispose();
            preview.Save(previewFileName);

            return preview;
        }

        public string GetVMT(int side)
        {
            return FileName + Sides[side] + ".vmt";
        }

        public string GetVTF(int side)
        {
            return FileName + Sides[side] + ".vtf";
        }
    }
}

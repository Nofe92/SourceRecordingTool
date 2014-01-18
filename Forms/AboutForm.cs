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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            this.richTextBox.SelectionTabs = new int[] { 50, 300 };
            this.richTextBox.SelectedText = "Contact\r\n" +
                                            "\tE-Mail:\thl3mukkel@gmail.com\r\n" +
                                            "\tYouTube:\thttp://www.youtube.com/hl2mukkel\r\n" +
                                            "\tProject Site:\thttps://sourceforge.net/p/sourcerecordingtool/\r\n" +
                                            "\r\n" +
                                            "Core\r\n" +
                                            "\tDesign\t- Aron\r\n" +
                                            "\tCode\t- Aron\r\n" +
                                            "\tScripts\t- Aron\r\n" +
                                            "\r\n" +
                                            "Config\r\n" +
                                            "\ttf2-movie.cfg\t- Aron\r\n" +
                                            "\ttf2-play.cfg\t- Aron\r\n" +
                                            "\tgeneric-movie.cfg\t- Aron\r\n" +
                                            "\tgeneric-play.cfg\t- Aron\r\n" +
                                            "\r\n" +
                                            "Custom\r\n" +
                                            "\ttf2-moviehud\t- python\r\n" +
                                            "\ttf2-no_sniper_crosshair\t- Aron\r\n" +
                                            "\ttf2-no_sniper_dot\t- Aron\r\n" +
                                            "\ttf2-pldx_particles\t- Bolty\r\n" +
                                            "\t\r\n" +
                                            "Skybox\r\n" +
                                            "\tDesert\t- komaokc\r\n" +
                                            "\tGalaxy\t- komaokc\r\n" +
                                            "\tSky41\t- komaokc\r\n" +
                                            "\tSky56\t- komaokc\r\n" +
                                            "\r\n" +
                                            "Tools\r\n" +
                                            "\tVTFLib\t- Neil Jedrzejewski & Ryan Gregg\r\n" +
                                            "\tLagarith Lossless Codec\t- Sir_Lagsalot\r\n" +
                                            "\tVirtualDub\t- Avery Lee\r\n" +
                                            "\r\n" +
                                            "Thanks to\r\n" +
                                            "\tValve\t- for making an awesome content delivery system!\r\n" +
                                            "\tKaiza\t- for betatesting\r\n" +
                                            "\r\n" +
                                            "External Sources\r\n" +
                                            "\thttp://en.wikipedia.org/wiki/Category:Source_engine_games\r\n" +
                                            "\thttp://forums.steampowered.com/forums/showthread.php?t=1444946\r\n" +
                                            "\thttp://forums.steampowered.com/forums/showthread.php?t=1445189\r\n" +
                                            "\thttp://nemesis.thewavelength.net/index.php?p=40\r\n" +
                                            "\thttp://teamfortress.tv/forum/thread/5119/1\r\n" +
                                            "\thttp://tf2wiki.net/wiki/Ultra_high_settings\r\n" +
                                            "\thttp://whisper.ausgamers.com/wiki/index.php/Source_Autoexec_Tweaks\r\n" +
                                            "\thttp://wiki.teamfortress.com/wiki/Help:Recording_demos\r\n" +
                                            "\thttp://www.tweakguides.com/HL2_1.html\r\n" +
                                            "\thttp://www.youtube.com/watch?v=1CJkYisfeDs\r\n" +
                                            "\thttps://chrisdown.name/tf2/\r\n" +
                                            "\thttps://developer.valvesoftware.com/wiki/Depth_buffer\r\n" +
                                            "\thttps://developer.valvesoftware.com/wiki/DirectX_Versions\r\n" +
                                            "\thttps://developer.valvesoftware.com/wiki/Mat_wireframe\r\n" +
                                            "\thttps://support.steampowered.com/kb_article.php?ref=7388-QPFN-2491\r\n" +
                                            "\thttps://support.steampowered.com/view.php?ticketref=5436-WPHV-7742";
            this.richTextBox.SelectionStart = 0;
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            FileSystem.Open(e.LinkText);
        }
    }
}

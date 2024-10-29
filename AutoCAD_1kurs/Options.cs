using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AutoCAD_1kurs
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
            button3.Click += v.ChangeColors;
            button4.Click += v.ChangeColors;
            button5.Click += v.ChangeColors;
            button6.Click += v.ChangeColors;
            button7.Click += v.ChangeColors;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (sender==button2) { Close(); return; }
            v.bw = new BinaryWriter(File.Open("options", FileMode.OpenOrCreate));
            v.BackGround = button3.BackColor;
            v.bw.Write(v.BackGround.R); v.bw.Write(v.BackGround.G); v.bw.Write(v.BackGround.B);
            v.Main = button4.BackColor;
            v.bw.Write(v.Main.R); v.bw.Write(v.Main.G); v.bw.Write(v.Main.B);
            v.Help = button5.BackColor;
            v.bw.Write(v.Help.R); v.bw.Write(v.Help.G); v.bw.Write(v.Help.B);
            v.Assistant = button6.BackColor;
            v.bw.Write(v.Assistant.R); v.bw.Write(v.Assistant.G); v.bw.Write(v.Assistant.B);
            v.OsnapColor = button7.BackColor;
            v.bw.Write(v.OsnapColor.R); v.bw.Write(v.OsnapColor.G); v.bw.Write(v.OsnapColor.B);
            v.bw.Write(v.OsnapSize = (byte)trackBar1.Value);
            v.bw.Write(v.MagnetSize = (byte)numericUpDown1.Value);
            v.bw.Write(v.tooltip.Active = checkBox1.Checked);
            v.bw.Write(v.tooltip.IsBalloon = checkBox2.Checked);

            v.bw.Close();


            v.penMain = new Pen(v.Main, 1);
            v.penHelp = new Pen(v.Help, 1);
            v.penAssistant = new Pen(v.Assistant, 1);
            Close();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            Pen p1 = new Pen(button7.BackColor, 1);
            Pen p2 = new Pen(button7.BackColor, 3);

            gr.DrawLine(p1, 69, 0, 69, 139);
            gr.DrawLine(p1, 0, 69, 139, 69);

            gr.DrawRectangle(p2, 69 - trackBar1.Value, 69 - trackBar1.Value, 2 * trackBar1.Value, 2 * trackBar1.Value);
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            
            pictureBox1.Invalidate();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            button3.BackColor = v.BackGround;
            button4.BackColor = v.Main;
            button5.BackColor = v.Help;
            button6.BackColor = v.Assistant;
            button7.BackColor = v.OsnapColor;
            trackBar1.Value = v.OsnapSize;
            numericUpDown1.Value = v.MagnetSize;
            checkBox1.Checked = v.tooltip.Active;
            checkBox2.Checked = v.tooltip.IsBalloon;
        }
    }
}

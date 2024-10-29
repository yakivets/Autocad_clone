using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using d2;
using d3;
using bd2;
using bd3;

namespace AutoCAD_1kurs
{
   public static class v
    {
        public static ColorDialog colordialog = new ColorDialog();
        public static Random rnd = new Random();
        public static ToolTip tooltip = new ToolTip();
        public static byte kod=100, MagnetSize, OsnapSize;
        public static float scale = 1, scaleTMP=1, zoomK = 1.1f;
        public static BinaryReader br;
        public static BinaryWriter bw;
        public static Brush SelectBrushGreen = new SolidBrush(Color.FromArgb(127, 0, 255, 0));
        public static Brush SelectBrushBlue = new SolidBrush(Color.FromArgb(127, 0, 0, 255));
        public static Options f_options = new Options();
        public static GraphicsPath tmpGP = new GraphicsPath();
        public static bool Shift, Ctrl, Alt, Ortho, tmpBool;
        public static Color BackGround, OsnapColor, Main, Help, Assistant,Selected=Color.Red;
        public static Pen penMain, penHelp, penAssistant, penSelected;
        public const double pi = Math.PI / 180;
        public static List<PointF> Ptmp = new List<PointF>();
        public static PointF[] Ptmp1;
        //  public static int;

        public static List<obj> elem = new List<obj>();
        public static List<obj> elemTMP = new List<obj>();
        public static List<int> Select = new List<int>();
        public static List<OsnapPoints> Osnap = new List<OsnapPoints>();
        public class obj
        {
            public byte Type;
            public List<PointF> p = new List<PointF>();
            public GraphicsPath GP = new GraphicsPath();

        }
        public class OsnapPoints
        {
            public byte Type;
            public PointF p = new PointF();
            
        }


        public static void ChangeColors(object sender, EventArgs e)
        {
            if (colordialog.ShowDialog() == DialogResult.OK) (sender as Button).BackColor = colordialog.Color;
            if (sender == f_options.button7)  f_options.pictureBox1.Invalidate(); 
        }
    }
}

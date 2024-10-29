using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using d2;
using System.IO;
using d3;
using bd2;
using bd3;
using Perevirka;

namespace AutoCAD_1kurs
{
    public partial class Form1 : Form
    {
        int i, j, k, a, b,kolp=0;
        PointF p1, p2, p3, p4, p5,phand, pmove, min,max;
        float w, h,dx,dy;
        bool hand;
        Cursor CursorTMP;

        private void R_rect_2P_Click(object sender, EventArgs e)
        {
            v.kod = (r_rect_2P.Checked) ? (byte)2 : (byte)21;
            
        }

        private void R_circ_CR_Click(object sender, EventArgs e)
        {
           v.kod = (r_circ_CR.Checked) ? (byte)3 : (r_circ_2PD.Checked) ? (byte)31 : (byte)32; 

        }

        private void R_poly_out_Click(object sender, EventArgs e)
        {
            v.kod = (r_poly_out.Checked) ? (byte)5 : (byte)51;
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            v.f_options.ShowDialog();
            BackColor = v.BackGround;
            Invalidate();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            ClearPanel3ControlsChecked(); ClearPanel4ControlsChecked();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

       
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {   if (v.kod==0) v.Ptmp.Clear();
                v.Select.Clear(); panel4.Enabled = false; v.elemTMP.Clear();
                v.kod = 100; kolp = 0; Cursor = Cursors.Default;
                PanelVisibleFalse(); ClearPerpendicularsToOsnap();
                ClearPanel3ControlsChecked(); ClearPanel4ControlsChecked();
            }
            if (e.KeyCode== Keys.ShiftKey) { v.Shift = true;  return; }

            Invalidate();
        }

        void ClearPanel3ControlsChecked()
        {
            for (i = 0; i < panel3.Controls.Count; i++) (panel3.Controls[i] as RadioButton).Checked = false;
        }
        void ClearPanel4ControlsChecked()
        {
            for (i = 0; i < panel4.Controls.Count; i++) (panel4.Controls[i] as RadioButton).Checked = false;
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            if (hand) gr.TranslateTransform(pmove.X - phand.X, pmove.Y - phand.Y);

            for (i = 0; i < v.elem.Count; i++)
            {
                if (v.Select.Contains(i)) gr.DrawPath(v.penSelected, v.elem[i].GP);
                else   gr.DrawPath(v.penMain, v.elem[i].GP);
            }

            if (checkBox1.Checked && v.kod < 95) //OSNAP
            {
                for (i = 0; i < v.Osnap.Count; i++)
                {
                    if (Class_d2.dlina(pmove, v.Osnap[i].p) <= v.MagnetSize)
                    {
                        pmove = v.Osnap[i].p; break;
                    }
                }
            }

            if (kolp > 0)
            {
                switch (v.kod)
                {
                    case 0:
                        switch (kolp)
                        {
                            case 1: gr.DrawLine(v.penAssistant, p1, pmove); break;
                            case 2: gr.DrawLine(v.penAssistant, p1, p2); gr.DrawLine(v.penAssistant, p2, pmove);
                                    gr.DrawBezier(v.penMain, p1, p2, p2, pmove);  break;
                            case 3: gr.DrawLine(v.penAssistant, p1, p2); gr.DrawLine(v.penAssistant, p2, p3);
                                    gr.DrawLine(v.penAssistant, p3, pmove);
                                    gr.DrawBezier(v.penMain, p1, p2, p3, pmove); break;
                        }
                        break;
                    case 1: gr.DrawLine(v.penMain, p1, pmove); break;
                    case 2: w = Math.Abs(p1.X - pmove.X); h = Math.Abs(p1.Y - pmove.Y);
                        p3.X = (p1.X < pmove.X) ? p1.X : pmove.X; p3.Y = (p1.Y < pmove.Y) ? p1.Y : pmove.Y;
                        gr.DrawRectangle(v.penMain, p3.X, p3.Y, w, h);
                        break;
                    case 21: w = Math.Abs(p1.X - pmove.X); h = Math.Abs(p1.Y - pmove.Y);
                        p3.X = (p1.X < pmove.X) ? p1.X-w : pmove.X; p3.Y = (p1.Y < pmove.Y) ? p1.Y-h : pmove.Y;
                        gr.DrawRectangle(v.penMain, p3.X, p3.Y, w*=2, h*=2);
                        break;
                    case 3:  w = (float)Math.Sqrt(Math.Pow(p1.X - pmove.X,2)+ Math.Pow(p1.Y - pmove.Y, 2)); 
                        gr.DrawEllipse(v.penMain, p1.X-w, p1.Y-w, w * 2, w * 2);
                        gr.DrawLine(v.penAssistant, p1, pmove);
                        break;
                    case 31: p5=Class_d2.center_otr(p1, pmove); w = Class_d2.dlina(p1, pmove)/2; 
                        gr.DrawEllipse(v.penMain, p5.X - w, p5.Y - w, w * 2, w * 2);
                        gr.DrawLine(v.penAssistant, p1, pmove);
                        break;
                    case 32: if (kolp == 1)
                        {
                            p5 = Class_d2.center_otr(p1, pmove); w = Class_d2.dlina(p1, pmove) / 2;
                            gr.DrawEllipse(v.penMain, p5.X - w, p5.Y - w, w * 2, w * 2);
                            gr.DrawLine(v.penAssistant, p1, pmove);
                        } else { float[] ccc = Class_d2.circle_3point(p1, p2, pmove);
                            w = ccc[2]; p5 = new PointF(ccc[0], ccc[1]);
                            gr.DrawEllipse(v.penMain, p5.X - w, p5.Y - w, w * 2, w * 2);
                            gr.DrawLine(v.penAssistant, p1, pmove); gr.DrawLine(v.penAssistant, p1, p2); gr.DrawLine(v.penAssistant, p2, pmove);
                        }
                        break;
                    case 5:  case 51: 
                        v.Ptmp1 = Class_d2.polygon_points(p1, (int)n_poly.Value, pmove, (v.kod == 51) ? true : false);
                        gr.DrawPolygon(v.penMain, v.Ptmp1);
                        gr.DrawLine(v.penAssistant, p1, pmove);
                        break;

                    case 11: case 12: dx = pmove.X - p1.X; dy = pmove.Y - p1.Y;
                        gr.DrawLine(v.penAssistant, p1, pmove);
                        gr.TranslateTransform(dx, dy);
                        for (i = 0; i < v.elemTMP.Count; i++)
                        {
                            gr.DrawPath(v.penHelp, v.elemTMP[i].GP);
                        }
                        gr.ResetTransform();
                        break;
                    case 16:
                        gr.DrawLine(v.penAssistant, p1, pmove);
                        for (i = 0; i < v.elemTMP.Count; i++)
                        {
                            gr.DrawPath(v.penHelp, v.elemTMP[i].GP);
                        }
                       
                        break;
                }
            }
            if (checkBox1.Checked && v.kod<95) //OSNAP
            {
                for (i = 0; i < v.Osnap.Count; i++)
                {
                    if (Class_d2.dlina(pmove, v.Osnap[i].p) <= v.MagnetSize)
                    {
                        Pen OsnapPen = new Pen(v.OsnapColor, 3);
                        Cursor.Position = new Point((int)v.Osnap[i].p.X, (int)v.Osnap[i].p.Y+23);
                        switch (v.Osnap[i].Type)
                        {
                            case 0: gr.DrawEllipse(OsnapPen, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y - v.OsnapSize, 2 * v.OsnapSize, 2 * v.OsnapSize);
                                break;
                            case 1: gr.DrawRectangle(OsnapPen, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y - v.OsnapSize, 2 * v.OsnapSize, 2 * v.OsnapSize);
                                break;
                            case 2: gr.DrawLine(OsnapPen, v.Osnap[i].p.X, v.Osnap[i].p.Y - v.OsnapSize,v.Osnap[i].p.X-v.OsnapSize,v.Osnap[i].p.Y+v.OsnapSize);
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X+v.OsnapSize, v.Osnap[i].p.Y + v.OsnapSize, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y + v.OsnapSize);
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X, v.Osnap[i].p.Y - v.OsnapSize, v.Osnap[i].p.X + v.OsnapSize, v.Osnap[i].p.Y + v.OsnapSize);
                                break;
                            case 3:
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y - v.OsnapSize, v.Osnap[i].p.X + v.OsnapSize, v.Osnap[i].p.Y + v.OsnapSize);
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X + v.OsnapSize, v.Osnap[i].p.Y- v.OsnapSize, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y + v.OsnapSize);

                                break;
                            case 4:
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X, v.Osnap[i].p.Y - v.OsnapSize, v.Osnap[i].p.X + v.OsnapSize, v.Osnap[i].p.Y );
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X + v.OsnapSize, v.Osnap[i].p.Y, v.Osnap[i].p.X , v.Osnap[i].p.Y + v.OsnapSize);
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X, v.Osnap[i].p.Y + v.OsnapSize, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y);
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y, v.Osnap[i].p.X , v.Osnap[i].p.Y - v.OsnapSize);
                                break;
                            case 5:
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y - v.OsnapSize, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y + v.OsnapSize);
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y + v.OsnapSize, v.Osnap[i].p.X + v.OsnapSize, v.Osnap[i].p.Y + v.OsnapSize);
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X - v.OsnapSize, v.Osnap[i].p.Y , v.Osnap[i].p.X , v.Osnap[i].p.Y);
                                gr.DrawLine(OsnapPen, v.Osnap[i].p.X, v.Osnap[i].p.Y, v.Osnap[i].p.X, v.Osnap[i].p.Y + v.OsnapSize);
                                break;
                        }
                        break;
                    }
                    
                }
            }
            if (v.kod == 98 && kolp>0)  //Zoom Window
            {
                
                    gr.FillRectangle(v.SelectBrushGreen, (min.X<pmove.X)? min.X:pmove.X, (min.Y < pmove.Y) ? min.Y : pmove.Y, Math.Abs(pmove.X - min.X), Math.Abs(pmove.Y - min.Y));
                    gr.DrawRectangle(new Pen(Color.Black, 0), (min.X < pmove.X) ? min.X : pmove.X, (min.Y < pmove.Y) ? min.Y : pmove.Y, Math.Abs(pmove.X - min.X), Math.Abs(pmove.Y - min.Y));

            }
            if (v.kod == 99)  //Selection by Rectangle
            {
                if (pmove.X > p1.X)
                {
                    gr.FillRectangle(v.SelectBrushGreen, p1.X, (p1.Y<pmove.Y)?p1.Y:pmove.Y, pmove.X - p1.X, Math.Abs(pmove.Y - p1.Y));
                    gr.DrawRectangle(new Pen(Color.Black, 0), p1.X, (p1.Y < pmove.Y) ? p1.Y : pmove.Y, pmove.X - p1.X, Math.Abs(pmove.Y - p1.Y));
                }
                else
                {
                    gr.FillRectangle(v.SelectBrushBlue, pmove.X, (p1.Y < pmove.Y) ? p1.Y : pmove.Y, p1.X - pmove.X, Math.Abs(pmove.Y - p1.Y));
                    gr.DrawRectangle(new Pen(Color.Black, 0), pmove.X, (p1.Y < pmove.Y) ? p1.Y : pmove.Y, p1.X - pmove.X, Math.Abs(pmove.Y - p1.Y));
                }
            }

            gr.ResetTransform();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) { v.Shift = false;  return; }
        }

       

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Text = "X=" + e.X + "   Y=" + e.Y;
            if (kolp == 0 && v.kod == 100) return;
            pmove = e.Location;

            if (kolp == 1)
            {
                switch (v.kod)
                {
                    case 16:
                        v.elemTMP.Clear();
                        for (i = v.Select.Count - 1; i > -1; i--)
                        {
                            v.obj tmp = new v.obj();
                            tmp.p = Class_d2.t_zerkalo(v.elem[v.Select[i]].p, p1, pmove);
                            tmp.Type = v.elem[v.Select[i]].Type;
                            switch (v.elem[v.Select[i]].Type)
                            {
                                case 0: tmp.GP.AddBeziers(tmp.p.ToArray()); break;
                                case 1: tmp.GP.AddLine(tmp.p[0], tmp.p[1]); break;
                                case 3:
                                    tmp.p[1] = new PointF(v.elem[v.Select[i]].p[1].X, 0);
                                    tmp.GP.AddEllipse(tmp.p[0].X - tmp.p[1].X, tmp.p[0].Y - tmp.p[1].X, 2 * tmp.p[1].X, 2 * tmp.p[1].X); break;
                                case 5: tmp.GP.AddPolygon(tmp.p.ToArray()); break;
                            }

                            v.elemTMP.Add(tmp);
                        }


                        break;
                }
            }
            Invalidate();
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (v.Select.Count == 0) return;
            v.Select.Sort();
            for (i = v.Select.Count - 1; i > -1; i--) v.elem.RemoveAt(v.Select[i]);
            v.Select.Clear();
            NewAllOsnapPoints();
            Invalidate();
        }

        void AddSelection()
        {
            p3 = new PointF((p1.X < p2.X) ? p1.X : p2.X, (p1.Y < p2.Y) ? p1.Y : p2.Y);
            RectangleF rect4 = new RectangleF(p3, new SizeF(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y)));
            for(int i4 = 0; i4 < v.elem.Count; i4++)
            {
                if (v.Select.Contains(i)) continue;
                switch (v.elem[i4].Type)
                {
                    case 1: case 5: if (Perevirka.Perevirka.IsAllPointsInsideRectangle(v.elem[i4].p, rect4)) v.Select.Add(i4); break;
                }
            }
            if (p2.X < p1.X)
            {
                for (int i4 = 0; i4 < v.elem.Count; i4++)
                {
                    if (v.Select.Contains(i)) continue;
                    switch (v.elem[i4].Type)
                    {
                        case 1:  if (Perevirka.Perevirka.IsSectionIntersectsRectangle(v.elem[i4].p[0], v.elem[i4].p[1], rect4)) v.Select.Add(i4); break;
                        case 2:  if (Perevirka.Perevirka.IsSectionIntersectsRectangle(v.elem[i4].p[0],new PointF(v.elem[i4].p[0].X+v.elem[i4].p[1].X,v.elem[i4].p[0].Y), rect4) ||
                                     Perevirka.Perevirka.IsSectionIntersectsRectangle(new PointF(v.elem[i4].p[0].X + v.elem[i4].p[1].X, v.elem[i4].p[0].Y), new PointF(v.elem[i4].p[0].X + v.elem[i4].p[1].X, v.elem[i4].p[0].Y+v.elem[i4].p[1].Y), rect4) ||
                                     Perevirka.Perevirka.IsSectionIntersectsRectangle(new PointF(v.elem[i4].p[0].X + v.elem[i4].p[1].X, v.elem[i4].p[0].Y + v.elem[i4].p[1].Y), new PointF(v.elem[i4].p[0].X, v.elem[i4].p[0].Y + v.elem[i4].p[1].Y), rect4) ||
                                     Perevirka.Perevirka.IsSectionIntersectsRectangle(v.elem[i4].p[0], new PointF(v.elem[i4].p[0].X, v.elem[i4].p[0].Y + v.elem[i4].p[1].Y), rect4)) v.Select.Add(i4);

                            break;

                    }
                }
            }

            if (v.Select.Count > 0) panel4.Enabled = true;

        }
      
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {   if (e.Button == MouseButtons.Middle && hand==false)
            {
                v.kod += 101;
                CursorTMP = Cursor; hand = true; phand = e.Location; Cursor = Cursors.Hand; return;
            }
            if (v.kod < 95) AddPerpendicularsToOsnap(e.Location);
            switch (kolp)
            {
                case 0: p1=min = e.Location; if (v.kod == 100) { v.kod = 99; pmove = p1; } break;
                case 1: p2 = e.Location; if (v.kod == 99 || v.kod==98) { if (v.kod == 98) { max = p2; ZoomWindow(); } v.kod = 100; kolp = -1; AddSelection(); } break;
                case 2: p3 = e.Location;  break;
                case 3: p4 = e.Location;  break;
                case 4: p5 = e.Location;  break;
            }
            kolp++;
            if (kolp == 1 && v.kod>10 && v.kod<16)
            {
                byte tmpBYTE;
                GraphicsPath tmpGP;
                PointF tmpPOINTF;
                v.Select.Sort();
                for (i = v.Select.Count-1; i>-1; i--)
                {
                    v.obj tmp = new v.obj();
                      tmpBYTE = v.elem[v.Select[i]].Type; tmp.Type = tmpBYTE;
                    for (j = 0; j < v.elem[v.Select[i]].p.Count; j++) { tmpPOINTF = v.elem[v.Select[i]].p[j]; tmp.p.Add(tmpPOINTF); }
                      tmpGP = v.elem[v.Select[i]].GP; tmp.GP = tmpGP;
                
                    v.elemTMP.Add(tmp);
                }
            }
            if (kolp == 2 && v.kod > 10 && v.kod < 16) // operations with selected
            {
                for (i = 0; i < v.elemTMP.Count; i++)
                {
                    v.elemTMP[i].GP.Reset();
                    switch (v.kod)
                    {
                        case 11: case 12:
                            switch (v.elemTMP[i].Type)
                            {
                                case 1: case 0: case 5: // Line Spline Polygon
                                    for (k = 0; k < v.elemTMP[i].p.Count; k++)
                                    v.elemTMP[i].p[k] = new PointF(v.elemTMP[i].p[k].X+dx, v.elemTMP[i].p[k].Y + dy);

                                    if (v.elemTMP[i].Type == 1) v.elemTMP[i].GP.AddLine(v.elemTMP[i].p[0], v.elemTMP[i].p[1]);
                                    if (v.elemTMP[i].Type == 0) v.elemTMP[i].GP.AddBeziers(v.elemTMP[i].p.ToArray());
                                    if (v.elemTMP[i].Type == 5) v.elemTMP[i].GP.AddPolygon(v.elemTMP[i].p.ToArray());
                                    break;
                                case 2:  case 3: // Rectangle Circle     Arc Ellipse
                                        v.elemTMP[i].p[0] = new PointF(v.elemTMP[i].p[0].X + dx, v.elemTMP[i].p[0].Y + dy);
                                    if (v.elemTMP[i].Type == 2) v.elemTMP[i].GP.AddRectangle(new RectangleF(v.elemTMP[i].p[0].X, v.elemTMP[i].p[0].Y, v.elemTMP[i].p[1].X, v.elemTMP[i].p[1].Y));
                                    if (v.elemTMP[i].Type == 3) v.elemTMP[i].GP.AddEllipse(new RectangleF(v.elemTMP[i].p[0].X-v.elemTMP[i].p[1].X, v.elemTMP[i].p[0].Y-v.elemTMP[i].p[1].X,
                                                                                                          v.elemTMP[i].p[1].X*2, v.elemTMP[i].p[1].X*2));
                                    break;
                            }
                           
                            
                            break;

                           
                    }
                    
                }
                v.elem.AddRange(v.elemTMP);
                v.elemTMP.Clear(); v.Select.Clear();
                if (v.kod == 11) DeleteToolStripMenuItem_Click(null, null);
                kolp = 0; v.kod = 100; Panel4CheckedFalse(); Cursor = Cursors.Default;
                NewAllOsnapPoints();
            }
            if (kolp == 2 && (v.kod<10 || v.kod>16) && v.kod!=0 && v.kod!=32) // DRAW
            {
                v.obj tmp = new v.obj();
                switch (v.kod)
                {
                    case 1: tmp.p.Add(p1); tmp.p.Add(p2); tmp.Type = 1;  // Line
                            tmp.GP.AddLine(p1, p2);
                            p1 = p2; kolp = 1;
                            break;
                    case 2:  case 21: tmp.p.Add(p3); tmp.p.Add(new PointF(w,h)); tmp.Type = 2; // REctangle
                            tmp.GP.AddRectangle(new RectangleF(p3.X, p3.Y, w, h));
                            kolp = 0;
                            break;
                    case 3: case 31: // Circle
                        if (v.kod == 31) p1 = p5;
                        tmp.p.Add(p1); tmp.p.Add(new PointF(w, 0)); tmp.Type = 3;
                        tmp.GP.AddEllipse(new RectangleF(p1.X-w, p1.Y-w, w*2, w*2));
                        kolp = 0;
                        break;
                    case 5: case 51:  // Polygon
                        tmp.p.AddRange(v.Ptmp1);  tmp.Type = 5;
                        tmp.GP.AddPolygon(v.Ptmp1);
                        kolp = 0;
                        break;

                }
                v.elem.Add(tmp);
                AddElemToOsnap(1);
                AddIntersectionPointsToOsnap(v.elem.Count-1);
            }
            if (kolp == 3 && v.kod!=0)
            {
                v.obj tmp = new v.obj();
                switch (v.kod)
                {
                    case 32:  tmp.p.Add(p5); tmp.p.Add(new PointF(w, 0)); tmp.Type = 3;
                        tmp.GP.AddEllipse(new RectangleF(p5.X - w, p5.Y - w, w * 2, w * 2));
                        kolp = 0;
                        break;

                }
                v.elem.Add(tmp);
                AddElemToOsnap(1);
                AddIntersectionPointsToOsnap(v.elem.Count - 1);
            }
            if (kolp == 4)
            {
                v.obj tmp = new v.obj();
                switch (v.kod)
                {
                    case 0: if (v.tmpBool == false) { v.Ptmp.Add(p1); tmp.p.Add(p1); } else v.elem.RemoveAt(v.elem.Count - 1);
                            v.Ptmp.Add(p2); v.Ptmp.Add(p3); v.Ptmp.Add(p4);
                        
                        tmp.p.Add(p2); tmp.p.Add(p3); tmp.p.Add(p4); tmp.Type = 0;
                        tmp.GP.AddBeziers(v.Ptmp.ToArray());
                        kolp = 1; p1 = p4;
                        v.tmpBool = true;
                        break;

                }
                v.elem.Add(tmp);
                AddElemToOsnap(1);
                AddIntersectionPointsToOsnap(v.elem.Count - 1);
            }

            Invalidate();
        }
        void Panel4CheckedFalse()
        {
            r_Move.Checked = r_Copy.Checked = r_Rotate.Checked = r_Array_Radial.Checked = r_Array_Rectangular.Checked = r_Mirror.Checked = false;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                v.kod -= 101; hand = false; Cursor = CursorTMP;
                if (phand == e.Location) return;
                dx = pmove.X - phand.X; dy = pmove.Y - phand.Y;
                MovePointsP();
                for (i = 0; i < v.elem.Count; i++) MoveElement(i);
                for (i = 0; i < v.Osnap.Count; i++)
                    v.Osnap[i].p = new PointF(v.Osnap[i].p.X + dx, v.Osnap[i].p.Y + dy);

                Invalidate();
            }
        }
        void MovePointsP()
        {
            p1 = new PointF(p1.X + dx, p1.Y + dy);
            p2 = new PointF(p2.X + dx, p2.Y + dy);
            p3 = new PointF(p3.X + dx, p3.Y + dy);
            p4 = new PointF(p4.X + dx, p4.Y + dy);
            p5 = new PointF(p5.X + dx, p5.Y + dy);
        }
        void MoveElement(int number)
        {
            v.elem[number].GP.Reset();
            v.elem[number].p[0] = new PointF(v.elem[number].p[0].X + dx, v.elem[number].p[0].Y + dy);
            switch (v.elem[number].Type)
            {
                case 1: v.elem[number].p[1] = new PointF(v.elem[number].p[1].X + dx, v.elem[number].p[1].Y + dy);
                        v.elem[number].GP.AddLine(v.elem[number].p[0], v.elem[number].p[1]);
                    break;
                case 2: v.elem[number].GP.AddRectangle(new RectangleF(v.elem[number].p[0].X, v.elem[number].p[0].Y, v.elem[number].p[1].X, v.elem[number].p[1].Y));
                    break;
                case 3: v.elem[number].GP.AddEllipse(new RectangleF(v.elem[number].p[0].X- v.elem[number].p[1].X, v.elem[number].p[0].Y- v.elem[number].p[1].X,
                     2*v.elem[number].p[1].X, 2 * v.elem[number].p[1].X));
                    break;
                case 0: case 5:
                    for (int i0 = 1; i0 < v.elem[number].p.Count; i0++)
                        v.elem[number].p[i0] = new PointF(v.elem[number].p[i0].X + dx, v.elem[number].p[i0].Y + dy);
                      if (v.elem[number].Type==5)  v.elem[number].GP.AddPolygon(v.elem[number].p.ToArray());
                      else v.elem[number].GP.AddBeziers(v.elem[number].p.ToArray());
                    break;
            }
        }
        void GetGabaritsAllElements()
        {  if (v.elem.Count == 0) return;
            for (i = 0; i < v.Osnap.Count; i++)
            {
                if (v.Osnap[i].Type == 1 || v.Osnap[i].Type == 4) { min = max = v.Osnap[i].p; break;   }
            }
            for (i = 0; i < v.Osnap.Count; i++)
            {
                if (v.Osnap[i].Type == 1 || v.Osnap[i].Type == 4)
                {
                    if (min.X > v.Osnap[i].p.X) min.X = v.Osnap[i].p.X;
                    if (min.Y > v.Osnap[i].p.Y) min.Y = v.Osnap[i].p.Y;
                    if (max.X < v.Osnap[i].p.X) max.X = v.Osnap[i].p.X;
                    if (max.Y < v.Osnap[i].p.Y) max.Y = v.Osnap[i].p.Y;
                }
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (sender == button1) // ZOOM +
            {
                v.scaleTMP *= v.zoomK;
            }
            if (sender == button2) // ZOOM -
            {
                v.scaleTMP /= v.zoomK;
            }

            if (sender == button5) // 1:1
            {
                v.scaleTMP = 1/v.scale;
            }
            if (sender == button3) // Zoom Window
            {
                v.kod = 98; Cursor = Cursors.Cross; return;
            }
            if (sender == button4) // ZOOM ALL
            {
                GetGabaritsAllElements(); ZoomWindow(); return;
            }


            ScaleAllPoints();
        }
        void ScaleAllPoints()
        {
            PointF t = new PointF((Width - 64) / 2+32, (Height - 110) / 2+78);
            //   if (SS) t = new PointF((Width - 64) / 2, (Height - 110) / 2);
            //   else t=new PointF(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
            for (i = 0; i < v.elem.Count; i++)
            {
                switch (v.elem[i].Type)
                {
                    case 0:
                    case 1:
                    case 5:
                        for (j = 0; j < v.elem[i].p.Count; j++)
                            v.elem[i].p[j] = Class_d2.t_scale(v.elem[i].p[j], t, v.scaleTMP, v.scaleTMP);
                        break;
                    case 2:
                    case 3:
                        v.elem[i].p[0] = Class_d2.t_scale(v.elem[i].p[0], t, v.scaleTMP, v.scaleTMP);
                        v.elem[i].p[1] = new PointF(v.elem[i].p[1].X * v.scaleTMP, v.elem[i].p[1].Y * v.scaleTMP);
                        break;
                }
                NewElemGP(i);
            }
            v.scale *= v.scaleTMP; v.scaleTMP = 1;
            NewAllOsnapPoints();
            Invalidate();
        }
        void ZoomWindow()
        {
            Cursor = Cursors.Default;
            float wz = (Width - 74) / Math.Abs(min.X - max.X);
            float hz = (Height - 120) / Math.Abs(min.Y - max.Y);
            v.scaleTMP *= (hz < wz) ? hz : wz;
            wz= Math.Abs(min.X - max.X); hz= Math.Abs(min.Y - max.Y);
            PointF t = new PointF((Width - 64) / 2+32, (Height - 110) / 2+78); 
            dx = t.X - ((min.X < max.X) ? (min.X + wz / 2) : (max.X + wz / 2));
            dy = t.Y - ((min.Y < max.Y) ? (min.Y + hz / 2) : (max.Y + hz / 2));
            for (i = 0; i < v.elem.Count; i++) MoveElement(i);
            ScaleAllPoints();
           
        }
        void NewElemGP(int kk)
        {
            v.elem[kk].GP.Reset();
            switch (v.elem[kk].Type)
            {
                case 0: v.elem[kk].GP.AddBeziers(v.elem[kk].p.ToArray()); break; // Bezier
                case 1: v.elem[kk].GP.AddLine(v.elem[kk].p[0], v.elem[kk].p[1]); break; // LINE
                case 2: v.elem[kk].GP.AddRectangle(new RectangleF(v.elem[kk].p[0].X, v.elem[kk].p[0].Y, v.elem[kk].p[1].X, v.elem[kk].p[1].Y)); break; // Rectangle
                case 3: v.elem[kk].GP.AddEllipse(new RectangleF(v.elem[kk].p[0].X- v.elem[kk].p[1].X, v.elem[kk].p[0].Y- v.elem[kk].p[1].X, 2*v.elem[kk].p[1].X, 2*v.elem[kk].p[1].X)); break; // Circle
                case 5: v.elem[kk].GP.AddPolygon(v.elem[kk].p.ToArray()); break; // polygon
            }
        }

        void NewAllOsnapPoints()
        {
            v.Osnap.Clear(); if (v.elem.Count == 0) return;
            AddElemToOsnap(v.elem.Count);
            for (int i2 = 0; i2 < v.elem.Count; i2++) AddIntersectionPointsToOsnap(i2);
        }
        void AddElemToOsnap(int k7)
        {   PointF tmpP;
            int j6;
            for(int i6=v.elem.Count - k7; i6 < v.elem.Count; i6++)
            {
                switch (v.elem[i6].Type)
                {
                    case 0:
                        for (j6 = 0; j6 < v.elem[i6].p.Count; j6++)
                           if (j6%3==0)
                            if (PerevirkaPointToOsnap(v.elem[i6].p[j6])) AddPointToOsnap(v.elem[i6].p[j6], 1);
                        
                        break;
                    case 1: if (PerevirkaPointToOsnap(v.elem[i6].p[0])) AddPointToOsnap(v.elem[i6].p[0], 1);
                            if (PerevirkaPointToOsnap(v.elem[i6].p[1])) AddPointToOsnap(v.elem[i6].p[1], 1);
                        tmpP = Class_d2.center_otr(v.elem[i6].p[0], v.elem[i6].p[1]);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 2);
                        break;
                    case 2:
                        tmpP = new PointF(v.elem[i6].p[0].X+ v.elem[i6].p[1].X/2, v.elem[i6].p[0].Y + v.elem[i6].p[1].Y/2);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 0);
                        tmpP = new PointF(v.elem[i6].p[0].X , v.elem[i6].p[0].Y);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 1);
                        tmpP = new PointF(v.elem[i6].p[0].X + v.elem[i6].p[1].X, v.elem[i6].p[0].Y);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 1);
                        tmpP = new PointF(v.elem[i6].p[0].X + v.elem[i6].p[1].X, v.elem[i6].p[0].Y + v.elem[i6].p[1].Y);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 1);
                        tmpP = new PointF(v.elem[i6].p[0].X , v.elem[i6].p[0].Y + v.elem[i6].p[1].Y );
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 1);
                        tmpP = new PointF(v.elem[i6].p[0].X + v.elem[i6].p[1].X / 2, v.elem[i6].p[0].Y );
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 2);
                        tmpP = new PointF(v.elem[i6].p[0].X + v.elem[i6].p[1].X, v.elem[i6].p[0].Y + v.elem[i6].p[1].Y / 2);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 2);
                        tmpP = new PointF(v.elem[i6].p[0].X + v.elem[i6].p[1].X / 2, v.elem[i6].p[0].Y + v.elem[i6].p[1].Y );
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 2);
                        tmpP = new PointF(v.elem[i6].p[0].X , v.elem[i6].p[0].Y + v.elem[i6].p[1].Y / 2);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 2);
                        break;
                    case 3:
                        if (PerevirkaPointToOsnap(v.elem[i6].p[0])) AddPointToOsnap(v.elem[i6].p[0], 0);
                        tmpP = new PointF(v.elem[i6].p[0].X, v.elem[i6].p[0].Y - v.elem[i6].p[1].X);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 4);
                        tmpP = new PointF(v.elem[i6].p[0].X + v.elem[i6].p[1].X, v.elem[i6].p[0].Y);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 4);
                        tmpP = new PointF(v.elem[i6].p[0].X, v.elem[i6].p[0].Y + v.elem[i6].p[1].X);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 4);
                        tmpP = new PointF(v.elem[i6].p[0].X - v.elem[i6].p[1].X, v.elem[i6].p[0].Y);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 4);
                        break;
                    case 5:  float[] ccc = Class_d2.circle_3point(v.elem[i6].p[0], v.elem[i6].p[1], v.elem[i6].p[2]);
                        tmpP = new PointF(ccc[0], ccc[1]);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 0);
                        for(j6=0;j6<v.elem[i6].p.Count;j6++) if (PerevirkaPointToOsnap(v.elem[i6].p[j6])) AddPointToOsnap(v.elem[i6].p[j6], 1);
                        for (j6 = 0; j6 < v.elem[i6].p.Count-1; j6++)
                        {
                            tmpP = Class_d2.center_otr(v.elem[i6].p[j6], v.elem[i6].p[j6+1]);
                            if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 2);
                        }
                        tmpP = Class_d2.center_otr(v.elem[i6].p[0], v.elem[i6].p[v.elem[i6].p.Count - 1]);
                        if (PerevirkaPointToOsnap(tmpP)) AddPointToOsnap(tmpP, 2);
                        break;
                }
            }

        }
        void AddIntersectionPointsToOsnap(int k8)
        {
            if (v.elem.Count <2) return;
            PointF tmp8;
            PointF[] tmp8M;
            for (int i8 = 0; i8 < v.elem.Count; i8++)
            {
                if (i8 == k8) continue;
                if (v.elem[i8].Type == 1 && v.elem[k8].Type == 1) // 2 отрезка
                {
                    if (Class_d2.position_2otr(v.elem[i8].p[0], v.elem[i8].p[1], v.elem[k8].p[0], v.elem[k8].p[1]) == 2)
                    {
                        tmp8 = Class_d2.t_per_2otr(v.elem[i8].p[0], v.elem[i8].p[1], v.elem[k8].p[0], v.elem[k8].p[1]);
                        if (PerevirkaPointToOsnap(tmp8)) AddPointToOsnap(tmp8, 3);
                    }
                }
                if (v.elem[i8].Type == 3 && v.elem[k8].Type == 3) // 2 окружности
                {
                    if (Class_d2.kol_intersect_2circle(v.elem[i8].p[0], v.elem[i8].p[1].X, v.elem[k8].p[0], v.elem[k8].p[1].X) >0)
                    {
                        tmp8M = Class_d2.t_per_2circle(v.elem[i8].p[0], v.elem[i8].p[1].X, v.elem[k8].p[0], v.elem[k8].p[1].X);
                        if (PerevirkaPointToOsnap(tmp8M[0])) AddPointToOsnap(tmp8M[0], 3);
                        if (tmp8M.Length>1 && PerevirkaPointToOsnap(tmp8M[1])) AddPointToOsnap(tmp8M[1], 3);
                    }
                }
            }
        }
        void AddPerpendicularsToOsnap(PointF PP)
        {  if (v.elem.Count == 0) return;
            ClearPerpendicularsToOsnap();
            PointF tmp0;
            PointF[] tmpM;
            for(int i0 = 0; i0 < v.elem.Count; i0++)
            {
                if (v.elem[i0].Type==1 && Class_d2.t_at_line(PP, v.elem[i0].p[0], v.elem[i0].p[1])==false)
                {
                    tmp0 = Class_d2.t_perpendicular(PP, v.elem[i0].p[0], v.elem[i0].p[1]);
                    if (PerevirkaPointToOsnap(tmp0)) AddPointToOsnap(tmp0, 5);
                }
                if (v.elem[i0].Type == 3)
                {
                    tmpM = Class_d2.intersect_otr_circle(PP, v.elem[i0].p[0], v.elem[i0].p[0],v.elem[i0].p[1].X);
                    if (PerevirkaPointToOsnap(tmpM[0])) AddPointToOsnap(tmpM[0], 5);
                    if (tmpM.Length>1 && PerevirkaPointToOsnap(tmpM[1])) AddPointToOsnap(tmpM[1], 5);
                }
            }

        }
        void ClearPerpendicularsToOsnap()
        {
            for (int i0 = v.Osnap.Count - 1; i0 > -1; i0--)
                if (v.Osnap[i0].Type != 5) return; else v.Osnap.RemoveAt(i0);
        }
        void AddPointToOsnap(PointF PP, byte TT)
        {
            v.OsnapPoints tmp = new v.OsnapPoints();
            tmp.Type = TT; tmp.p = PP;
            v.Osnap.Add(tmp);
        }
        bool PerevirkaPointToOsnap(PointF m)
        {
            for(int i7 = 0; i7 < v.Osnap.Count; i7++)
            {
                if (v.Osnap[i7].p.Equals(m))  return false;
            }
            return true;
        }

        void PanelVisibleFalse()
        {
            pan_rect.Visible =pan_circle.Visible=pan_poly.Visible=pan_ARectangular.Visible=
            pan_A_Radial.Visible=pan_Rotate_Mirror.Visible=    false;
        }
     
        private void R_Line_Click(object sender, EventArgs e)
        {
            PanelVisibleFalse();  kolp = 0;
            Cursor = Cursors.Cross; v.Select.Clear();
            if (sender==r_Line) { v.kod = 1; }
            if (sender == r_Rectangle) { pan_rect.Visible = true; v.kod =(r_rect_2P.Checked)?(byte)2: (byte)21; }
            if (sender == r_Circle) { pan_circle.Visible = true; v.kod = (r_circ_CR.Checked) ? (byte)3 : (r_circ_2PD.Checked) ? (byte)31:(byte)32; }
            if (sender == r_Ellipse) { v.kod = 4; }
            if (sender == r_Arc) { v.kod = 6; }
            if (sender == r_Polygon) { pan_poly.Visible = true; v.kod = (r_poly_out.Checked) ? (byte)5 : (byte)51; }
            if (sender == r_Spline) { v.kod = 0; v.tmpBool = false;  }

            panel4.Enabled = false;
            Invalidate();
        }
        private void R_Move_Click(object sender, EventArgs e)
        {
            kolp = 0; PanelVisibleFalse();
            Cursor = Cursors.Cross; 
            if (sender == r_Move) { v.kod = 11; }
            if (sender == r_Copy) { v.kod = 12; }
            if (sender == r_Rotate) { v.kod = 13; pan_Rotate_Mirror.Visible = true; }
            if (sender == r_Array_Radial) { v.kod = 14; pan_A_Radial.Visible = true; }
            if (sender == r_Array_Rectangular) { v.kod = 15; pan_ARectangular.Visible = true; }
            if (sender == r_Mirror) { v.kod = 16; pan_Rotate_Mirror.Visible = true; }
            if (sender == r_Explode)  ExplodeObjects();

            
            Invalidate();
        }

        void ExplodeObjects()
        {
            if (v.Select.Count == 0) return;
            for (i = 0; i < v.Select.Count; i++)
            {
                switch (v.elem[v.Select[i]].Type)
                {
                    case 0: if (v.elem[v.Select[i]].p.Count > 4)
                        {
                            PointF[][] ccc1 = Class_bd2.Explode_bez(v.elem[v.Select[i]].p.ToArray());
                            for (j = 0; j < ccc1.GetLength(0); j++) AddObject_BEZIER(ccc1[j]);
                        }
                        break;
                    case 2: AddObject_LINE(v.elem[v.Select[i]].p[0],
                        new PointF(v.elem[v.Select[i]].p[0].X + v.elem[v.Select[i]].p[1].X, v.elem[v.Select[i]].p[0].Y));
                        AddObject_LINE(new PointF(v.elem[v.Select[i]].p[0].X + v.elem[v.Select[i]].p[1].X, v.elem[v.Select[i]].p[0].Y),
                        new PointF(v.elem[v.Select[i]].p[0].X + v.elem[v.Select[i]].p[1].X, v.elem[v.Select[i]].p[0].Y+ v.elem[v.Select[i]].p[1].Y));
                        AddObject_LINE(new PointF(v.elem[v.Select[i]].p[0].X, v.elem[v.Select[i]].p[0].Y + v.elem[v.Select[i]].p[1].Y),
                        new PointF(v.elem[v.Select[i]].p[0].X + v.elem[v.Select[i]].p[1].X, v.elem[v.Select[i]].p[0].Y + v.elem[v.Select[i]].p[1].Y));
                        AddObject_LINE(v.elem[v.Select[i]].p[0],
                        new PointF(v.elem[v.Select[i]].p[0].X , v.elem[v.Select[i]].p[0].Y + v.elem[v.Select[i]].p[1].Y));
                        break;
                    case 3: PointF[] ccc=Class_bd2.Circle_to_bez_13p(v.elem[v.Select[i]].p[0], v.elem[v.Select[i]].p[1].X);
                            AddObject_BEZIER(ccc);  break;
                    case 5: for (j = 0; j < v.elem[v.Select[i]].p.Count - 1; j++)
                            AddObject_LINE(v.elem[v.Select[i]].p[j], v.elem[v.Select[i]].p[j + 1]);
                            AddObject_LINE(v.elem[v.Select[i]].p[0], v.elem[v.Select[i]].p[j]);
                        break;
                }
            }
            v.Select.Sort();
            for (i = v.Select.Count - 1; i > -1; i--)
            {
                if (v.elem[v.Select[i]].Type == 1 || (v.elem[v.Select[i]].Type == 0 && v.elem[v.Select[i]].p.Count == 4)) continue;
                v.elem.RemoveAt(v.Select[i]);
            }
            v.Select.Clear(); panel4.Enabled =r_Explode.Checked= false; Cursor = Cursors.Default;
            Invalidate(); 
        }
        void  AddObject_LINE(PointF a1, PointF a2)
        {
            v.obj tmp5 = new v.obj();
            tmp5.p.Add(a1); tmp5.p.Add(a2); tmp5.Type = 1;
            tmp5.GP.AddLine(a1, a2);
            v.elem.Add(tmp5);
        }
        void AddObject_BEZIER(PointF[] a1)
        {
            v.obj tmp5 = new v.obj();
            tmp5.p.AddRange(a1); tmp5.Type = 0;
            tmp5.GP.AddBeziers(a1);
            v.elem.Add(tmp5);
        }
        public Form1()
        {
            InitializeComponent();

            r_Line.Image = Image.FromFile("icons//Line.png");
            r_Rectangle.Image = Image.FromFile("icons//Rectangle.png");
            r_Circle.Image = Image.FromFile("icons//Circle.png");
            r_Arc.Image = Image.FromFile("icons//Arc3Point.png");
            r_Ellipse.Image = Image.FromFile("icons//Sphere.png");
            r_Polygon.Image = Image.FromFile("icons//Polygon.png");
            r_Spline.Image = Image.FromFile("icons//124.png");
            r_rect_2P.Image = Image.FromFile("icons//Rectangle2P.png");
            r_rect_CP.Image = Image.FromFile("icons//RectangleCP.png");
            r_circ_CR.Image = Image.FromFile("icons//CircleCR.png");
            r_circ_2PD.Image = Image.FromFile("icons//Circle2PD.png");
            r_circ_3P.Image = Image.FromFile("icons//Circle3P.png");
            r_poly_out.Image = Image.FromFile("icons//Polygon.png");
            r_poly_in.Image = Image.FromFile("icons//PolygonCP1.png");
            r_Move.Image = Image.FromFile("icons//Move.png");
            r_Copy.Image = Image.FromFile("icons//Copy.png");
            r_Mirror.Image = Image.FromFile("icons/Mirror.png");
            r_Array_Radial.Image = Image.FromFile("icons//ArrayRadial.png");
            r_Array_Rectangular.Image = Image.FromFile("icons//ArrayRectangular.png");
            r_Rotate.Image = Image.FromFile("icons//Redraw.png");
            r_Explode.Image = Image.FromFile("icons//Lamp.png");


            v.br = new BinaryReader(File.Open("options", FileMode.Open));

            BackColor= v.BackGround = Color.FromArgb(v.br.ReadByte(), v.br.ReadByte(), v.br.ReadByte());
            v.Main = Color.FromArgb(v.br.ReadByte(), v.br.ReadByte(), v.br.ReadByte());
            v.Help = Color.FromArgb(v.br.ReadByte(), v.br.ReadByte(), v.br.ReadByte());
            v.Assistant = Color.FromArgb(v.br.ReadByte(), v.br.ReadByte(), v.br.ReadByte());
            v.OsnapColor = Color.FromArgb(v.br.ReadByte(), v.br.ReadByte(), v.br.ReadByte());
            v.OsnapSize = v.br.ReadByte();
            v.MagnetSize = v.br.ReadByte();
            v.tooltip.Active = v.br.ReadBoolean();
            v.tooltip.IsBalloon = v.br.ReadBoolean();

            v.br.Close();

           v.penMain = new Pen(v.Main, 1);
           v.penHelp = new Pen(v.Help, 1);
           v.penAssistant = new Pen(v.Assistant, 1);
           v.penSelected = new Pen(v.Selected, 1);
        /*    switch ()
            {
                case 0: v.penSelected.DashStyle = DashStyle.Solid; break;
                case 1: v.penSelected.DashStyle = DashStyle.Dot; break;
                case 2: v.penSelected.DashStyle = DashStyle.Dash; break;
            } */
            



            v.tooltip.SetToolTip(r_Line, "Line");
            v.tooltip.SetToolTip(r_Rectangle, "Rectangle");
            v.tooltip.SetToolTip(r_Ellipse, "Ellipse");
            v.tooltip.SetToolTip(r_Circle, "Circle");
            v.tooltip.SetToolTip(r_Arc, "Arc");
            v.tooltip.SetToolTip(r_Polygon, "Polygon");
            v.tooltip.SetToolTip(r_Spline, "Spline");
            v.tooltip.SetToolTip(r_rect_2P, "Rectangle by 2 Points");
            v.tooltip.SetToolTip(r_Move, "Move");
            v.tooltip.SetToolTip(r_Copy, "Copy");
            v.tooltip.SetToolTip(r_Rotate, "Rotate");
            v.tooltip.SetToolTip(r_Array_Radial, "ArrayRadial");
            v.tooltip.SetToolTip(r_Array_Rectangular, "ArrayRectangular");
            v.tooltip.SetToolTip(r_Mirror, "Mirror");
            v.tooltip.SetToolTip(r_Explode, "Explode");



        }
    }
}

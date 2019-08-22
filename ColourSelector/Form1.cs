﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColourSelector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Bitmap b = new Bitmap(310, 310);
        public Bitmap back = new Bitmap(310, 310);

        private void Form1_Load(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(b);
            Graphics o = Graphics.FromImage(back);
            DrawColorWheel(g, Color.Black, 0, 0, 309, 309);
            DrawColorWheel(o, Color.Black, 0, 0, 309, 309);
            pictureBox1.Image = b;
            doGraphicShit(null);
        }

        //csharphelper.com
        private void DrawColorWheel(Graphics gr, Color outline_color, int xmin, int ymin, int wid, int hgt)
        {
            Rectangle rect = new Rectangle(xmin, ymin, wid, hgt);
            GraphicsPath wheel_path = new GraphicsPath();
            wheel_path.AddEllipse(rect);
            wheel_path.Flatten();

            float num_pts = (wheel_path.PointCount - 1) / 6;
            Color[] surround_colors = new Color[wheel_path.PointCount];

            int index = 0;
            InterpolateColors(surround_colors, ref index, 1 * num_pts, 255, 255, 0, 0, 255, 255, 0, 255);
            InterpolateColors(surround_colors, ref index, 2 * num_pts, 255, 255, 0, 255, 255, 0, 0, 255);
            InterpolateColors(surround_colors, ref index, 3 * num_pts, 255, 0, 0, 255, 255, 0, 255, 255);
            InterpolateColors(surround_colors, ref index, 4 * num_pts, 255, 0, 255, 255, 255, 0, 255, 0);
            InterpolateColors(surround_colors, ref index, 5 * num_pts, 255, 0, 255, 0, 255, 255, 255, 0);
            InterpolateColors(surround_colors, ref index, wheel_path.PointCount, 255, 255, 255, 0, 255, 255, 0, 0);

            using (PathGradientBrush path_brush =  new PathGradientBrush(wheel_path))
            {
                path_brush.CenterColor = Color.White;
                path_brush.SurroundColors = surround_colors;

                gr.FillPath(path_brush, wheel_path);

                using (Pen thick_pen = new Pen(outline_color, 2))
                {
                    gr.DrawPath(thick_pen, wheel_path);
                }
            }
        }

        //csharphelper.com
        private void InterpolateColors(Color[] surround_colors, ref int index, float stop_pt, int from_a, int from_r, int from_g, int from_b, int to_a, int to_r, int to_g, int to_b)
        {
            int num_pts = (int)stop_pt - index;
            float a = from_a, r = from_r, g = from_g, b = from_b;
            float da = (to_a - from_a) / (num_pts - 1);
            float dr = (to_r - from_r) / (num_pts - 1);
            float dg = (to_g - from_g) / (num_pts - 1);
            float db = (to_b - from_b) / (num_pts - 1);

            for (int i = 0; i < num_pts; i++)
            {
                surround_colors[index++] = Color.FromArgb((int)a, (int)r, (int)g, (int)b);
                a += da;
                r += dr;
                g += dg;
                b += db;
            }
        }

        private void doGraphicShit(MouseEventArgs mev)
        {
            int baseSize = 40;

            try
            {
                baseSize = Int32.Parse(textBox1.Text);
            }
            catch
            {
                textBox1.Text = "40";
            }

            Random rnd = new Random();

            Point e = new Point();
            if (mev != null)
            {
                e.X = mev.X;
                e.Y = mev.Y;
            }
            else
            {
                e = new Point(rnd.Next(30, 280), rnd.Next(30, 280));
            }

            Graphics g = Graphics.FromImage(b);
            g.FillRectangle(Brushes.White, 0, 0, 310, 310);
            g.DrawImage(back, 0, 0);
            Point p = new Point(e.X, e.Y);
            Point o = new Point(150 - (e.X - 150), 150 - (e.Y - 150));
            Color colorOp;
            if (p.X > 0 && p.Y > 0)
            {
                colorOp = b.GetPixel(p.X, p.Y);
            }
            else
            {
                colorOp = Color.Black;
            }

            g.DrawRectangle(Pens.Black, p.X, p.Y, 1, 1);
            g.DrawLine(Pens.Black, p, o);
            getCircle(g, Pens.Black, p, baseSize / 2);
            getCircle(g, Pens.Red, o, baseSize / 2);
            pictureBox1.Image = b;
            label2.BackColor = colorOp;

            List<Color> pixels = new List<Color> { };

            for (int i = p.X - baseSize / 2; i < p.X + baseSize / 2; i++)
            {
                for (int f = p.Y - baseSize / 2; f < p.Y + baseSize / 2; f++)
                {
                    if ((i > 0 && f > 0) && (i < 310 && f < 310))
                    {
                        Color cur = b.GetPixel(i, f);
                        pixels.Add(cur);
                    }
                    else
                    {
                        pixels.Add(Color.Black);
                    }
                }
            }

            Bitmap redraw = new Bitmap(baseSize, baseSize);
            Graphics gr = Graphics.FromImage(redraw);

            int count = 0;
            for (int i = 0; i < baseSize; i++)
            {
                for (int f = 0; f < baseSize; f++)
                {
                    count++;
                    Color cur;
                    try
                    {
                        cur = pixels[count];
                    }
                    catch
                    {
                        cur = Color.Black;
                    }

                    gr.DrawRectangle(new Pen(cur), i, f, 1, 1);

                }
            }


            for(int i = 0; i < pixels.Count(); i++)
            {
                if(pixels[i] == Color.FromArgb(0, 0, 0))
                {
                    pixels.RemoveAt(i);
                }
            }

            Color c1 = pixels[rnd.Next(0, pixels.Count())];
            Color c2 = pixels[rnd.Next(0, pixels.Count())];
            Color c3 = pixels[rnd.Next(0, pixels.Count())];

            c1 = ControlPaint.Light(c1, rnd.Next(0, 75));
            c2 = ControlPaint.Light(c2, rnd.Next(0, 75));
            c3 = ControlPaint.Light(c3, rnd.Next(0, 75));

            label3.BackColor = c1;
            label4.BackColor = c2;
            label5.BackColor = c3;

            Color color;
            color = b.GetPixel(o.X + 5, o.Y + 5);
            if(color == Color.Black)
            {
                color = b.GetPixel(o.X - 5, o.Y - 5);
            }
            
            label1.BackColor = color;

            gr.DrawRectangle(Pens.Black, 0, 0, baseSize - 1, baseSize - 1);
            pictureBox2.Image = redraw;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            doGraphicShit(e);
        }

        private void getCircle(Graphics drawingArea, Pen penToUse, Point center, int radius)
        {
            Rectangle rect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            Point rectP = new Point(rect.X, rect.Y);
            drawingArea.DrawRectangle(penToUse, rect);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            doGraphicShit(null);
        }
    }
}

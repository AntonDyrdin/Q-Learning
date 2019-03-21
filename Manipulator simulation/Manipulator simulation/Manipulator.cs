using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Manipulator_simulation
{
    class Manipulator
    {
        public List<Link> links;
        public Form1 form1;
        public PictureBox picBox;
        public Graphics g;
        public Bitmap bitmap;
        public int baseX;
        public int baseY;

        public Manipulator(Form1 form1, int baseX, int baseY)
        {
            this.baseX = baseX;
            this.baseY = baseY;
            links = new List<Link> { };
            this.form1 = form1;
            picBox = form1.picBox;
            bitmap = new Bitmap(picBox.Width, picBox.Height);
            g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

        }
        public void setLinkAngle(int linkIndex, double angle)
        {
            double df = angle - links[linkIndex].angle;
            links[linkIndex].setAngle(angle);

            for (int i = linkIndex + 1; i < links.Count; i++)
            {
                links[i].x1 = links[i - 1].x2;
                links[i].y1 = links[i - 1].y2;
                links[i].setAngle(links[i].angle + df);
            }
        }
        public void AddLink(double length)
        {
            Link newLink = new Link(length);
            if (links.Count == 0)
            {
                newLink.x1 = baseX;
                newLink.y1 = baseY;
            }
            else
            {
                newLink.x1 = links[links.Count - 1].x2;
                newLink.y1 = links[links.Count - 1].y2;
            }
            links.Add(newLink);
        }
        double targetX = 0;
        double targetY = 0;
        public void moveToThePoint(double x, double y)
        {
            drawLine(Color.Red, 2, x, y, x + 1, y + 1);


            targetX = toLocalCS_X(x);
            targetY = toLocalCS_Y(y);
            double A; ;
            if (targetX > 0)
                A = Math.Atan(targetY / targetX);
            else
                A = Math.Atan(targetY / targetX) - (Math.PI * 2 / 2);

            for (int i = 0; i < links.Count; i++)
            {
                setLinkAngle(i, A);
            }
            double e = 1000;
            //Math.Sqrt(((x - links[links.Count - 1].x2) * (x - links[links.Count - 1].x2)) + ((links[links.Count - 1].y2) * (links[links.Count - 1].y2)));
            double oldE = 1000;
            while (e <= oldE)
            {
                double df = 0.05;

                for (int i = 0; i < links.Count / 2; i++)
                {
                    setLinkAngle(i, links[i].angle + (df * (links[i + links.Count / 2].l / links[i].l)));
                }
                for (int i = links.Count / 2; i < links.Count; i++)
                {
                    setLinkAngle(i, links[i].angle - (df * (links[i - links.Count / 2].l / links[i].l)));
                }
                oldE = e;
                e = Math.Sqrt(((x - links[links.Count - 1].x2) * (x - links[links.Count - 1].x2)) + ((y - links[links.Count - 1].y2) * (y - links[links.Count - 1].y2)));
              
            }


            // double e = Math.Sqrt(((x - links[links.Count - 1].x2) * (x - links[links.Count - 1].x2)) + ((links[links.Count - 1].y2) * (links[links.Count - 1].y2)));

        }
        public double toGlobalCS_X(double x)
        {
            return x + baseX;
        }
        public double toGlobalCS_Y(double y)
        {
            return baseY - y;
        }
        public double toLocalCS_X(double x)
        {
            return x - baseX;
        }
        public double toLocalCS_Y(double y)
        {
            return baseY - y;
        }
        public void draw(Form1 form1,double x,double y)
        {
            drawLine(Color.Red, 1, x, y, x+1, y+1);

            //РИСОВАТЬ ЗДЕСЬ


            for (int i = 0; i < links.Count; i++)
                drawLine(Color.White, 1, links[i].x1, links[i].y1, links[i].x2, links[i].y2);
        }
        public void refresh()
        {



            picBox.Image = bitmap;
            form1.voidDelegate = new Form1.VoidDelegate(form1.Refresh);
            bitmap = new Bitmap(picBox.Width, picBox.Height);
            g = Graphics.FromImage(bitmap);
        }

        public delegate void DrawStringDelegate(string s, double depth, double x, double y);
        public void drawString(string s, double depth, double x, double y)
        {
            if (picBox.InvokeRequired)
            {
                picBox.Invoke(new DrawStringDelegate(drawString), new Object[] { s, depth, x, y }); // вызываем эту же функцию обновления состояния, но уже в UI-потоке
            }
            else
            {
                if (x > picBox.Width)
                    picBox.Width = Convert.ToInt16(x);
                else
                if (y > picBox.Height)
                    picBox.Height = Convert.ToInt16(y);
                else
                    try
                    {
                        g.DrawString(s, new Font(form1.logBox.Font.Name, Convert.ToInt16(depth)), Brushes.White, new Point(Convert.ToInt16(Math.Round(x)), Convert.ToInt16(Math.Round(y))));
                    }
                    catch { }
            }
        }

        public delegate void DrawStringDelegate2(string s, Brush brush, double depth, double x, double y);
        /////////////////////////////////Brushes.[Color]
        public void drawString(string s, Brush brush, double depth, double x, double y)
        {
            if (picBox.InvokeRequired)
            {
                picBox.Invoke(new DrawStringDelegate2(drawString), new Object[] { s, brush, depth, x, y }); // вызываем эту же функцию обновления состояния, но уже в UI-потоке
            }
            else
            {
                if (x > picBox.Width)
                    picBox.Width = Convert.ToInt16(x);
                else
                if (y > picBox.Height)
                    picBox.Height = Convert.ToInt16(y);
                else
                    try
                    {
                        g.DrawString(s, new Font(form1.logBox.Font.Name, Convert.ToInt16(depth)), brush, new Point(Convert.ToInt16(Math.Round(x)), Convert.ToInt16(Math.Round(y))));
                    }
                    catch { }
            }
        }

        public delegate void DrawLineDelegate(Color col, double depth, double x1, double y1, double x2, double y2);
        public void drawLine(Color col, double depth, double x1, double y1, double x2, double y2)
        {
            if (picBox.InvokeRequired)
            {
                picBox.Invoke(new DrawLineDelegate(drawLine), new Object[] { col, depth, x1, y1, x2, y2 }); // вызываем эту же функцию обновления состояния, но уже в UI-потоке
            }
            else
            {
                if (x1 > picBox.Width)
                    picBox.Width = Convert.ToInt16(x1);
                else
                if (x2 > picBox.Width)
                    picBox.Width = Convert.ToInt16(x2);
                else
                if (y1 > picBox.Height)
                    picBox.Height = Convert.ToInt16(y1);
                else
                if (y2 > picBox.Height)
                    picBox.Height = Convert.ToInt16(y2);
                else
                    g.DrawLine(new Pen(col, Convert.ToInt16(depth)), Convert.ToInt16(Math.Round(x1)), Convert.ToInt16(Math.Round(y1)), Convert.ToInt16(Math.Round(x2)), Convert.ToInt16(Math.Round(y2)));
            }
        }
    }
}

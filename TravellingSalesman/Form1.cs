using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravellingSalesman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            g.FillRectangle(new SolidBrush(Color.Aquamarine), 0, 0, 512, 512);
            pictureBox.Image = image;
        }

        public static Random gen = new Random();
        static Bitmap image = new Bitmap(512, 512);
        static Graphics g = Graphics.FromImage(image);
        static Point[] arr;

        private void button1_Click(object sender, EventArgs e)
        {
            g.FillRectangle(new SolidBrush(Color.Aquamarine), 0, 0, 512, 512);
            arr = new Point[(int)QuantityOfPoints.Value];          

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new Point((float)(gen.NextDouble() * 500 + 6), (float)(gen.NextDouble() * 500 + 6));
                g.FillEllipse(new SolidBrush(Color.Red), arr[i].X - 2, arr[i].Y - 2, 5, 5);
            }

            pictureBox.Image = image;
            pictureBox.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            g.FillRectangle(new SolidBrush(Color.Aquamarine), 0, 0, 512, 512);           

            if (arr != null)
            {
                int[] path = new int[arr.Length];

                for (int i = 0; i < arr.Length; i++)
                {
                    g.FillEllipse(new SolidBrush(Color.Red), arr[i].X - 2, arr[i].Y - 2, 5, 5);
                }

                double.TryParse(textBox2.Text, out double tmax);
                double tmin = 1;

                DateTime time = DateTime.Now;
                path = Annealing(arr, ref tmax, ref tmin);

                textBox1.AppendText($"Time: {DateTime.Now - time}\r\n");
                textBox1.AppendText($"Iterations: {tmax}, distance: {tmin:F3}\r\n");

                DrawPath(path);
                pictureBox.Image = image;
                pictureBox.Invalidate();
            }        
        }

        public static void DrawPath(int[] candidate)
        {
            Pen p = new Pen(new SolidBrush(Color.Black));

            for (int i = 0; i < candidate.Length - 1; i++)
            {
                g.DrawLine(p, arr[candidate[i]].X, arr[candidate[i]].Y, arr[candidate[i + 1]].X, arr[candidate[i + 1]].Y);
            }

            g.DrawLine(p, arr[candidate[0]].X, arr[candidate[0]].Y, arr[candidate[arr.Length - 1]].X, arr[candidate[arr.Length - 1]].Y);
        }

        public static int[] Annealing(Point[] arr, ref double tmax, ref double tmin)
        {
            int[] candidate = new int[arr.Length];
            int[] newcandidate = new int[arr.Length];
            int num = 1;

            for (int i = 0; i < arr.Length ; i++)
            {
                candidate[i] = i;
            }

            double candval = F(arr, candidate);

            double bestval = candval;
            int[] bestarr = new int[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                bestarr[i] = candidate[i];
            }

            while (tmax > tmin)
            {              
                newcandidate = NewCandidate1(candidate);
                double newcandval = F(arr, newcandidate);

                if (candval >= newcandval)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        candidate[i] = newcandidate[i];
                    }

                    candval = newcandval;
                    if(newcandval < bestval)
                    {
                        bestval = newcandval;

                        for (int i = 0; i < arr.Length; i++)
                        {
                            bestarr[i] = newcandidate[i];
                        }
                    } 
                }                  
                else if (GaussianNorm(candval - newcandval, tmax))
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        candidate[i] = newcandidate[i];
                    }

                    candval = newcandval;
                }

                tmax *= 1 - 0.0003;

                num++;
            }

            tmax = num;
            tmin = bestval;

            return bestarr;
        }

        private static bool GaussianNorm(double factor, double temperature)
        {
            if (gen.NextDouble() < Math.Exp(factor / temperature))
                return true;
            else
                return false;
        }

        private static double F(Point[] arr, int[] candidate)
        {
            double distance = 0;

            for (int i = 0; i < arr.Length - 1; i++)
            {
                distance += Distance(arr[candidate[i]], arr[candidate[i + 1]]);
            }

            return distance + Distance(arr[candidate[0]], arr[candidate[arr.Length - 1]]);
        }

        public static int[] NewCandidate1(int[] candidate)
        {
            int[] reversed = new int[candidate.Length];

            for (int i = 0; i < candidate.Length; i++)
            {
                reversed[i] = candidate[i];
            }

            int temp = gen.Next(0, arr.Length - 2);

            Array.Reverse(reversed, temp, gen.Next(2, candidate.Length - temp));

            return reversed;
        }

        public static int[] NewCandidate2(int[] candidate)
        {
            int[] reversed = new int[candidate.Length];

            for (int i = 0; i < candidate.Length; i++)
            {
                reversed[i] = candidate[i];
            }

            for(int i = 0; i < 2; i++)
            {
                int a = gen.Next(0, arr.Length - 1);
                int b = gen.Next(0, arr.Length - 1);

                int t = reversed[a];
                reversed[a] = reversed[b];
                reversed[b] = t;
            }

            return reversed;
        }

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
    }

    public class Point
    {
        public float X { get; set; } 

        public float Y { get; set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }     
    }
}

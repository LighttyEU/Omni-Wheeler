using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Omni_Wheeler
{
    public partial class Form1 : Form
    {
        Bitmap streckenBild;
        int aktKurs = 1;
        float StrBreite = 69f;

        public class Strasse
        {
            public List<PointF> Mitte = new List<PointF>();
            public List<PointF> Links = new List<PointF>();
            public List<PointF> Rechts = new List<PointF>();
        }

        public Form1()
        {
            InitializeComponent();
            ZeichneKurs();
        }

        private void btnKurs1_Click(object sender, EventArgs e)
        {
            aktKurs = 1;
            ZeichneKurs();
        }

        private void btnKurs2_Click(object sender, EventArgs e)
        {
            aktKurs = 2;
            ZeichneKurs();
        }

        private void ZeichneKurs()
        {
            if (streckenBild != null)
            {
                streckenBild.Dispose();
            }

            streckenBild = new Bitmap(picStrecke.Width, picStrecke.Height);

            Graphics g = Graphics.FromImage(streckenBild);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);


            List<PointF> mLinie = GetmLinie();
            Strasse strasse = ErzStr(mLinie, StrBreite);
            ZeichStr(g, strasse);

            g.Dispose();

            picStrecke.Image = streckenBild;
        }

        private List<PointF> GetmLinie()
        {
            if (aktKurs == 1)
            {
                return K1Punkte();
            }

            return K2Punkte();
        }

        private List<PointF> K1Punkte()
        {
            return new List<PointF>()
            {
                new Point(50, 250),
                new Point(150, 250),
                new Point(250, 200),
                new Point(350, 150),
                new Point(500, 150)
            };
        }

        private List<PointF> K2Punkte()
        {
            return new List<PointF>()
            {
                new Point(50, 200),
                new Point(150, 100),
                new Point(250, 250),
                new Point(350, 100),
                new Point(500, 200)
            };
        }

        private Strasse ErzStr(List<PointF> mLinie, float breite)
        {
            Strasse strasse = new Strasse();
            for (int i = 0; i < mLinie.Count - 1; i++)
            {
                PointF p1 = mLinie[i];
                PointF p2 = mLinie[i + 1];
                strasse.Mitte.Add(p1);
                strasse.Mitte.Add(p2);
                float dx = p2.X - p1.X;
                float dy = p2.Y - p1.Y;
                float length = (float)Math.Sqrt(dx * dx + dy * dy);
                float X = -dy / length * breite / 2;
                float Y = dx / length * breite / 2;
                strasse.Links.Add(new PointF(p1.X + X, p1.Y + Y));
                strasse.Links.Add(new PointF(p2.X + X, p2.Y + Y));
                strasse.Rechts.Add(new PointF(p1.X - X, p1.Y - Y));
                strasse.Rechts.Add(new PointF(p2.X - X, p2.Y - Y));
            }
            return strasse;
        }

        private void ZeichStr(Graphics g, Strasse strasse)
        {
            for (int i = 0; i < strasse.Mitte.Count - 1; i += 2)
            {
                PointF p1 = strasse.Mitte[i];
                PointF p2 = strasse.Mitte[i + 1];
                g.DrawLine(Pens.Gray, p1, p2);
            }
            for (int i = 0; i < strasse.Links.Count - 1; i += 2)
            {
                PointF p1 = strasse.Links[i];
                PointF p2 = strasse.Links[i + 1];
                g.DrawLine(Pens.Black, p1, p2);
            }
            for (int i = 0; i < strasse.Rechts.Count - 1; i += 2)
            {
                PointF p1 = strasse.Rechts[i];
                PointF p2 = strasse.Rechts[i + 1];
                g.DrawLine(Pens.Black, p1, p2);
            }
        }




    }
}

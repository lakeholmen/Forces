// przewijanie w przód i w tył
// programowanie ruchu ciała
// programowanie toru i w tym celu obliczanie niezbędnych manewrów
// 3D
// zoom, przesuwanie, etc.
// ręczne dokładanie impulsu (w ramach programowania ? ) 
// pokazywanie lub nie sił, prędkości etc.
// zmiana masy etc.
// ustalanie koloru na początku (+podpisów), żeby wszystko było rysowane jednakowo
// definiowanie jakos calego zestawu (xml?)



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forces
{
    public class World
    {
        public static    double forceFac = 0.01;
    }

    public class Body
    {

        public List<PointD> track = new List<PointD>();
        public List<PointD> velocity = new List<PointD>();
        public List<PointD> forces = new List<PointD>();
        public double mass;
        public PointD startP;
        public PointD startVel;
        public string name;

        public Body(PointD start, PointD vel,string n, double m)
        {
            name = n;
            track.Add(start);
            velocity.Add(vel);
            forces.Add(PointD.Zero);
            mass = m;
            startP = start;
            startVel = vel;
            current = start;
            currentV = vel;
            currentF = PointD.Zero;
        }

        double prevD = 0;
        double minD = Double.MaxValue;
        double maxD = Double.MinValue;
        double prevAngle = -Math.PI;
        double prevT = 0;
        long i = 0;
        public PointD current;
        public PointD currentV;
        public PointD currentF;
        public PointD newPos;
        public PointD newV;
        public PointD newF;

        public void SetCurrent(double dt,PointD f,double t)
        {
            var c  = current;
            var v  = currentV;
            newPos = c + v * dt;
            newV   = v + f * dt;
            newF   = f;
        }

        internal void Commit()
        {
            current = newPos;
            currentV = newV;
            currentF = newF;
            ++i;
            if (i%1000==0)
            {
                track.Add(current);
                forces.Add(currentF);
                velocity.Add(currentV);
            }
        }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            InitializeTrack();
        }

        static PointD center = PointD.Zero;
        static PointD mx = new PointD(800, 400,100);
        static PointD start1 = new PointD(00, 200,50);
        static PointD start2 = new PointD(200, 200,80);
        static PointD start3 = new PointD(-1200, 200,0);
        static PointD startVel1 = new PointD(5.4, 0,0);
        static PointD startVel2 = new PointD(3.2, 0,0);
        static PointD startVel3 = new PointD(1.6, 0,0);

        List<Body> bodies = new List<Forces.Body>();

        Body b1 = new Body(start1,startVel1,"1",100);
        Body b2 = new Body(start2,startVel2,"2", 10);
        Body b3 = new Body(start3, startVel3,"3", 10);
        Body star = new Body(PointD.Zero, PointD.Zero, "S", 500000);

        private void InitializeTrack()
        {
            bodies.Add(b1);
            bodies.Add(b2);
          //  bodies.Add(b3);
            bodies.Add(star);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for(int i=0;i<50000;++i)
                Progress(1/1000.0);
            Redraw();
        }

        private void Redraw()
        {
            Invalidate();
            Refresh();
        }

        double timeMs = 0;

        private void Progress(double t)
        {
            timeMs += t;
            bodies.ForEach(x => move(t, x, timeMs));
            bodies.ForEach(x => x.Commit());
        }

        private void move(double timeDelta, Body b, double finalTime)
        {
            var force = PointD.Zero;
            foreach (var c in bodies)
            {
                if (c == b) continue;
                var fac = World.forceFac * c.mass;
                var d = c.current - b.current;
                var dist = d.Length;
                if (dist < 0.0001) continue;
                force += d.Normalize() * (fac / (dist * dist));
            }
            b.SetCurrent(timeDelta, force, finalTime);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.White);
            paintClock(g);
            paintTrack(g);
            var ctr = star.current.toPF(mx, sc);
            g.DrawEllipse(Pens.Black, ctr.X, ctr.Y, 5, 5);
        }

        Pen forcePen = new Pen(Color.FromArgb(50,255,0,0),0.1f);
        double sc = 0.5;
        private void paintTrack(Graphics g)
        {
            DrawBodyTrack(g, b1, new Pen(Color.Blue, 2));
            DrawBodyTrack(g, b2, new Pen(Color.Green, 2));
          //  DrawBodyTrack(g, b3, new Pen(Color.Pink, 2));
        }

        private void DrawBodyTrack(Graphics g, Body body,Pen trackPen)
        {
            var _track = body.track.Where((x, i) => i % 1 == 0).ToList();
            var _forces = body.forces.Where((x, i) => i % 1 == 0).ToList();
            if (_track.Count > 1)
                g.DrawCurve(trackPen, _track.Select(x => x.toPF(mx, sc)).ToArray());
            for (int i = 0; i < _track.Count - 1; ++i)
                if (_forces[i].Length > 0)
                    g.DrawLine(forcePen, _track[i].toPF(mx, sc), (_track[i] + _forces[i] * 500).toPF(mx, sc));
           var cur = body.current.toPF(mx, sc);
           g.DrawLine(forcePen, cur, (body.current + body.currentF * 1000).toPF(mx, sc));
           g.DrawEllipse(trackPen, cur.X-3, cur.Y-3, 6, 6);
        }

        Rectangle clockPos = new Rectangle(20, 20, 100, 100);
        private bool alreadySwitched;

        private void paintClock(Graphics g)
        {
            g.DrawEllipse(Pens.LightGray, clockPos);
            var rad = clockPos.Width / 2 * 0.9f;
            var ctr = new PointF(clockPos.Left + clockPos.Width / 2, clockPos.Top + clockPos.Height / 2);
            g.DrawLine(Pens.Gray, ctr, handdPos(ctr, rad, (timeMs % 60) / 60f));
            g.DrawLine(Pens.Gray, ctr, handdPos(ctr, rad*0.8f, (timeMs % (60*12)) / (60 * 12f)));
        }

        
        private PointF handdPos(PointF ctr, float rad,double v)
        {
            var dx = rad * Math.Sin(v*Math.PI*2);
            var dy = -rad * Math.Cos(v * Math.PI * 2);
            return new PointF(ctr.X + (float)dx, ctr.Y + (float)dy);
        }
    }
}

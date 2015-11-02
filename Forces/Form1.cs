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

        public List<Point3D> track = new List<Point3D>();
        public List<Point3D> velocity = new List<Point3D>();
        public List<Point3D> forces = new List<Point3D>();
        public double mass;
        public Point3D startP;
        public Point3D startVel;
        public string name;

        public Body(Point3D start, Point3D vel,string n, double m)
        {
            name = n;
            track.Add(start);
            velocity.Add(vel);
            forces.Add(Point3D.Zero);
            mass = m;
            startP = start;
            startVel = vel;
            current = start;
            currentV = vel;
            currentF = Point3D.Zero;
        }

        double prevD = 0;
        double prevAngle = -Math.PI;
        double prevT = 0;
        long i = 0;
        public Point3D current;
        public Point3D currentV;
        public Point3D currentF;
        public Point3D newPos;
        public Point3D newV;
        public Point3D newF;

        public void SetCurrent(double dt,Point3D f,double t)
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

        static Point3D center = Point3D.Zero;
        static Matrix3D mx =  Matrix3D.D(200, 400, 0) * Matrix3D.S(0.5) * Matrix3D.X(Math.PI / 2);
        static Point3D start1 = new Point3D(200, 200,100);
        static Point3D start2 = new Point3D(200, 200,0);
        static Point3D start3 = new Point3D(-1200, 200,0);
        static Point3D startVel1 = new Point3D(5.4, 0,0);
        static Point3D startVel2 = new Point3D(3.2, 0,0);
        static Point3D startVel3 = new Point3D(1.6, 0,0);

        List<Body> bodies = new List<Forces.Body>();

        Body b1 = new Body(start1,startVel1,"1",100);
        Body b2 = new Body(start2,startVel2,"2", 10);
        Body b3 = new Body(start3, startVel3,"3", 10);
        Body star = new Body(Point3D.Zero, Point3D.Zero, "S", 500000);

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
            var force = Point3D.Zero;
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
            var ctr = star.current.toPF(mx);
            g.DrawEllipse(Pens.Black, ctr.X, ctr.Y, 5, 5);
        }

        Pen forcePen = new Pen(Color.FromArgb(50,255,0,0),0.1f);
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
            var cv = _track.Select(x => x.toPF(mx)).ToArray();
            if (cv.Length > 1) g.DrawCurve(trackPen, cv);
            for (int i = 0; i < _track.Count - 1; ++i)
                if (_forces[i].Length > 0)
                    g.DrawLine(forcePen, _track[i].toPF(mx), (_track[i] + _forces[i] * 500).toPF(mx));
           var cur = body.current.toPF(mx);
          // g.DrawLine(forcePen, cur, (body.current + body.currentF * 1000).toPF(mx));
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

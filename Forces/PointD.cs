using System;
using System.Collections.Generic;
using System.Drawing;

namespace Forces
{
    public class PointD
    {
        public double X;
        public double Y;
        public double Z;

        public PointD(double x, double y,double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static PointD Zero = new PointD(0, 0,0);



        public double Length { get { return Math.Sqrt(X * X + Y * Y+Z*Z); } }

        internal PointF toPF()
        {
            return new PointF((float)X, (float)Y);
        }

        public static PointD operator +(PointD @this,PointD that)
        {
            return new PointD(@this.X + that.X, @this.Y + that.Y,@this.Z+that.Z); 
        }

        public static PointD operator -(PointD @this, PointD that)
        {
            return new PointD(@this.X - that.X, @this.Y - that.Y, @this.Z - that.Z);
        }

        public static PointD operator *(PointD @this, double fac)
        {
            return new PointD(@this.X *fac, @this.Y *fac,@this.Z*fac);
        }

        public static implicit operator PointF(PointD @this)
        {
            return @this.toPF();
        }

        internal PointF toPF(PointF mx, double scale)
        {
            return new PointF((float)(X*scale) + mx.X, (float)(Y * scale) + mx.Y);
        }

        internal PointD Normalize()
        {
            return this * (1 / Length);
        }
    }
}
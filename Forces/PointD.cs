using System;
using System.Collections.Generic;
using System.Drawing;

namespace Forces
{
   

    public class Point3D
    {
        public double X;
        public double Y;
        public double Z;

        public Point3D(double x, double y,double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Point3D Zero = new Point3D(0, 0,0);



        public double Length { get { return Math.Sqrt(X * X + Y * Y+Z*Z); } }

        internal PointF toPF()
        {
            return new PointF((float)X, (float)Y);
        }

        public static Point3D operator +(Point3D @this,Point3D that)
        {
            return new Point3D(@this.X + that.X, @this.Y + that.Y,@this.Z+that.Z); 
        }

        public static Point3D operator -(Point3D @this, Point3D that)
        {
            return new Point3D(@this.X - that.X, @this.Y - that.Y, @this.Z - that.Z);
        }

        public static Point3D operator *(Point3D @this, double fac)
        {
            return new Point3D(@this.X *fac, @this.Y *fac,@this.Z*fac);
        }

        public static implicit operator PointF(Point3D @this)
        {
            return @this.toPF();
        }

        internal PointF toPF(Matrix3D mx)
        {
            var mul = mx * this;
            return mul.toPF();
        }

        internal Point3D Normalize()
        {
            return this * (1 / Length);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace Forces
{
    public class Matrix3D : Object
    {
        public double[] v = new double[16];

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Matrix3D)) return false;
            if (Object.ReferenceEquals(this, obj)) return true;
            return eq((Matrix3D)obj, this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Matrix3D m1,Matrix3D m2)
        {
            return eq(m1, m2);
        }

        private static bool eq(Matrix3D m1, Matrix3D m2)
        {
            if (Object.ReferenceEquals(m1, null) || Object.ReferenceEquals(m2, null)) return false;
            for (int i = 0; i < 16; ++i)
                if (Math.Abs(m1.v[i] - m2.v[i]) > 0.0000001) return false;
            return true;
        }

        public static bool operator !=(Matrix3D m1, Matrix3D m2)
        {
            return !eq(m1,m2);
        }

        public Matrix3D()
        { }

        public Matrix3D(params double[] mx)
        {
            v = mx;
        }

        public Matrix3D(double xx,double yy, double zz)
        {
            v = new double[] { xx, 0, 0, 0, 0, yy, 0, 0, 0, 0, zz, 0, 0, 0, 0, 1 };
        }


        public static Matrix3D One
        {
            get { return new Matrix3D(1,1,1); }
        }

        public static Point3D operator*(Matrix3D mx, Point3D p)
        {
            var x = p.X;
            var y = p.Y;
            var z = p.Z;
            var m = mx.v;
            return new Point3D(
                m[0] * x + m[1] * y + m[2] * z + m[3],
                m[4] * x + m[5] * y + m[6] * z + m[7],
                m[8] * x + m[9] * y + m[10] * z + m[11]
            );
        }

        public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
        {
            var m = m1.v;
            var n = m2.v;
             return new Matrix3D(
                m[0] * n[0] + m[1] * n[4] + m[2] * n[8] + m[3]*n[12],
                m[0] * n[1] + m[1] * n[5] + m[2] * n[9] + m[3] * n[13],
                m[0] * n[2] + m[1] * n[6] + m[2] * n[10] + m[3] * n[14],
                m[0] * n[3] + m[1] * n[7] + m[2] * n[11] + m[3] * n[15],

                m[4] * n[0] + m[5] * n[4] + m[6] * n[8]  + m[7] * n[12],
                m[4] * n[1] + m[5] * n[5] + m[6] * n[9]  + m[7] * n[13],
                m[4] * n[2] + m[5] * n[6] + m[6] * n[10] + m[7] * n[14],
                m[4] * n[3] + m[5] * n[7] + m[6] * n[11] + m[7] * n[15],

                m[8] * n[0] + m[9] * n[4] + m[10] * n[8]  + m[11] * n[12],
                m[8] * n[1] + m[9] * n[5] + m[10] * n[9]  + m[11] * n[13],
                m[8] * n[2] + m[9] * n[6] + m[10] * n[10] + m[11] * n[14],
                m[8] * n[3] + m[9] * n[7] + m[10] * n[11] + m[11] * n[15],

                m[12] * n[0] + m[13] * n[4] + m[14] * n[8]  + m[15] * n[12],
                m[12] * n[1] + m[13] * n[5] + m[14] * n[9]  + m[15] * n[13],
                m[12] * n[2] + m[13] * n[6] + m[14] * n[10] + m[15] * n[14],
                m[12] * n[3] + m[13] * n[7] + m[14] * n[11] + m[15] * n[15]
            );
        }
    }

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

        internal PointF toPF(PointF mx, double scale)
        {
            return new PointF((float)(X*scale) + mx.X, (float)(Y * scale) + mx.Y);
        }

        internal Point3D Normalize()
        {
            return this * (1 / Length);
        }
    }
}
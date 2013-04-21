using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BradleyXboxUtils
{
    public class RotatedRectangle
    {
        public Point p1;
        public Point p2;
        public Point p3;
        public Point p4;
        public RotatedRectangle(Point pt1,Point pt2,Point pt3, Point pt4)
        {
            p1 = pt1;
            p2 = pt2;
            p3 = pt3;
            p4 = pt4;
        }
        public RotatedRectangle(Point center, double width, double height, double rotation)
        {
            define(center, width, height, rotation);
        }
        public void update(Point c, double width, double height, double rotation)
        {
            define(c, width, height, rotation);
        }
        private void define(Point center, double width, double height, double rotation)
        {
            Point px1 = new Point((int)(center.X + UTIL.magD(width / 2, rotation).X), (int)(center.Y + UTIL.magD(width/2, rotation).Y));
            Point px2 = new Point((int)(center.X - UTIL.magD(width / 2, rotation).X), (int)(center.Y - UTIL.magD(width / 2, rotation).Y));
            p1 = new Point((int)(px1.X + UTIL.magD(height / 2, rotation - Math.PI / 2).X), (int)(px1.Y + UTIL.magD(height / 2, rotation - Math.PI / 2).Y));
            p2 = new Point((int)(px1.X - UTIL.magD(height / 2, rotation - Math.PI / 2).X), (int)(px1.Y - UTIL.magD(height / 2, rotation - Math.PI / 2).Y));
            p3 = new Point((int)(px2.X - UTIL.magD(height / 2, rotation - Math.PI / 2).X), (int)(px2.Y - UTIL.magD(height / 2, rotation - Math.PI / 2).Y));
            p4 = new Point((int)(px2.X + UTIL.magD(height / 2, rotation - Math.PI / 2).X), (int)(px2.Y + UTIL.magD(height / 2, rotation - Math.PI / 2).Y));
        }
        public Boolean Contains(Point p)
        {
            double m1 = (double)(p2.Y - p1.Y) / (p2.X - p1.X);
            double m2 = (double)(p3.Y - p2.Y) / (p3.X - p2.X);

            double b1a = p1.Y - m1 * p1.X;
            double b1b = p2.Y - m1 * p1.X;
            double b11 = Math.Min(b1a, b1b);
            double b12 = Math.Max(b1a, b1b);
            double b1n = p.Y - m1 * p.X;

            double b2a = p2.Y - m2 * p2.X;
            double b2b = p3.Y - m2 * p3.X;
            double b21 = Math.Min(b2a, b2b);
            double b22 = Math.Max(b2a, b2b);
            double b2n = p.Y - m2 * p.X;

            if ((b1n > b11 && b1n < b12) && (b2n > b21 && b2n < b22))
                return true;
            return false;
        }
    }
}

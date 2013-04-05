using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FRCVirtualRobotics
{
    class Frisbee
    {
        private Vector2 location;
        private double direction;
        private Vector2 origin;
        private float rotation;

        private static Texture2D image;
        public static double SPEED;

        private int countdown;
        private Boolean red;
        private static List<Frisbee> frisbees;

        public Frisbee(Vector2 loc, double dir)
        {
            location = new Vector2(loc.X, loc.Y);
            direction = dir;
            countdown = 70;
            red = true;
            rotation = 0;
            origin = new Vector2(image.Width / 2, image.Height / 2);
        }
        public Frisbee(Vector2 loc, double dir, Boolean r)
        {
            location = new Vector2(loc.X, loc.Y);
            direction = dir;
            countdown = 70;
            red = r;
            rotation = 0;
            origin = new Vector2(image.Width / 2, image.Height / 2);
        }

        public int run()
        {
            countdown--;
            location += magD(SPEED, direction);
            rotation += (float) Math.PI / 6;

            if (countdown == 0 || offScreen())
            {
                removeSelfFromList();
                return -1;
            }
            return 0;
        }

        public static Texture2D getImage()
        {
            return image;
        }

        public static void setImage(Texture2D pic)
        {
            image = pic;
        }
        public static void setFrisbees(List<Frisbee> frisb)
        {
            frisbees = frisb;
        }

        public Vector2 getOrigin()
        {
            return origin;
        }

        public float getDirection()
        {
            return rotation;
        }

        public Vector2 getLocation()
        {
            return location;
        }

        public Boolean getRed()
        {
            return red;
        }

        public Color getColor()
        {
            if (red)
                return Color.Red;
            else
                return Color.Blue;
        }

        private Vector2 magD(double mag, double dir)
        {
            int x = (int)(mag * Math.Cos(dir));
            int y = (int)(mag * Math.Sin(dir));
            return new Vector2(x, y);
        }

        private void removeSelfFromList()
        {
            frisbees.Remove(this);
        }
        
        private Boolean offScreen()
        {
            return  (location.X < 0) || (location.Y < 0) || (location.X > Field.X) || (location.Y > Field.Y);
        }
    }
}

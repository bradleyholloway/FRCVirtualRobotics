using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BradleyXboxUtils;

namespace FRCVirtualRobotics
{
    public class Frisbee
    {
        private Vector2 location;
        private double direction;
        private Vector2 origin;
        private float rotation;
        private static Field field;

        private static Texture2D image;
        public static double SPEED;

        private int countdown;
        private Boolean red;
        private Boolean collided;
        private static List<Frisbee> frisbees;

        public Frisbee(Vector2 loc, double dir)
        {
            location = new Vector2(loc.X, loc.Y);
            direction = dir % (Math.PI * 2);
            countdown = 80;
            red = true;
            rotation = 0;
            collided = false;
            origin = new Vector2(image.Width / 2, image.Height / 2);
        }
        public Frisbee(Vector2 loc, double dir, Boolean r)
        {
            location = new Vector2(loc.X, loc.Y);
            direction = UTIL.normalizeDirection(dir);
            countdown = 80;
            red = r;
            rotation = 0;
            collided = false;
            origin = new Vector2(image.Width / 2, image.Height / 2);
        }
        public static void setField(Field f)
        {
            field = f;
        }
        public Boolean colidedWith(Frisbee frisbee2)
        {
            if (collided)
                return false;
            return BradleyXboxUtils.UTIL.distance(location, frisbee2.getLocation()) <= image.Width * .06;
        }
        public void checkCollision(Frisbee frisbee2)
        {
           if(!frisbee2.Equals(this))
            if (colidedWith(frisbee2))
            {
                collided = true;
                double avg = (this.direction + frisbee2.getDirection()) / 2;
                double diff = avg - Math.Min(this.direction, frisbee2.getDirection());
                if (diff > Math.PI / 2)
                {
                    avg += Math.PI;
                    diff = Math.PI - diff;
                }
                diff /= 3;
                if (this.direction > frisbee2.getDirection())
                {
                    this.setDirection(avg - diff);
                    frisbee2.setDirection(avg + diff);
                }
                else
                {
                    this.setDirection(avg + diff);
                    frisbee2.setDirection(avg - diff);
                }
                
            }
        }
        public void setDirection(double dir)
        {
            direction = dir;
        }


        public int run()
        {
            countdown--;
            location += magD(SPEED, direction);
            rotation += (float) Math.PI / 6;

            if (countdown == 0 || offScreen())
            {
                field.addFrisbee(!red);
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

        public void removeSelfFromList()
        {
            frisbees.Remove(this);
        }
        
        private Boolean offScreen()
        {
            return (location.X < 0) || (location.Y < 0) || (location.X > Field.X) || (location.Y > Field.Y);
        }
    }
}

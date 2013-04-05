using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FRC_Virtual_Robotics
{
    class IterativeRobot
    {
        private double leftMotorSpeed;
        private double rightMotorSpeed;
        private double directionForward;
        private double magnitude;
        private Boolean red;
        private int scalar;
        private static Texture2D image;
        private Vector2 location;
        private int ammo;
        private int windowX;
        private int windowY;

        public IterativeRobot(int maxSpeed, GraphicsDevice window, Boolean r)
        {
            leftMotorSpeed = 0;
            rightMotorSpeed = 0;
            scalar = maxSpeed;
            directionForward = Math.PI * 3 / 2;
            location = new Vector2(0, 0);
            image = null;
            windowX = window.Viewport.Width;
            windowY = window.Viewport.Height;
            red = r;
            ammo = 3;
            reset();
        }

        public void run()
        {
            magnitude = (leftMotorSpeed + rightMotorSpeed) / 2 * scalar;
            directionForward += .1 * (rightMotorSpeed - leftMotorSpeed);

            
            if(!((location+magD(magnitude,directionForward)).X<75 || (location+magD(magnitude,directionForward)).X>windowX-95) &&
                !((location+magD(magnitude,directionForward)).Y<50 || (location+magD(magnitude,directionForward)).Y>windowY-50))
                location += magD(magnitude, directionForward);
        }

        public Color getColor()
        {
            if (red)
                return Color.Red;
            else
                return Color.Blue;
        }

        public Boolean getRed()
        {
            return red;
        }

        public void setMotorValues(double left, double right)
        {
            leftMotorSpeed = left;
            rightMotorSpeed = right;
        }

        public float getDirection()
        {
            return (float)directionForward;
        }

        public Vector2 getOrigin()
        {
            return new Vector2(((float)image.Width) / 2, ((float)image.Height) / 2);
        }

        public Texture2D getImage()
        {
            return image;//with rotation
        }

        public Vector2 getLocation()
        {
            return location;
        }

        private Vector2 magD(double mag, double dir)
        {
            int x = (int)(mag * Math.Cos(dir));
            int y = (int)(mag * Math.Sin(dir));
            return new Vector2(x, y);
        }

        public static void setImage(Texture2D picture)
        {
            image = picture;
        }

        public void reset()
        {
            if (red)
            {
                location = new Vector2(100, 100);
                directionForward = 0;
            }
            else
            {
                location = new Vector2(windowX - 150, 100);
                directionForward = Math.PI;
            } 
        }
        public Boolean fire()
        {
            if (ammo > 0)
            {
                ammo--;
                return true;
            }
            return false;
        }
        public void feed()
        {
            if (ammo < 4)
                ammo++;
        }
            
    }
}

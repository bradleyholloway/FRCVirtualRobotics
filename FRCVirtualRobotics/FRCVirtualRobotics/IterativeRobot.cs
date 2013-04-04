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
        private int scalar;
        private Texture2D image;
        private Vector2 location;

        public IterativeRobot(int maxSpeed)
        {
            leftMotorSpeed = 0;
            rightMotorSpeed = 0;
            scalar = maxSpeed;
            directionForward = Math.PI * 3 / 2;
            location = new Vector2(0, 0);
            image = null;
        }

        public void run()
        {
            magnitude = (leftMotorSpeed + rightMotorSpeed) / 2 * scalar;
            directionForward += .1 * (rightMotorSpeed - leftMotorSpeed);

            location += magD(magnitude, directionForward);
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

        public void setImage(Texture2D picture)
        {
            image = picture;
        }

        public void reset()
        {
            location = new Vector2(100, 100);
            directionForward = 0;
        }
    }
}

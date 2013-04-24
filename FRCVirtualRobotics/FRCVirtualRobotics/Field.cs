using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BradleyXboxUtils;
using FRC_Virtual_Robotics;

namespace FRCVirtualRobotics
{
    class Field
    {
        static int threeTop;
        static int threeBottom;
        static int twoTop1;
        static int twoBottom1;
        static int twoTop2;
        static int twoBottom2;
        static int leftX;
        static int rightX;
        public static int X;
        public static int Y;
        private int redWhiteFrisbees;
        private int redRedFrisbees;
        private int blueWhiteFrisbees;
        private int blueBlueFrisbees;
        List<FieldObjects> objects;
        static List<Point> pyramidPoints;
        static List<Point> climbPoints;
        static Point bluePyramidCenter;
        static Point redPyramidCenter;
   
        public Field(GraphicsDevice window)
        {
            X = window.Viewport.Width;
            Y = window.Viewport.Height;

            redRedFrisbees = 5;
            blueBlueFrisbees = 5;
            redWhiteFrisbees = 45;
            blueWhiteFrisbees = 45;

            objects = new List<FieldObjects>();

            //Scoring
            threeTop = (int)(.4 * Y);
            threeBottom = (int)(.6 * Y);

            twoTop1 = (int)(.1 * Y);
            twoBottom1 = (int)(.3 * Y);

            twoTop2 = (int)(.7 * Y);
            twoBottom2 = (int)(.9 * Y);

            leftX = 0+20;
            rightX = X-20;

            //Pyramid
            pyramidPoints = new List<Point>();
            climbPoints = new List<Point>();
            double levelOneFracConst = .8;
            //Blue Pyramid
            pyramidPoints.Add(new Point((int)(X * .2), (int)(Y * .4)));//8,13
            pyramidPoints.Add(new Point((int)(X * .2), (int)(Y * .7)));
            pyramidPoints.Add(new Point((int)(X * .4), (int)(Y * .4)));//3,13
            pyramidPoints.Add(new Point((int)(X * .4), (int)(Y * .7)));
            bluePyramidCenter = UTIL.midpoint(pyramidPoints.ElementAt<Point>(0), pyramidPoints.ElementAt<Point>(3));
            
            climbPoints.Add(UTIL.midpoint(pyramidPoints.ElementAt<Point>(0), pyramidPoints.ElementAt<Point>(3)));//Center Point
            climbPoints.Add(UTIL.vectorToPoint(UTIL.magD(levelOneFracConst * UTIL.distance(pyramidPoints.ElementAt<Point>(0), climbPoints.ElementAt<Point>(0)), Math.PI * 1 / 4) + UTIL.pointToVector(bluePyramidCenter)));
            climbPoints.Add(UTIL.vectorToPoint(UTIL.magD(levelOneFracConst * UTIL.distance(pyramidPoints.ElementAt<Point>(0), climbPoints.ElementAt<Point>(0)), Math.PI * 3 / 4) + UTIL.pointToVector(bluePyramidCenter)));
            climbPoints.Add(UTIL.vectorToPoint(UTIL.magD(levelOneFracConst * UTIL.distance(pyramidPoints.ElementAt<Point>(0), climbPoints.ElementAt<Point>(0)), Math.PI * 5 / 4) + UTIL.pointToVector(bluePyramidCenter)));
            climbPoints.Add(UTIL.vectorToPoint(UTIL.magD(levelOneFracConst * UTIL.distance(pyramidPoints.ElementAt<Point>(0), climbPoints.ElementAt<Point>(0)), Math.PI * 7 / 4) + UTIL.pointToVector(bluePyramidCenter)));
            //Red Pyramid
            pyramidPoints.Add(new Point((int)(X * .6), (int)(Y * .4)));
            pyramidPoints.Add(new Point((int)(X * .6), (int)(Y * .7)));
            pyramidPoints.Add(new Point((int)(X * .8), (int)(Y * .4)));
            pyramidPoints.Add(new Point((int)(X * .8), (int)(Y * .7)));
            redPyramidCenter = UTIL.midpoint(pyramidPoints.ElementAt<Point>(4), pyramidPoints.ElementAt<Point>(7));
            climbPoints.Add(UTIL.midpoint(pyramidPoints.ElementAt<Point>(4), pyramidPoints.ElementAt<Point>(7)));//Center Point
            climbPoints.Add(UTIL.vectorToPoint(UTIL.magD(levelOneFracConst * UTIL.distance(pyramidPoints.ElementAt<Point>(4), climbPoints.ElementAt<Point>(5)), Math.PI * 1 / 4) + UTIL.pointToVector(redPyramidCenter)));
            climbPoints.Add(UTIL.vectorToPoint(UTIL.magD(levelOneFracConst * UTIL.distance(pyramidPoints.ElementAt<Point>(4), climbPoints.ElementAt<Point>(5)), Math.PI * 3 / 4) + UTIL.pointToVector(redPyramidCenter)));
            climbPoints.Add(UTIL.vectorToPoint(UTIL.magD(levelOneFracConst * UTIL.distance(pyramidPoints.ElementAt<Point>(4), climbPoints.ElementAt<Point>(5)), Math.PI * 5 / 4) + UTIL.pointToVector(redPyramidCenter)));
            climbPoints.Add(UTIL.vectorToPoint(UTIL.magD(levelOneFracConst * UTIL.distance(pyramidPoints.ElementAt<Point>(4), climbPoints.ElementAt<Point>(5)), Math.PI * 7 / 4) + UTIL.pointToVector(redPyramidCenter)));
            climbPoints.Remove(UTIL.midpoint(pyramidPoints.ElementAt<Point>(0), pyramidPoints.ElementAt<Point>(3)));
            climbPoints.Remove(UTIL.midpoint(pyramidPoints.ElementAt<Point>(4), pyramidPoints.ElementAt<Point>(7)));
            bluePyramidCenter = UTIL.midpoint(pyramidPoints.ElementAt<Point>(0), pyramidPoints.ElementAt<Point>(3));
            redPyramidCenter = UTIL.midpoint(pyramidPoints.ElementAt<Point>(4), pyramidPoints.ElementAt<Point>(7));


        }
        public static List<Point> getClimbPoints()
        {
            return climbPoints;
        }
        public static List<int> getGoals()
        {
            List<int> list = new List<int>();
            list.Add(twoTop1);
            list.Add(twoBottom1);
            list.Add(threeTop);
            list.Add(threeBottom);
            list.Add(twoTop2);
            list.Add(twoBottom2);
            list.Add(leftX);
            list.Add(rightX);
            return list;
        }
        public List<FieldObjects> getObjects()
        {
            return objects;
        }

        public int score(Vector2 location, Boolean red)
        {//Left = blue goal, right = red goal
            if (red)
            {
                if (location.X >= rightX)
                {
                    if (location.Y >= twoTop1 && location.Y <= twoBottom1)
                        return 2;
                    if (location.Y >= threeTop && location.Y <= threeBottom)
                        return 3;
                    if (location.Y >= twoTop2 && location.Y <= twoBottom2)
                        return 2;
                }
            }
            else
            {
                if (location.X <= leftX)
                {
                    if (location.Y >= twoTop1 && location.Y <= twoBottom1)
                        return 2;
                    if (location.Y >= threeTop && location.Y <= threeBottom)
                        return 3;
                    if (location.Y >= twoTop2 && location.Y <= twoBottom2)
                        return 2;
                }
            }
            return 0;
        }
        public Boolean feeding(Vector2 location, Boolean red)
        {//red feeds left, and blue feeds right
            if (red)
            {//95 left tollerance
                if (location.X < 95 && (location.Y < 70 || location.Y > Y - 70))
                {
                    if (redWhiteFrisbees > 0)
                    {
                        return true;
                    }
                }
            }
            else
            {//115 right tollerance
                if (location.X > X - 95 && (location.Y < 70 || location.Y > Y - 70))
                {
                    if (blueWhiteFrisbees > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void feed(Boolean red)
        {
            if (red)
                redWhiteFrisbees--;
            else
                blueWhiteFrisbees--;
        }

        public static Boolean didCollideWithPyramid(RotatedRectangle robot)
        {
            Boolean collided = false;
            foreach (Point pyramidPoint in pyramidPoints)
            {
                if (robot.Contains(pyramidPoint))
                    collided = true;
            }
            return collided;
        }

        public static Boolean climbing(IterativeRobot robot)
        {
            if (!robot.getClimberUp())
                return false;
            Point pyramidCenter = (robot.getRed()) ? redPyramidCenter : bluePyramidCenter;
            Point location = UTIL.vectorToPoint(robot.getLocation());
            double direction = UTIL.normalizeDirection(robot.getDirection());
            double goalDirection = UTIL.getDirectionTward(location, pyramidCenter);
            double dErr = Math.Abs(goalDirection - direction);
            if (dErr < Math.PI / 24)
            {
                return isOnBoundry(location, robot.getRed());
            }
            else
                return false;
        }
        private static Boolean isOnBoundry(Point p, Boolean red)
        {
            Boolean valid = false;
            int p1; int p2;
            if(red)
            {
                p1 = 4;
                p2 = 6;
            }
            else
            {
                p1 = 0;
                p2 = 2;
            }
            if (UTIL.tolerant(p.X, climbPoints.ElementAt<Point>(p1).X, 5) || UTIL.tolerant(p.X, climbPoints.ElementAt<Point>(p2).X, 5))
            {
                if (p.Y < climbPoints.ElementAt<Point>(p2).Y && p.Y > climbPoints.ElementAt<Point>(p1).Y)
                    valid = true;
            }
            if (UTIL.tolerant(p.Y, climbPoints.ElementAt<Point>(p1).Y, 5) || UTIL.tolerant(p.Y, climbPoints.ElementAt<Point>(p2).Y, 5))
            {
                if (p.X > climbPoints.ElementAt<Point>(p2).X && p.X < climbPoints.ElementAt<Point>(p1).X)
                    valid = true;
            }
            return valid;
        }

    }
    class FieldObjects
    {
        Texture2D image;
        Vector2 location;
        Vector2 origin;
        float scaleFactor;
        float rotation;
        Color color;
        Boolean setOrigin;
        public FieldObjects(Texture2D pic, String name)
        {
            List<int> field = Field.getGoals();
            Vector2 loc = Vector2.Zero;
            float scale = 1;
            float rot = 0;
            setOrigin = false;
            color = Color.White;
            if (name.Equals("topGoalRed"))
            {
                scale = calcScale(pic.Height, 0, field);
                loc = new Vector2(field.ElementAt<int>(7), field.ElementAt<int>(0) + (int)(.1 * Field.Y));
                rot = (float)Math.PI;
                color = Color.Red;
            }
            else if (name.Equals("topGoalBlue"))
            {
                scale = calcScale(pic.Height, 0, field);
                loc = new Vector2(field.ElementAt<int>(6), field.ElementAt<int>(0) + (int)(.1 * Field.Y));
                color = Color.Blue;
            }
            else if (name.Equals("midGoalRed"))
            {
                scale = calcScale(pic.Height, 2, field);
                loc = new Vector2(field.ElementAt<int>(7), field.ElementAt<int>(2) + (int)(.1 * Field.Y));
                rot = (float)Math.PI;
                color = Color.Red;
            }
            else if (name.Equals("midGoalBlue"))
            {
                scale = calcScale(pic.Height, 2, field);
                loc = new Vector2(field.ElementAt<int>(6), field.ElementAt<int>(2) + (int)(.1 * Field.Y));
                color = Color.Blue;
            }
            else if (name.Equals("botGoalRed"))
            {
                scale = calcScale(pic.Height, 4, field);
                loc = new Vector2(field.ElementAt<int>(7), field.ElementAt<int>(4) + (int)(.1 * Field.Y));
                rot = (float)Math.PI;
                color = Color.Red;
            }
            else if (name.Equals("botGoalBlue"))
            {
                scale = calcScale(pic.Height, 4, field);
                loc = new Vector2(field.ElementAt<int>(6), field.ElementAt<int>(4) + (int)(.1 * Field.Y));
                color = Color.Blue;
            }
            else if (name.Equals("blueFeedBot"))
            {
                rot = (float)Math.PI / 4 * 3;
                loc = new Vector2(Field.X, Field.Y);
                color = Color.Blue;
            }
            else if (name.Equals("blueFeedTop"))
            {
                rot = (float)Math.PI * 5 / 4;
                loc = new Vector2(Field.X, 0);
                color = Color.Blue;
            }
            else if (name.Equals("redFeedTop"))
            {
                rot = (float) Math.PI * 7 / 4;
                loc = new Vector2(0, 0);
                color = Color.Red;
            }
            else if (name.Equals("redFeedBot"))
            {
                rot = (float) Math.PI / 4;
                loc = new Vector2(0, Field.Y);
                color = Color.Red;
            }
            else if (name.Equals("bluePyramid"))
            {
                rot = 0f;
                loc = new Vector2((float)(Field.X * .2), (float)(Field.Y * .4));
                color = Color.Blue;
                scale = (float) (Field.Y * .3) / pic.Height;
                origin = Vector2.Zero;
                setOrigin = true;
            }
            else if (name.Equals("redPyramid"))
            {
                rot = 0f;
                loc = new Vector2((float)(Field.X * .6), (float)(Field.Y * .4));
                color = Color.Red;
                scale = (float)(Field.Y * .3) / pic.Height;
                origin = Vector2.Zero;
                setOrigin = true;
            }
            construct(pic, loc, scale, rot);
        }
        private float calcScale(int height, int indexTop, List<int> field)
        {
            int difference = field.ElementAt<int>(indexTop+1) - field.ElementAt<int>(indexTop);
            return (float) difference / height;
        }
        public FieldObjects(Texture2D pic, Vector2 loc)
        {
            construct(pic, loc, 1, 0);
        }
        public FieldObjects(Texture2D pic, Vector2 loc, float scale)
        {
            construct(pic, loc, scale, 0);
        }
        public FieldObjects(Texture2D pic, Vector2 loc, float scale, float rot)
        {
            construct(pic, loc, scale, rot);
        }
        public FieldObjects(Texture2D pic, Vector2 loc, float scale, float rot, Color c)
        {
            construct(pic, loc, scale, rot);
            color = c;
        }
        private void construct(Texture2D pic, Vector2 loc, float scale, float rot)
        {
            image = pic;
            location = loc;
            scaleFactor = scale;
            rotation = rot;
            if(!setOrigin)
                origin = new Vector2(pic.Width / 2, pic.Height / 2);
            if (color == null)
                color = Color.White;
        }
        public Texture2D getImage()
        {
            return image;
        }
        public Vector2 getLocation()
        {
            return location;
        }
        public Vector2 getOrigin()
        {
            return origin;
        }
        public float getScale()
        {
            return scaleFactor;
        }
        public float getRotation()
        {
            return rotation;
        }
        public Color getColor()
        {
            return color;
        }
    }//Field Object

}

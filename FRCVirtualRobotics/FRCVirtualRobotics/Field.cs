using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
        List<FieldObjects> objects;
   
        public Field(GraphicsDevice window)
        {
            X = window.Viewport.Width;
            Y = window.Viewport.Height;

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
                if(location.X < 95 && (location.Y < 70 || location.Y > Y-70))
                    return true;
            }
            else
            {//115 right tollerance
                if(location.X > X - 95 && (location.Y < 70 || location.Y > Y-70))
                    return true;
            }
            return false;
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
        public FieldObjects(Texture2D pic, String name)
        {
            List<int> field = Field.getGoals();
            Vector2 loc = Vector2.Zero;
            float scale = 1;
            float rot = 0;
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

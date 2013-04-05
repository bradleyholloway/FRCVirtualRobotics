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
        int threeTop;
        int threeBottom;
        int twoTop1;
        int twoBottom1;
        int twoTop2;
        int twoBottom2;
        int leftX;
        int rightX;
        public static int X;
        public static int Y;
   
        public Field(GraphicsDevice window)
        {
            X = window.Viewport.Width;
            Y = window.Viewport.Height;
            
            //Scoring
            threeTop = (int).4 * Y;
            threeBottom = (int).6 * Y;

            twoTop1 = (int).1 * Y;
            twoBottom1 = (int).3 * Y;

            twoTop2 = (int).7 * Y;
            twoBottom2 = (int).9 * Y;

            leftX = 0+20;
            rightX = X+20;

            //Pyramid



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
                if(location.X < X - 115 && (location.Y < 70 || location.Y > Y-70))
                    return true;
            }
            return false;
        }

    }
}

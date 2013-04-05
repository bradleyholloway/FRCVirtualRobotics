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
        int fieldX;
        int fieldY;
   
        public Field(GraphicsDevice window)
        {
            fieldY = window.Viewport.Height;
            fieldX = window.Viewport.Width;
            
            //Scoring
            threeTop = (int).4 * fieldY;
            threeBottom = (int).6 * fieldY;

            twoTop1 = (int).1 * fieldY;
            twoBottom1 = (int).3 * fieldY;

            twoTop2 = (int).7 * fieldY;
            twoBottom2 = (int).9 * fieldY;

            leftX = 0+20;
            rightX = fieldX+20;

            //Pyramid



        }
        public Boolean score(Vector2 location, Boolean red)
        {//Left = blue goal, right = red goal
            if (red)
            {
                if (location.X >= rightX)
                {
                    if (location.Y >= twoTop1 && location.Y <= twoBottom1)
                        return true;
                    if (location.Y >= threeTop && location.Y <= threeBottom)
                        return true;
                    if (location.Y >= twoTop2 && location.Y <= twoBottom2)
                        return true;
                }
            }
            else
            {
                if (location.X <= leftX)
                {
                    if (location.Y >= twoTop1 && location.Y <= twoBottom1)
                        return true;
                    if (location.Y >= threeTop && location.Y <= threeBottom)
                        return true;
                    if (location.Y >= twoTop2 && location.Y <= twoBottom2)
                        return true;
                }
            }
            return false;
        }
        public Boolean feeding(Vector2 location, Boolean red)
        {//red feeds left, and blue feeds right
            if (red)
            {//95 left tollerance
                if(location.X < 95 && (location.Y < 70 || location.Y > fieldY-70))
                    return true;
            }
            else
            {//115 right tollerance
                if(location.X < fieldX - 115 && (location.Y < 70 || location.Y > fieldY-70))
                    return true;
            }
            return false;
        }

    }
}

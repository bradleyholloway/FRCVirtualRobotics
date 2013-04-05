using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BradleyXboxUtils
{
    public class ControllerInput
    {
        PlayerIndex p;

        public ControllerInput()
        {
            p = PlayerIndex.One;
        }

        public ControllerInput(PlayerIndex player)
        {
            p = player;
        }

        public double getLeftX()
        {
            return GamePad.GetState(p).ThumbSticks.Left.X;
        }
        public double getRightX()
        {
            return GamePad.GetState(p).ThumbSticks.Right.X;
        }
        public double getLeftY()
        {
            return GamePad.GetState(p).ThumbSticks.Left.Y;
        }
        public double getRightY()
        {
            return GamePad.GetState(p).ThumbSticks.Right.Y;
        }
        public Boolean getBottomActionButton()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.A);
        }
        public Boolean getLeftActionButton()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.X);
        }
        public Boolean getTopActionButton()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.Y);
        }
        public Boolean getRightActionButton()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.B);
        }
        public Boolean getUpDPad()
        {
            return (GamePad.GetState(p).DPad.Up == ButtonState.Pressed);
        }
        public Boolean getRightDPad()
        {
            return (GamePad.GetState(p).DPad.Right == ButtonState.Pressed);
        }
        public Boolean getDownDPad()
        {
            return (GamePad.GetState(p).DPad.Down == ButtonState.Pressed);
        }
        public Boolean getLeftDPad()
        {
            return (GamePad.GetState(p).DPad.Left == ButtonState.Pressed);
        }
        public Boolean getStart()
        {
            return (GamePad.GetState(p).IsButtonDown(Buttons.Start));
        }
        public Boolean getBack()
        {
            return (GamePad.GetState(p).IsButtonDown(Buttons.Back));
        }
        public Boolean getBigButton()
        {
            return (GamePad.GetState(p).IsButtonDown(Buttons.BigButton));
        }
        public Boolean getRightBumper()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.RightShoulder);
        }
        public Boolean getLeftBumper()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.LeftShoulder);
        }
        public Boolean getRightStickDown()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.RightStick);
        }
        public Boolean getLeftStickDown()
        {
            return GamePad.GetState(p).IsButtonDown(Buttons.LeftStick);
        }
        public double getRightTrigger()
        {
            return GamePad.GetState(p).Triggers.Right;
        }
        public double getLeftTrigger()
        {
            return GamePad.GetState(p).Triggers.Left;
        }
    }//Controller Input

    public class Toggle
    {
        Boolean value;
        Boolean lastValue;

        public Toggle()
        {
            value = false;
            lastValue = false;
        }
        public Toggle(Boolean a)
        {
            value = a;
            lastValue = false;
        }

        public Boolean update(Boolean a)
        {
            if (a != lastValue)
            {
                if (a)
                {
                    value = !value;
                    lastValue = true;
                }
                else if (!a)
                {
                    lastValue = false;
                }
            }
            return value;
        }
        public Boolean get()
        {
            return value;
        }
    }//Toggle

    public class ControlButton
    {
        Boolean last;
        Boolean state;

        public ControlButton()
        {
            last = false;
            state = false;
        }
        public ControlButton(Boolean start)
        {
            state = start;
            last = false;
        }

        /*public void update(Boolean current)
        {
            if (!last && current)
                state = true;
            last = current;
        }*/

        public Boolean update(Boolean current)
        {
            if (!last && current)
                state = true;
            last = current;
            Boolean temp = state;
            state = false;
            return temp;
        }

        public Boolean get()
        {
            Boolean temp = state;
            state = false;
            return temp;
        }
    }// ControlButton

    public class MenuItem
    {
        private Color colord;
        private Vector2 locationd;
        private String textd;

        public MenuItem(String t, Vector2 l, Color c)
        {
            colord = c;
            textd = t;
            locationd = l;
        }
        public Color color()
        {
            return colord;
        }
        public Vector2 location()
        {
            return locationd;
        }
        public String text()
        {
            return textd;
        }
    }//MenuItem

}

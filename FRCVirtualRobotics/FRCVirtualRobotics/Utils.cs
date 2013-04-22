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
            lastValue = true;
        }
        public Toggle(Boolean a)
        {
            value = a;
            lastValue = true;
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
            last = true;
            state = false;
        }
        public ControlButton(Boolean start)
        {
            state = start;
            last = true;
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

    public class PID
    {
        private double pConst;
        private double iConst;
        private double dConst;

        private double desiredVal;
        private double previousVal;
        private double errorSum;
        private double errorIncrement;
        private double errorEpsilon;
        private double doneRange;

        private Boolean firstCycle;
        private double maxOutput;
        private int minCycleCount;
        private int cycleCount;

        public PID(double p, double i, double d, double eps)
        {
            pConst = p;
            iConst = i;
            dConst = d;
            errorEpsilon = eps;
            doneRange = eps;

            desiredVal = 0.0;
            firstCycle = true;
            maxOutput = 1.0;
            errorIncrement = 1.0;

            cycleCount = 0;
            minCycleCount = 0;
        }
        public void setConstants(double p, double i, double d)
        {
            pConst = p;
            iConst = i;
            dConst = d;
        }
        public void setDoneRange(double range)
        {
            doneRange = range;
        }
        public void setErrorEpsilon(double eps)
        {
            errorEpsilon = eps;
        }
        public void setDesiredValue(double val)
        {
            desiredVal = val;
        }
        public void setMaxOutput(double max)
        {
            if (max < 0.0)
            {
                maxOutput = 0.0;
            }
            else if (max > 1.0)
            {
                maxOutput = 1.0;
            }
            else
            {
                maxOutput = max;
            }
        }
        public void setMinDoneCycles(int num)
        {
            minCycleCount = num;
        }
        public void resetErrorSum()
        {
            errorSum = 0.0;
        }
        public double getDesiredVal()
        {
            return desiredVal;
        }
        public double getPreviousVal()
        {
            return previousVal;
        }
        public double calcPID(double currentVal)
        {
            double pVal = 0.0;
            double iVal = 0.0;
            double dVal = 0.0;

            if (firstCycle)
            {
                previousVal = currentVal;
                firstCycle = false;
            }

            //P Calculation
            double error = desiredVal - currentVal;
            pVal = pConst * error;

            //I Calculation
            if (error > errorEpsilon)
            {
                if (errorSum < 0.0)
                {
                    errorSum = 0.0;
                }
                errorSum += Math.Min(error, errorIncrement);
            }
            else if (error < -1 * errorEpsilon)
            {
                if (errorSum > 0.0)
                {
                    errorSum = 0.0;
                }
                errorSum += Math.Max(error, -1 * errorIncrement);
            }
            else
            {
                errorSum = 0.0;
            }
            iVal = iConst * errorSum;

            //D Calculation
            double deriv = currentVal - previousVal;
            dVal = dConst * deriv;

            //PID calculation
            double output = pVal + iVal - dVal;

            output = UTIL.limitValue(output, maxOutput);

            previousVal = currentVal;
            return output;
        }
        public Boolean isDone()
        {
            double currError = Math.Abs(desiredVal - previousVal);
            if (currError <= doneRange)
            {
                cycleCount++;
            }
            else
            {
                cycleCount = 0;
            }
            return cycleCount > minCycleCount;
        }
        public void resetPreviousVal()
        {
            firstCycle = true;
        }
    }//PID

    public static class UTIL
    {
        public static double limitValue(double input, double limit)
        {
            if (input > limit)
                return limit;
            else if (input < -limit)
                return -limit;
            else
                return input;
        }
        public static double distance(Vector2 point1, Vector2 point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }
        public static double distance(Point point1, Point point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }
        public static Vector2 magD(double mag, double d)
        {
            return new Vector2((float)(mag * Math.Cos(d)), (float)(mag * Math.Sin(d)));
        }
        public static Point midpoint(Point a, Point b)
        {
            return new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }
        public static Vector2 pointToVector(Point a)
        {
            return new Vector2(a.X, a.Y);
        }
        public static Point vectorToPoint(Vector2 a)
        {
            return new Point((int)a.X, (int)a.Y);
        }
        public static double getDirectionTward(Point start, Point goal)
        {
            return Math.Atan2((goal.Y - start.Y), (goal.X - start.X));
        }
        public static double getDirectionTward(Vector2 start, Vector2 goal)
        {
            return Math.Atan2((goal.Y - start.Y), (goal.X - start.X));
        }
    }//static Util

}

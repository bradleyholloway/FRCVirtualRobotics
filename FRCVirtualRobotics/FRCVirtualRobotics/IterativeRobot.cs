using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BradleyXboxUtils;
using Microsoft.Xna.Framework.Audio;
using FRCVirtualRobotics;

namespace FRC_Virtual_Robotics
{
    public class RobotState
    {
        int state;
        
        public RobotState(int s)
        {
            state = s;
        }
        protected int getState()
        {
            return state;
        }
        public Boolean equals(RobotState s)
        {
            return s.getState() == this.getState();
        }
    }//RobotState protection class
    public class Robot
    {
        private RobotState state;

        public static RobotState DISABLED = new RobotState(0);
        public static RobotState AUTONOMOUS = new RobotState(1);
        public static RobotState TELEOP = new RobotState(2);
        public static RobotState PreAUTONOMOUS = new RobotState(3);


        public Robot()
        {
            state = DISABLED;
        }

        public void setState(RobotState s)
        {
            state = s;
        }

        public RobotState getState()
        {
            return state;
        }
    }
    public class IterativeRobot : Robot
    {
        private double leftMotorSpeed;
        private double rightMotorSpeed;
        private double directionForward;
        private double magnitude;
        private int feedDelay;
        private Boolean red;
        private int scalar;
        private static Texture2D image;
        private Vector2 location;
        private int ammo;
        private int windowX;
        private int windowY;
        private PID leftMotorPID;
        private PID rightMotorPID;
        private Rectangle rect;
        private float scale;
        private SoundEffectInstance drive;
        private List<Frisbee> frisbees;
        private Random rand;
        private AutonomousManager auto;
        
        public IterativeRobot(int maxSpeed, GraphicsDevice window, Boolean r, float sc, SoundEffectInstance driving, List<Frisbee> fbs)
        {
            rand = new Random();
            auto = new AutonomousManager(this);
            auto.load(3);
            leftMotorPID = new PID(.2, 0, 0, .15);
            rightMotorPID = new PID(.2, 0, 0, .15);
            leftMotorSpeed = 0;
            rightMotorSpeed = 0;
            scalar = maxSpeed;
            directionForward = Math.PI * 3 / 2;
            location = new Vector2(0, 0);
            image = null;
            windowX = window.Viewport.Width;
            windowY = window.Viewport.Height;
            red = r;
            feedDelay = 0;
            ammo = 3;
            scale = sc;
            drive = driving;
            drive.IsLooped = true;
            drive.Volume = .5f;
            frisbees = fbs;
            reset();
            
        }
        public Rectangle getRectangle()
        {
            return rect;
        }
        public void runAuto(double gameTime)
        {
            auto.run(gameTime);
        }

        public float getScale()
        {
            return scale;
        }
        public Boolean intersects(IterativeRobot rob)
        {
            return rect.Contains(rob.getRectangle()) || rect.Intersects(rob.getRectangle());
        }

        public void run(List<IterativeRobot> robots)
        {
            rightMotorSpeed += rightMotorPID.calcPID(rightMotorSpeed);
            leftMotorSpeed += leftMotorPID.calcPID(leftMotorSpeed);

            magnitude = (leftMotorSpeed + rightMotorSpeed) / 2 * scalar;
            directionForward += .1 * (rightMotorSpeed - leftMotorSpeed);

            Vector2 tempLocation = location + magD(magnitude, directionForward);
            
            rect = new Rectangle((int) tempLocation.X, (int) tempLocation.Y, (int)(image.Width*scale), (int)(image.Height*scale));

            if (!((location + magD(magnitude, directionForward)).X < 75 || (location + magD(magnitude, directionForward)).X > windowX - 75) &&
                !((location + magD(magnitude, directionForward)).Y < 50 || (location + magD(magnitude, directionForward)).Y > windowY - 50))
            {
                Boolean pyramidCollided = Field.didCollideWithPyramid(rect);
                if (!pyramidCollided)
                {
                    Boolean collisionFree = true;
                    IterativeRobot collidedWith = null;
                    foreach (IterativeRobot rob in robots)
                    {
                        if (rob != null)
                            if (!rob.Equals(this))
                                if (intersects(rob))
                                {
                                    collisionFree = false;
                                    rect = new Rectangle((int)location.X, (int)location.Y, (int)(image.Width * scale), (int)(image.Height * scale));
                                    collidedWith = rob;
                                }
                    }

                    if (collisionFree)
                    {
                        if (drive.State.Equals(SoundState.Stopped))
                            drive.Play();
                        location += magD(magnitude, directionForward);
                    }
                    else
                    {
                        drive.Stop();
                        Vector2 result = magD(magnitude, directionForward) + magD(collidedWith.magnitude, collidedWith.directionForward);
                        result /= 3;
                        if (collidedWith.push(result, robots))
                            this.push(result, robots);
                    }
                }
                else
                {
                    drive.Stop();
                    rightMotorSpeed = 0;
                    leftMotorSpeed = 0;
                }//pyramid Collided
            }
            else
            {
                drive.Stop();
                rightMotorSpeed = 0;
                leftMotorSpeed = 0;
            }
            if (Math.Abs(magnitude) < 0.2)
                drive.Stop();
        }
        public Boolean push(Vector2 collision, List<IterativeRobot> robots)
        {
            Vector2 tempLocation = location + collision;

            if (!((tempLocation).X < 75 || (tempLocation).X > windowX - 75) &&
                !(tempLocation.Y < 50 || (tempLocation).Y > windowY - 50))
            {
                Boolean collisionFree = true;
                IterativeRobot collidedWith = null;
                foreach (IterativeRobot rob in robots)
                {
                    if (rob != null)
                        if (!rob.Equals(this))
                            if (intersects(rob))
                            {
                                collisionFree = false;
                                rect = new Rectangle((int)location.X, (int)location.Y, (int)(image.Width * scale), (int)(image.Height * scale));
                                collidedWith = rob;
                            }
                }
                if (collisionFree)
                {
                    location += collision;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }
        public void endGame()
        {
            drive.Stop();
            reset();
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
            leftMotorPID.setDesiredValue(left);
            rightMotorPID.setDesiredValue(right);
            
            //leftMotorSpeed = left;
            //rightMotorSpeed = right;
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
            if (ammo > 0 && getState().Equals(Robot.DISABLED)==false)
            {
                ammo--;
                frisbees.Add(new Frisbee(getLocation(), getDirection() + (rand.NextDouble() - .5) / 5, getRed()));
                return true;
            }
            return false;
        }
        public Boolean feed()
        {
            if (feedDelay > 0)
            {
                feedDelay--;
            }
            if (ammo < 4 && feedDelay == 0)
            {
                ammo++;
                feedDelay = 18;
                return true;
            }
            return false;
            
        }
            
    }
}

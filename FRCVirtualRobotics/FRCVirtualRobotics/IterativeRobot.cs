using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BradleyXboxUtils;
using Microsoft.Xna.Framework.Audio;
using FRCVirtualRobotics;
using WindowsFRCRobotics;

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
        private int shootDelay;
        private Boolean red;
        private int scalar;
        private Texture2D image;
        private Vector2 location;
        private int ammo;
        private int colAmmo;
        private int windowX;
        private int windowY;
        private PID leftMotorPID;
        private PID rightMotorPID;
        //private Rectangle rect;
        private RotatedRectangle rect2;
        private float scale;
        private SoundEffectInstance drive;
        private List<Frisbee> frisbees;
        private Random rand;
        private AutonomousManager auto;
        private static int redRobots;
        private static int blueRobots;
        private Boolean firstRobot;
        private Boolean climber;
        private Boolean climbLock;
        private Boolean comp;
        private PID aiTurnPID;
        private PID aiDrivePID;
        private List<AICommands> aiCommands;
        List<AICommands>.Enumerator aiEnum;
        private AICommands aiCommand;
        private int aiFailCount;
        private Boolean defencive;
        private int robotNum;

        public IterativeRobot(int maxSpeed, GraphicsDevice window, Boolean r, float sc, SoundEffectInstance driving, List<Frisbee> fbs, Texture2D i, Boolean ai)
        {
            image = i;
            rand = new Random();
            auto = new AutonomousManager(this);
            if (!ai)
                auto.load(3);
            defencive = false;
            leftMotorPID = new PID(.2, 0, 0, .15);
            rightMotorPID = new PID(.2, 0, 0, .15);
            aiTurnPID = new PID(.2, 0, .1, .1);
            aiDrivePID = new PID(.01, 0, 0, 100);
            aiDrivePID.setMaxOutput(.7);
            leftMotorSpeed = 0;
            rightMotorSpeed = 0;
            scalar = maxSpeed;
            directionForward = Math.PI * 3 / 2;
            location = new Vector2(0, 0);
            windowX = window.Viewport.Width;
            windowY = window.Viewport.Height;
            red = r;
            feedDelay = 0;
            shootDelay = 0;
            ammo = 3;
            scale = sc;
            drive = driving;
            drive.IsLooped = true;
            drive.Volume = .5f;
            frisbees = fbs;
            climber = false;
            climbLock = false;
            colAmmo = 0;
            comp = ai;
            if (red)
            {
                redRobots++;
                robotNum = redRobots;
            }
            else
            {
                blueRobots++;
                robotNum = blueRobots;
            }
            
            reset();
            rect2 = new RotatedRectangle(new Point((int)(location.X), (int)(location.Y)), image.Width * scale, image.Height * scale, directionForward);
            aiCommands = new List<AICommands>();
            String robotString = "";
            if (red)
                robotString += "Red";
            else
                robotString += "Blue";

            robotString += "" + robotNum;


            aiCommands.Add(new AICommands("feed" + robotString, 6));
            aiCommands.Add(new AICommands("feed" + robotString, .4));
            aiCommands.Add(new AICommands("feed" + robotString, .4));
            aiCommands.Add(new AICommands("feed" + robotString, .4));
            aiCommands.Add(new AICommands("shoot" + robotString, 5));
            aiCommands.Add(new AICommands("shoot" + robotString, .4));
            aiCommands.Add(new AICommands("shoot" + robotString, .4));
            aiCommands.Add(new AICommands("shoot" + robotString, .4));
            aiCommands.Add(new AICommands("middle" + robotString, 2));
            aiCommand = aiCommands.ElementAt<AICommands>(0);
            aiEnum = aiCommands.GetEnumerator();



        }
        public static void resetPlayers()
        {
            redRobots = 0;
            blueRobots = 0;
        }

        public void stopMoving()
        {
            drive.Stop();
        }
        public Boolean getAI()
        {
            return comp;
        }
        public RotatedRectangle getRectangle()
        {
            return rect2;
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
            Boolean intersecting = false;
            if (rect2.Contains(rob.getRectangle().p1))
                intersecting = true;
            if (rect2.Contains(rob.getRectangle().p2))
                intersecting = true;
            if (rect2.Contains(rob.getRectangle().p3))
                intersecting = true;
            if (rect2.Contains(rob.getRectangle().p4))
                intersecting = true;
            if (rect2.Contains(UTIL.vectorToPoint(rob.getLocation())))
                intersecting = true;
            return intersecting;
        }

        public Boolean run(List<IterativeRobot> robots, double inGameTime)
        {
            Boolean vibrate = false;

            if (shootDelay > 0)
                shootDelay--;

            if (!climbLock)
            {
                if (comp && (getState().equals(Robot.TELEOP) || getState().equals(Robot.AUTONOMOUS)))
                    calcAIDriveValues(robots, inGameTime);

                rightMotorSpeed += rightMotorPID.calcPID(rightMotorSpeed);
                leftMotorSpeed += leftMotorPID.calcPID(leftMotorSpeed);

                magnitude = (leftMotorSpeed + rightMotorSpeed) / 2 * scalar;
                directionForward += .1 * (rightMotorSpeed - leftMotorSpeed);

                Vector2 tempLocation = location + magD(magnitude, directionForward);

                //rect = new Rectangle((int) tempLocation.X, (int) tempLocation.Y, (int)(image.Width*scale), (int)(image.Height*scale));
                rect2 = new RotatedRectangle(new Point((int)tempLocation.X, (int)tempLocation.Y), image.Width * scale, image.Height * scale, directionForward);

                if (!((location + magD(magnitude, directionForward)).X < 75 || (location + magD(magnitude, directionForward)).X > windowX - 75) &&
                    !((location + magD(magnitude, directionForward)).Y < 50 || (location + magD(magnitude, directionForward)).Y > windowY - 50))
                {
                    Boolean pyramidCollided = Field.didCollideWithPyramid(rect2);
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
                                        //rect = new Rectangle((int)location.X, (int)location.Y, (int)(image.Width * scale), (int)(image.Height * scale));
                                        rect2 = new RotatedRectangle(new Point((int)location.X, (int)location.Y), image.Width * scale, image.Height * scale, directionForward);
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
                            //drive.Stop();
                            vibrate = true;//GamePad.SetVibration(playerIndex, .5f, .5f);
                            Vector2 result = magD(magnitude, directionForward) + magD(collidedWith.magnitude, collidedWith.directionForward);
                            result /= 3;
                            if (this.push(result, robots))
                            {
                                this.pushed(result);
                            }
                            if (collidedWith.push(result, robots))
                            {
                                collidedWith.pushed(result);
                            }


                        }
                    }
                    else
                    {
                        vibrate = true;// GamePad.SetVibration(playerIndex, .5f, .5f);
                        drive.Stop();
                        rightMotorSpeed = 0;
                        leftMotorSpeed = 0;
                    }//pyramid Collided
                }
                else
                {
                    vibrate = true; // GamePad.SetVibration(playerIndex, .5f, .5f);
                    drive.Stop();
                    rightMotorSpeed = 0;
                    leftMotorSpeed = 0;

                }
                if (Math.Abs(magnitude) < 0.2)
                    drive.Stop();
                if (Field.climbing(this))
                {
                    climbLock = true;
                    drive.Stop();
                }
            }
            return vibrate;
        }
        public void pushed(Vector2 collision)
        {
            Vector2 tempLocation = location + collision;
            rect2 = new RotatedRectangle(new Point((int)tempLocation.X, (int)tempLocation.Y), image.Width * scale, image.Height * scale, directionForward);

            Boolean pyramidCollided = Field.didCollideWithPyramid(rect2);

            if (!((tempLocation).X < 75 || (tempLocation).X > windowX - 75) &&
                !(tempLocation.Y < 50 || (tempLocation).Y > windowY - 50))
            {
                Boolean collisionFree = !pyramidCollided;
                /*IterativeRobot collidedWith = null;
                foreach (IterativeRobot rob in robots)
                {
                    if (rob != null)
                        if (!rob.Equals(this))
                            if (intersects(rob))
                            {
                                collisionFree = false;
                                collidedWith = rob;
                            }
                }*/
                if (collisionFree)
                {
                    location += collision;
                    //return true;
                }
                else
                {
                    rect2 = new RotatedRectangle(new Point((int)location.X, (int)location.Y), image.Width * scale, image.Height * scale, directionForward);
                    //return false;
                }
            }
            //else
            //return false;
        }

        public void setDefensive(Boolean def)
        {
            if (defencive != def)
            {
                aiDrivePID = new PID(.01, 0, 0, (defencive) ? 100 : 53);
                //aiDrivePID.setErrorEpsilon((defencive) ? 53 : 100);
            }
            defencive = def;

        }

        public Boolean push(Vector2 collision, List<IterativeRobot> robots)
        {
            if (climbLock)
                return false;
            Vector2 tempLocation = location + collision;
            rect2 = new RotatedRectangle(new Point((int)tempLocation.X, (int)tempLocation.Y), image.Width * scale, image.Height * scale, directionForward);

            Boolean pyramidCollided = Field.didCollideWithPyramid(rect2);

            if (!((tempLocation).X < 75 || (tempLocation).X > windowX - 75) &&
                !(tempLocation.Y < 50 || (tempLocation).Y > windowY - 50))
            {
                Boolean collisionFree = !pyramidCollided;
                /*IterativeRobot collidedWith = null;*/
                foreach (IterativeRobot rob in robots)
                {
                    if (rob != null)
                        if (!rob.Equals(this))
                            if (intersects(rob))
                            {
                                return false;
                            }
                }
                if (collisionFree)
                {
                    //location += collision;
                    return true;
                }
                else
                {
                    rect2 = new RotatedRectangle(new Point((int)location.X, (int)location.Y), image.Width * scale, image.Height * scale, directionForward);
                    return false;
                }
            }
            else
                return false;
        }

        private void calcAIDriveValues(List<IterativeRobot> robots, double inGameTime)
        {
            Point p = getAIPoint(robots);
            if (aiFailCount > 4)
            {
                p = UTIL.vectorToPoint(location + UTIL.magD(120, Field.getEscapeDirection(location)));
                if (Field.isEscaped(location))
                    aiFailCount = 0;
            }
            double desiredAngle = UTIL.normalizeDirection(UTIL.getDirectionTward(UTIL.vectorToPoint(location), p));
            double currentForward = UTIL.normalizeDirection(directionForward);
            if ((desiredAngle - currentForward) < -Math.PI)
                currentForward -= Math.PI * 2;
            if ((desiredAngle - currentForward) > Math.PI)
                currentForward += Math.PI * 2;


            aiTurnPID.setDesiredValue(desiredAngle);
            aiDrivePID.setDesiredValue(0.00);
            double y = -1 * aiTurnPID.calcPID(currentForward);
            double x = -1 * aiDrivePID.calcPID(UTIL.distance(p, UTIL.vectorToPoint(location)));
            if (aiDrivePID.isDone())
                x = 0;


            //if (x < .2)
            //    x = 0;



            setMotorValues(x + y, x - y);
            if (aiCommand.isDone(this, inGameTime))
            {
                if (aiCommand.timedOut())
                    aiFailCount++;
                else
                    aiFailCount = 0;
                if (!aiEnum.MoveNext() || aiEnum.Current == null)
                    aiEnum = aiCommands.GetEnumerator();
                if (aiEnum.Current != null)
                    aiCommand = aiEnum.Current;
                else
                    aiCommand = aiCommands.ElementAt<AICommands>(0);
            }

        }
        private Point getAIPoint(List<IterativeRobot> robots)
        {
            if (!defencive)
                return aiCommand.getLocation();
            int robotDefend = robotNum - 1;
            robotDefend = (robotDefend * 2) + ((red) ? 1 : 0);
            
            /*if (red && firstRobot)
                robotDefend = 1;
            else if (red && !firstRobot)
                robotDefend = 1;
            else if (!red && firstRobot)
                robotDefend = 0;
            else if (!red && !firstRobot)
                robotDefend = 0;
            */
            return UTIL.vectorToPoint(robots.ElementAt<IterativeRobot>(robotDefend).getLocation() + new Vector2(0, 0));
        }

        public void setClimber(Boolean state)
        {
            climber = state;
        }
        public Boolean getClimberUp()
        {
            return climber;
        }
        public Boolean getClimb()
        {
            return climbLock;
        }
        public void endGame()
        {
            drive.Stop();
            reset();
        }

        public Color getColor()
        {
            if (climbLock)
                return Color.Green;
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

        //public static void setImage(Texture2D picture)
        //{
        //    image = picture;
        // }

        public void reset()
        {
            if (red)
            {
                if(robotNum == 1)
                    location = new Vector2(75, 250);
                if (robotNum == 2)
                    location = new Vector2(75, 430);
                if (robotNum == 3)
                    location = new Vector2(75, 65);
                directionForward = 0;
            }
            else
            {
                if (robotNum == 1)
                    location = new Vector2(windowX - 75, 250);
                if (robotNum == 2)
                    location = new Vector2(windowX - 75, 430);
                if (robotNum == 3)
                    location = new Vector2(windowX - 75, 65);
                directionForward = Math.PI;
            }
        }
        public Boolean fire()
        {
            if (shootDelay == 0)
            {
                if (ammo > 0 && getState().Equals(Robot.DISABLED) == false)
                {
                    ammo--;
                    frisbees.Add(new Frisbee(getLocation(), getDirection() + (rand.NextDouble() - .5) / 4, getRed(), false));
                    shootDelay = 20;
                    return true;
                }
                if (colAmmo > 0 && getState().Equals(Robot.DISABLED) == false)
                {
                    colAmmo--;
                    frisbees.Add(new Frisbee(getLocation(), getDirection() + (rand.NextDouble() - .5) / 4, getRed(), true));
                    shootDelay = 20;
                    return true;
                }
            }
            return false;
        }
        public int getAmmo()
        {
            return ammo + colAmmo;
        }
        public Boolean feed(Boolean colored)
        {
            if (feedDelay > 0)
            {
                feedDelay--;
            }
            if (ammo < 4 && feedDelay == 0)
            {
                if (!colored)
                    ammo++;
                else
                    colAmmo++;
                feedDelay = 18;
                return true;
            }
            return false;

        }

    }
}

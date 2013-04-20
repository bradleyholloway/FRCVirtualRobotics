using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRC_Virtual_Robotics;
using BradleyXboxUtils;
using Microsoft.Xna.Framework;

namespace FRCVirtualRobotics
{
    class AutonomousManager
    {
        private List<AutoCommands> commandList;
        private int index;
        private IterativeRobot robot;

        public AutonomousManager(IterativeRobot thisRobot)
        {
            commandList = new List<AutoCommands>();
            index = 0;
            robot = thisRobot;
        }
        public void run(double gameTime)
        {
            if (index < commandList.Count)
            {

                if (commandList.ElementAt<AutoCommands>(index).run(gameTime, robot))
                    index++;

            }
        }
        public void load(double time)
        {
            commandList.Add(new Wait(2));
            commandList.Add(new Shoot());
            commandList.Add(new Wait(2));
            commandList.Add(new Shoot());
            commandList.Add(new Wait(2));
            commandList.Add(new Shoot());
            commandList.Add(new Wait(.5));
            commandList.Add(new Turn(Math.PI, 2));
            commandList.Add(new Stop());
        }
        public IterativeRobot getRobot()
        {
            return robot;
        }


    }

    public abstract class AutoCommands
    {
        public abstract Boolean run(double gameTime, IterativeRobot robot);
    }
    public class Shoot : AutoCommands
    {
        public Shoot()
        { }
        
        public override Boolean run(double gameTime, IterativeRobot robot)
        {
            Boolean shot = robot.fire();
            if (shot)
            {

            }
            return shot;
        }
    }
    public class Wait : AutoCommands
    {
        private double time;
        private double timeStart;
        private bool firstCycle;
        public Wait(double seconds)
        {
            time = seconds;
            firstCycle = true;
        }
        public override Boolean run(double gameTime, IterativeRobot robot)
        {
            if (firstCycle)
            {
                timeStart = gameTime;
                firstCycle = false;
            }
            return gameTime-timeStart > time;
        }
    }
    public class DriveStraight : AutoCommands
    {
        private double distance;
        private PID distancePID;
        private Boolean firstCycle;
        private Boolean negative;
        private Vector2 startPoint;
        private double endTime;
        public DriveStraight(double distance, double timeOut)
        {
            this.distance = 0;
            distancePID = new PID(.2, 0, 0, 1);
            distancePID.setDesiredValue(distance);
            negative = distance < 0;
            endTime = timeOut;
            firstCycle = true;
        }
        public override bool run(double gameTime, IterativeRobot robot)
        {
            if(firstCycle)
            {
                startPoint = robot.getLocation();
                endTime += gameTime;
                firstCycle = false;
            }
            double distance = UTIL.distance(startPoint, robot.getLocation());
            if (negative)
                distance *= -1;
            double speed = distancePID.calcPID(distance);
            robot.setMotorValues(speed, speed);

            if (gameTime > endTime)
                return true;

            return distancePID.isDone();
        }

    }
    public class Turn : AutoCommands
    {
        private double goalAngle;
        private PID turnPID;
        private Boolean firstCycle;
        private double endTime;
        public Turn(double radians, double timeOut)
        {
            firstCycle = true;
            turnPID = new PID(.18, 0, 0, .1);
            goalAngle = radians;
            endTime = timeOut;
        }
        public override bool run(double gameTime, IterativeRobot robot)
        {
            if (firstCycle)
            {
                goalAngle += robot.getDirection();
                turnPID.setDesiredValue(goalAngle);
                endTime += gameTime;
                firstCycle = false;
            }
            double speed = turnPID.calcPID(robot.getDirection());
            robot.setMotorValues(-speed, speed);

            if (gameTime > endTime)
                return true;

            return turnPID.isDone();
        }
    }
    public class Stop : AutoCommands
    {
        public Stop() { }
        public override bool run(double gameTime, IterativeRobot robot)
        {
            robot.setMotorValues(0, 0);
            return false;
        }
    }
}

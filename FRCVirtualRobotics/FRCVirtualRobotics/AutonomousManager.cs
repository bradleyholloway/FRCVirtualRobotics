using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRC_Virtual_Robotics;

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
            commandList.Add(new Wait(2, time));
            commandList.Add(new Shoot());
            commandList.Add(new Wait(2, time));
            commandList.Add(new Shoot());
            commandList.Add(new Wait(2, time));
            commandList.Add(new Shoot());
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
        public Wait(double seconds, double gameTimeStart)
        {
            time = seconds; gameTimeStart = timeStart;
        }
        public override Boolean run(double gameTime, IterativeRobot robot)
        {
            return gameTime-timeStart > time;
        }
    }
}

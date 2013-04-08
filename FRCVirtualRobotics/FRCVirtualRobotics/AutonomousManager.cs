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
        public AutonomousManager()
        {
            commandList = new List<AutoCommands>();
            index = 0;
        }
        public void run()
        {
            if (index < commandList.Count)
            {

                if (commandList.ElementAt<AutoCommands>(index).run())
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


    }

    abstract class AutoCommands
    {
        public abstract Boolean run(double gameTime);
    }
    public class Shoot : AutoCommands
    {
        public Shoot()
        { }
        public Boolean run(double gameTime)
        {
            return RobotDriver.getRobots().ElementAt<IterativeRobot>(index);
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
        public Boolean run(double gameTime)
        {
            return gameTime-timeStart > time;
        }
    }
}

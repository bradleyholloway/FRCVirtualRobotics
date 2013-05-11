using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FRC_Virtual_Robotics;
using FRCVirtualRobotics;

namespace WindowsFRCRobotics
{
    class AICommands
    {
        Point location;
        Boolean shoot;
        Boolean justDrive;
        Boolean firstCycle;
        double timeout;
        double startTime;

        public AICommands(String command, double timeOut)
        {
            timeout = timeOut;
            shoot = false;
            if (command.Equals("feedRed1"))
                location = new Point(0, 0);
            if (command.Equals("shootRed1"))
            {
                shoot = true;
                location = new Point(650, 170);
            }
            if (command.Equals("feedBlue1"))
                location = new Point(800, 0);
            if (command.Equals("shootBlue1"))
            {
                shoot = true;
                location = new Point(150, 160);
            }
            if (command.Equals("feedRed2"))
                location = new Point(0, 450);
            if (command.Equals("shootRed2"))
            {
                shoot = true;
                location = new Point(650, 310);
            }
            if (command.Equals("feedBlue2"))
                location = new Point(800, 450);
            if (command.Equals("shootBlue2"))
            {
                shoot = true;
                location = new Point(150, 320);
            }
            if (command.Equals("middleRed2") || command.Equals("middleBlue2"))
            {
                location = new Point(400, 410);
                justDrive = true;
            }
            if (command.Equals("middleRed1") || command.Equals("middleBlue1"))
            {
                location = new Point(400, 100);
                justDrive = true;
            }
            firstCycle = true;
        }

        public Point getLocation()
        {
            return location;
        }

        public Boolean isDone(IterativeRobot robot, double inGameTime)
        {
            if (firstCycle)
            {
                startTime = inGameTime;
                firstCycle = false;
            }
            if (inGameTime - startTime > timeout)
            {
                firstCycle = true;
                return true;
            }
            if (!shoot)
            {
                if (justDrive)
                    return BradleyXboxUtils.UTIL.tolerant(BradleyXboxUtils.UTIL.distance(location, BradleyXboxUtils.UTIL.vectorToPoint(robot.getLocation())), 0.0, 100);
                if (!BradleyXboxUtils.UTIL.tolerant(BradleyXboxUtils.UTIL.distance(location, BradleyXboxUtils.UTIL.vectorToPoint(robot.getLocation())), 0.0, 100))
                    return false;
                //return true;
                if (Field.feeding(robot.getLocation(), robot.getRed()) != 0)
                    if (robot.feed((Field.feeding(robot.getLocation(), robot.getRed()) == 2)))
                    {
                        //return true;
                        Field.feed(robot.getRed(), (Field.feeding(robot.getLocation(), robot.getRed()) == 2));
                        return true;
                    }
               return robot.getAmmo() == 4;
            }
            if (!BradleyXboxUtils.UTIL.tolerant(BradleyXboxUtils.UTIL.distance(location, BradleyXboxUtils.UTIL.vectorToPoint(robot.getLocation())), 0.0, 100))
                return false;
            return robot.fire() || robot.getAmmo()==0;
        }
    }
}

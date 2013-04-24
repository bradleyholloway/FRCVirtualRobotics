using System;

namespace FRC_Virtual_Robotics
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            using (RobotDriver game = new RobotDriver())
            {
                game.Run();
            }
        }
    }
#endif
}


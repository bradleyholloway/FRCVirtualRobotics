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
using BradleyXboxUtils;
using FRCVirtualRobotics;

namespace FRC_Virtual_Robotics
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class RobotDriver : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        IterativeRobot robot;
        private List<Frisbee> frisbees;
        ControllerInput driverInput;
        ControlButton fire;

        public RobotDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            robot = new IterativeRobot(20, GraphicsDevice);
            driverInput = new ControllerInput();
            frisbees = new List<Frisbee>();
            Frisbee.SPEED = 10;
            Frisbee.setFrisbees(frisbees);
            fire = new ControlButton();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            robot.setImage(Content.Load<Texture2D>("robot"));
            Frisbee.setImage(Content.Load<Texture2D>("frisbee"));
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            processInput();
            
            robot.run();

            for (int index = 0; index < frisbees.Count; index++)
            {
                index += frisbees.ElementAt<Frisbee>(index).run();
            }

            //foreach (Frisbee frisbee in frisbees)
            //{
            //    frisbee.run();
            //}

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.Begin();

            spriteBatch.Draw(robot.getImage(), robot.getLocation(), null, Color.White, robot.getDirection(), robot.getOrigin(), 0.5f, SpriteEffects.None, 0f);
            
            foreach (Frisbee frisbee in frisbees)
            {
                spriteBatch.Draw(frisbee.getImage(), frisbee.getLocation(), null, frisbee.getColor(), frisbee.getDirection(), frisbee.getOrigin(), .1f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected void processInput()
        {
            double y = driverInput.getLeftY();
            double x = -driverInput.getRightX();
            robot.setMotorValues(deadband(y + x), deadband(y - x));

            

            if(fire.update(driverInput.getRightBumper()))
                frisbees.Add(new Frisbee(robot.getLocation(), robot.getDirection()));

            if (driverInput.getBack())
                robot.reset();

            if (driverInput.getDownDPad())
                this.Exit();
        }
        private double deadband(double a)
        {
            if (a > 1)
                return 1;
            else if (a < -1)
                return -1;
            else
                return a;
        }

    }
}

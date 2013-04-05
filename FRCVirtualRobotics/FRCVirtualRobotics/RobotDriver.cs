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

        Field field;
        List<IterativeRobot> robots;
        List<Frisbee> frisbees;
        List<ControllerInput> driverInputs;
        List<ControlButton> fire;
        List<int> players;
        private int blueScore;
        private int redScore;

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
            field = new Field(GraphicsDevice);
            players = new List<int>();
            // TODO: use this.Content to load your game content here
            robots = new List<IterativeRobot>();
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                robots.Add(new IterativeRobot(6, GraphicsDevice, true));
                players.Add(0);
            }
            else
                robots.Add(null);

            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                robots.Add(new IterativeRobot(6, GraphicsDevice, false));
                players.Add(1);
            }
            else
                robots.Add(null);

            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
            {
                //robots.Add(new IterativeRobot(6, GraphicsDevice, true));
                players.Add(2);
            }
            else
                robots.Add(null);

            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
            {
                robots.Add(new IterativeRobot(6, GraphicsDevice, false));
                players.Add(3);
            }
            else
                robots.Add(null);

            driverInputs = new List<ControllerInput>();
            driverInputs.Add(new ControllerInput(PlayerIndex.One));
            driverInputs.Add(new ControllerInput(PlayerIndex.Two));
            driverInputs.Add(new ControllerInput(PlayerIndex.Three));
            driverInputs.Add(new ControllerInput(PlayerIndex.Four));

            frisbees = new List<Frisbee>();

            Frisbee.SPEED = 10;
            Frisbee.setFrisbees(frisbees);

            fire = new List<ControlButton>();
            for (int a = 0; a < 4; a++)
                fire.Add(new ControlButton());

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            IterativeRobot.setImage(Content.Load<Texture2D>("robot"));
            Frisbee.setImage(Content.Load<Texture2D>("frisbee"));

            redScore = blueScore = 0;
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
            foreach (int player in players)
                processInput(player);
            
            foreach (int player in players)
                robots.ElementAt<IterativeRobot>(player).run();

            for (int index = 0; index < frisbees.Count; index++)
            {
                if (frisbees.ElementAt<Frisbee>(index).getRed())
                {
                    redScore += field.score(frisbees.ElementAt<Frisbee>(index).getLocation(), frisbees.ElementAt<Frisbee>(index).getRed());
                }
                else
                {
                    blueScore += field.score(frisbees.ElementAt<Frisbee>(index).getLocation(), frisbees.ElementAt<Frisbee>(index).getRed());
                }
                index += frisbees.ElementAt<Frisbee>(index).run();
                
            }

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

            foreach (IterativeRobot robot in robots)
            {
                if(robot != null)
                    spriteBatch.Draw(robot.getImage(), robot.getLocation(), null, robot.getColor(), robot.getDirection(), robot.getOrigin(), .3f, SpriteEffects.None, 0f);
            }
            foreach (Frisbee frisbee in frisbees)
            {
                spriteBatch.Draw(Frisbee.getImage(), frisbee.getLocation(), null, frisbee.getColor(), frisbee.getDirection(), frisbee.getOrigin(), .06f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected void processInput(int player)
        {
            double y = driverInputs.ElementAt<ControllerInput>(player).getLeftY();
            double x = -driverInputs.ElementAt<ControllerInput>(player).getRightX();
            robots.ElementAt<IterativeRobot>(player).setMotorValues(deadband(y + x), deadband(y - x));

            if(field.feeding(robots.ElementAt<IterativeRobot>(player).getLocation(), robots.ElementAt<IterativeRobot>(player).getRed()))
                robots.ElementAt<IterativeRobot>(player).feed();

            if (fire.ElementAt<ControlButton>(player).update(driverInputs.ElementAt<ControllerInput>(player).getRightBumper()) && robots.ElementAt<IterativeRobot>(player).fire())
                frisbees.Add(new Frisbee(robots.ElementAt<IterativeRobot>(player).getLocation(), robots.ElementAt<IterativeRobot>(player).getDirection(), robots.ElementAt<IterativeRobot>(player).getRed()));

            if (driverInputs.ElementAt<ControllerInput>(player).getBack())
                robots.ElementAt<IterativeRobot>(player).reset();

            if (driverInputs.ElementAt<ControllerInput>(player).getDownDPad())
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

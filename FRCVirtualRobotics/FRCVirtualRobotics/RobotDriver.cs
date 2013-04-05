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
        SpriteFont spriteFont;
        List<MenuItem> menuItems;
        int menuSelection;
        TimeSpan startGameTime;

        RobotState robotStates;

        Field field;
        List<IterativeRobot> robots;
        List<Frisbee> frisbees;
        List<ControllerInput> driverInputs;
        List<ControlButton> fire;
        List<int> players;
        ControlButton menuUp;
        ControlButton menuDown;
        private int blueScore;
        private int redScore;
        public int gameState;

        public RobotDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameState = 0;
            menuSelection = 0;
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

            //Menu control
            menuUp = new ControlButton();
            menuDown = new ControlButton();
            
            menuItems.Add(new MenuItem("Play Game", new Vector2(200,200), Color.Red));
            menuItems.Add(new MenuItem("Information", new Vector2(200,400), Color.White));
            menuItems.Add(new MenuItem("Exit", new Vector2(200,600), Color.Blue));

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            IterativeRobot.setImage(Content.Load<Texture2D>("robot"));
            Frisbee.setImage(Content.Load<Texture2D>("frisbee"));
            spriteFont = Content.Load<SpriteFont>("TimesNewRoman");

            redScore = blueScore = 0;
            robotStates = Robot.DISABLED;
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
            if (gameState == 1)
            {
                //Robot Control States
                if (gameTime.TotalGameTime.Subtract(startGameTime).TotalSeconds < 15)
                {
                    robotStates = Robot.AUTONOMOUS;
                }
                else if (gameTime.TotalGameTime.Subtract(startGameTime).TotalSeconds < 135)
                {
                    robotStates = Robot.TELEOP;
                }
                else if (gameTime.TotalGameTime.Subtract(startGameTime).TotalSeconds >= 135)
                {
                    robotStates = Robot.DISABLED;
                    gameState = 0;
                }
                foreach (int player in players)
                    robots.ElementAt<IterativeRobot>(player).setState(robotStates);

                foreach (int player in players)
                {
                    if (robots.ElementAt<IterativeRobot>(player).getState().equals(Robot.TELEOP))
                        processInput(player);
                }
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
            }//If inGame
            else if (gameState == 0)
            {
                Menu();
                startGameTime = gameTime.TotalGameTime;
            }
            base.Update(gameTime);
        }

        protected void Menu()
        {
            menuUp.update(driverInputs.ElementAt<ControllerInput>(0).getRightX() > .8);
            menuDown.update(driverInputs.ElementAt<ControllerInput>(0).getRightX() > .8);
            fire.ElementAt<ControlButton>(0).update(driverInputs.ElementAt<ControllerInput>(0).getRightBumper());

            if (menuUp.get())
                menuSelection++;
            if (menuDown.get())
                menuSelection--;

            if (menuSelection < 0)
                menuSelection = menuItems.Count-1;

            if (menuSelection >= menuItems.Count)
                menuSelection = 0;

            if (fire.ElementAt<ControlButton>(0).get())
            {
                if (menuSelection == 0)
                {
                    LoadContent();
                    gameState = 1;
                }
                else if (menuSelection == 1)
                {
                    //Display more information
                }
                else if (menuSelection == 2)
                {
                    this.Exit();
                }
            }//menuMapping
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.Begin();

            if (gameState == 1)
            {
                foreach (IterativeRobot robot in robots)
                {
                    if (robot != null)
                        spriteBatch.Draw(robot.getImage(), robot.getLocation(), null, robot.getColor(), robot.getDirection(), robot.getOrigin(), .3f, SpriteEffects.None, 0f);
                }
                foreach (Frisbee frisbee in frisbees)
                {
                    spriteBatch.Draw(Frisbee.getImage(), frisbee.getLocation(), null, frisbee.getColor(), frisbee.getDirection(), frisbee.getOrigin(), .06f, SpriteEffects.None, 0f);
                }
            }
            else if (gameState == 0)
            {
                foreach (MenuItem menuItem in menuItems)
                    spriteBatch.DrawString(spriteFont, menuItem.text(), menuItem.location(), menuItem.color());
                spriteBatch.Draw(Frisbee.getImage(), menuItems.ElementAt<MenuItem>(menuSelection).location()- new Vector2( 100, 0), null, Color.White, 0f, Vector2.Zero, .1f, SpriteEffects.None, 0f);
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

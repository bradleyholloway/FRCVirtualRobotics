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
        List<MenuItem> menuText;
        List<MenuItem> scoreText;
        List<MenuItem> infoText;
        ControlButton playerOneA;
        float menuSpin;
        int menuSelection;
        TimeSpan startGameTime;
        double inGameTime;

        RobotState robotStates;

        Field field;
        private static List<IterativeRobot> robots;
        List<Frisbee> frisbees;
        List<ControllerInput> driverInputs;
        List<ControlButton> fire;
        List<Toggle> preAutoReady;
        List<Toggle> climbers;
        List<int> players;
        ControlButton menuUp;
        ControlButton menuDown;

        private int blueFrisbeeScore;
        private int redFrisbeeScore;
        private int blueAutoScore;
        private int redAutoScore;
        private int blueClimbScore;
        private int redClimbScore;

        public int gameState;
        public int inGameState;
        public int autoState;
        Random rand;

        SoundEffect launch;
        SoundEffect score;
        SoundEffect teleOp;
        SoundEffect start;
        SoundEffect buzzer;
        SoundEffect feed;
        SoundEffect driving;
        SoundEffect endGame;
        SoundEffect titleScreen;
        SoundEffect Transcendence;
        SoundEffectInstance transcendenceInstance;
        SoundEffectInstance titleScreenInstance;
        SoundEffectInstance endGameInstance;
        private Boolean endGameFirstCycle;

        public RobotDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameState = 0;
            menuSelection = 0;
            rand = new Random();

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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D robot1477 = (Content.Load<Texture2D>("robot2"));
            float scale1477 = .2f;
            Texture2D robot118 = Content.Load<Texture2D>("118robot");
            float scale118 = .4f;

            Frisbee.setImage(Content.Load<Texture2D>("frisbee"));

            driving = Content.Load<SoundEffect>("Driving");
            field = new Field(GraphicsDevice);
            Frisbee.setField(field);
            players = new List<int>();
            frisbees = new List<Frisbee>();

            // TODO: use this.Content to load your game content here
            robots = new List<IterativeRobot>();
            IterativeRobot.resetPlayers();
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                robots.Add(new IterativeRobot(6, GraphicsDevice, true, scale1477, driving.CreateInstance(), frisbees, robot1477));
                players.Add(0);
            }
            else
                robots.Add(null);

            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                robots.Add(new IterativeRobot(6, GraphicsDevice, false, scale118, driving.CreateInstance(), frisbees, robot118));
                players.Add(1);
            }
            else
                robots.Add(null);

            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
            {
                robots.Add(new IterativeRobot(6, GraphicsDevice, true, scale1477, driving.CreateInstance(), frisbees, robot1477));
                players.Add(2);
            }
            else
                robots.Add(null);

            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
            {
                robots.Add(new IterativeRobot(6, GraphicsDevice, false, scale1477, driving.CreateInstance(), frisbees, robot1477));
                players.Add(3);
            }
            else
                robots.Add(null);

            driverInputs = new List<ControllerInput>();
            driverInputs.Add(new ControllerInput(PlayerIndex.One));
            driverInputs.Add(new ControllerInput(PlayerIndex.Two));
            driverInputs.Add(new ControllerInput(PlayerIndex.Three));
            driverInputs.Add(new ControllerInput(PlayerIndex.Four));

            Frisbee.SPEED = 10;
            Frisbee.setFrisbees(frisbees);

            climbers = new List<Toggle>();
            for (int a = 0; a < 4; a++) 
                climbers.Add(new Toggle(false));
            
            preAutoReady = new List<Toggle>();
            for (int a = 0; a < 4; a++)
                preAutoReady.Add(new Toggle(false));

            fire = new List<ControlButton>();
            for (int a = 0; a < 4; a++)
                fire.Add(new ControlButton());

            //Score Initalization
            redAutoScore = 0;
            redClimbScore = 0;
            redFrisbeeScore = 0;
            blueAutoScore = 0;
            blueClimbScore = 0;
            blueFrisbeeScore = 0;


            //Menu control
            menuSpin = 0;
            menuUp = new ControlButton();
            menuDown = new ControlButton();
            menuItems = new List<MenuItem>();
            menuText = new List<MenuItem>();
            scoreText = new List<MenuItem>();
            infoText = new List<MenuItem>();
            menuItems.Add(new MenuItem("Play Game", new Vector2(200,200), Color.Red));
            menuItems.Add(new MenuItem("Information", new Vector2(200,300), Color.White));
            menuItems.Add(new MenuItem("Exit", new Vector2(200,400), Color.Blue));
            //menuItems.Add(new MenuItem("Test", new Vector2(200, 600), Color.White));
            menuText.Add(new MenuItem("Ultimate - Ascent! (Unofficial)", new Vector2(120,30), Color.White));
            menuText.Add(new MenuItem("By: Texas Torque, Team 1477", new Vector2(120,80),Color.White));
            infoText.Add(new MenuItem("This game is an arcade version of", new Vector2(100, 30), Color.White));
            infoText.Add(new MenuItem("Ultimate Ascent. The game play is", new Vector2(100, 80), Color.White));
            infoText.Add(new MenuItem("in accordance with the real game ", new Vector2(100, 130), Color.White));
            infoText.Add(new MenuItem("except that there are no penalties.", new Vector2(100, 180), Color.White));
            infoText.Add(new MenuItem("", new Vector2(100, 230), Color.White));
            infoText.Add(new MenuItem("Autonomous mode is pre-made,", new Vector2(50, 250), Color.White));
            infoText.Add(new MenuItem("firing three shots and then turning.", new Vector2(50, 300), Color.White));
            infoText.Add(new MenuItem("The 'Pre-Autonomous' mode lets you", new Vector2(50, 350), Color.White));
            infoText.Add(new MenuItem("to place the robot for its shots.", new Vector2(50, 400), Color.White));

            // Create a new SpriteBatch, which can be used to draw textures.
            
            if (launch == null)
            {
                launch = Content.Load<SoundEffect>("Piston");
                score = Content.Load<SoundEffect>("Score");
                feed = Content.Load<SoundEffect>("Feed");
                endGame = Content.Load<SoundEffect>("Fractilite");
                driving = Content.Load<SoundEffect>("Driving");
                buzzer = Content.Load<SoundEffect>("Buzzer");
                teleOp = Content.Load<SoundEffect>("TeleOp");
                start = Content.Load<SoundEffect>("TrumpetStart");
                titleScreen = Content.Load<SoundEffect>("CantHoldUs");
                Transcendence = Content.Load<SoundEffect>("transcendence");
                transcendenceInstance = Transcendence.CreateInstance();
                titleScreenInstance = titleScreen.CreateInstance();
                endGameInstance = endGame.CreateInstance();
                endGameInstance.IsLooped = false;
                titleScreenInstance.IsLooped = true;
            }
            endGameFirstCycle = true;
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "topGoalRed"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "midGoalRed"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "botGoalRed"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "topGoalBlue"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "midGoalBlue"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "botGoalBlue"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "redFeedTop"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "redFeedBot"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "blueFeedBot"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("goal"), "blueFeedTop"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("pyramid"), "bluePyramid"));
            field.getObjects().Add(new FieldObjects(Content.Load<Texture2D>("pyramid"), "redPyramid"));
            spriteFont = Content.Load<SpriteFont>("TimesNewRoman");

            redFrisbeeScore = blueFrisbeeScore = 0;
            robotStates = Robot.PreAUTONOMOUS;
            inGameState = 0; autoState = 0;
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
                if (titleScreenInstance.State.Equals(SoundState.Playing))
                    titleScreenInstance.Stop();
                if (transcendenceInstance.State.Equals(SoundState.Stopped))
                    transcendenceInstance.Play();

                Boolean ready = true;
                foreach (int play in players)
                    if (!preAutoReady.ElementAt<Toggle>(play).get())
                        ready = false;
                
                inGameTime = gameTime.TotalGameTime.Subtract(startGameTime).TotalSeconds;

                //Robot Control States
                if (!ready)
                {
                    inGameState = 0;
                    startGameTime = gameTime.TotalGameTime;
                    robotStates = Robot.PreAUTONOMOUS;
                    foreach (int play in players)
                    {
                        preAutoReady.ElementAt<Toggle>(play).update(driverInputs.ElementAt<ControllerInput>(play).getBottomActionButton());
                    }

                }
                else if (gameTime.TotalGameTime.Subtract(startGameTime).TotalSeconds < 15)
                {
                    if (inGameState == 0)
                        start.Play();
                    robotStates = Robot.AUTONOMOUS;
                    inGameState = 1;
                }
                else if (gameTime.TotalGameTime.Subtract(startGameTime).TotalSeconds < 135)
                {
                    if (inGameState == 1)
                        teleOp.Play();
                    robotStates = Robot.TELEOP;
                    inGameState = 2;
                }
                else if (gameTime.TotalGameTime.Subtract(startGameTime).TotalSeconds >= 135)
                {
                    buzzer.Play();
                    robotStates = Robot.DISABLED;
                    foreach (int player in players)
                    {
                        robots.ElementAt<IterativeRobot>(player).endGame();
                        robots.ElementAt<IterativeRobot>(player).setState(robotStates);
                    }
                    gameState = 2;
                }
                foreach (int player in players)
                    robots.ElementAt<IterativeRobot>(player).setState(robotStates);
                    
                foreach (int player in players)
                {
                    if (robots.ElementAt<IterativeRobot>(player).getState().equals(Robot.TELEOP) || robots.ElementAt<IterativeRobot>(player).getState().equals(Robot.PreAUTONOMOUS))
                        processInput(player);
                    else if(robots.ElementAt<IterativeRobot>(player).getState().equals(Robot.AUTONOMOUS))
                    {
                        robots.ElementAt<IterativeRobot>(player).runAuto(inGameTime);
                        /*if (inGameTime > 3 && autoState < players.Count)
                        {
                            if (robots.ElementAt<IterativeRobot>(player).fire())
                            {
                                launch.Play();
                                frisbees.Add(new Frisbee(robots.ElementAt<IterativeRobot>(player).getLocation(), robots.ElementAt<IterativeRobot>(player).getDirection() + (rand.NextDouble() - .5) / 5, robots.ElementAt<IterativeRobot>(player).getRed()));
                                autoState++;
                            }   
                        }
                        else if (inGameTime > 8 && autoState < 2*players.Count)
                        {
                            if (robots.ElementAt<IterativeRobot>(player).fire())
                            {
                                launch.Play();
                                frisbees.Add(new Frisbee(robots.ElementAt<IterativeRobot>(player).getLocation(), robots.ElementAt<IterativeRobot>(player).getDirection() + (rand.NextDouble() - .5) / 5, robots.ElementAt<IterativeRobot>(player).getRed()));
                                autoState++;
                            }
                        }
                        else if (inGameTime > 13 && autoState < 3*players.Count)
                        {
                            if (robots.ElementAt<IterativeRobot>(player).fire())
                            {
                                launch.Play();
                                frisbees.Add(new Frisbee(robots.ElementAt<IterativeRobot>(player).getLocation(), robots.ElementAt<IterativeRobot>(player).getDirection() + (rand.NextDouble() - .5) / 5, robots.ElementAt<IterativeRobot>(player).getRed()));
                                autoState++;
                            }
                        }*/
                    }
                }


                foreach (int player in players)
                    robots.ElementAt<IterativeRobot>(player).run(robots);

                for (int index = 0; index < frisbees.Count; index++)
                {
                    int frisbeeScore = 0;
                    if (frisbees.ElementAt<Frisbee>(index).getRed())
                    {
                        frisbeeScore = field.score(frisbees.ElementAt<Frisbee>(index).getLocation(), frisbees.ElementAt<Frisbee>(index).getRed());
                        redFrisbeeScore += frisbeeScore;
                        if (inGameTime < 15)
                        {
                            redAutoScore += 2 * frisbeeScore;
                            redFrisbeeScore -= frisbeeScore;
                        }

                    }
                    else
                    {
                        frisbeeScore = field.score(frisbees.ElementAt<Frisbee>(index).getLocation(), frisbees.ElementAt<Frisbee>(index).getRed());
                        blueFrisbeeScore += frisbeeScore;
                        if (inGameTime < 15)
                        {
                            blueAutoScore += 2 * frisbeeScore;
                            blueFrisbeeScore -= frisbeeScore;
                        }
                    }
                    if (frisbeeScore != 0)
                    {
                        score.Play();
                        frisbees.ElementAt<Frisbee>(index).removeSelfFromList();
                        index--;
                    }
                    else
                    {
                        foreach (Frisbee f2 in frisbees)
                            frisbees.ElementAt<Frisbee>(index).checkCollision(f2);
                        index += frisbees.ElementAt<Frisbee>(index).run();
                    }
                }
            }//If inGame
            else if (gameState == 0)
            {
                Menu();
                if(endGameInstance.State.Equals(SoundState.Playing))
                    endGameInstance.Stop();
                if (!titleScreenInstance.State.Equals(SoundState.Playing))
                    titleScreenInstance.Play();
                startGameTime = gameTime.TotalGameTime;
            }
            else if (gameState == 2)
            {
                if (endGameFirstCycle)
                {
                    transcendenceInstance.Stop();
                    foreach (int player in players)
                    {
                        if (robots.ElementAt<IterativeRobot>(player).getClimb())
                        {
                            if (robots.ElementAt<IterativeRobot>(player).getRed())
                                redClimbScore += 10;
                            else
                                blueClimbScore += 10;
                        }
                    }
                    
                    endGameFirstCycle = false;
                    endGameInstance.Play();
                    scoreText.Add(new MenuItem("Final Score", new Vector2(250,30), Color.White));
                    scoreText.Add(new MenuItem("Red Alliance", new Vector2(60,100), Color.Red));
                    scoreText.Add(new MenuItem("Blue Alliance", new Vector2(370,100), Color.Blue));
                    scoreText.Add(new MenuItem("Climb: " + redClimbScore, new Vector2(60, 200), Color.Red));
                    scoreText.Add(new MenuItem("Auto: " + redAutoScore, new Vector2(60, 250), Color.Red));
                    scoreText.Add(new MenuItem("Disk: " + redFrisbeeScore, new Vector2(60, 300), Color.Red));
                    scoreText.Add(new MenuItem("Climb: " + blueClimbScore, new Vector2(370, 200), Color.Blue));
                    scoreText.Add(new MenuItem("Auto: " + blueAutoScore, new Vector2(370, 250), Color.Blue));
                    scoreText.Add(new MenuItem("Disk: " + blueFrisbeeScore, new Vector2(370, 300), Color.Blue));
                    scoreText.Add(new MenuItem("Total: " + (redClimbScore+redFrisbeeScore+redAutoScore), new Vector2(60, 350), Color.Red));
                    scoreText.Add(new MenuItem("Total: " + (blueClimbScore+blueFrisbeeScore+blueAutoScore), new Vector2(370, 350), Color.Blue));
                    foreach (int player in players)
                        robots.ElementAt<IterativeRobot>(player).stopMoving();
                }
                if (endGameInstance.State.Equals(SoundState.Stopped) || driverInputs.ElementAt<ControllerInput>(0).getBottomActionButton())
                {
                    fire.ElementAt<ControlButton>(0).update(true);
                    gameState = 0;
                }
            }
            else if (gameState == 9)
            {
                //titleScreenInstance.Stop();
                playerOneA = new ControlButton(false);
                gameState = 10;
            }
            else if (gameState == 10)
            {
                if (playerOneA.update(driverInputs.ElementAt<ControllerInput>(0).getBottomActionButton()))
                {
                    launch.Play();
                    gameState = 0;
                }
            }
            base.Update(gameTime);
        }

        protected void Menu()
        {


            menuSpin += (float) Math.PI / 10;

            if (menuUp.update(driverInputs.ElementAt<ControllerInput>(0).getRightY() < -.8 || driverInputs.ElementAt<ControllerInput>(0).getLeftY() < -.8 || driverInputs.ElementAt<ControllerInput>(0).getDownDPad()))
                menuSelection++;
            if (menuDown.update(driverInputs.ElementAt<ControllerInput>(0).getRightY() > .8 || driverInputs.ElementAt<ControllerInput>(0).getLeftY() > .8 || driverInputs.ElementAt<ControllerInput>(0).getUpDPad()))
                menuSelection--;

            if (menuSelection < 0)
                menuSelection = menuItems.Count-1;

            if (menuSelection >= menuItems.Count)
                menuSelection = 0;

            if (fire.ElementAt<ControlButton>(0).update(driverInputs.ElementAt<ControllerInput>(0).getRightBumper() || driverInputs.ElementAt<ControllerInput>(0).getBottomActionButton()))
            {
                launch.Play();
                if (menuSelection == 0)
                {
                    LoadContent();
                    gameState = 1;
                }
                else if (menuSelection == 1)
                {
                    gameState = 9;
                }
                else if (menuSelection == 2)
                {
                    this.Exit();
                }
            }//menuMapping
            if (driverInputs.ElementAt<ControllerInput>(0).getBack())
                this.Exit();
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
                    {
                        spriteBatch.Draw(robot.getImage(), robot.getLocation(), null, robot.getColor(), robot.getDirection(), robot.getOrigin(), robot.getScale(), SpriteEffects.None, 0f);
                    }
                }
                foreach (Frisbee frisbee in frisbees)
                {
                    spriteBatch.Draw(Frisbee.getImage(), frisbee.getLocation(), null, frisbee.getColor(), frisbee.getDirection(), frisbee.getOrigin(), .06f, SpriteEffects.None, 0f);
                }
                foreach (FieldObjects fo in field.getObjects())
                {
                    spriteBatch.Draw(fo.getImage(), fo.getLocation(), null, fo.getColor(), fo.getRotation(), fo.getOrigin(), fo.getScale(), SpriteEffects.None, 0f);
                }
              //  List<double> calcs = robots.ElementAt<IterativeRobot>(0).getRectangle().getLastCalc();
              //  spriteBatch.DrawString(spriteFont, calcs.ElementAt<double>(0) + " " + calcs.ElementAt<double>(1) + " " + calcs.ElementAt<double>(2), new Vector2(120, 40), Color.White);
                spriteBatch.DrawString(spriteFont, (redFrisbeeScore + redAutoScore)+"", new Vector2(60, 0), Color.Red);
                spriteBatch.DrawString(spriteFont, (blueFrisbeeScore + blueAutoScore)+"", new Vector2(GraphicsDevice.Viewport.Width - 130,0), Color.Blue);
                spriteBatch.DrawString(spriteFont, Math.Round(inGameTime) + "", new Vector2(GraphicsDevice.Viewport.Width / 2, 0), Color.White);
            }
            else if (gameState == 0)
            {
                foreach (MenuItem menuItem in menuItems)
                    spriteBatch.DrawString(spriteFont, menuItem.text(), menuItem.location(), menuItem.color());
                foreach (MenuItem menuItem in menuText)
                    spriteBatch.DrawString(spriteFont, menuItem.text(), menuItem.location(), menuItem.color());
                spriteBatch.Draw(Frisbee.getImage(), menuItems.ElementAt<MenuItem>(menuSelection).location()- new Vector2( 100, 0), null, Color.White, menuSpin, new Vector2(Frisbee.getImage().Width/2,Frisbee.getImage().Height/2), .1f, SpriteEffects.None, 0f);
            }
            else if (gameState == 2)
            {
                foreach (MenuItem menuItem in scoreText)
                    spriteBatch.DrawString(spriteFont, menuItem.text(), menuItem.location(), menuItem.color());
            }
            else if (gameState == 10)
            {
                foreach (MenuItem menuItem in infoText)
                    spriteBatch.DrawString(spriteFont, menuItem.text(), menuItem.location(), menuItem.color());
            }

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected void processInput(int player)
        {
            double y = driverInputs.ElementAt<ControllerInput>(player).getLeftY();
            double x = -driverInputs.ElementAt<ControllerInput>(player).getRightX();
            robots.ElementAt<IterativeRobot>(player).setMotorValues(deadband(y + x), deadband(y - x));

            if (!robots.ElementAt<IterativeRobot>(player).getState().equals(Robot.PreAUTONOMOUS))
            {
                if (field.feeding(robots.ElementAt<IterativeRobot>(player).getLocation(), robots.ElementAt<IterativeRobot>(player).getRed()))
                    if (robots.ElementAt<IterativeRobot>(player).feed())
                    {
                        field.feed(robots.ElementAt<IterativeRobot>(player).getRed());
                        feed.Play();
                    }
                if (fire.ElementAt<ControlButton>(player).update((driverInputs.ElementAt<ControllerInput>(player).getRightBumper()||driverInputs.ElementAt<ControllerInput>(player).getRightTrigger()>.6)) && robots.ElementAt<IterativeRobot>(player).fire())
                {
                    launch.Play();
                    //frisbees.Add(new Frisbee(robots.ElementAt<IterativeRobot>(player).getLocation(), robots.ElementAt<IterativeRobot>(player).getDirection() + (rand.NextDouble() - .5) / 5, robots.ElementAt<IterativeRobot>(player).getRed()));
                }
            }//PreAutonmous disable feeding and shooting

            climbers.ElementAt<Toggle>(player).update(driverInputs.ElementAt<ControllerInput>(player).getRightStickDown());
            if (climbers.ElementAt<Toggle>(player).get() != robots.ElementAt<IterativeRobot>(player).getClimberUp())
                launch.Play();
            robots.ElementAt<IterativeRobot>(player).setClimber(climbers.ElementAt<Toggle>(player).get());

            //if (driverInputs.ElementAt<ControllerInput>(player).getBack())
            //    robots.ElementAt<IterativeRobot>(player).reset();

            if (driverInputs.ElementAt<ControllerInput>(player).getDownDPad())
            {
                gameState = 2;
            }
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
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
#endregion

namespace CombinedCode
{
    public enum stateOfGame { Menu, Game, Pause, Quit, LevelSelect, Results};


    public class Game1 : Game
    {
        #region FIELDS
        public static string debug;             // Debug information displayed onscreen

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //CREATE ALL MANAGER CLASSES HERE//
        ProjectileManager projectileManager;
        EnemyManager enemyManager;
        GameManager gameManager;
        ///////////////////////////////////

        MagicDraw spriteDraw;                   // Used for drawing all sprites
                                                //  Including players, enemies, buildings, and bullets

        SpriteFont font;
        Player player;
        MouseState mouseState;
        MouseState prevMouseState;
        KeyboardState kbState;
        GamePadState gpState;
        // Menus
        MainMenu mainMenu;
        PauseMenu pauseMenu;
        LevelSelectMenu levSelMenu;
        ResultsMenu resultsMenu;

        //Menu textures
        Texture2D levelSelectBTex;
        Texture2D quitToDeskBTex;
        Texture2D quitToMenuBTex;
        Texture2D resumeBTex;
        Texture2D starBG;
        Texture2D planetTex;
        Texture2D titleTex;
        Texture2D starPlanetBG;

        Texture2D texEnemy;                     // Temporary texture of the enemy
        Texture2D texProjectile;                // Temporary texture of the basic shot.
        Texture2D texProjectilePlayer;
                                                
        Texture2D gunslingerSheet;              // The gunslinger player spritesheet

        List<Map> setOfLevels;
        string[] levelList;
        int selectedLevel;
        List<Texture2D> textures;
        int numOfLevels;

        int moveSpeed;                          //**MOVE THIS TO THE PLAYER CLASS**

        Audio soundSystem;

        int playerInitialHealth;

        ForWhomTheBellTolls timeMarchesOn;      // This is, as clearly stated by its name,
                                                //  an object that deals with death animations
        Texture2D reticle;
        Rectangle reticleLoc;

        
        stateOfGame state;
        #endregion

        #region GAME1 CONSTRUCTOR
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);

            
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();
             

            Content.RootDirectory = "Content";
        }
        #endregion

        #region INITIALIZE METHOD
        protected override void Initialize()
        {
            debug = "<unused>";
            IsMouseVisible = true;  // Mouse visible for testing

            spriteDraw = new MagicDraw(font);
            timeMarchesOn = new ForWhomTheBellTolls(spriteDraw);

            soundSystem = new Audio(Content);

            player = new Player(100, 1500, 500, 30, 30, spriteDraw, texProjectile);
                                    // Will need to change this later according to new sprites
            player.SoundSys = soundSystem;

            font = this.Content.Load<SpriteFont>("Arial14");

            levelList = Directory.GetFiles("Content/Levels");

            // Managers
            projectileManager = new ProjectileManager(spriteDraw, timeMarchesOn);
            enemyManager = new EnemyManager(timeMarchesOn);
            enemyManager.SoundSys = soundSystem;
            gameManager = new GameManager("Content/highscores.txt");
            // Menus
            mainMenu = new MainMenu(175, 100, GraphicsDevice.Viewport, default(Texture2D), default(Texture2D), default(Texture2D), default(Texture2D), default(Texture2D));
            pauseMenu = new PauseMenu(175, 100, GraphicsDevice, default(Texture2D), default(Texture2D), default(Texture2D), default(Texture2D));
            levSelMenu = new LevelSelectMenu(250, 100, levelList, GraphicsDevice, default(Texture2D), default(Texture2D));
            resultsMenu = new ResultsMenu(175, 80, GraphicsDevice, gameManager, default(Texture2D));
                                    
            moveSpeed = 4;       
                                    

            setOfLevels = new List<Map>();

            numOfLevels = levelList.Length;    // Temp variable until we have something better
            for (int n = 0; n < numOfLevels; ++n )
            {
                setOfLevels.Add(
                new Map(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height,
                spriteDraw));        // Creates a new Map object, holds all map info and drawing
            }

            playerInitialHealth = player.Health;

            state = stateOfGame.Menu;

            base.Initialize();
        }
        #endregion

        #region LOAD CONTENT METHOD
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            soundSystem.LoadSounds();

            // Texture loading, will need to change this slightly if we implement a draw manager class that holds the textures
            System.IO.Stream playerStream = TitleContainer.OpenStream("Content/TestSprite.png");
            player.Texture = Texture2D.FromStream(GraphicsDevice, playerStream);
            playerStream.Close();
            texProjectile = this.Content.Load<Texture2D>("ShotEnemy.png");
            texProjectilePlayer = this.Content.Load<Texture2D>("ShotPlayer.png");
            texEnemy = this.Content.Load<Texture2D>("EnemyTest.png");
            gunslingerSheet = this.Content.Load<Texture2D>("SpriteSheetGunslinger.png");
            spriteDraw.Gunslinger = gunslingerSheet;
            reticle = this.Content.Load<Texture2D>("Reticle.png");

            
            levelSelectBTex = this.Content.Load<Texture2D>("MenuImages/SelectLevel.png");
            quitToDeskBTex = this.Content.Load<Texture2D>("MenuImages/QuitToDesktop.png");
            starBG = this.Content.Load<Texture2D>("MenuImages/space.jpg");
            planetTex = this.Content.Load<Texture2D>("MenuImages/planet.png");
            titleTex = this.Content.Load<Texture2D>("MenuImages/Logo2.png");
            quitToMenuBTex = this.Content.Load<Texture2D>("MenuImages/QuitToMenu.png");
            resumeBTex = this.Content.Load<Texture2D>("MenuImages/Resume.png");
            starPlanetBG = this.Content.Load<Texture2D>("MenuImages/planetOnSpace.jpg");

            mainMenu = new MainMenu(175, 100, GraphicsDevice.Viewport, levelSelectBTex, quitToDeskBTex, starBG, titleTex, planetTex);
            levSelMenu = new LevelSelectMenu(250, 100, levelList, GraphicsDevice, starBG, planetTex);
            pauseMenu = new PauseMenu(175, 100, GraphicsDevice, resumeBTex, quitToMenuBTex, quitToDeskBTex, starPlanetBG);
            resultsMenu = new ResultsMenu(175, 80, GraphicsDevice, gameManager, quitToMenuBTex);

            timeMarchesOn.GSSpriteSheet = this.Content.Load<Texture2D>("GunslingDieSS.png");
            timeMarchesOn.BloodSheet = this.Content.Load<Texture2D>("BloodSpatterSS.png");
            timeMarchesOn.GunFlashSheet = this.Content.Load<Texture2D>("gunshotSS.png");

            for(int i = 0; i < levelList.Length; i++)
            {
                setOfLevels[i].InitialBlockArray(levelList[i]);
            }
                


            // Loads in textures (and also does some other stuff using that data)
            textures = new List<Texture2D>();
            string[] fileList = Directory.GetFiles("Content/MapTiles1(City)");
            System.IO.Stream fileStream;
            for (int n = 0; n < fileList.Length; ++n)
            {
                fileStream = TitleContainer.OpenStream(fileList[n]);
                textures.Add(Texture2D.FromStream(
                    this.GraphicsDevice,
                    fileStream));
                fileStream.Close();
            }

            for (int n = 0; n < numOfLevels; ++n )
            {
                setOfLevels[n].LoadingInTextures(textures);
                // Sets up the vertex matrix
                setOfLevels[n].SetUpMatrix();
                setOfLevels[n].SetUpNeighbors();
            }
            
            // Loads in player bullet texture
            player.BulletTexture = texProjectilePlayer;
            
            // Loads in enemy bullet texture (and supplies a spriteDraw object for drawing)
            enemyManager.ShotTex = texProjectile;
            enemyManager.SpriteDraw = spriteDraw;

            //Font loading, currently only for testing purposes
            font = this.Content.Load<SpriteFont>("Arial14");
        }
        #endregion/

        #region UNLOAD CONTENT METHOD
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region START GAME METHOD
        private void StartGame(int selectedLevel)
        {
            //Reset/set variables
            player.Health = 100;
            enemyManager.ClearEnemies();
            enemyManager.GenerateSpawnFields(setOfLevels[selectedLevel]);
            enemyManager.PopulateSpawnFields(setOfLevels[selectedLevel]);
            gameManager.SetUpVars(enemyManager.Enemies.Count, selectedLevel);
            // Set player start point in chosen level
            Vector2 playerStartPos = setOfLevels[selectedLevel].GetPlayerStart();
            player.X = (int)playerStartPos.X;
            player.Y = (int)playerStartPos.Y;
            // Uses the above to set the screen location
            setOfLevels[selectedLevel].screenLoc = new Vector2(
                player.X - GraphicsDevice.Viewport.Width / 2,
                player.Y - GraphicsDevice.Viewport.Height / 2);
            // Makes sure camera is over map in chosen level:
            if (setOfLevels[selectedLevel].screenLoc.X < 0)
                setOfLevels[selectedLevel].screenLoc.X = 0;
            if (setOfLevels[selectedLevel].screenLoc.Y < 0)
                setOfLevels[selectedLevel].screenLoc.Y = 0;
            if (setOfLevels[selectedLevel].screenLoc.X > (setOfLevels[selectedLevel].mapPixelSize.X - setOfLevels[selectedLevel].WindowSize.X))
                setOfLevels[selectedLevel].screenLoc.X = setOfLevels[selectedLevel].mapPixelSize.X - setOfLevels[selectedLevel].WindowSize.X;
            if (setOfLevels[selectedLevel].screenLoc.Y > (setOfLevels[selectedLevel].mapPixelSize.Y - setOfLevels[selectedLevel].WindowSize.Y))
                setOfLevels[selectedLevel].screenLoc.Y = setOfLevels[selectedLevel].mapPixelSize.Y - setOfLevels[selectedLevel].WindowSize.Y;
        }
        #endregion

        #region UPDATE METHOD
        protected override void Update(GameTime gameTime)
        {
            //Store previouse mouse state
            prevMouseState = mouseState;

            //Update mouse, keyboard, and gamepad states
            mouseState = Mouse.GetState();
            kbState = Keyboard.GetState();
            gpState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            spriteDraw.PlayerMoving = false;
            
            if(state == stateOfGame.Menu)
            {
                IsMouseVisible = true;
                if (enemyManager.Enemies.Count >= 0)
                    enemyManager.Enemies.Clear();
                state = mainMenu.UpdateMenu(mouseState, prevMouseState);
                spriteDraw.Time = gameTime;
                timeMarchesOn.Time = gameTime;

                if (state == stateOfGame.LevelSelect)
                    levSelMenu.ResetLevSelMenu();
            }
            else if(state == stateOfGame.LevelSelect)
            {
                gameManager.ResetVars();
                IsMouseVisible = true;
                selectedLevel = levSelMenu.UpdateLevelSelect(mouseState, prevMouseState, gameTime);
                if(selectedLevel >= 0)
                {
                    StartGame(selectedLevel);
                    gameManager.SetUpVars(enemyManager.Enemies.Count, selectedLevel);
                    gameManager.CheckScoreFile();
                    state = stateOfGame.Game;
                    player.PreviousMouseState = true;  //Stops player from firing on entry
                }
            }
            else if(state == stateOfGame.Game)
            {
                IsMouseVisible = false;
                int scrollBuffer = 3;
                                        // The value that the screen is divided by
                                        //  in order to decide where the player needs
                                        //  to be on screen to make it scroll
                spriteDraw.Clear();
                                        // Empties list of sprites to draw,
                                        //  so that it's contents don't start adding up
                                        //  and slowing down the game
                if(kbState.IsKeyDown(Keys.Escape) ||
                    gpState.IsButtonDown(Buttons.Start))
                {
                    state = stateOfGame.Pause;
                }

                #region Map
                // Updates the window size in the Map class
                setOfLevels[selectedLevel].WindowSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                // Creates a List of all the blocks to be drawn, in order
                setOfLevels[selectedLevel].putGridOnScreen();
                #endregion

                #region Projectiles
                // Projectile and Obstacle collision
                projectileManager.CheckObstacleCollisions(setOfLevels[selectedLevel].MapForDraw, setOfLevels[selectedLevel].screenLoc);
                // Projectile and Character collision
                projectileManager.CheckCharacterCollisions(player, enemyManager.Enemies, setOfLevels[selectedLevel].screenLoc, gameManager);

                // All projectiles move
                projectileManager.MoveProjectiles();
                projectileManager.AddProjectilesToDrawList(setOfLevels[selectedLevel].screenLoc);

                //Remove any projectiles that are offscreen
                projectileManager.KillOffscreenProjectiles(
                    GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height,
                    setOfLevels[selectedLevel].screenLoc);
                // Remove any projectiles that collided with anything
                projectileManager.KillDeadProjectiles();
                #endregion

                #region Reticle

                if(gpState.IsConnected == false)
                {
                    reticleLoc = new Rectangle(
                            mouseState.X - reticle.Width / 2,
                            mouseState.Y - reticle.Height / 2,
                            reticle.Width,
                            reticle.Height);
                }
                else
                {
                    double xDiff = gpState.ThumbSticks.Right.X;
                    double yDiff = -gpState.ThumbSticks.Right.Y;

                    double hypoteneuse = (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

                    xDiff = xDiff / hypoteneuse;
                    yDiff = yDiff / hypoteneuse;

                    int radius = (int)((double)300 * hypoteneuse);

                    xDiff = xDiff * radius;
                    yDiff = yDiff * radius;

                    Vector2 localPos = player.ConvertWorldToLocal(setOfLevels[selectedLevel].screenLoc);

                    reticleLoc = new Rectangle(
                            (int)localPos.X + (int)xDiff - reticle.Width / 2,
                            (int)localPos.Y - 40 + (int)yDiff - reticle.Height / 2,
                            reticle.Width,
                            reticle.Height);
                }


                #endregion

                #region Player
                // Player movement
                player.MovePlayer(
                    moveSpeed,
                    mouseState,
                    kbState,
                    gpState,
                    scrollBuffer,
                    spriteDraw,
                    new Vector2(
                        GraphicsDevice.Viewport.Width,
                        GraphicsDevice.Viewport.Height),
                    setOfLevels[selectedLevel],
                    projectileManager,
                    gameManager);

                // Player drawing
                player.AddToDrawList(setOfLevels[selectedLevel].screenLoc, true);

                // Checking the player's state
                if (!player.IsAlive())
                {
                    IsMouseVisible = true;
                    resultsMenu.SetUpVars();
                    state = stateOfGame.Results;
                }

                // Screen scrolling
                player.ScreenScroll(mouseState, setOfLevels[selectedLevel], gpState, new Vector2(reticleLoc.X, reticleLoc.Y));
                #endregion

                #region Enemy
                // Enemy AI
                enemyManager.CheckHealth(gameManager);
                enemyManager.RunShootingAI(player.CollisionBox, setOfLevels[selectedLevel], projectileManager, gameTime);
                enemyManager.RunMovementAI(player.CollisionBox, setOfLevels[selectedLevel], gameTime, player);

                // Adds enemy to draw list
                foreach(Enemy e in enemyManager.Enemies)
                {
                    e.Texture = texEnemy;
                    e.AddEnemyToDrawList(setOfLevels[selectedLevel].screenLoc, false, e);
                }
                #endregion

                #region DeathAnimationsEtc
                timeMarchesOn.ScreenLoc = setOfLevels[selectedLevel].screenLoc;
                timeMarchesOn.Update();
                // Updates the death animation thing
                #endregion

                #region GameManager
                // Debug stuff
                //debug = gameManager.ShotsHit.ToString() + " " + gameManager.ShotsFired.ToString() + " " + gameManager.GetAccuracy().ToString();
                debug = gameManager.CalculateCurrentScore(player.Health).ToString();
                // Update level time
                gameManager.UpdateElapsedLevelTime(gameTime);
                // Check for winning
                if (gameManager.CheckWinCondition())
                {
                    resultsMenu.SetUpVars();
                    IsMouseVisible = true;
                    state = stateOfGame.Results;
                }
                #endregion
            }
            else if(state == stateOfGame.Pause)
            {
                IsMouseVisible = true;
                state = pauseMenu.UpdatePauseMenu(mouseState, prevMouseState);
                player.PreviousMouseState = true;  //Stops player from firing on entry
            }
            else if(state == stateOfGame.Quit)
            {
                IsMouseVisible = true;
                Exit();
            }
            else if(state==stateOfGame.Results)
            {
                resultsMenu.ManageDisplaytime(gameTime);
                state = resultsMenu.UpdateMenu(mouseState, prevMouseState, gameTime);
            }
            base.Update(gameTime);
        }
        #endregion

        #region DRAW METHOD
        protected override void Draw(GameTime gameTime)
        {
            // Default background color
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (state == stateOfGame.Menu)
            {
                mainMenu.DrawMenu(spriteBatch, font, GraphicsDevice.Viewport);
            }
            else if(state == stateOfGame.LevelSelect)
            {
                levSelMenu.DrawLevelSelectMenu(spriteBatch, font, GraphicsDevice.Viewport);
            }
            else if (state == stateOfGame.Game)
            {
                // Draws the base map
                setOfLevels[selectedLevel].Draw(spriteBatch);

                // Draws buildings, player, enemies, and eventually bullets
                spriteDraw.Draw(this.spriteBatch);

                //Debugging collision box
                Texture2D debugPixel = new Texture2D(GraphicsDevice, 1, 1);
                debugPixel.SetData<Color>(new Color[] { Color.White });

                Vector2 playerLocalCoords;

                playerLocalCoords = player.ConvertWorldToLocal(setOfLevels[selectedLevel].screenLoc);

                //            //
                // HEALTH BAR //
                //            //
                spriteBatch.Draw(
                    debugPixel,
                    new Rectangle(
                        15,
                        15,
                        210,
                        35),
                    Color.Black);
                spriteBatch.Draw(
                    debugPixel,
                    new Rectangle(
                        20, 
                        20, 
                        (int)(200 * ((float)player.Health/(float)playerInitialHealth)),
                        25),
                    Color.LimeGreen);

                // Draw reticle
                spriteBatch.Draw(
                    reticle,
                    reticleLoc,
                    Color.LimeGreen);


                //
                // Prints debug info text onscreen
                //
    /*                //Displays angle sprite is pointing in
                    spriteBatch.DrawString(
                        font,
                        "Angle : " + player.FaceAngle,
                        new Vector2(30, 30),
                        Color.White);
                    //Displays whether controller connected
                    spriteBatch.DrawString(
                        font,
                        "Controller Input: " + gpState.ThumbSticks.Left.X,
                        new Vector2(30, 50),
                        Color.White);
                    //Displays player coordinates
                    spriteBatch.DrawString(
                        font,
                        "Player loc: " + player.X + ", " + player.Y,
                        new Vector2(30, 70),
                        Color.White);
//                    debug = "Number of projectiles = " + projectileManager.PlayerProjectiles.Count +
//                        "\nIf screen is too big, \nchange dimensions in \nGame1 constructor";
                    //Displays debug line
     */
                Color tempCol;
                if (gameManager.CurrentScore < gameManager.HighScore)
                {
                    tempCol = Color.Red;
                }
                else if (gameManager.CurrentScore > gameManager.HighScore)
                {
                    tempCol = Color.Green;
                }
                else
                {
                    tempCol = Color.White;
                }
                    spriteBatch.DrawString(
                        font,
                        "        Score: " + (int)gameManager.CurrentScore,
                        new Vector2(1300, 30),
                        tempCol);
                    spriteBatch.DrawString(
                        font,
                        "High Score: " + (int)gameManager.HighScore,
                        new Vector2(1300, 50),
                        Color.White);
     //*/
            }
            else if(state == stateOfGame.Pause)
            {
                pauseMenu.DrawPauseMenu(spriteBatch, font, GraphicsDevice.Viewport);
            }
            else if (state == stateOfGame.Results)
            {
                resultsMenu.DrawMenu(spriteBatch, font);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}

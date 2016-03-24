#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace LevelEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState ms;
        KeyboardState kb;

        Map map;

        SpriteFont font;

        Vector2 selectedSize;
        Texture2D arrow;
        bool leftSideArrow;     // For use in changing map size
        bool prevUDKeysState;    // Whether the left mouse button was clicked last frame

        enum state {MapSize, MapEditor, EnemyCount};
        state editorState;
        bool tabDownPrev;
        bool enterDownPrev;

        bool mapSetUp;

        // Typing number
        Keys previousKeyDown;
        string numInput;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

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

            IsMouseVisible = true;
            map = new Map(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            mapSetUp = false;

            selectedSize = new Vector2(20, 20);
            leftSideArrow = true;
            prevUDKeysState = false;

            tabDownPrev = false;
            enterDownPrev = false;

            editorState = state.MapSize;

            previousKeyDown = Keys.Q;
            numInput = "";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = this.Content.Load<SpriteFont>("Arial14");

            map.LoadingInTextures(this.GraphicsDevice, font);

            System.IO.Stream fileStream;
            fileStream = TitleContainer.OpenStream("Content/EnemyTest.png");
            arrow = Texture2D.FromStream(
                GraphicsDevice,
                fileStream);
            fileStream.Close();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ms = Mouse.GetState();
            kb = Keyboard.GetState();

            if (editorState == state.MapSize)
            {
                #region Keyboard changes to map size
                if (kb.IsKeyDown(Keys.Right) && leftSideArrow == true)
                {
                    leftSideArrow = false;
                }
                if (kb.IsKeyDown(Keys.Left) && leftSideArrow == false)
                {
                    leftSideArrow = true;
                }
                if (kb.IsKeyUp(Keys.Up) && kb.IsKeyUp(Keys.Down) && prevUDKeysState == true)
                {
                    prevUDKeysState = false;
                }
                if(prevUDKeysState == false)
                {
                    if(kb.IsKeyDown(Keys.Up))
                    {
                        if (leftSideArrow == true)
                            selectedSize.X++;
                        else
                            selectedSize.Y++;
                        prevUDKeysState = true;
                    }
                    if (kb.IsKeyDown(Keys.Down))
                    {
                        if (leftSideArrow == true)
                            selectedSize.X--;
                        else
                            selectedSize.Y--;
                        prevUDKeysState = true;
                    }
                }

                if(kb.IsKeyDown(Keys.Enter) && enterDownPrev == false)
                {
                    editorState = state.MapEditor;
                    enterDownPrev = true;
                }
                #endregion
            }

            #region Map Editor
            if (editorState == state.MapEditor)
            {
                if(mapSetUp == false)
                {
                    map.SetUpMap((int)selectedSize.X, (int)selectedSize.Y);
                    mapSetUp = true;
                }

                #region Tab Key Switch
                if (kb.IsKeyDown(Keys.Tab) && tabDownPrev == false)
                {
                    map.SwitchTileSpawn();
                    tabDownPrev = true;
                }
                if (kb.IsKeyUp(Keys.Tab) && tabDownPrev == true)
                {
                    tabDownPrev = false;
                }
                #endregion

                map.putGridOnScreen();      // Prints the grid
                map.WhereIsMouse(ms.X, ms.Y);
                                            // Finds where the mouse is

                if(ms.LeftButton == ButtonState.Pressed)
                {
                    map.Click(true);
                }
                if (ms.RightButton == ButtonState.Pressed)
                {
                    map.Click(false);
                }

                // Press enter to save map to file
                if (kb.IsKeyDown(Keys.Enter))
                {
                    if (enterDownPrev == false)
                    {
                        editorState = state.EnemyCount;
                        enterDownPrev = true;
                    }
                }
                else enterDownPrev = false;
            }
            #endregion

            if (editorState == state.EnemyCount)
            {
                // Upon hitting Enter, this state happens
                // Save Window pops up, asks for enemy count
                // Use inputs of numbers and backspace key
                // Hit enter again to save

                #region Typing Numbers
                if (kb.IsKeyDown(Keys.D0) && previousKeyDown != Keys.D0)
                {
                    numInput += "0";
                    previousKeyDown = Keys.D0;
                }
                else if (kb.IsKeyDown(Keys.D1) && previousKeyDown != Keys.D1)
                {
                    numInput += "1";
                    previousKeyDown = Keys.D1;
                }
                else if (kb.IsKeyDown(Keys.D2) && previousKeyDown != Keys.D2)
                {
                    numInput += "2";
                    previousKeyDown = Keys.D2;
                }
                else if (kb.IsKeyDown(Keys.D3) && previousKeyDown != Keys.D3)
                {
                    numInput += "3";
                    previousKeyDown = Keys.D3;
                }
                else if (kb.IsKeyDown(Keys.D4) && previousKeyDown != Keys.D4)
                {
                    numInput += "4";
                    previousKeyDown = Keys.D4;
                }
                else if (kb.IsKeyDown(Keys.D5) && previousKeyDown != Keys.D5)
                {
                    numInput += "5";
                    previousKeyDown = Keys.D5;
                }
                else if (kb.IsKeyDown(Keys.D6) && previousKeyDown != Keys.D6)
                {
                    numInput += "6";
                    previousKeyDown = Keys.D6;
                }
                else if (kb.IsKeyDown(Keys.D7) && previousKeyDown != Keys.D7)
                {
                    numInput += "7";
                    previousKeyDown = Keys.D7;
                }
                else if (kb.IsKeyDown(Keys.D8) && previousKeyDown != Keys.D8)
                {
                    numInput += "8";
                    previousKeyDown = Keys.D8;
                }
                else if (kb.IsKeyDown(Keys.D9) && previousKeyDown != Keys.D9)
                {
                    numInput += "9";
                    previousKeyDown = Keys.D9;
                }
                else if (kb.IsKeyDown(Keys.Back) && 
                    previousKeyDown != Keys.Back &&
                    numInput.Length > 0)    // Is this backspace? Yes
                {
                    numInput = numInput.Substring(0, numInput.Length - 1);
                    previousKeyDown = Keys.Back;
                }
                else if(kb.IsKeyUp(Keys.D0) &&
                    kb.IsKeyUp(Keys.D1) &&
                    kb.IsKeyUp(Keys.D2) &&
                    kb.IsKeyUp(Keys.D3) &&
                    kb.IsKeyUp(Keys.D4) &&
                    kb.IsKeyUp(Keys.D5) &&
                    kb.IsKeyUp(Keys.D6) &&
                    kb.IsKeyUp(Keys.D7) &&
                    kb.IsKeyUp(Keys.D8) &&
                    kb.IsKeyUp(Keys.D9) &&
                    kb.IsKeyUp(Keys.Back))
                {
                    previousKeyDown = Keys.Q;
                }
                #endregion

                if (kb.IsKeyDown(Keys.Enter))
                {
                    if (enterDownPrev == false)
                    {
                        if(numInput.Length > 0)
                        {
                            int enemyNum = int.Parse(numInput);
                            map.EnemyCount = enemyNum;
                            map.WriteToFile("basicMap.txt");
                        }
                        editorState = state.MapEditor;
                        enterDownPrev = true;
                    }
                }
                else enterDownPrev = false;
                
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            if (editorState == state.MapSize)
            {
                #region Map Size Selection
                spriteBatch.DrawString(
                    font,
                    "Press left and right keys to switch between map x and y values.\n" +
                    "Press up and down keys to increase and decreas values.\n" +
                    "Press enter to submit map size.",
                    new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 - 100),
                    Color.Black);

                if(leftSideArrow)
                {
                    spriteBatch.DrawString(
                    font,
                    "> " + (int)selectedSize.X + " x " + (int)selectedSize.Y,
                    new Vector2(GraphicsDevice.Viewport.Width/4, GraphicsDevice.Viewport.Height/2 - 15),
                    Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(
                    font,
                    (int)selectedSize.X + " x > " + (int)selectedSize.Y,
                    new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2 - 15),
                    Color.Black);
                }

                spriteBatch.DrawString(
                    font,
                    "IN EDITOR:\n" +
                    "Click to select a tile from the list on the right\n  and click on the map to place it\n" +
                    "Right click on a tile on the list to set all tiles to that one\n" +
                    "Press tab to switch between tile editor and spawn editor",
                    new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 + 100),
                    Color.Black);
                #endregion

            }
            if (editorState == state.MapEditor)
            {
                map.Draw(spriteBatch);
            }
            if (editorState == state.EnemyCount)
            {
                // Upon hitting Enter, this state happens
                // Save Window pops up, asks for enemy count
                // Use inputs of numbers and backspace key
                // Hit enter again to save
                spriteBatch.DrawString(
                    font,
                    numInput,
                    new Vector2(GraphicsDevice.Viewport.Width/4, GraphicsDevice.Viewport.Height/2 - 15),
                    Color.Black);

                spriteBatch.DrawString(
                    font,
                    "Enter the number of enemies to spawn:",
                    new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2 - 45),
                    Color.Black);
            }

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace CombinedCode
{
    class LevelSelectMenu
    {


        private List<Button> buttonList;
        private String[] levelList;
        private int buttonX;
        private int buttonY;

        private Rectangle planetLocation;

        private Texture2D background;
        private Texture2D planet;

        private double planetSpeed;
        private float planetRotation;
            // Wanted to make the planet slowly rotate, but I can't get it to work.

        public LevelSelectMenu(int buttonWidth, int buttonHeight, String[] lvlList, GraphicsDevice graphicsDevice, Texture2D bgTex, Texture2D planetTex)
        {
            buttonX = 100;
            buttonY = 0;

            background = bgTex;
            planet = planetTex;

            planetSpeed = 20;
            planetRotation = 0;

            planetLocation = new Rectangle(200, 200, 200, 200);

            buttonList = new List<Button>();
            levelList = lvlList;


            foreach(String levelPath in levelList)
            {
                AddMapButton(Path.GetFileNameWithoutExtension(levelPath), buttonWidth, buttonHeight, graphicsDevice);

            }

        }

        private void AddMapButton(string mapName, int width, int height, GraphicsDevice gd)
        {
            buttonY += 125;

            if(buttonY > gd.Viewport.Height - height)
            {
                buttonY = 125;    
                buttonX += width + 100;
            }


            buttonList.Add(new Button(new Rectangle(buttonX, buttonY, width, height),
                 mapName, Color.DarkGray, Color.White, gd));
        }

        /// <summary>
        /// Updates level select menu to check for button presses, returns index of level when one is selected
        /// </summary>
        /// <param name="mState"></param>
        /// <param name="prevMouseState"></param>
        /// <returns>Index of selected level in list of maps</returns>
        public int UpdateLevelSelect(MouseState mState, MouseState prevMouseState, GameTime gameTime)
        {
            planetSpeed -= (gameTime.ElapsedGameTime.Milliseconds / (double)75);
            planetRotation += 0.01f;
            if (planetRotation == 2 * Math.PI) planetRotation = 0;


            if (planetSpeed > 0)
            {
                planetLocation.Width += (int)planetSpeed;
                planetLocation.Height += (int)planetSpeed;
            }

            

            
            

            int i = 0;

            foreach (Button b in buttonList)
            {
                b.UpdateButton(mState);

                if (mState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed && b.highlighted)
                {
                    return i;

                }

                i++;
            }

            return -1;


        }

        public void DrawLevelSelectMenu(SpriteBatch sb, SpriteFont font, Viewport vp)
        {

            sb.Draw(background, new Rectangle(0, 0, vp.Width, vp.Height), Color.White);

            sb.Draw(planet, planetLocation, Color.White);

            foreach(Button b in buttonList)
            {
                b.DrawButton(sb, font);
            }
            

            
        }

        public void ResetLevSelMenu()
        {
            planetSpeed = 20;

            planetLocation.Width = 200;
            planetLocation.Height = 200;
        }

    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CombinedCode
{
    class Menu
    {
        Button levelSelectButton;
        Button quitButton;

        public Menu(int buttonWidth, int buttonHeight, GraphicsDevice graphicsDevice)
        {
            levelSelectButton = new Button(
                new Rectangle((graphicsDevice.Viewport.Width - buttonWidth) / 2, graphicsDevice.Viewport.Height/4, buttonWidth, buttonHeight),
                "Level Select", Color.DarkGray, Color.White, graphicsDevice);

            quitButton = new Button(
                new Rectangle((graphicsDevice.Viewport.Width - buttonWidth) / 2, graphicsDevice.Viewport.Height/4 + 200, buttonWidth, buttonHeight),
                "Quit", Color.DarkGray, Color.White, graphicsDevice);

        }

        public stateOfGame UpdateMenu(MouseState mState, MouseState prevMouseState)
        {
            levelSelectButton.UpdateButton(mState);
            quitButton.UpdateButton(mState);

            if (mState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed)
            {
                if(levelSelectButton.highlighted)
                {
                    return stateOfGame.Game;
                }

                else if(quitButton.highlighted)
                {
                    return stateOfGame.Quit;
                }
            }



            return stateOfGame.Menu;
        }


        public void DrawMenu(SpriteBatch sb, SpriteFont font)
        {
            levelSelectButton.DrawButton(sb, font);
            quitButton.DrawButton(sb, font);
            
        }



    }
}

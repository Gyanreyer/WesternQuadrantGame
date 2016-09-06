using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CombinedCode
{
    class PauseMenu
    {
        Button resumeButton;
        Button quitToMainButton;
        Button quitToDesktopButton;

        Texture2D background;

        public PauseMenu(int buttonWidth, int buttonHeight, GraphicsDevice graphicsDevice, Texture2D resumeBTex, Texture2D quitMainBTex, Texture2D quitDeskBTex, Texture2D bgTex)
        {
            resumeButton = new Button(
                new Rectangle(
                    (graphicsDevice.Viewport.Width - buttonWidth) / 2, 
                    graphicsDevice.Viewport.Height / 4, 
                    buttonWidth, 
                    buttonHeight),
                Color.DarkGray, Color.White, resumeBTex);

            quitToMainButton = new Button(
                new Rectangle(
                    (graphicsDevice.Viewport.Width - buttonWidth) / 2, 
                    graphicsDevice.Viewport.Height / 4 + 125, 
                    buttonWidth, 
                    buttonHeight),
                Color.DarkGray, Color.White, quitMainBTex);

            quitToDesktopButton = new Button(
                new Rectangle(
                    (graphicsDevice.Viewport.Width - buttonWidth) / 2, 
                    graphicsDevice.Viewport.Height / 4 + 250, 
                    buttonWidth, 
                    buttonHeight),
                Color.DarkGray, Color.White, quitDeskBTex);

            background = bgTex;
        }


        public stateOfGame UpdatePauseMenu(MouseState mState, MouseState prevMouseState)
        {
            resumeButton.UpdateButton(mState);
            quitToMainButton.UpdateButton(mState);
            quitToDesktopButton.UpdateButton(mState);

            if (mState.LeftButton == ButtonState.Pressed && 
                prevMouseState.LeftButton != ButtonState.Pressed)
            {
                if (resumeButton.highlighted)
                {
                    return stateOfGame.Game;
                }

                else if (quitToMainButton.highlighted)
                {
                    return stateOfGame.Menu;
                }

                else if (quitToDesktopButton.highlighted)
                {
                    return stateOfGame.Quit;
                }
            }

            return stateOfGame.Pause;
        }


        public void DrawPauseMenu(SpriteBatch sb, SpriteFont font, Viewport vp)
        {
            sb.Draw(
                background, 
                new Rectangle(
                    0,
                    0,
                    vp.Width,
                    vp.Height), 
                Color.White);

            resumeButton.DrawButton(sb, font);
            quitToMainButton.DrawButton(sb, font);
            quitToDesktopButton.DrawButton(sb, font);

        }

    }
}

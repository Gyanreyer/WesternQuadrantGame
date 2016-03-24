using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CombinedCode
{
    class MainMenu
    {
        Button levelSelectButton;
        Button quitButton;

        Texture2D background;
        Texture2D gameTitle;

        Texture2D planet;

        public MainMenu(int buttonWidth, int buttonHeight, Viewport vp, Texture2D levSelTex, Texture2D quitTex, Texture2D bgTex, Texture2D titleTex, Texture2D planetTex)
        {
            levelSelectButton = new Button(
                new Rectangle((vp.Width - buttonWidth) / 2, 450, buttonWidth, buttonHeight), Color.DarkGray, Color.White, levSelTex);

            quitButton = new Button(
                new Rectangle((vp.Width - buttonWidth) / 2, 575, buttonWidth, buttonHeight), Color.DarkGray, Color.White, quitTex);

            background = bgTex;
            gameTitle = titleTex;
            planet = planetTex;
        }

        public stateOfGame UpdateMenu(MouseState mState, MouseState prevMouseState)
        {
            levelSelectButton.UpdateButton(mState);
            quitButton.UpdateButton(mState);

            if (mState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed)
            {
                if (levelSelectButton.highlighted)
                {
                    return stateOfGame.LevelSelect;
                }

                else if (quitButton.highlighted)
                {
                    return stateOfGame.Quit;
                }
            }



            return stateOfGame.Menu;
        }


        public void DrawMenu(SpriteBatch sb, SpriteFont font, Viewport vp)
        {
            sb.Draw(background, new Rectangle(0, 0, vp.Width, vp.Height), Color.White);

            sb.Draw(planet, new Rectangle(200, 200, 200, 200), Color.White);

            sb.Draw(gameTitle, new Rectangle((vp.Width - gameTitle.Width)/2, 100, gameTitle.Width, gameTitle.Height), Color.White);

            levelSelectButton.DrawButton(sb, font);
            quitButton.DrawButton(sb, font);

        }



    }
}


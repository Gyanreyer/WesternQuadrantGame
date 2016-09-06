using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CombinedCode
{
    class Button
    {
        //Fields
        public Rectangle border;
        public String text;
        public bool highlighted;
        private Texture2D buttonTexture;
        private Color borderColor;
        private Color textColor;
        private Color highlightColor;
        


        //Default constructor for plain buttons without textures
        public Button(Rectangle buttonBorder, String buttonText, Color brdrColor, Color txtColor, GraphicsDevice graphicsDevice)
        {
            border = buttonBorder;
            text = buttonText;

            borderColor = brdrColor;
            textColor = txtColor;
            highlightColor = Color.White;

            highlighted = false;

            buttonTexture = new Texture2D(graphicsDevice, 1, 1);
            buttonTexture.SetData<Color>(new Color[]{borderColor});
        }

        //Constructor for buttons with textures
        public Button(Rectangle buttonBorder, Color brdrColor, Color txtColor, Texture2D texture)
        {
            border = buttonBorder;

            borderColor = brdrColor;
            textColor = txtColor;
            highlightColor = Color.White;

            highlighted = false;

            buttonTexture = texture;
        }

        /// <summary>
        /// Update method for button
        /// </summary>
        public void UpdateButton(MouseState mState)
        {
            if (border.Contains(mState.X, mState.Y))
            {
                highlightColor = Color.Gray;
                highlighted = true;
            }
            else
            {
                highlightColor = Color.White;
                highlighted = false;
            }
        }


        /// <summary>
        /// Draw the button w/ spritebatch
        /// </summary>
        public void DrawButton(SpriteBatch sb, SpriteFont font)
        {
            sb.Draw(
                buttonTexture,
                border,
                highlightColor);

            if (text != null)
            {
                sb.DrawString(
                    font,
                    text,
                    CenterTextPos(font),
                    textColor);
            }

        }

        /// <summary>
        /// Calculate and return coordinates that text should be given to be centered on button
        /// </summary>
        /// <returns></returns>
        private Vector2 CenterTextPos(SpriteFont font)
        {
            Vector2 textDimensions = font.MeasureString(text);

            Vector2 textPosition = new Vector2();

            textPosition.X = (border.X + border.Width / 2) - textDimensions.X / 2;

            textPosition.Y = (border.Y + border.Height / 2) - textDimensions.Y / 2;

            return textPosition;


        }



        
    }
}

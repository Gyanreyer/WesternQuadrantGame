using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CombinedCode
{
    class ResultsMenu
    {
        // Fields
        Button mainMenuButton;
        GameManager gameMan;
        Rectangle onScreenloc, offScreenloc;
        int victoryBonus;
        int flawlessBonus;
        float sniperBonus;
        double speedrunBonus;
        int finalTotalScore;
        // Display vars, used for the progressive increment draw thing
        int displayValue;
        int displayIndex;
        double displayIncrement;
        bool displaying = false;
        bool displayedAll = false;

        // Constructor
        public ResultsMenu(int buttonWidth, int buttonHeight, GraphicsDevice graphicsDevice, GameManager gameMan, Texture2D menuButtonTex)
        {
            onScreenloc = new Rectangle(
                (graphicsDevice.Viewport.Width - buttonWidth) / 2, 
                graphicsDevice.Viewport.Height - (graphicsDevice.Viewport.Height / 8),
                buttonWidth, 
                buttonHeight);
            offScreenloc = new Rectangle(-200, -200, 10, 10);
            mainMenuButton = new Button(
                offScreenloc,
                Color.DarkGray, Color.White, menuButtonTex);
            this.gameMan = gameMan;
            victoryBonus = 0;
            flawlessBonus = 0;
            sniperBonus = 0;
            speedrunBonus = 0;
            displayValue = 0;
            displayIndex = -1;
            displayIncrement = 0;
        }

        //
        // Methods
        //
        /// <summary>
        /// Call this when the game switches over to this menu
        /// </summary>
        public void SetUpVars()
        {
            victoryBonus = gameMan.VictoryBonus();
            flawlessBonus = gameMan.FlawlessVictoryBonus();
            sniperBonus = gameMan.SniperBonus();
            speedrunBonus = gameMan.SpeedrunBonus();
            finalTotalScore = (int)(gameMan.ScoreAccuracy + gameMan.ScoreHealth + gameMan.ScoreKills + gameMan.ScoreTime + victoryBonus + flawlessBonus + sniperBonus + speedrunBonus);
            gameMan.CheckToWriteHighscore(finalTotalScore);
        }
        private void ResetVars()
        {
            victoryBonus = 0;
            flawlessBonus = 0;
            sniperBonus = 0;
            speedrunBonus = 0;
            displayValue = 0;
            displayIndex = -1;
            displayIncrement = 0;
            mainMenuButton.border = offScreenloc;
            displayedAll = false;
            displaying = false;
        }
        /// <summary>
        /// Call this every frame, handles the progressive display thing
        /// </summary>
        public void ManageDisplaytime(GameTime time)
        {
            // Only do anything if there's still stuff to do
            if (!displayedAll)
            {
                // If displayValue still needs to increase...
                if (displaying)
                {
                    // Increment display value
                    displayValue += time.ElapsedGameTime.Milliseconds * 2;
                    // Check display value
                    if (displayIndex == 0 && 
                        displayValue >= gameMan.ScoreKills) 
                    {
                        displayValue = (int)gameMan.ScoreKills;
                        displaying = false;
                    }
                    else if (displayIndex == 1 && 
                        displayValue >= gameMan.ScoreAccuracy)
                    {
                        displayValue = (int)gameMan.ScoreAccuracy;
                        displaying = false;
                    }
                    else if (displayIndex == 2 && 
                        displayValue >= gameMan.ScoreHealth)
                    {
                        displayValue = (int)gameMan.ScoreHealth;
                        displaying = false;
                    }
                    else if (displayIndex == 3 && 
                        displayValue >= Math.Abs(gameMan.ScoreTime))
                    {
                        displayValue = (int)gameMan.ScoreTime;
                        displaying = false;
                    }
                    else if (displayIndex == 4 && 
                        displayValue >= victoryBonus)
                    {
                        displayValue = (int)victoryBonus;
                        displaying = false;
                    }
                    else if (displayIndex == 5 && 
                        displayValue >= flawlessBonus)
                    {
                        displayValue = (int)flawlessBonus;
                        displaying = false;
                    }
                    else if (displayIndex == 6 &&
                        displayValue >= sniperBonus)
                    {
                        displayValue = (int)sniperBonus;
                        displaying = false;
                    }
                    else if (displayIndex == 7 && 
                        displayValue >= speedrunBonus)
                    {
                        displayValue = (int)speedrunBonus;
                        displaying = false;
                    }
                }
                else // If the number finished displaying
                {
                    // Increment the waiting timer
                    displayIncrement += time.ElapsedGameTime.TotalMilliseconds;
                    // If one half second has passed...
                    if (displayIncrement >= 500)
                    {
                        // Reset the variables
                        displayIncrement = 0;
                        displayValue = 0;
                        // Display the next thing
                        displayIndex++;
                        // But stop entirely if you're done
                        if (displayIndex >= 8)
                        {
                            displayedAll = true;
                        }
                        else
                        {
                            displaying = true;
                        }
                    }
                }
            }
            else
            {
                mainMenuButton.border = onScreenloc;
            }
        }
        /// <summary>
        /// Call this every frame
        /// </summary>
        public stateOfGame UpdateMenu(MouseState mState, MouseState prevMouseState, GameTime time)
        {
            mainMenuButton.UpdateButton(mState);
            if (mState.LeftButton == ButtonState.Pressed &&
                prevMouseState.LeftButton != ButtonState.Pressed)
            {
                if (mainMenuButton.highlighted)
                {
                    this.ResetVars();
                    return stateOfGame.Menu;
                }
            }
            return stateOfGame.Results;
        }
        /// <summary>
        /// Draw this every frame
        /// </summary>
        public void DrawMenu(SpriteBatch sb, SpriteFont font)
        {
            string text = "";
            // Draw the button
            mainMenuButton.DrawButton(sb, font);
            // Draw the title text
            sb.DrawString(
                font, 
                "RESULTS:", 
                new Vector2(20, 20), 
                Color.White);
            // Draw the individual fields

            // Kills
                sb.DrawString(font, 
                    "Kills:", 
                    new Vector2(20, 100),
                    Color.White);
                sb.DrawString(
                    font, 
                    gameMan.Kills.ToString(),
                    new Vector2(150, 100), 
                    Color.White);
                if (displayIndex < 0)
                    text = "0pts";
                else if (displayIndex == 0)
                    text = displayValue.ToString() + "pts";
                else
                    text = gameMan.ScoreKills + "pts";
                sb.DrawString(
                    font,
                    text, 
                    new Vector2(1200, 100), 
                    Color.White);

            // Accuracy
                sb.DrawString(
                    font,
                    "Accuracy:", 
                    new Vector2(20, 160),
                    Color.White);
                sb.DrawString(
                    font, 
                    ((int)(gameMan.GetAccuracy())).ToString() + "%", 
                    new Vector2(150, 160), 
                    Color.White);
                if (displayIndex < 1)
                    text = "0pts";
                else if (displayIndex == 1)
                    text = displayValue.ToString() + "pts";
                else
                    text = (int)gameMan.ScoreAccuracy + "pts";
                sb.DrawString(
                    font, 
                    text,
                    new Vector2(1200, 160),
                    Color.White);

            // Health
                sb.DrawString(
                    font, 
                    "Health:",
                    new Vector2(20, 220), 
                    Color.White);
                sb.DrawString(
                    font, 
                    gameMan.PlayerHealth.ToString() + "%",
                    new Vector2(150, 220),
                    Color.White);
                if (displayIndex < 2)
                    text = "0pts";
                else if (displayIndex == 2)
                    text = displayValue.ToString() + "pts";
                else
                    text = gameMan.ScoreHealth + "pts";
                sb.DrawString(
                    font, 
                    text,
                    new Vector2(1200, 220),
                    Color.White);

            // Time
                sb.DrawString(
                    font, 
                    "Time:",
                    new Vector2(20, 280), 
                    Color.White);
                sb.DrawString(
                    font, 
                    ((int)(gameMan.LevelTime / 1000)).ToString() + " Seconds", 
                    new Vector2(150, 280), 
                    Color.White);
                if (displayIndex < 3)
                    text = "0pts";
                else if (displayIndex == 3)
                    text = displayValue.ToString() + "pts";
                else
                    text = (int)gameMan.ScoreTime + "pts";
                sb.DrawString(
                    font, 
                    text, 
                    new Vector2(1200, 280),
                    Color.White);


            // Draw the bonus fields

                // Victory
                sb.DrawString(
                    font, 
                    "Victory Bonus:", 
                    new Vector2(20, 360), 
                    Color.White);
                //sb.DrawString(font, gameMan.Kills.ToString(), new Vector2(150, 360), Color.White);
                if (displayIndex < 4)
                    text = "0pts";
                else if (displayIndex == 4)
                    text = displayValue.ToString() + "pts";
                else
                    text = victoryBonus + "pts";
                sb.DrawString(
                    font,
                    text,
                    new Vector2(1200, 360), 
                    Color.White);

                // Flawless
                sb.DrawString(
                    font,
                    "Flawless Bonus:", 
                    new Vector2(20, 420), 
                    Color.White);
                //sb.DrawString(font, gameMan.Kills.ToString(), new Vector2(150, 360), Color.White);
                if (displayIndex < 5)
                    text = "0pts";
                else if (displayIndex == 5)
                    text = displayValue.ToString() + "pts";
                else
                    text = flawlessBonus + "pts";
                sb.DrawString(
                    font,
                    text, 
                    new Vector2(1200, 420),
                    Color.White);

                // Sniper
                sb.DrawString(
                    font,
                    "Sniper Bonus:", 
                    new Vector2(20, 480),
                    Color.White);
                //sb.DrawString(font, gameMan.Kills.ToString(), new Vector2(150, 360), Color.White);
                if (displayIndex < 6)
                    text = "0pts";
                else if (displayIndex == 6)
                    text = displayValue.ToString() + "pts";
                else
                    text = (int)sniperBonus + "pts";
                sb.DrawString(
                    font, 
                    text, 
                    new Vector2(1200, 480),
                    Color.White);

                // Speedrun
                sb.DrawString(
                    font, 
                    "Speedrun Bonus:",
                    new Vector2(20, 540),
                    Color.White);
                //sb.DrawString(font, gameMan.Kills.ToString(), new Vector2(150, 360), Color.White);
                if (displayIndex < 7)
                    text = "0pts";
                else if (displayIndex == 7)
                    text = displayValue.ToString() + "pts";
                else
                    text = speedrunBonus + "pts";
                sb.DrawString(
                    font, 
                    text, 
                    new Vector2(1200, 540), 
                    Color.White);

            // Draw totals
            if (displayedAll) 
            {
                sb.DrawString(
                    font, 
                    "Total:", 
                    new Vector2(20, 620),
                    Color.White);
                sb.DrawString(
                    font,
                    finalTotalScore.ToString() + "pts",
                    new Vector2(1200, 620),
                    Color.White);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace CombinedCode
{
    /// <summary>
    /// TODO:
    /// Highscores
    /// Print score vs highscore in Game1
    /// </summary>
    class GameManager
    {
        #region Fields
        int numOfEnemies;   // Game over when this hits zero
        int kills;          // Used when calculating score
        double currentScore;   // Will be finalized at the end of the level (with bonuses and whatnot)
        double highScore;      // Will be read from a textfile
        double levelTime;   // Used to calculate level score. Units = milliseconds
        int shotsFired;     // Used to calculate score
        int shotsHit;       // ^^
        int playerHealth;
        string filepath;
        string[] lines;
        int currentLevel;
            #region Score Vars
        int scoreKills;
        float scoreAccuracy;
        int scoreHealth;
        double scoreTime;
            #endregion
        #endregion

        public GameManager(string path)
        {
            numOfEnemies = 0;
            currentScore = 0;
            highScore = 0;
            levelTime = 0;
            shotsFired = 0;
            shotsHit = 0;
            kills = 0;
            scoreKills = 0;
            scoreAccuracy = 0;
            scoreHealth = 0;
            scoreTime = 0;
            filepath = path;
        }

        #region Properties
        public double LevelTime
        { get { return levelTime; } }
        public int PlayerHealth
        { get { return playerHealth; } }
        public int Kills
        { get { return kills; } }
        public int ScoreKills
        { get { return scoreKills; } }
        public float ScoreAccuracy
        { get { return scoreAccuracy; } }
        public int ScoreHealth
        { get { return scoreHealth; } }
        public double ScoreTime
        { get { return scoreTime; } }
        public int ShotsFired
        { get { return shotsFired; } }
        public int ShotsHit
        { get { return shotsHit; } }
        public int NumOfEnemies
        {get { return numOfEnemies; }set { numOfEnemies = value; }}
        public double CurrentScore
        { get { return currentScore; } set { currentScore = value; } }
        public double HighScore
        { get { return highScore; } set { highScore = value; } }
        #endregion

        #region Methods
            #region Variable Stuff
        /// <summary>
        /// Must be called each time the main menu is accessed
        /// </summary>
        public void ResetVars()
        {
            numOfEnemies = 0;
            currentScore = 0;
            highScore = 0;
            levelTime = 0;
            shotsFired = 0;
            shotsHit = 0;
            kills = 0;
            lines = null; 
        }
        /// <summary>
        /// Must be called each time a level starts, AFTER the enemy manager spawns all the enemies
        /// </summary>
        public void SetUpVars(int numOfEnemies, int selectedLevel)
        {
            this.numOfEnemies = numOfEnemies;
            currentLevel = selectedLevel;
            // High score stuff 
            try
            {
                lines = System.IO.File.ReadAllLines(filepath);
                highScore = Int32.Parse(lines[currentLevel]);
            }
            catch(Exception e)
            {
                highScore = 0;
            }
        }
        /// <summary>
        /// Call this method whenever the player shoots
        /// </summary>
        public void IncrementShotsFired()
        {
            shotsFired++;
        }
        /// <summary>
        /// Call this method whenever a PlayerProjectile hits an enemy
        /// </summary>
        public void IncrementShotsHit()
        {
            shotsHit++;
        }
        /// <summary>
        /// Call this whenever an enemy dies
        /// </summary>
        public void IncrementKills()
        {
            kills++;
        }
        /// <summary>
        /// Returns the accuracy, i.e. shotsHit/shotsFired
        /// </summary>
        public float GetAccuracy()
        {
            return ((float)shotsHit / (float)shotsFired) * 100;
        }
        #endregion
            #region Update Stuff (call each frame)
        /// <summary>
        /// Check this every frame
        /// </summary>
        public bool CheckWinCondition()
        {
            return (numOfEnemies == 0);
        }
        /// <summary>
        /// Must call this every frame
        /// </summary>
        public void UpdateElapsedLevelTime(GameTime time)
        {
            levelTime += time.ElapsedGameTime.TotalMilliseconds;
        }
        /// <summary>
        /// Adds some stuff up to get the current score, can be called each frame to update the score counter
        /// </summary>
        public double CalculateCurrentScore(int health)
        {
            playerHealth = health;
            // Reset to re-calculate
            scoreKills = 0;
            scoreAccuracy = 0;
            scoreTime = 0;
            scoreHealth = 0;
            // Add for each enemy killed
            scoreKills += (kills * 100);
            // Add for accuracy
            float accuracy = GetAccuracy();
            if (!float.IsNaN(accuracy)) // Prevent division by zero
            scoreAccuracy += (accuracy * accuracy) / 10;    // Adds a huge bonus for accuracy to greatly promote precise shooting (If the bonus was too small, nobody would care about accuracy)
                                                            // For reference, a 100% accuracy rating will give a bonus of 1000 points, equal to 10 extra kills
                                                            // ... but a 1% rating will give a bonus of 0.1 point, equal to 0.001 extra kills
            // Add for health
            scoreHealth += playerHealth * 10;
            // Subtract for time
            scoreTime -= levelTime / 100; // 1 second = -10 points
            // Return the sum
            currentScore = scoreKills + scoreAccuracy + scoreHealth + scoreTime;
            return currentScore;
        }
        #endregion
            #region Results Stuff (call at the results screen)
        /// <summary>
        /// Returns a large bonus if the player perfected the level
        /// </summary>
        public int FlawlessVictoryBonus()
        {
            if (playerHealth == 100)
                return 1500;
            else
                return 0;
        }
        /// <summary>
        /// Returns a small bonus if the player even beat the level
        /// </summary>
        public int VictoryBonus()
        {
            if (numOfEnemies == 0)
                return 500;
            else
                return 0;
        }
        /// <summary>
        /// Returns a large bonus if the player had high accuracy; an even higher bonus if they somehow (almost) never missed a shot
        /// </summary>
        public float SniperBonus()
        {
            float acc = GetAccuracy();
            if (acc >= 99)
                return 1500;
            else if (acc >= 90) // Bonus granted anyway for 90% or higher
                return (acc * 10);
            else
                return 0f;
        }
        /// <summary>
        /// Returns a bonus if they won in under a minute
        /// </summary>
        public double SpeedrunBonus()
        {
            if (numOfEnemies == 0 && levelTime < 60000)
                return 500;
            else
                return 0;
        }
        /// <summary>
        /// Call this when the results screen first pops up
        /// </summary>
        public void CheckToWriteHighscore(int finalTotalScore)
        {
            if (finalTotalScore > highScore)
            {
                lines[currentLevel] = finalTotalScore.ToString();
            }

            System.IO.File.WriteAllLines(filepath, lines);
        }
        /// <summary>
        /// Call this each time a level starts
        /// </summary>
        public void CheckScoreFile()
        {
            // If there's no score text for a new map...
            while (lines.Length < currentLevel)
            {
                /*
                // Copy old text
                string[] oldLines = lines;
                lines = new string[currentLevel + 1];
                for (int i = 0; i < oldLines.Length; i++)
                {
                    lines[i] = oldLines[i];
                }
                // Make some text up
                lines[currentLevel] = "0";
                // Write it back
                System.IO.File.WriteAllLines(filepath, lines);
                 */
                System.IO.File.AppendText("0");
                lines = System.IO.File.ReadAllLines(filepath);
            }
        }
        #endregion
        #endregion
    }
}

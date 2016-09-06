using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CombinedCode
{
    class ProjectileManager
    {
        //
        // Fields
        //
        List<PlayerProjectile> playerProjectiles;
        List<EnemyProjectile> enemyProjectiles;

        MagicDraw drawList;
        ForWhomTheBellTolls deathList;


        //
        // Properties
        //
        #region Properties
        public List<PlayerProjectile> PlayerProjectiles
        {
            get { return playerProjectiles; }
        }
        public List<EnemyProjectile> EnemyProjectiles
        {
            get { return enemyProjectiles; }
        }
        #endregion


        //
        // Constructor
        //
        public ProjectileManager(MagicDraw list, ForWhomTheBellTolls dList)
        {
            playerProjectiles = new List<PlayerProjectile>();
            enemyProjectiles = new List<EnemyProjectile>();
            drawList = list;
            deathList = dList;
        }

        #region Methods
            #region AddToDrawList
        public void AddProjectilesToDrawList(Vector2 screenLoc)
        {
            float newX;
            float newY;
            // Player
            for (int i = 0; i < playerProjectiles.Count; i++)
            {
                newX = (playerProjectiles[i].X - screenLoc.X);
                newY = (playerProjectiles[i].Y - screenLoc.Y);

                Vector2 coordinates = new Vector2(newX, newY);
                Texture2D image = playerProjectiles[i].Texture;

                int y;
                if (playerProjectiles[i].YForDrawing == int.MaxValue)
                    y = (int)(coordinates.Y + image.Height);
                else
                    y = playerProjectiles[i].YForDrawing;
                
                bool character = false;
                double rotation = 0;
                drawList.Add(coordinates, y, image, character, rotation, false);
            }
            // Enemy
            for (int i = 0; i < enemyProjectiles.Count; i++)
            {
                newX = (enemyProjectiles[i].X - screenLoc.X);
                newY = (enemyProjectiles[i].Y - screenLoc.Y);

                Vector2 coordinates = new Vector2(newX, newY);
                Texture2D image = enemyProjectiles[i].Texture;
                int y = (int)(coordinates.Y + image.Height);
                bool character = false;
                double rotation = 0;
                drawList.Add(coordinates, y, image, character, rotation, false);
            }
        }
        #endregion

            #region ProjectileSpawning
        /// <summary>
        /// Spawn player projectiles- used when player fires weapon
        /// </summary>
        /// <param name="collisionBox">Collision box of projectile</param>
        /// <param name="directionX">X component of projectile's directional movement</param>
        /// <param name="directionY">Y component of projectile's directional movement</param>
        public void SpawnPlayerProjectile(Rectangle collisionBox, float directionX, float directionY, int speed, Texture2D tex)
        {
            playerProjectiles.Add(new PlayerProjectile(collisionBox, directionX, directionY, speed, tex));

            deathList.AddDeadGuy(
                    new Vector2(
                        playerProjectiles[playerProjectiles.Count - 1].CollisionBox.X + playerProjectiles[playerProjectiles.Count - 1].CollisionBox.Width,
                        playerProjectiles[playerProjectiles.Count - 1].CollisionBox.Y + playerProjectiles[playerProjectiles.Count - 1].CollisionBox.Height),
                        false,
                        false,
                        true);
        }
        public void SpawnPlayerProjectile(Rectangle collisionBox, float directionX, float directionY, int speed, Texture2D tex, int yForDrawing)
        {
            playerProjectiles.Add(new PlayerProjectile(collisionBox, directionX, directionY, speed, tex));
            playerProjectiles[playerProjectiles.Count - 1].YForDrawing = yForDrawing;

            deathList.AddDeadGuy(
                    new Vector2(
                        playerProjectiles[playerProjectiles.Count - 1].CollisionBox.X + playerProjectiles[playerProjectiles.Count - 1].CollisionBox.Width,
                        playerProjectiles[playerProjectiles.Count - 1].CollisionBox.Y + playerProjectiles[playerProjectiles.Count - 1].CollisionBox.Height),
                        false,
                        false,
                        true);
        }

        /// <summary>
        /// Spawn enemy projectiles- used for AI attacking and stuff
        /// </summary>
        public void SpawnEnemyProjectile(Rectangle collisionBox, float directionX, float directionY, int speed, Texture2D tex)
        {
            enemyProjectiles.Add(new EnemyProjectile(collisionBox, directionX, directionY, speed, tex));

            deathList.AddDeadGuy(
                    new Vector2(
                        enemyProjectiles[enemyProjectiles.Count - 1].CollisionBox.X + enemyProjectiles[enemyProjectiles.Count - 1].CollisionBox.Width,
                        enemyProjectiles[enemyProjectiles.Count - 1].CollisionBox.Y + enemyProjectiles[enemyProjectiles.Count - 1].CollisionBox.Height),
                        false,
                        false,
                        true);
        }
        public void SpawnEnemyProjectile(Rectangle collisionBox, float directionX, float directionY, int speed, Texture2D tex, int yForDrawing)
        {
            enemyProjectiles.Add(new EnemyProjectile(collisionBox, directionX, directionY, speed, tex));
            enemyProjectiles[enemyProjectiles.Count - 1].YForDrawing = yForDrawing;

            deathList.AddDeadGuy(
                    new Vector2(
                        enemyProjectiles[enemyProjectiles.Count - 1].CollisionBox.X + enemyProjectiles[enemyProjectiles.Count - 1].CollisionBox.Width,
                        enemyProjectiles[enemyProjectiles.Count - 1].CollisionBox.Y + enemyProjectiles[enemyProjectiles.Count - 1].CollisionBox.Height),
                        false,
                        false,
                        true);
        }
        #endregion
            #region Projectile Movement
        /// <summary>
        /// Moves all projectiles automatically according to their current direciton vector
        /// </summary>
        public void MoveProjectiles()
        {
            foreach (Projectile p in playerProjectiles)
            {
                p.Move();
            }
            foreach (Projectile pE in enemyProjectiles)
            {
                pE.Move();
            }
        }
        #endregion

            #region KillOffscreenProjectiles
        /// <summary>
        /// Wipes out all projectiles that are offscreen
        /// </summary>
        public void KillOffscreenProjectiles(int screenWidth, int screenHeight, Vector2 screenLoc)
        {
            // Player projectiles
            for (int i = playerProjectiles.Count-1; i >= 0; i-- )
            {
                PlayerProjectile p = playerProjectiles[i];

                int newX = (int)(p.X - screenLoc.X);
                int newY = (int)(p.Y - screenLoc.Y);

                if (newX > screenWidth || newY > screenHeight || newX < 0 || newY < 0)
                {
                    p = null;
                    playerProjectiles.RemoveAt(i);
                }
            }
            // Enemy projectiles
            for (int i = enemyProjectiles.Count - 1; i >= 0; i--)
            {
                EnemyProjectile p = enemyProjectiles[i];

                int newX = (int)(p.X - screenLoc.X);
                int newY = (int)(p.Y - screenLoc.Y);

                if (newX > screenWidth || newY > screenHeight || newX < 0 || newY < 0)
                {
                    p = null;
                    enemyProjectiles.RemoveAt(i);
                }
            }
        }
        #endregion
            #region KillDeadProjectiles
        /// <summary>
        /// Kills all projectiles marked for death (i.e. collided with an enemy/player)
        /// </summary>
        public void KillDeadProjectiles()
        {
            // Player projectiles
            for (int i = 0; i < playerProjectiles.Count; i++) 
            {
                PlayerProjectile p = playerProjectiles[i];
                if (!p.Alive)
                {
                    p = null;
                    playerProjectiles.RemoveAt(i);
                }
            }
            // Enemy projectiles
            for (int i = 0; i < enemyProjectiles.Count; i++) 
            {
                EnemyProjectile e = enemyProjectiles[i];
                if(!e.Alive)
                {
                    e = null;
                    enemyProjectiles.RemoveAt(i);
                }
            }
        }
        #endregion

            #region CheckObstacleCollisions()
        /// <summary>
        /// Checks for collisions and deletes projectiles if they collide with stuff
        /// NOTE: THIS CURRENTLY USES THE BRUTE FORCE METHOD.
        /// </summary>
        /// <param name="gridSpaces">List of GridSpaces to check with as objects</param>
        public void CheckObstacleCollisions(List<GridSpace> gridSpaces, Vector2 screenLoc)
        {
            PlayerProjectile p;
            Rectangle gRect;
            for (int i = 0; i < playerProjectiles.Count; ++i)
            {
                p = playerProjectiles[i];

                Vector2 screenPos = new Vector2(
                (p.CollisionBox.X - screenLoc.X),
                (p.CollisionBox.Y - screenLoc.Y));
                Rectangle screenCollisionBox = new Rectangle((int)screenPos.X, (int)screenPos.Y, p.CollisionBox.Width, p.CollisionBox.Height);

                //                                                      //
                // Bullets collide with objects onscreen and disappear  //
                //                                                      //

                for(int j=0; j<gridSpaces.Count; j++)
                {
                    gRect = new Rectangle(
                        (int)gridSpaces[j].PositionOnScreen.X,
                        (int)gridSpaces[j].PositionOnScreen.Y,
                        gridSpaces[j].Image.Width, 
                        gridSpaces[j].Image.Height - 40);

                    if (gridSpaces[j].ObstacleBool && screenCollisionBox.Intersects(gRect))
                    {
                        p = null;
                        playerProjectiles.RemoveAt(i);
                        break;
                    }
                }
            }
            // Enemy Projectiles
            EnemyProjectile pE;
            for (int i = 0; i < enemyProjectiles.Count; ++i)
            {
                pE = enemyProjectiles[i];
                Vector2 screenPos = new Vector2(
                (pE.CollisionBox.X - screenLoc.X),
                (pE.CollisionBox.Y - screenLoc.Y));
                Rectangle screenCollisionBox = new Rectangle((int)screenPos.X, (int)screenPos.Y, pE.CollisionBox.Width, pE.CollisionBox.Height);
                //                                                      //
                // Bullets collide with objects onscreen and disappear  //
                //                                                      //
                for (int j = 0; j < gridSpaces.Count; j++)
                {
                    gRect = new Rectangle((int)gridSpaces[j].PositionOnScreen.X, (int)gridSpaces[j].PositionOnScreen.Y, gridSpaces[j].Image.Width, gridSpaces[j].Image.Height);
                    if (gridSpaces[j].ObstacleBool && screenCollisionBox.Intersects(gRect))
                    {
                        pE = null;
                        enemyProjectiles.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        #endregion
            #region CheckCharacterCollisions()
        /// <summary>
        /// Checks for collisions and deletes projectiles if they collide with stuff
        /// </summary>
        /// <param name="gridSpaces">List of GridSpaces to check with as objects</param>
        public void CheckCharacterCollisions(Player p, List<Enemy> enemies, Vector2 screenLoc, GameManager gameMan)
        {
            // PlayerProjectiles vs. Enemy collisions
            for (int i = 0; i < playerProjectiles.Count; ++i)
            {
                if (playerProjectiles[i].CheckEnemyCollisions(enemies, deathList)) 
                {
                    // If the player hits an enemy, then increase their accuracy rating
                    gameMan.IncrementShotsHit();
                }
            }
            // EnemyProjectiles vs. Player collisions
            for (int i = 0; i < enemyProjectiles.Count; ++i)
            {
                enemyProjectiles[i].CheckPlayerCollisions(p, deathList);
            }
        }
        #endregion
        #endregion
    }
}

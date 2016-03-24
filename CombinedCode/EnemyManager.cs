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
    class EnemyManager
    {
        #region Fields
        List<Enemy> enemies;
        List<SpawnField> spawnFields;
        Random rand;
        MagicDraw spriteDraw;
        Texture2D shotTex;

        Audio soundSys;

        ForWhomTheBellTolls deathObject;
        #endregion

        //
        // Constructor
        //
        public EnemyManager(ForWhomTheBellTolls deathObj)
        {
            enemies = new List<Enemy>();
            rand = new Random();
            spawnFields = new List<SpawnField>();
            
            deathObject = deathObj;
        }

        #region Properties
        public List<Enemy> Enemies
        {
            get { return enemies; }
        }
        public Texture2D ShotTex
        {
            set { shotTex = value; }
        }
        public MagicDraw SpriteDraw
        {
            set { spriteDraw = value; }
        }
        public Audio SoundSys { set { soundSys = value; } }
        #endregion

        #region Methods
            #region Spawning and Enemy management
        /// <summary>
        /// Adds a new enemy to the list of enemies
        /// </summary>
        /// <param name="e"></param>
        public void SpawnEnemy(Enemy e)
        {
            if (e != null)
            {
                enemies.Add(e);
                enemies[(enemies.Count - 1)].SoundSys = soundSys;
            }
                
        }

        /// <summary>
        /// Clears the list of enemies
        /// </summary>
        public void ClearEnemies()
        {
            enemies.Clear();
        }

        /// <summary>
        /// Effectively converts the enemy spawn tiles vector list into a list of spawnfield objects
        /// This MUST be run before running "PopulateSpawnFields"
        /// </summary>
        public void GenerateSpawnFields(Map mapData)
        {
            spawnFields.Clear();
            List<Vector2> locations = mapData.EnemySpawnTiles;
            Vector2 scaledLocationVector;
            foreach (Vector2 v in locations)
            {
                scaledLocationVector = new Vector2(v.X * mapData.BlockSize.X, v.Y * mapData.BlockSize.Y);
                spawnFields.Add(new SpawnField(scaledLocationVector, (int)mapData.BlockSize.X, (int)mapData.BlockSize.Y));
            }
            //Game1.debug = "SpawnFields.Count = " + spawnFields.Count;
        }

        /// <summary>
        /// Actually spawns enemies within the spawn fields. Only one enemy per field, and not all fields will be used
        /// NOTE: "EnemyCount" MUST BE GREATER THAN THE NUMBER OF SPAWN FIELD TILES PLACED IN THE MAP EDITOR
        /// </summary>
        public void PopulateSpawnFields(Map mapData)
        {
            enemies.Clear();
            int enemiesSpawned = 0;
            int spawnIndex = 0;
            int randomTypeIndex = 0;
            Enemy tempE;
            // Loop to spawn the given number of enemies
            //while (enemiesSpawned < spawnFields.Count && enemiesSpawned < mapData.EnemyCount)
            while (spawnFields.Count > 0 && enemiesSpawned < mapData.EnemyCount)
            {
                // Get an index for a random field
                spawnIndex = rand.Next(0, spawnFields.Count);
                // Get a random spawn point from a random field
                Vector2 spawnPoint = spawnFields[spawnIndex].GetRandomSpawnPoint(rand);
                // Create an enemy with a randomized type
                randomTypeIndex = rand.Next(0, 4);
                switch(randomTypeIndex)
                {
                    case 0:
                        tempE = new Enemy(100, (int)spawnPoint.X, (int)spawnPoint.Y, 32, 32, spriteDraw, shotTex, EType.Fast);
                        break;
                    case 1:
                        tempE = new Enemy(100, (int)spawnPoint.X, (int)spawnPoint.Y, 32, 32, spriteDraw, shotTex, EType.Normal);
                        break;
                    case 2:
                        tempE = new Enemy(100, (int)spawnPoint.X, (int)spawnPoint.Y, 32, 32, spriteDraw, shotTex, EType.Sniper);
                        break;
                    case 3:
                        tempE = new Enemy(100, (int)spawnPoint.X, (int)spawnPoint.Y, 32, 32, spriteDraw, shotTex, EType.Strong);
                        break;
                    default:
                        tempE = new Enemy(100, (int)spawnPoint.X, (int)spawnPoint.Y, 32, 32, spriteDraw, shotTex, EType.Normal);
                        break;
                }
                // Force loading texture
                tempE.TexProjectile = shotTex;
                tempE.DeathObject = deathObject;

                // Spawn an enemy in that field
                SpawnEnemy(tempE);
                spawnFields[spawnIndex].Occupied = true;

                // Remove the field from the list to avoid duplicates
                //*
                //spawnFields.RemoveAt(spawnIndex);
                //*

                // Prevent infinite looping
                enemiesSpawned++;
            }
        }
        #endregion
            #region AI
        /// <summary>
        /// Exectues enemy methods on all enemies in the list
        /// </summary>
        public void RunMovementAI(Rectangle target, Map mapDat, GameTime time, Player p)
        {
            foreach(Enemy e in enemies)
            {
                // Only run A* once every 4 - 6 seconds
                if (e.IsReadyForPathfinding(time, rand.Next(4, 6)))
                {
                    e.Path = mapDat.AStar(mapDat.GetVertexAtLocation(e.X, e.Y), mapDat.GetVertexAtLocation(target.X, target.Y));
                    // Need to empty the priority queue
                    while (mapDat.PQ.Dequeue() != null) ;
                }
                //e.Path = mapDat.AStar(new Vertex(e.CollisionBox, true), new Vertex(target, false)); // need to get the acutal vertex
                if (e.Path != null && e.Path.Count > 1)// && HelperMethods.GetDist(new Vector2(p.CollisionBox.X, p.CollisionBox.Y), new Vector2(e.CollisionBox.X, e.CollisionBox.Y)) > e.DistBeforeStopping)
                    e.UpdateMovement(time, rand.Next(1, 6));
            }
        }

        /// <summary>
        /// Tells all the enemies to try to shoot at the center of the given target
        /// </summary>
        public void RunShootingAI(Rectangle targetBounds, Map mapData, ProjectileManager projMan, GameTime time)
        {
            int newX, newY;
            double dist;
            foreach (Enemy e in enemies)
            {
                newX = (int)(e.X - mapData.screenLoc.X);
                newY = (int)(e.Y - mapData.screenLoc.Y);
                dist = HelperMethods.GetDist(new Vector2(targetBounds.X, targetBounds.Y), new Vector2(e.CollisionBox.X, e.CollisionBox.Y));
                // Only do this for enemies that are on the screen and that are within range
                if (newX <= mapData.WindowSize.X && newY <= mapData.WindowSize.Y && newX >= 0 && newY >= 0 && dist <= e.SightRange)  // && rand.Next(0, 1) == 0 
                //NOTE: We could add a random chance here (see comment in the above line). Not the optimal solution but it might help with performance a tiny bit since there honestly isn't a whole lot else I can think of, in terms of enemy stuff.
                //Quadtrees would totally do the trick and most likely remove all performance problems entirely but I have higher priorities at the moment so I'm not going to work on that now 
                    e.UpdateShooting(targetBounds, mapData, projMan, time, rand);
            }
        }

        /* ControlEnemies() (A debug method)
        /// <summary>
        /// USED FOR DEBUGGING, will be removed later
        /// </summary>
        /// <param name="dir"></param>
        public void ControlEnemies(int dir)
        {
            switch (dir)
            {
                case 0:
                    foreach (Enemy e in enemies)
                        e.Y -= 4;
                    break;
                case 1:
                    foreach (Enemy e in enemies)
                        e.Y += 4;
                    break;
                case 2:
                    foreach (Enemy e in enemies)
                        e.X -= 4;
                    break;
                case 3:
                    foreach (Enemy e in enemies)
                        e.X += 4;
                    break;
            }
        }*/

        #endregion
            #region Logic Stuff
        public void CheckHealth(GameManager gameMan)
        {
            for (int i = 0; i < enemies.Count; i++) 
            {
                if (enemies[i].Health <= 0)
                {
                    // Run the kill enemy method (does nothing at the moment)
                    enemies[i].KillEnemy(rand.Next(5), gameMan);
                    // Remove the reference to the enemy
                    enemies[i] = null;
                    // Remove the enemy from the list
                    enemies.RemoveAt(i);
                    // Reduce index to compensate
                    i--;
                }
            }
        }
        #endregion
        #endregion
    }
}

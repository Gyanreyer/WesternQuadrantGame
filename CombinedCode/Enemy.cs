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
    /// The Enemy Type. "Normal" for a regular enemy, a color for a more difficult enemy.
    /// For example, a Blue enemy could move faster and a Red one could have more HP. We can figure out the details later.
    /// </summary>
    enum EType
    {
        Normal, // Red
        Fast, //Blue
        Strong, //Yellow
        Sniper, //Green
        //<insert others here>
    }

    class Enemy : Entity
    {
        #region Fields
        EType type;
        Vector2 loc; // Because the whole collisionBox thing is not good enough
        // Shooting fields
        int sightRange; 
        Texture2D texProjectile;
        int shotSpread;
        int shotSpeed;
        Vector2 direction;
        // Shot interval fields:
        int shotInterval;
        int timeSinceShooting;
        bool readyToShoot;
        // Pathfinding stuff:
        List<Vertex> path;
        Vertex targetVertex;
        Vector2 targetVector;
        Vector2 normalizedMoveVector;
        int distBeforeStopping;
        int timeToTogglePathfinding;
        int timeSincePathfinding;
        // Movement stuff:
        int timeToToggleMovement;
        int timeSinceMovement;
        float moveSpeed = 1.5f; //default value
        bool moving;
        // Animation stuff:
        Animation anim;

        // Death stuff
        ForWhomTheBellTolls deathObject;
        #endregion

        //
        // Constructor
        //
        public Enemy(int health, int x, int y, int width, int height, MagicDraw draw, Texture2D shotTex, EType type) 
            : base(health,x,y,width,height, draw)
        {
            texProjectile = shotTex;
            timeSinceMovement = 0;
            moving = false;
            timeSinceShooting = 0;
            readyToShoot = false;
            loc = new Vector2(x, y);
            path = null;
            targetVertex = null;
            anim = new Animation(7, 10);
            this.type = type;
            #region Type-based custom vars
            switch (this.type)
            {
                case EType.Normal:
                    shotSpread = 2; //NOTE: Keep this at just 1 or 2. Anything higher gets too extreme. 0 for perfect accuracy.
                    shotSpeed = 5;
                    shotInterval = 2500;  // Change this as necessary. Units = milliseconds
                    timeToToggleMovement = 2000;
                    moveSpeed = 1.5f;
                    sightRange = 555; // change as necessary. Units = pixels
                    base.health = 100;
                    break;
                case EType.Fast:
                    shotSpread = 2;
                    shotSpeed = 4;
                    moveSpeed = 2.5f;
                    shotInterval = 2000; 
                    timeToToggleMovement = 2500;
                    sightRange = 420;
                    base.health = 100;
                    break;
                case EType.Sniper:
                    shotSpread = 0;
                    shotSpeed = 8;
                    moveSpeed = 0.7f;
                    shotInterval = 4750; 
                    timeToToggleMovement = 4000;
                    sightRange = 720;
                    base.health = 200;
                    break;
                case EType.Strong:
                    shotSpread = 1;
                    shotSpeed = 4;
                    moveSpeed = 1.2f;
                    shotInterval = 2250; 
                    timeToToggleMovement = 2000;
                    sightRange = 575;
                    base.health = 600;
                    break;
            }
            #endregion

            distBeforeStopping = 200;   // Distance where the enemy will stop moving
                                        // NOTE: THE GAME WILL CRASH IF THIS IS TOO SMALL (200 seems to be a decent value)
                                        // and I'm not going to bother fixing it since it looks awkward when the enemies move that close anyway
                                        // Edit: this variable might not even be necessary, now that I've used the path count in its place
            
        }

        #region Properties
        public Animation Animation { get { return anim; } }
        public int SightRange
        { get { return sightRange; } }
        public EType Type
        {
            get { return type; }
        }
        public Vector2 DirectionVector
        { get { return direction; } }
        public float DirectionAngle
        {
            get
            {
                if (direction.X == 0)
                {
                    if (direction.Y > 0)
                        return (float)Math.PI / 2;
                    else
                        return (float)(-0.5 * Math.PI);
                }
                else
                {
                    return (float)Math.Atan2(direction.Y, direction.X);
                }
            }
        }
        public bool IsMoving
        { get { return moving; } }
        public int DistBeforeStopping
        { get { return distBeforeStopping; } }
        public Texture2D TexProjectile
        {
            get { return texProjectile; }
            set { texProjectile = value; }
        }
        public bool ReadyToShoot
        {
            get { return readyToShoot; }
            set { readyToShoot = value; }
        }
        public float ShotInterval
        {
            get { return shotInterval; }
        }
        public List<Vertex> Path
        { get { return path; } set { path = value; } }

        public ForWhomTheBellTolls DeathObject { set { deathObject = value; } }
        #endregion
        
        //
        // Methods
        //
        #region AI Stuff
            #region CheckLineOfSight()
        /// <summary>
        /// Checks line of sight towards a target rectangle
        /// </summary>
        /// <param name="targetBounds"></param>
        /// <param name="sightDist"></param>
        /// <param name="mapData"></param>
        /// <returns></returns>
        private bool CheckLineOfSight(Rectangle targetBounds, float sightDist, Map mapData)
        {
            //create a variable for the grid spaces
            List<GridSpace> gridSpaces = mapData.MapForDraw;
            //create position vector
            Vector2 position = new Vector2(this.collisionBox.X, this.collisionBox.Y);
            //create a line that points towards the target of magnitude 1
            direction = new Vector2(targetBounds.X, targetBounds.Y) - position;
            direction.Normalize();
            // ... But increase the size of the vector enough to keep accuracy while boosting speed
            direction *= (targetBounds.Width * 1.25f); //NOTE: IF DETECTION PROBLEMS ARISE, IT'S PROBABLY BECAUSE THIS NEW VECTOR IS TOO BIG
                                                      //EDIT: Meh, who cares. A bigger scaling vector means it'll run faster, and we need that at this point.
            //create a line that points towards the target, beginning at magnitude 1
            Vector2 line = direction;
            //create a vector that will be used in collision detection
            Vector2 testPoint;
            //create a rectangle used for checking collisions
            Rectangle testRect;
            // Loop for collision detection. Keep going until you've reached the LOS limit
            int dist = (int)Math.Pow(sightDist, 2.0d);
            while (line.LengthSquared() < dist)
            {
                // Scale the vector forward a bit
                line = line + direction;

                // Update the test point
                testPoint = position + line; // Enemy's position plus the line of sight vector

                // Check for the target
                testRect = new Rectangle((int)testPoint.X, (int)testPoint.Y, targetBounds.Width, targetBounds.Height);
                if (targetBounds.Intersects(testRect)) 
                {
                    //Game1.debug = "FOUND @ " + testPoint + ", Player = " + targetBounds.X + ", " + targetBounds.Y;
                    return true;    // Finding the target was a success
                }

                // Scan for map objects - QUADTREES WOULD SPEED THIS UP
                for (int i = 0; i < gridSpaces.Count; i++)
                {
                    Vector2 worldVector = HelperMethods.ConvertLocalToWorld(new Vector2(mapData.MapForDraw[i].PositionOnScreen.X, mapData.MapForDraw[i].PositionOnScreen.Y), mapData.screenLoc);
                    if (gridSpaces[i].ObstacleBool && testRect.Intersects(new Rectangle((int)worldVector.X, (int)worldVector.Y, gridSpaces[i].Image.Width, gridSpaces[i].Image.Height))) 
                    {
                        //Game1.debug = "NOT FOUND, stuck @ " + testPoint + ", Player = " + targetBounds.X + ", " + targetBounds.Y;
                        return false;   // Finding target failed
                    }
                }
            }
            return false;   // Give up if the line never hit anything at all
        }
        #endregion
            #region ShootAtTarget()
        /// <summary>
        /// Actually shoots bullets... after checking line of sight
        /// </summary>
        /// <param name="targetBounds"></param>
        /// <param name="mapData"></param>
        /// <param name="projMan"></param>
        private void ShootAtTarget(Rectangle targetBounds, Map mapData, ProjectileManager projMan, Random rand)
        {
            if (CheckLineOfSight(targetBounds, sightRange, mapData))
            {
                Vector2 position = new Vector2(this.collisionBox.X, this.collisionBox.Y);
                direction = new Vector2(targetBounds.X + (targetBounds.Width / 2), targetBounds.Y + (targetBounds.Height / 2)) - position;
                direction.Normalize();
                direction *= shotSpeed;
                projMan.SpawnEnemyProjectile(new Rectangle(this.collisionBox.X, this.collisionBox.Y, 8, 8), direction.X + (rand.Next(-shotSpread, shotSpread)), direction.Y + (rand.Next(-shotSpread, shotSpread)), shotSpeed, texProjectile, this.collisionBox.Y + this.texProjectile.Height);
                SoundSys.PlaySoundEffect("Gunshot");
            }
        }
        #endregion
            #region UpdateShooting()
        /// <summary>
        /// Should be run every frame via the enemy manager. Handles all shooting stuff
        /// </summary>
        /// <param name="targetBounds"></param>
        /// <param name="mapData"></param>
        /// <param name="projMan"></param>
        /// <param name="time"></param>
        public void UpdateShooting(Rectangle targetBounds, Map mapData, ProjectileManager projMan, GameTime time, Random rand)
        {
            if (!readyToShoot)
            {
                // Increase time since previous shot
                timeSinceShooting += time.ElapsedGameTime.Milliseconds;
                // Compare with shooting interval
                if (timeSinceShooting > shotInterval)
                {
                    // Get ready to shoot
                    readyToShoot = true;
                }
            }
            else
            {
                // Try to shoot at the target
                ShootAtTarget(targetBounds, mapData, projMan, rand);
                // Reset the timeSinceShooting variable
                timeSinceShooting = rand.Next(0, 1000);
                // Don't shoot again for a while
                readyToShoot = false;
            }
        }
        #endregion
            #region UpdateMovement()
        /// <summary>
        /// Manages enemy movement. Doesn't look perfect, but the slight randomness helps keep enemies from walking on top of each other
        /// </summary>
        public void UpdateMovement(GameTime time, int rand)
        {
            timeSinceMovement += time.ElapsedGameTime.Milliseconds;
            if(moving)
            {
                this.Move();
                if (timeSinceMovement > timeToToggleMovement)
                {
                    moving = false;
                    timeSinceMovement = 0;
                    timeToToggleMovement = 1000 * rand / 2;//rand.Next(1, 3);
                }
            }
            else
            {
                if (timeSinceMovement > timeToToggleMovement)
                {
                    moving = true;
                    timeSinceMovement = 0;
                    timeToToggleMovement = 1000 * rand;//rand.Next(1, 6);
                }
            }
        }
        /// <summary>
        /// Manages enemy pathfinding vars
        /// </summary>
        public bool IsReadyForPathfinding(GameTime time, int rand)
        {
            timeSincePathfinding += time.ElapsedGameTime.Milliseconds;
            // If you're ready to look for a new path...
            if (timeSincePathfinding > timeToTogglePathfinding)
            {
                // Reset vars
                timeSincePathfinding = 0;
                timeToTogglePathfinding = 1000 * rand;
                return true;
            }
            else
                return false;
        }
            #endregion
            #region Move()
        public void Move()  // Note: assumes that path.Count > 1
        {
            // Get vector to move towards
            //targetVertex = this.path[path.Count - 1];
            targetVertex = this.path[1];
            targetVector = new Vector2(targetVertex.Loc.X + (targetVertex.Loc.Width / 2), targetVertex.Loc.Y + (targetVertex.Loc.Height / 2));
            // Use it to get a movement vector
            normalizedMoveVector = (targetVector - this.loc);
            normalizedMoveVector.Normalize(); // Magnitude 1
            normalizedMoveVector *= moveSpeed;
            // Move using that vector
            this.loc.X += normalizedMoveVector.X;
            this.loc.Y += normalizedMoveVector.Y;
            // Update direction vector
            direction = normalizedMoveVector;
            // Do the collisionbox thing
            this.collisionBox.X = (int)loc.X;
            this.collisionBox.Y = (int)loc.Y;
            // Check to remove vertex (does this even do anything?)
            if (targetVertex.Loc.Contains(this.loc))
            {
                //this.path.RemoveAt(path.Count - 1);
                this.path.RemoveAt(0);
            }
        }
        #endregion
        #endregion

        #region Logic Stuff
        public void KillEnemy(int randInt, GameManager gameMan)
        {
            // Death SFX
            switch (randInt)
            {
                case 0:
                    SoundSys.PlaySoundEffect("Wilhelm");
                    break;
                case 1:
                    SoundSys.PlaySoundEffect("Mein Leben!");
                    break;            
            }
            
            // Death animation
            deathObject.AddDeadGuy(new Vector2(this.X, this.Y), true, false, false);
            //System.Diagnostics.Debug.WriteLine("WorldPos: (" + this.X + ", " + this.Y + ")");

            // Reduce enemy count
            gameMan.NumOfEnemies--;
            // Tally up confirmed kills
            gameMan.IncrementKills();
        }
        #endregion
    }
}

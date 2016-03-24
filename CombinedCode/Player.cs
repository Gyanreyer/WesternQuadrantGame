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
    class Player : Entity
    {
        Texture2D bulletTexture;
        bool prevMouseState;
        Rectangle prevLoc; //Previous location; used for collision response

        Boolean shootinHand;

        float xDiff;
        float yDiff;    // Made public for gamepad reasons

        #region Properties
        public Rectangle PrevLoc
        {
            get { return prevLoc; }
            set { prevLoc = value; }
        }
        public Texture2D BulletTexture
        {
            get
            {
                return bulletTexture;
            }
            set
            {
                bulletTexture = value;
            }
        }
        public bool PreviousMouseState
        {
            set
            {
                prevMouseState = value;
            }
        }
        #endregion

        //Constructor
        public Player(int health, int x, int y, int width, int height, MagicDraw drawList, Texture2D bulletTex)
            : base(health, x, y, width, height, drawList)
        {
            bulletTexture = bulletTex;
            prevMouseState = false;

            shootinHand = true;

            xDiff = 1;
            yDiff = 0;
        }

        #region Aiming
        /// <summary>
        /// Changes player's angle based on the position of the mouse on the screen
        /// </summary>
        /// <param name="mState">Mouse state to be used for calculations</param>
        public void Aim(MouseState mState, ProjectileManager projManager, Map mapData, GameManager gameMan, GamePadState gpState)
        {
            int screenMoveBuffer = 3;

            //Get x and y differences between mouse position and player position
            Vector2 local = ConvertWorldToLocal(mapData.screenLoc);

            if(gpState.IsConnected == false)
            {
                xDiff = mState.X - (int)local.X;
                yDiff = mState.Y - ((int)local.Y - 40);
            }
            else if (gpState.ThumbSticks.Right.X != 0 || -gpState.ThumbSticks.Right.Y != 0)
            {
                xDiff = gpState.ThumbSticks.Right.X;
                yDiff = -gpState.ThumbSticks.Right.Y;
            }

            if (xDiff == 0)
            {
                if (yDiff > 0)
                    faceAngle = (float)Math.PI / 2;

                else
                    faceAngle = (float)(-0.5 * Math.PI);


            }
            else
            {
                //Get angle to mouse w/ inverse tangent
                FaceAngle = (float)Math.Atan2(yDiff, xDiff);
            }

            if (mState.X < (mapData.screenLoc.X / screenMoveBuffer) && local.X < 2*(mapData.screenLoc.X / screenMoveBuffer))
            {
                mapData.screenLoc.X -= 3;
            }

            Vector2 v = HelperMethods.AngleToVector(faceAngle);

            //Projectile shooting
            if (mState.LeftButton == ButtonState.Pressed || gpState.IsButtonDown(Buttons.RightTrigger) || gpState.IsButtonDown(Buttons.RightShoulder))
            {
                if (prevMouseState == false)
                {
                    shootinHand = !shootinHand;

                    double rotation = faceAngle;

                    if (rotation <= 0)
                        rotation = -rotation;
                    else
                        rotation = (2 * Math.PI) - rotation;
                    rotation = rotation * (180 / Math.PI);


                    Rectangle bulletRectangle = new Rectangle(X, Y, 10, 10);

                    //East
                    if ((rotation >= 0 &&
                        rotation < 22.5) || (rotation < 360 &&
                        rotation >= 337.5))
                    {
                        bulletRectangle.Y = Y-collisionBox.Height - 20;
                        bulletRectangle.X = X + collisionBox.Width;

                        if (shootinHand) bulletRectangle.Y -= 5;
                        else bulletRectangle.Y += 5;
                    }

                    // NorthEast
                    else if (rotation >= 22.5 &&
                        rotation < 67.5)
                    {
                        bulletRectangle.Y = Y - collisionBox.Height - 30;
                        bulletRectangle.X = X + collisionBox.Width - 10;

                        if (shootinHand) bulletRectangle.X -= 20;
                        else bulletRectangle.X += 0;
                    }

                    // North
                    else if (rotation >= 67.5 &&
                        rotation < 112.5)
                    {
                        bulletRectangle.Y = Y - collisionBox.Height - 35;
                        //bulletRectangle.X = X - 10;     

                        if (shootinHand) bulletRectangle.X -= 10;
                        else bulletRectangle.X += 5;
                    }

                    // NorthWest
                    else if (rotation >= 112.5 &&
                        rotation < 157.5)
                    {
                        bulletRectangle.Y = Y - collisionBox.Height - 30;
                        bulletRectangle.X = X - 10;

                        if (shootinHand) bulletRectangle.X -= 15;
                        else bulletRectangle.X += 5;
                    }

                    // West
                    else if (rotation >= 157.5 &&
                        rotation < 202.5)
                    {
                        bulletRectangle.Y = Y - (int)(0.8 * collisionBox.Height) - 20;
                        bulletRectangle.X = X - collisionBox.Width;

                        if (shootinHand) bulletRectangle.Y -= 10;
                        else bulletRectangle.Y += 0;
                    }

                    // SouthWest
                    else if (rotation >= 202.5 &&
                        rotation < 247.5)
                    {
                        bulletRectangle.Y = Y - (int)(0.6 * collisionBox.Height) - 15;
                        bulletRectangle.X = X - collisionBox.Width / 2 - 15;

                        if (shootinHand) bulletRectangle.X -= 15;
                        else bulletRectangle.X += 15;
                    }

                    // South
                    else if (rotation >= 247.5 &&
                        rotation < 292.5)
                    {
                        bulletRectangle.Y = Y - (collisionBox.Height / 2) - 10;
                        bulletRectangle.X = X + (collisionBox.Width / 2) - 15;

                        if (shootinHand) bulletRectangle.X -= 10;
                        else bulletRectangle.X += 5;
                    }

                    // SouthEast
                    else if (rotation >= 292.5 &&
                        rotation < 337.5)
                    {
                        bulletRectangle.Y = Y - (int)(0.6 * collisionBox.Height) - 15;
                        bulletRectangle.X = X + collisionBox.Width;

                        if (shootinHand) bulletRectangle.X -= 15;
                        else bulletRectangle.X += 5;
                    }

                    // And if all else fails...
                    else
                    {
                       // bulletRectangle.X = X - (collisionBox.Width / 2);
                        bulletRectangle.Y = Y - (collisionBox.Height / 2);
                    }
                    

                    projManager.SpawnPlayerProjectile(bulletRectangle, (float)(10 * Math.Cos(faceAngle)), (float)(10 * Math.Sin(faceAngle)), 10, bulletTexture, Y + 1);
                    gameMan.IncrementShotsFired();

                    SoundSys.PlaySoundEffect("Gunshot");

                    prevMouseState = true;
                }
            }
            else
            {
                if (prevMouseState == true)
                {
                    prevMouseState = false;
                }
            }
        }

        /// <summary>
        /// Changes player's angle based on right stick
        /// </summary>
        /// <param name="gpState">Gamepad state for calculations</param>
        /// 
        public void Aim(GamePadState gpState, ProjectileManager projManager)
        {
            /*
            float x = gpState.ThumbSticks.Right.X;
            float y = -gpState.ThumbSticks.Right.Y;

            if (!(x == 0 && y == 0)) //Prevents sprite from snapping back to default when thumbsticks not being used
            {
                faceAngle = (float)Math.Atan2(y, x);

                //Avoid division by zero, although I tested this quite vigorously and it seemed nearly impossible to get the x axis to exactly 0
                if (x == 0)
                {
                    if (y > 0)
                        faceAngle = (float)Math.PI / 2;

                    else
                        faceAngle = (float)(1.5 * Math.PI);
                }
            }

            if(gpState.IsButtonDown(Buttons.RightTrigger))
            {
                if(prevMouseState == false)
                {
                    projManager.SpawnPlayerProjectile(new Rectangle(X, Y, 10, 10), (float)(10*Math.Cos(faceAngle)), (float)(10*Math.Sin(faceAngle)), 10, bulletTexture);

                    SoundSys.PlaySoundEffect("Gunshot");

                    prevMouseState = true;
                }
            }
            else
            {
                if(prevMouseState == true)
                {
                    prevMouseState = false;
                }
            }
             */
        }
        #endregion

        #region Scrolling
        /// <summary>
        /// Scrolls the screen based on mouse movement. A polish effect.
        /// </summary>
        public void ScreenScroll(MouseState mState, Map mapDat, GamePadState gpState, Vector2 reticleLoc)
        {
            // Mouse's location on the screen
            Vector2 mouseLocOnScreen = new Vector2(mState.X, mState.Y);

            if(gpState.IsConnected)
            {
                mouseLocOnScreen = reticleLoc;
                Vector2 pPos = ConvertWorldToLocal(mapDat.screenLoc);
                float distance;
                Vector2.Distance(ref mouseLocOnScreen, ref pPos, out distance);
                //distance = (float)Math.Sqrt((pPos.X - mouseLocOnScreen.X)*(pPos.X - mouseLocOnScreen.X) + (pPos.Y - mouseLocOnScreen.Y)*(pPos.Y - mouseLocOnScreen.Y));
                if (distance > 400) mouseLocOnScreen = pPos;

                
                //if(gpState.IsButtonDown(Buttons.LeftShoulder))
                //{
                //    System.Diagnostics.Debug.WriteLine("Distance is " + distance);
                //}
                
            }

            // Point in the exact middle of the screen
            Vector2 halfScreen = new Vector2(mapDat.WindowSize.X / 2, mapDat.WindowSize.Y / 2);
            // Ratio to scale the screen by
            Vector2 scaleRatio = new Vector2(Math.Abs(halfScreen.X - mouseLocOnScreen.X) / halfScreen.X, 
                                            Math.Abs(halfScreen.Y - mouseLocOnScreen.Y) / halfScreen.Y);
            // Scale the scaling ratio (because the full effect is a bit extreme) - CHANGE THIS AS NECESSARY... maybe have an options menu for it?
            scaleRatio *= 0.25f; 
            // Determine direction
            if (mouseLocOnScreen.X < halfScreen.X)
                scaleRatio.X = -scaleRatio.X;
            if (mouseLocOnScreen.Y < halfScreen.Y)
                scaleRatio.Y = -scaleRatio.Y;
            // Vector of the player's location (not 100% stable due to collision box integer rounding)
            Vector2 loc = new Vector2(this.collisionBox.X, this.collisionBox.Y);
            // Final vector to write to the screen location
            Vector2 result = new Vector2(loc.X - halfScreen.X + (halfScreen.X * scaleRatio.X), loc.Y - halfScreen.Y + (halfScreen.Y * scaleRatio.Y));
            // Preventing the scrolling from going out of bounds
            if (result.X < 0)
                result.X = 0;
            if (result.Y < 0)
                result.Y = 0;
            if (result.X + mapDat.WindowSize.X > mapDat.mapPixelSize.X)
                result.X = mapDat.mapPixelSize.X - mapDat.WindowSize.X;
            if (result.Y + mapDat.WindowSize.Y > mapDat.mapPixelSize.Y)
                result.Y = mapDat.mapPixelSize.Y - mapDat.WindowSize.Y;
            // Write the vector to the screen location
            mapDat.screenLoc = result;
        }
        #endregion

        #region CheckCollision()
        /// <summary>
        /// Checks for collisions with the given list of gridspaces and responds in the given direction. Uses the brute force method. We should really change this later.
        /// </summary>
        /// <param name="gridSpaces"></param>
        /// <param name="directionIsX">1=up, 2=down, 3=left, 4=right</param>
        public void CheckCollision(List<GridSpace> gridSpaces, int direction, Vector2 screenLoc)
        {
            // New collision box based on what's on screen:
            Vector2 screenPos = ConvertWorldToLocal(screenLoc);

            Rectangle screenCollision = new Rectangle(
                    (int)screenPos.X - collisionBox.Width / 3,
                    (int)screenPos.Y - collisionBox.Height / 3,
                    collisionBox.Width,
                    collisionBox.Height * 2 / 3);

            // Collision detection
            for (int i = 0; i < gridSpaces.Count; i++)
            {
                if (gridSpaces[i].ObstacleBool 
                    && screenCollision.Intersects(
                    new Rectangle(
                        (int)gridSpaces[i].PositionOnScreen.X,
                        (int)gridSpaces[i].PositionOnScreen.Y,
                        gridSpaces[i].Image.Width, 
                        gridSpaces[i].Image.Height)))
                {
                    //Game1.debug = "collision detected @ " + gridSpaces[i].PositionOnScreen.X + ", " + gridSpaces[i].PositionOnScreen.Y;
                    //Collision response in the specified direction
                    switch(direction)
                    {
                        case 1:
                            collisionBox.Y = (int)prevLoc.Y;
                            break;
                        case 2:
                            collisionBox.Y = (int)prevLoc.Y;
                            break;
                        case 3:
                            collisionBox.X = (int)prevLoc.X;
                            break;
                        case 4:
                            collisionBox.X = (int)prevLoc.X;
                            break;
                    }
                }
            }
        }
#endregion

        #region MovePlayer()
        public void MovePlayer(
            int moveSpeed, 
            MouseState mouseState,
            KeyboardState kbState,
            GamePadState gpState,
            int scrollBuffer,
            MagicDraw spriteDraw,
            Vector2 windowSize,
            Map mapData,
            ProjectileManager projectileManager,
            GameManager gameMan)
        {
            //First, update the player's previous location before moving, in case of triggering a collision response
            this.PrevLoc = new Rectangle(this.X, this.Y, this.CollisionBox.Width, this.CollisionBox.Height);

                // If the controller is in,
                //  use Gamepad controls
                bool controllerIn = gpState.IsConnected;
            
                Vector2 local = this.ConvertWorldToLocal(mapData.screenLoc);

                //Sprint key increases move speed
                if(kbState.IsKeyDown(Keys.LeftShift) || gpState.IsButtonDown(Buttons.LeftTrigger))
                {
                    moveSpeed = (int)(moveSpeed * 1.75);
                }

                if (gpState.ThumbSticks.Left.Y > 0 ||
                    kbState.IsKeyDown(Keys.W) &&
                    kbState.IsKeyUp(Keys.S))                                        // UP BUTTON
                {
                    spriteDraw.PlayerMoving = true;

                    if(gpState.IsConnected)
                        this.Y -= (int)(moveSpeed * 0.99 * gpState.ThumbSticks.Left.Y);
                    else
                        this.Y -= (int)(moveSpeed * 0.99);                          // If so, this should change their world position
                                                                                    //  and a method in the Player/Entity class
                                                                                    //  should convert that to the onscreen position,
                                                                                    //  then pass that into the MagicDraw object
                    if (local.Y < windowSize.Y / scrollBuffer)
                    {
                        //mapData.screenLoc.Y -= (int)(moveSpeed * .99);

                        this.PrevLoc = new Rectangle(
                            this.PrevLoc.X, 
                            this.PrevLoc.Y + (int)(moveSpeed * .99), 
                            this.PrevLoc.Width, 
                            this.PrevLoc.Height);   //used for collision detection; will be changed later

                        if (mapData.screenLoc.Y < 0)                                //Keep the map from going out of bounds
                            mapData.screenLoc.Y = 0;
                    }
                    // Collision detection
                    this.CheckCollision(mapData.MapForDraw, 1, mapData.screenLoc);
                }
                else if (gpState.ThumbSticks.Left.Y < 0 ||
                    kbState.IsKeyDown(Keys.S) &&
                    kbState.IsKeyUp(Keys.W))                                        // DOWN BUTTON
                {
                    spriteDraw.PlayerMoving = true;

                    if (gpState.IsConnected)
                        this.Y += (int)(moveSpeed * 0.99 * gpState.ThumbSticks.Left.Y * -1);
                    else
                        this.Y += (int)(moveSpeed * 0.99);

                    if (local.Y >= (scrollBuffer - 1) * windowSize.Y / scrollBuffer &&
                        mapData.screenLoc.Y != (int)(mapData.mapPixelSize.Y - windowSize.Y)
                        )
                    {
                        //mapData.screenLoc.Y += (int)(moveSpeed * .99);

                        this.PrevLoc = new Rectangle(
                            this.PrevLoc.X, 
                            this.PrevLoc.Y - (int)(moveSpeed * .99), 
                            this.PrevLoc.Width, 
                            this.PrevLoc.Height);

                        if (mapData.screenLoc.Y >= (int)(mapData.mapPixelSize.Y - windowSize.Y))
                            mapData.screenLoc.Y = (int)(mapData.mapPixelSize.Y - windowSize.Y);
                    }
                    // Collision detection
                    this.CheckCollision(mapData.MapForDraw, 2, mapData.screenLoc);
                }

                if (gpState.ThumbSticks.Left.X < 0 ||
                    kbState.IsKeyDown(Keys.A) &&
                    kbState.IsKeyUp(Keys.D))                                        // LEFT BUTTON
                {
                    spriteDraw.PlayerMoving = true;

                    if (gpState.IsConnected)
                        this.X -= (int)(moveSpeed * 0.99 * gpState.ThumbSticks.Left.X * -1);
                    else
                        this.X -= (int)(moveSpeed * 0.99);

                    if (local.X <= windowSize.X / scrollBuffer
                        && mapData.screenLoc.X != 0)
                    {
                        //mapData.screenLoc.X -= (int)(moveSpeed * .99);

                        this.PrevLoc = new Rectangle(
                            this.PrevLoc.X + (int)(moveSpeed * .99), 
                            this.PrevLoc.Y, 
                            this.PrevLoc.Width, 
                            this.PrevLoc.Height);

                        if (mapData.screenLoc.X < 0)
                            mapData.screenLoc.X = 0;
                    }
                    // Collision detection
                    this.CheckCollision(mapData.MapForDraw, 3, mapData.screenLoc);
                }


                else if (gpState.ThumbSticks.Left.X > 0 ||
                    kbState.IsKeyDown(Keys.D) &&
                    kbState.IsKeyUp(Keys.A))                                        // RIGHT BUTTON
                {
                    spriteDraw.PlayerMoving = true;

                    if (gpState.IsConnected)
                        this.X += (int)(moveSpeed * 0.99 * gpState.ThumbSticks.Left.X);
                    else
                        this.X += (int)(moveSpeed * 0.99);

                    if (local.X >= (scrollBuffer - 1) * windowSize.X / scrollBuffer
                        && mapData.screenLoc.X != (int)(mapData.mapPixelSize.X - windowSize.X))
                    {
                        //mapData.screenLoc.X += (int)(moveSpeed * .99);

                        this.PrevLoc = new Rectangle(
                            this.PrevLoc.X - (int)(moveSpeed * .99),
                            this.PrevLoc.Y, 
                            this.PrevLoc.Width, 
                            this.PrevLoc.Height);

                        if (mapData.screenLoc.X >= (int)(mapData.mapPixelSize.X - windowSize.X))
                            mapData.screenLoc.X = (int)(mapData.mapPixelSize.X - windowSize.X);
                    }
                    // Collision detection
                    this.CheckCollision(mapData.MapForDraw, 4, mapData.screenLoc);
                }
                // Update player's angle based on mouse pos
                this.Aim(mouseState, projectileManager, mapData, gameMan, gpState);
            
        }
        #endregion

        #region IsAlive()
        /// <summary>
        /// Returns the state of the player's life
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return health > 0;
        }
        #endregion
    }
}

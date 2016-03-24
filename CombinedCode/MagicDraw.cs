using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace CombinedCode
{

    // I should probably change the class name... nah
    class MagicDraw
    {
        #region Fields and Properties

        static List<MagicDrawObject> drawList;      // List of objects/sprites to draw
        SpriteFont font;                            // Font for any text drawn (probably for debugging)
        Texture2D gunslinger;                       // The gunslinger sprite

        int timePassed;                             // Time passed since last sprite frame change
        bool moveKeyDown;                           // Whether a movement key is down
                                                    //  (therefore, whether the player is moving)

        public List<MagicDrawObject> DrawList
        {
            get
            {
                return drawList;
            }
        }

        GameTime time;                              // Time, updated each frame
        Animation animGS;                           // Gunslinger animation
        //Animation animEnemyGS;                      // Enemy gunslinger animation
        bool playerMoving;                          // If the player is moving
        int xValGS;    

        public Texture2D Gunslinger
        {
            get
            {
                return gunslinger;
            }
            set
            {
                gunslinger = value;
            }
        }

        public bool MoveKeyDown
        {
            set
            {
                moveKeyDown = value;
            }
        }                               // Where on GS sheet is current frame

        public GameTime Time
        {
            set
            {
                time = value;
            }
        }

        public bool PlayerMoving
        {
            set
            {
                playerMoving = value;
            }
        }

        #endregion

        public MagicDraw (SpriteFont f)
        {
            font = f;
            drawList = new List<MagicDrawObject>();
            timePassed = 0;
            moveKeyDown = false;

            time = default(GameTime);
            animGS = new Animation(7, 10);
            playerMoving = false;
        }

        #region Adding Things!

        public void Add(Vector2 coordinates, int y, Texture2D image, bool character, double rotation, bool player)
        {
            MagicDrawObject thing = new MagicDrawObject(coordinates, y, image, character, rotation, player);

            if(drawList.Count == 0)
                drawList.Add(thing);
            else
                FitIntoList(thing);
        }

        public void AddDeath(Vector2 coordinates, int y, Texture2D image, int frame, bool death)
        {
            MagicDrawObject thing = new MagicDrawObject(coordinates, y, image, frame, true);

            if (drawList.Count == 0)
                drawList.Add(thing);
            else
                FitIntoList(thing);
        }

        public void AddEnemy(Vector2 coordinates, int y, Texture2D image, bool character, double rotation, bool player, Enemy obj)
        {
            MagicDrawObject thing = new MagicDrawObject(coordinates, y, image, character, rotation, player);
            thing.EnemyObj = obj;

            if (drawList.Count == 0)
                drawList.Add(thing);
            else
                FitIntoList(thing);
        }

        #endregion

        /// <summary>
        /// Puts things into the list ordered by their y value
        /// </summary>
        /// <param name="obj"></param>
        void FitIntoList(MagicDrawObject obj)
        {
            /*Binary Search:*/
            int left = 0;
            int right = drawList.Count - 1;
            int center = 0; // Will be assigned properly at the beginning of the loop
            bool lookingUp = true;
            while (right >= left)
            {
                // Assign the center of the L & R indices at the beginning of the loop
                center = left + ((right - left) / 2);
                if (obj.Y > drawList[center].Y)
                {
                    // Need to look higher...
                    lookingUp = true;
                    left = center + 1;
                }
                else if (obj.Y < drawList[center].Y)
                {
                    // Need to look lower...
                    lookingUp = false;
                    right = center - 1;
                }
                else if (obj.Y == drawList[center].Y)
                {
                    // Already found a good location
                    break;
                }
            }
            // There may or may not be an issue with these insertion statements
            if (left < drawList.Count - 1)
            {
                if (lookingUp)
                    drawList.Insert(center + 1, obj);
                else
                    drawList.Insert(center, obj);
                
            }
            else
            {
                drawList.Add(obj);
            }
            /*Brute force:
            int y = obj.Y;
            int yCheck;
            int index = drawList.Count / 2;
            bool inserted = false;
            
            while(!inserted)
            {
                yCheck = drawList[index].Y;
                if (yCheck >= y)
                {
                    drawList.Insert(index, thing);
                    inserted = true;
                }
            }

            for(int n = 0; n < drawList.Count; ++n)
            {
                yCheck = drawList[n].Y;

                if(yCheck >= y)
                {
                    drawList.Insert(n, obj);
                    n = drawList.Count; // Ends loop
                    inserted = true;
                }
            }

            if(inserted == false)
            {
                if(y < drawList[0].Y)
                {
                    drawList.Insert(0, obj);
                    inserted = true;
                }
                else
                {
                    drawList.Add(obj);
                    inserted = true;
                }
            }*/
        }

        public void Clear()
        {
            drawList.Clear();
        }

        #region Drawing

        public void Draw(SpriteBatch sb)
        {
            for (int n = 0; n < drawList.Count; ++n)
            {
                if(drawList[n].Character == true)
                {
                    DrawCharacter(n, sb);
                }
                else
                {
                    sb.Draw(
                    drawList[n].Image,
                    drawList[n].Coordinates,
                    Color.White);
                }
            }
        }

        void DrawCharacter(int index, SpriteBatch sb)
        {
            //Draws the player (with rotation)

            if (drawList[index].DeathAnim == true)
            {
                DrawDeath(index, sb);
                //System.Diagnostics.Debug.WriteLine("Dead dude to be drawn");
            }
            else if (drawList[index].Player)
                DrawGunslingerSpritesheet(index, sb);
            else
                DrawEnemyGunslinger(index, sb);

            // Uncomment the following to see where the triangle sprite is:
            /*
            sb.Draw(
                drawList[index].Image,
                new Rectangle((int)drawList[index].Coordinates.X, (int)drawList[index].Coordinates.Y, 30, 30),  // Note the 30x30 size - will need to change
                null,
                Color.White,
                (float)drawList[index].Rotation + (float)(Math.PI / 2),
                new Vector2(drawList[index].Image.Width / 2, drawList[index].Image.Height / 2),
                SpriteEffects.None,
                1.0f);
             */
        }

        void DrawGunslingerSpritesheet(int index, SpriteBatch sb)
        {
            int yVal;               // The y value of where on the spriteSheet the sprite is found
            Vector2 spriteSize;     // X and Y of sprite size on spriteSheet
            SpriteEffects flip = SpriteEffects.None;

            #region Rotation
            // Rotation:
            
            // Start by standardizing rotation for ease in calculation
            // If rotation <= 0, rotation = -rotation
            // if rotation > 0, rotation = 2Pi - rotation
            // This gives a standard rotation value
            // Convert this to degrees to make things really easy

            double rotation = drawList[index].Rotation;
            if (rotation <= 0) rotation = -rotation;
            else rotation = (2*Math.PI) - rotation;
            rotation = rotation * (180.0 / Math.PI);

            //System.Diagnostics.Debug.Write(drawList[index].Rotation + "\n");

            //East
            if (rotation >= 0 &&
                rotation < 22.5)
            {
                yVal = 699;     // Numerical values specified on spritesheet
                spriteSize = new Vector2(275, 325);
            }
            // and also
            else if (rotation < 360 &&
                rotation >= 337.5)
            {
                yVal = 699;
                spriteSize = new Vector2(275, 325);
            }

            // NorthEast
            else if (rotation >= 22.5 &&
                rotation < 67.5)
            {
                yVal = 1042;
                spriteSize = new Vector2(247, 343);
            }

            // North
            else if (rotation >= 67.5 &&
                rotation < 112.5)
            {
                yVal = 1367;
                spriteSize = new Vector2(125, 372);
            }

            // NorthWest
            else if (rotation >= 112.5 &&
                rotation < 157.5)
            {
                yVal = 1042;
                spriteSize = new Vector2(247, 343);
                flip = SpriteEffects.FlipHorizontally;
            }

            // West
            else if (rotation >= 157.5 &&
                rotation < 202.5)
            {
                yVal = 699;
                spriteSize = new Vector2(275, 325);
                flip = SpriteEffects.FlipHorizontally;
            }

            // SouthWest
            else if (rotation >= 202.5 &&
                rotation < 247.5)
            {
                yVal = 351;
                spriteSize = new Vector2(231, 348);
                flip = SpriteEffects.FlipHorizontally;
            }

            // South
            else if (rotation >= 247.5 &&
                rotation < 292.5)
            {
                yVal = 0;
                spriteSize = new Vector2(124, 351);
            }

            // SouthEast
            else if (rotation >= 292.5 &&
                rotation < 337.5)
            {
                yVal = 351;
                spriteSize = new Vector2(231, 348);
            }

            // And if all else fails...
            else
            {
                // Set default SouthEast values
                yVal = 351;
                spriteSize = new Vector2(231, 348);
            }
            #endregion

            // If movement is happening
            int frame = animGS.GetFrame(time, playerMoving);
            xValGS = frame * (int)spriteSize.X;
            

            //
            // Sprite drawing goes here
            //

            float sizePercent = 0.2f;
            Color color = Color.White;

            int coordinatesX = (int)drawList[index].Coordinates.X + (int)(spriteSize.Y / 4 * sizePercent);
            //if (flip.Equals(SpriteEffects.FlipHorizontally))
            //    coordinatesX = coordinatesX - (int)(spriteSize.Y / 2 * sizePercent);
            if (flip.Equals(SpriteEffects.FlipHorizontally))
                coordinatesX = coordinatesX - (int)(spriteSize.Y / 2 * sizePercent - 10);
            int coordinatesY = (int)drawList[index].Coordinates.Y - (int)(spriteSize.Y / 2 * sizePercent);

            sb.Draw(
                gunslinger,
                new Rectangle(
                    coordinatesX,
                    coordinatesY,
                    (int)((float)spriteSize.X * sizePercent),
                    (int)((float)spriteSize.Y * sizePercent)),
                new Rectangle(
                    xValGS,
                    yVal,
                    (int)spriteSize.X,
                    (int)spriteSize.Y),
                color,
                0f,
                new Vector2(drawList[index].Image.Width / 2, drawList[index].Image.Height / 2),
                flip,
                1.0f);

        }

        void DrawEnemyGunslinger(int index, SpriteBatch sb)
        {
            int yVal;               // The y value of where on the spriteSheet the sprite is found
            Vector2 spriteSize;     // X and Y of sprite size on spriteSheet
            SpriteEffects flip = SpriteEffects.None;

            #region Rotation
            // Rotation:

            // Start by standardizing rotation for ease in calculation
            // If rotation <= 0, rotation = -rotation
            // if rotation > 0, rotation = 2Pi - rotation
            // This gives a standard rotation value
            // Convert this to degrees to make things really easy

            double rotation = drawList[index].Rotation;
            if (rotation <= 0) rotation = -rotation;
            else rotation = (2 * Math.PI) - rotation;
            rotation = rotation * (180.0 / Math.PI);

            //System.Diagnostics.Debug.Write(drawList[index].Rotation + "\n");

            //East
            if (rotation >= 0 &&
                rotation < 22.5)
            {
                yVal = 699;     // Numerical values specified on spritesheet
                spriteSize = new Vector2(275, 325);
            }
            // and also
            else if (rotation < 360 &&
                rotation >= 337.5)
            {
                yVal = 699;
                spriteSize = new Vector2(275, 325);
            }

            // NorthEast
            else if (rotation >= 22.5 &&
                rotation < 67.5)
            {
                yVal = 1042;
                spriteSize = new Vector2(247, 343);
            }

            // North
            else if (rotation >= 67.5 &&
                rotation < 112.5)
            {
                yVal = 1367;
                spriteSize = new Vector2(125, 372);
            }

            // NorthWest
            else if (rotation >= 112.5 &&
                rotation < 157.5)
            {
                yVal = 1042;
                spriteSize = new Vector2(247, 343);
                flip = SpriteEffects.FlipHorizontally;
            }

            // West
            else if (rotation >= 157.5 &&
                rotation < 202.5)
            {
                yVal = 699;
                spriteSize = new Vector2(275, 325);
                flip = SpriteEffects.FlipHorizontally;
            }

            // SouthWest
            else if (rotation >= 202.5 &&
                rotation < 247.5)
            {
                yVal = 351;
                spriteSize = new Vector2(231, 348);
                flip = SpriteEffects.FlipHorizontally;
            }

            // South
            else if (rotation >= 247.5 &&
                rotation < 292.5)
            {
                yVal = 0;
                spriteSize = new Vector2(124, 351);
            }

            // SouthEast
            else if (rotation >= 292.5 &&
                rotation < 337.5)
            {
                yVal = 351;
                spriteSize = new Vector2(231, 348);
            }

            // And if all else fails...
            else
            {
                // Set default SouthEast values
                yVal = 351;
                spriteSize = new Vector2(231, 348);
            }
            #endregion


            // If movement is happening
            bool moving = drawList[index].EnemyObj.IsMoving;
            int frame = drawList[index].EnemyObj.Animation.GetFrame(time, moving);
            xValGS = frame * (int)spriteSize.X;


            //
            // Sprite drawing goes here
            //

            float sizePercent = 0.2f;

            // Determining enemy color...
            EType typeOfEnemy = drawList[index].EnemyObj.Type;
            Color color = Color.Red;
            switch(typeOfEnemy)
            {
                case EType.Normal:
                    color = Color.Red;
                    break;
                case EType.Sniper:
                    color = Color.Yellow;
                    break;
                case EType.Fast:
                    color = Color.Green;
                    break;
                case EType.Strong:
                    color = Color.Orange;
                    break;
                default:
                    color = Color.Red;
                    break;
            }

            int coordinatesX = (int)drawList[index].Coordinates.X + (int)(spriteSize.Y / 4 * sizePercent);
            if (flip.Equals(SpriteEffects.FlipHorizontally)) coordinatesX = coordinatesX - (int)(spriteSize.Y / 2 * sizePercent);
            int coordinatesY = (int)drawList[index].Coordinates.Y - (int)(spriteSize.Y / 2 * sizePercent);

            sb.Draw(
                gunslinger,
                new Rectangle(
                    coordinatesX,
                    coordinatesY,
                    (int)((float)spriteSize.X * sizePercent),
                    (int)((float)spriteSize.Y * sizePercent)),
                new Rectangle(
                    xValGS,
                    yVal,
                    (int)spriteSize.X,
                    (int)spriteSize.Y),
                color,
                0f,
                new Vector2(drawList[index].Image.Width / 2, drawList[index].Image.Height / 2),
                flip,
                1.0f);

        }

        void DrawDeath(int index, SpriteBatch sb)
        {
            float sizePercentage = 351f / 1355f * 0.2f;

            sb.Draw(
                drawList[index].Image,
                new Rectangle(
                    (int)(drawList[index].Coordinates.X - drawList[index].Image.Width / 6 * sizePercentage / 2),
                    (int)(drawList[index].Coordinates.Y - drawList[index].Image.Height * sizePercentage / 2 * 5/3),
                    (int)((float)drawList[index].Image.Width / 6 * sizePercentage),
                    (int)((float)drawList[index].Image.Height * sizePercentage)),
                new Rectangle(
                    drawList[index].DeathFrame * drawList[index].Image.Width/6,
                    0,
                    drawList[index].Image.Width / 6,
                    drawList[index].Image.Height),
                Color.White,
                0f,
                new Vector2(drawList[index].Image.Width * sizePercentage / 2, drawList[index].Image.Height * sizePercentage / 2),
                SpriteEffects.None,
                1.0f);
        }

        #endregion

    }
}

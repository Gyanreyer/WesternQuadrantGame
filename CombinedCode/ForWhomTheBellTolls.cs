#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
#endregion

namespace CombinedCode
{
    class ForWhomTheBellTolls
    {
        //                                                         //
        // THIS CLASS HANDLES DEATH ANIMATIONS FOR CHARACTERS      //
        //                                                         //
        // IT CONTAINS A LIST OF OBJECTS OF THE CLASS "DeadObject" //
        //  WHICH IS ALSO IN THIS FILE.                            //



        #region Fields and Properties
        Texture2D gSSpriteSheet;
            // The sprite sheet with the animation of the gunslinger dying
        public Texture2D GSSpriteSheet { set { gSSpriteSheet = value; } }
        Texture2D bloodSheet;
        public Texture2D BloodSheet { set { bloodSheet = value; } }
        Texture2D gunFlashSheet;
        public Texture2D GunFlashSheet { set { gunFlashSheet = value; } }

        List<DeadObject> objectList;
            // List of all current dead objects
        MagicDraw drawingList;
            // The Drawing thing that draws stuff in order
        Vector2 screenLoc;
            // Camera location on screen
        public Vector2 ScreenLoc { get { return screenLoc; } set { screenLoc = value; } }
            // Must be updated each frame
        GameTime time;
            // Time in the game
        public GameTime Time { set { time = value; } }
        #endregion

        public ForWhomTheBellTolls(
            MagicDraw magicDraw)
        {
            drawingList = magicDraw;
            objectList = new List<DeadObject>();
        }

        public void AddDeadGuy(Vector2 worldCoordinates, bool deadGuy, bool blood, bool flash)
        {
            if(deadGuy)
                objectList.Add(new DeadObject(worldCoordinates, 6, objectList, gSSpriteSheet));
            if (blood)
                objectList.Add(new DeadObject(worldCoordinates, 6, objectList, bloodSheet));
            if(flash)
                objectList.Add(new DeadObject(worldCoordinates, 6, objectList, gunFlashSheet));
        }

        public void Update()
        {
            for (int n = objectList.Count - 1; n >= 0; --n )
            {
                objectList[n].UpdateFrame(time);
                //System.Diagnostics.Debug.WriteLine("Updated a frame");
            }

            #region AddToDrawList
            for (int n = 0; n < objectList.Count; ++n)
            {
                Vector2 screenCo = objectList[n].ScreenCoordinates(screenLoc);
                drawingList.AddDeath(
                    screenCo,
                    (int)screenCo.Y + gSSpriteSheet.Height,
                    objectList[n].Image,
                    objectList[n].CurrentFrame,
                    true);
                //System.Diagnostics.Debug.WriteLine("Added a dead guy to the drawing list");
            }
            #endregion
        }
    }

    class DeadObject
    {
        #region Fields and Properties

        List<DeadObject> objectList;
            // List of all current dead objects
        Animation anim;
            // The animation for this dead guy
        int currentFrame;
            // The current frame of the animation;
        public int CurrentFrame { get { return currentFrame; } }
            // For getting the current frame
        int totalFrames;
            // The total number of frames in the animation
        Vector2 worldCoordinates;
            // Where this is in the world
        Texture2D image;
        public Texture2D Image { get { return image; } }

        #endregion

        public DeadObject(Vector2 coordinates, int frames, List<DeadObject> list, Texture2D img)
        {
            objectList = list;
            anim = new Animation(frames + 1, 10);
            totalFrames = frames;
            currentFrame = 0;  // Starter value, as update makes it 0, the first frame
            worldCoordinates = coordinates;
            image = img;
        }

        /// <summary>
        /// Returns the coordinates of thsi object relative to the screen
        /// </summary>
        /// <param name="screenLoc"></param>
        /// <returns></returns>
        public Vector2 ScreenCoordinates(Vector2 screenLoc)
        {
            return new Vector2(
                (worldCoordinates.X - screenLoc.X),
                (worldCoordinates.Y - screenLoc.Y));
        }

        /// <summary>
        /// Increments the animation frame
        /// </summary>
        public void UpdateFrame(GameTime time)
        {
            currentFrame = anim.GetFrame(time, true);
            //System.Diagnostics.Debug.WriteLine("Current frame for this stiff is: " + currentFrame);
            if(currentFrame == totalFrames)
            {
                objectList.RemoveAt(0); // Removes the first item in the list
                                        // As deaths are put into the list in the order they happens,
                                        //  they will also end in that order
                                        // So every time an animation runs out frames,
                                        //  we can be sure that it's the first one in the list 
            }
        }
    }
}

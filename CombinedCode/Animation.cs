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
    class Animation
    {
        int frame;
        double timeCounter;
        double fps;
        double timePerFrame;

        int walkFPS;
        int runFPS;

        int frameCount;

        KeyboardState kbState;
        GamePadState gpState;

        public Animation(int frames, double framesPerSec)
        {
            frameCount = frames;
            walkFPS = (int)framesPerSec;
            runFPS = (int)(framesPerSec * 2);
            fps = framesPerSec;
            timePerFrame = 1 / fps;
            timeCounter = 0;
            frame = 0;

            kbState = Keyboard.GetState();
        }

        public int GetFrame(GameTime time, bool playerMoving)
        {
            kbState = Keyboard.GetState();
            gpState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            if (kbState.IsKeyDown(Keys.LeftShift) || gpState.IsButtonDown(Buttons.LeftTrigger))
                fps = runFPS;
            else
                fps = walkFPS;

            timePerFrame = 1 / fps;

            if(playerMoving)
            {
                timeCounter += time.ElapsedGameTime.TotalSeconds;
                if(timeCounter >= timePerFrame)
                {
                    frame += 1;
                    if (frame >= frameCount)
                        frame = 0;
                    timeCounter -= timePerFrame;
                }
            }
            

            return frame;
        }
    }
}

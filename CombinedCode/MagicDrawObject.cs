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
    class MagicDrawObject
    {
        #region Fields and Properties

        Vector2 coordinates;
        int yPos;
        Texture2D image;
        bool character;
        bool player;
        double rotation;

        Animation walkAnim;

        Enemy enemyObject;

        bool deathAnim;
        public bool DeathAnim { get { return deathAnim; } }
        int deathFrame;
        public int DeathFrame { get { return deathFrame; } }

        public Enemy EnemyObj { get { return enemyObject; } set { enemyObject = value; } }

        public bool Player
        {
            get
            {
                return player;
            }
        }

        public Vector2 Coordinates
        {
            get
            {
                return coordinates;
            }
        }

        public int Y
        {
            get
            {
                return yPos;
            }
        }

        public Texture2D Image
        {
            get
            {
                return image;
            }
        }

        public bool Character
        {
            get
            {
                return character;
            }
        }

        public double Rotation
        {
            get
            {
                return rotation;
            }
        }

        #endregion

        #region Constructors

        public MagicDrawObject (
            Vector2 where, 
            int y, 
            Texture2D who, 
            bool person, 
            double rot,
            bool playerChar)
        {
            coordinates = where;
            yPos = y;
            image = who;
            character = person;
            rotation = rot;
            player = playerChar;
        }

        public MagicDrawObject (
            Vector2 where, 
            int y, 
            Texture2D who, 
            bool person, 
            double rot,
            int frames,
            int fps,
            bool playerChar)
        {
            coordinates = where;
            yPos = y;
            image = who;
            character = person;
            rotation = rot;
            player = playerChar;

            walkAnim = new Animation(frames, fps);
        }

        // For the death animations
        public MagicDrawObject(
           Vector2 where,
           int y,
           Texture2D who,
           int frame,
           bool death)
        {
            coordinates = where;
            yPos = y;
            image = who;
            deathAnim = true;
            deathFrame = frame;
            character = true;
        }

        #endregion
    }
}

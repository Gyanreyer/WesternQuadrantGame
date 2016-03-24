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
    abstract class Entity
    {
        //**Fields**//
        #region Fields
        protected int health; //Health of entity
        protected float faceAngle; //Angle entity is facing
        protected Rectangle collisionBox; //Collision box of entity for collisions
        protected Texture2D texture; //Texture of entity
        protected MagicDraw drawList;
        Audio soundSys;
        #endregion

        //**Properties**//
         #region Properties

        public Audio SoundSys { get { return soundSys; } set { soundSys = value; } }

        //Get and set entity's health
        public int Health
        {
            get { return health; }

            set
            {
                if (value >= 0)
                    health = value;
            }
        }
        //Get and set angle entity is facing
        public float FaceAngle
        {
            get { return faceAngle; }

            set
            {
                faceAngle = value;
            }
        }

        //Get and set collision box
        public Rectangle CollisionBox
        {
            get { return collisionBox; }

            set
            {
                collisionBox = value;
            }
        }
        //Get and set X position
        public int X
        {
            get { return collisionBox.X; }

            set
            {
                collisionBox.X = value;
            }
        }

        //Get and set Y position
        public int Y
        {
            get { return collisionBox.Y; }

            set
            {
                collisionBox.Y = value;
            }
        }

        //Get and set texture
        public Texture2D Texture
        {
            get { return texture; }

            set
            {
                texture = value;
            }
        }
        #endregion

        // Constructor
        public Entity(int health, int x, int y, int width, int height, MagicDraw draw)
        {
            this.health = health;
            collisionBox = new Rectangle(x, y, width, height);

            faceAngle = 0;

            drawList = draw;
        }

        // Methods
        #region Methods
        public Vector2 ConvertWorldToLocal(Vector2 screenLoc)
        {
            float newX = (this.X - screenLoc.X);
            float newY = (this.Y - screenLoc.Y);

            return new Vector2(newX, newY);
        }

        public void AddToDrawList(Vector2 screenLoc, bool player)
        {
            Vector2 local = ConvertWorldToLocal(screenLoc);
            drawList.Add(new Vector2(local.X, local.Y), (int)(local.Y + collisionBox.Height/2), texture, true, faceAngle, player);
        }

        // Adds enemy to drawList with enemy object
        public void AddEnemyToDrawList(Vector2 screenLoc, bool player, Enemy obj)
        {
            Vector2 local = ConvertWorldToLocal(screenLoc);
            drawList.AddEnemy(new Vector2(local.X, local.Y), (int)(local.Y + collisionBox.Height / 2), texture, true, obj.DirectionAngle, player, obj);
        }
        #endregion
    }
}

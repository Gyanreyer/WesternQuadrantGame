using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CombinedCode
{
    abstract class Projectile
    {
        //
        // Fields
        //
        protected Rectangle collisionBox; //Collision box of the projectile
        protected Vector2 position; //Accurate position coordinates w/ floats
        protected Vector2 direction;  //Direction of the projectile
        protected Texture2D texture;
        protected bool alive;
        protected int damage;
        protected float y;            // the y location for where this is drawn on screen, for depth purposes    

        #region Properties
        public int YForDrawing { get { return (int)y; } set { y = value; } }
            //Get and set collision box
        public Rectangle CollisionBox
        {
            get { return collisionBox; }

            set
            {
                collisionBox = value;
            }
        }
        //Get and set position vector
        public Vector2 PositionVector
        {
            get { return position; }

            set
            {
                position = value;
            }
        }
        //Get and set x position
        public float X
        {
            get { return position.X; }

            set
            {
                position.X = value;
            }
        }
        //Get and set y position
        public float Y
        {
            get { return position.Y; }

            set
            {
                position.Y = value;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
        }
        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
            #endregion

        //
        // Constructors
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cB">Collision box of projectile</param>
        /// <param name="dX">X component of direction projectile moves in</param>
        /// <param name="dY">Y component of direction projectile moves in</param>
        public Projectile(Rectangle cB, float dX, float dY, int speed, Texture2D tex)
        {
            collisionBox = cB;

            direction = Vector2.Normalize(new Vector2(dX, dY)) * speed;

            position = new Vector2(collisionBox.X, collisionBox.Y);

            texture = tex;

            alive = true;

            damage = 100; // Currently, bullets will instantly kill. We can/should change this later

            y = int.MaxValue;
        }


        //
        // Methods
        //
        /// <summary>
        /// Moves the projectile in its current direction
        /// </summary>
        public void Move()
        {
            position.X += direction.X;
            position.Y += direction.Y;
            y += direction.Y;

            collisionBox.X = (int)position.X;
            collisionBox.Y = (int)position.Y;
        }
    }
}

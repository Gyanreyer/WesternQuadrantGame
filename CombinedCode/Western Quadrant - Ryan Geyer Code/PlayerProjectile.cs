using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CombinedCode
{
    class PlayerProjectile : Projectile
    {
        //
        //  Constructors
        //
        public PlayerProjectile(Rectangle cB, float dX, float dY, int speed, Texture2D tex)
            : base(cB, dX, dY, speed, tex)
        {
            
        }

        //
        // Methods
        //
        /// <summary>
        /// Checks collisions with enemies
        /// </summary>
        /// <returns>NOTE: Returns True if a collision is found, False if not</returns>
        public bool CheckEnemyCollisions(List<Enemy> enemies, ForWhomTheBellTolls deathList)
        {
            foreach (Enemy e in enemies)
            {
                Rectangle newCollision = new Rectangle(
                    ((int)e.CollisionBox.X),
                    (int)e.CollisionBox.Y - (int)(e.CollisionBox.Height * 2),
                    (int)(e.CollisionBox.Width / 2),
                    (int)(e.CollisionBox.Height * 2.2));

                if (newCollision.Intersects(this.collisionBox))
                {
                    // ACTIVATE BLOOD
                    deathList.AddDeadGuy(
                        new Vector2(newCollision.X, newCollision.Y + newCollision.Height),
                            false,
                            true,
                            false);
                    // Mark the bullet for death
                    alive = false;
                    // Deal damage to the enemy
                    e.Health -= damage;
                    // End the loop
                    return true;
                }
            }
            return false;
        }
    }
}

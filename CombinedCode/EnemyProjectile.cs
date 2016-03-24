using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CombinedCode
{
    class EnemyProjectile : Projectile
    {
        //
        //  Constructors
        //
        public EnemyProjectile(Rectangle cB, float dX, float dY, int speed, Texture2D tex)
            : base(cB, dX, dY, speed, tex)
        {
            damage = 10;
        }

        //
        // Methods
        //
        public void CheckPlayerCollisions(Player p, ForWhomTheBellTolls deathList)
        {
            Rectangle newCollision = new Rectangle(
                    ((int)p.CollisionBox.X),
                    (int)p.CollisionBox.Y - (int)(p.CollisionBox.Height * 2),
                    (int)(p.CollisionBox.Width / 2),
                    (int)(p.CollisionBox.Height * 2.2));

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
                // Deal damage to the player
                p.Health -= damage;
            }
        }
    }
}

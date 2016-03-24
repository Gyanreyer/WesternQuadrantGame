using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CombinedCode
{
    class SpawnField
    {
        // Fields
        bool occupied; // Each field should only spawn one enemy
        Vector2 loc; // Note: this location is global
        int width, height;

        // Constructor
        public SpawnField(Vector2 loc, int width, int height)
        {
            this.loc = loc;
            occupied = false;
            this.width = width;
            this.height = height;
        }

        // Properties
        public bool Occupied
        {
            get { return occupied; }
            set { occupied = value; }
        }
        public Vector2 Location
        {
            get { return loc; }
            set { loc = value; }
        }

        // Methods
        /// <summary>
        /// Returns a random spawn point within this spawn field.
        /// NOTE: This point will be in global coordinates
        /// </summary>
        public Vector2 GetRandomSpawnPoint(Random rand)
        {
            return new Vector2(loc.X + rand.Next(0, width), loc.Y + rand.Next(0, height));
        }
    }
}

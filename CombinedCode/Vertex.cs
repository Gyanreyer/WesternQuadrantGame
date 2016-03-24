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
    class Vertex
    {
        // Fields
        Vertex parent;           // Used for finding an actual path
        List<Vertex> neighbors;  // Used when checking neighbors
        int distFromStart;          // Used to compare and find a short path
        int priority;               // Kinda like 'distFromStart'
        bool walkable;              // Used to determine if it's a valid path
        Rectangle loc;              // Used for getting coordinates

        // Constructor
        public Vertex(Rectangle loc, bool walkable)
        {
            this.distFromStart = 0;
            this.priority = 0;
            this.neighbors = new List<Vertex>();
            this.parent = null;
            this.loc = loc;
            this.walkable = walkable;
        }

        // Properties
        public int DistFromStart
        { get { return distFromStart; } set { distFromStart = value; } }
        public int Priority
        { get { return priority; } set { priority = value; } }
        public List<Vertex> Neighbors
        { get { return neighbors; } set { neighbors = value; } }
        public Vertex Parent
        { get { return parent; } set { parent = value; } }
        public Rectangle Loc
        { get { return loc; } set { loc = value; } }
        public bool Walkable
        {
            get { return walkable; }
            set { walkable = value; }
        }
    }
}

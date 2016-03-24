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
    class GridSpace
    {
        #region Fields and Properties

        Vector2 blockSpot;          // Which block this is
        Vector2 positionOnScreen;   // What the pixel coordinates are.
        Texture2D image;            // The Texture2D
        bool obstacle;              // If this is an obstacle
        Vector2 obstacleLoc;        // Pretty sure this is useless. Probably delete

        public Vector2 BlockSpot
        {
            get
            {
                return blockSpot;
            }
        }
        public Vector2 PositionOnScreen
        {
            get
            {
                return positionOnScreen;
            }
        }
        public Texture2D Image
        {
            get
            {
                return image;
            }
        }
        public bool ObstacleBool
        {
            get
            {
                return obstacle;
            }
        }
        public Vector2 ObstacleLoc
        {
            get
            {
                return obstacleLoc;
            }
        }

        #endregion

        // Constructor class
        public GridSpace(Vector2 blockSpot,
            Vector2 positionOnScreen,
            Texture2D image,
            bool obstacle,
            Vector2 obstacleLoc,
            Vector2 blockSize)
        {
            this.blockSpot = blockSpot;
            this.positionOnScreen = positionOnScreen;
            this.image = image;
            this.obstacle = obstacle;
            this.obstacleLoc = obstacleLoc;
        }

    }
}

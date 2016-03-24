using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;

namespace CombinedCode
{
    class Map
    {
        #region Fields and Properties

        Vector2 mapSize;                    // Grid size (int x int)
        Vector2 blockSize;                  // How many pixels across and down each block in grid is
        public Vector2 mapPixelSize;        // Actual pixels of map
        public Vector2 screenLoc;           // Where the top left hand corner of the screen is
                                            //  (what pixel on the total map of pixels)
        Vector2 windowSize;                 // Size of the game window
        List<GridSpace> mapDrawingArray;    // Holds each grid space for drawing, in drawing order, with data
        Vertex[,] vertexMatrix;             // Used for pathfinding; a matrix of all the gridSpaces
        PriorityQueue pq;                   // Also used for pathfinding (speeding it up, specifically)
        int[,] mapArrayFromFile;            // Takes map data from file, puts in basic array
        List<Texture2D> textures;           // Holds all textures, assigns number to each
        MagicDraw drawList;                 // Links to the MagicDraw class, for drawing sprites

        List<Vector2> enemySpawnTiles;      // List of enemy spawn areas
        Vector2 playerSpawn;                // Player spawn block
        int enemyCount;

        public Vector2 WindowSize
        //
        // Sets the window size
        // Must be updated every frame
        //
        {
            set
            {
                windowSize = value;
            }
            get
            {
                return windowSize;
            }
        }

        public Vector2 BlockSize
        {
            get { return blockSize; }
        }

        public int EnemyCount
        {
            get
            {
                return enemyCount;
            }
        }
        public List<GridSpace> MapForDraw
        {
            get
            {
                return mapDrawingArray;
            }
        }

        public List<Vector2> EnemySpawnTiles
        {
            get
            {
                return enemySpawnTiles;
            }
        }

        public Vector2 PlayerSpawn
        {
            get
            {
                return playerSpawn;
            }
        }

        public PriorityQueue PQ
        { get { return pq; } set { pq = value; } }

        #endregion

        #region Map Setup

        public Map(int winW, int winH, MagicDraw draw)
        {
            // Sets initial window size (for reference by positioning things),
            //  and initializes the List used for drawing the map
            //  and the List of Texture2Ds
            windowSize = new Vector2(winW, winH);
            mapDrawingArray = new List<GridSpace>();
            textures = new List<Texture2D>();
            drawList = draw;

            pq = new PriorityQueue();

            enemySpawnTiles = new List<Vector2>();
            playerSpawn = new Vector2(10, 6);  // Default start, given none from the file

            enemyCount = 50;    // Default enemy count
        }


        public void LoadingInTextures(List<Texture2D> texList)
        {
            textures = texList;
            blockSize = new Vector2(textures[0].Width, textures[0].Height);
            mapPixelSize = new Vector2(mapSize.X * blockSize.X, mapSize.Y * blockSize.Y);
        }

        public void InitialBlockArray(string file)
        {
            // Fills the initial array with map data
            StreamReader mapText = new StreamReader(file);
            string line = null;

            while ((line = mapText.ReadLine()) != null)
            {
                if (line.Contains("Size"))
                {
                    line = mapText.ReadLine();
                    mapSize.X = int.Parse(line);
                    line = mapText.ReadLine();
                    mapSize.Y = int.Parse(line);

                    mapArrayFromFile = new int[(int)mapSize.X, (int)mapSize.Y];
                }

                else if (line.Contains("Grid"))
                {
                    for (int y = 0; y < mapSize.Y; ++y)
                    {
                        line = mapText.ReadLine();

                        string[] listOfX;
                        listOfX = line.Split(' ');

                        for (int x = 0; x < mapSize.X; ++x)
                        {
                             mapArrayFromFile[x, y] = int.Parse(listOfX[x]);
                        }
                    }
                }

                else if (line.Contains("Player Spawn"))
                {
                    int x = int.Parse(mapText.ReadLine());
                    int y = int.Parse(mapText.ReadLine());
                    playerSpawn = new Vector2(x, y);
                }

                else if (line.Contains("Enemy Spawns"))
                {
                    while (!(line = mapText.ReadLine()).Equals(""))
                    {
                        int x = int.Parse(line);
                        int y = int.Parse(mapText.ReadLine());
                        enemySpawnTiles.Add(new Vector2(x, y));
                    }
                }

                else if (line.Contains("Enemy Count"))
                {
                    enemyCount = int.Parse(mapText.ReadLine());
                }

            }
            mapText.Close();
        }

        void MapSetup(int blockX, int blockY, int posX, int posY)
        {
           if (textures[mapArrayFromFile[blockX, blockY]].Height == textures[0].Height)
            {
                //System.Diagnostics.Debug.WriteLine("Done did a non-collision at " + blockX + ", " + blockY);
                mapDrawingArray.Add(new GridSpace(new Vector2(blockX, blockY), new Vector2(posX, posY), textures[mapArrayFromFile[blockX, blockY]], false, default(Vector2), blockSize));
            }

            if (textures[mapArrayFromFile[blockX, blockY]].Height > textures[0].Height)
            {
                Vector2 objectImgSize = new Vector2(textures[mapArrayFromFile[blockX, blockY]].Width, textures[mapArrayFromFile[blockX, blockY]].Height);
                Vector2 bottomCorner = new Vector2(posX, (posY + blockSize.Y)); //bottom corner of lowermost/leftmost block of obstacle
                Vector2 imgDrawLocation = new Vector2(bottomCorner.X, (bottomCorner.Y - objectImgSize.Y));

                mapDrawingArray.Add(new GridSpace(new Vector2(blockX, blockY), new Vector2(posX, posY), textures[0], true, default(Vector2), blockSize));
                drawList.Add(imgDrawLocation, (int)imgDrawLocation.Y + textures[mapArrayFromFile[blockX, blockY]].Height, textures[mapArrayFromFile[blockX, blockY]], false, 0, false);
            }


        }

        public void putGridOnScreen()
        {
            mapDrawingArray.Clear();                                // Empties the map drawing array

            Vector2 topLeftBlock;
            // Finds the point that becomes
            //  the top left pixel of the game screen
            // Uses the top left pixel divided by
            //  the pixels in a single block to
            //  determine the leftmost block
            int startBlockX = (int)(Math.Floor(screenLoc.X / blockSize.X));
            int startBlockY = (int)(Math.Floor(screenLoc.Y / blockSize.Y));
            topLeftBlock = new Vector2(startBlockX, startBlockY);




            // Finds how many columns and rows of blocks to print
            // Divides the window size by the block size,
            //  and rounds up to find the most blocks that
            //  can be shown on screen at once.
            //  Then adds an extra one
            //   (Two? had to make it two to fill in for left shift)
            //  for good measure.
            int screenBlocksX = (int)Math.Ceiling(windowSize.X / blockSize.X) + 1;
            int screenBlocksY = (int)Math.Ceiling(windowSize.Y / blockSize.Y) + 1;

            screenBlocksY += 3;
                // Adding to this so that tall buildings etc don't just pop in.

            // Places each block at the calculated xPos and yPos
            //  (top left of block) on the screen
            for (int x = (int)topLeftBlock.X; x < topLeftBlock.X + screenBlocksX; ++x)
            {
                for (int y = (int)topLeftBlock.Y; y < topLeftBlock.Y + screenBlocksY; ++y)
                {
                    if(mapSize.X > x && mapSize.Y > y)
                    {
                        int xPos = (int)((x * blockSize.X) - screenLoc.X);
                        int yPos = (int)((y * blockSize.Y) - screenLoc.Y);

                        MapSetup(x, y, xPos, yPos);
                    }

                }
            }
        }

        #endregion

        public void Draw(SpriteBatch sb)
        {
            // Draws each block's texture from the List of visible blocks
            for (int n = 0; n < mapDrawingArray.Count; ++n)
            {
                sb.Draw(
                    mapDrawingArray[n].Image,
                    mapDrawingArray[n].PositionOnScreen,
                    Color.White);
            }
        }

        #region A* Pathfinding

        /// <summary>
        /// Sets up the vertex matrix for AI pathfinding. Must be run once and only once at the beginning
        /// </summary>
        public void SetUpMatrix()
        {
            // Set up the matrix to its proper size
            vertexMatrix = new Vertex[(int)mapSize.X, (int)mapSize.Y];
            // Add grid spaces based on whether or not they're obstacles
            for (int i = 0; i < mapArrayFromFile.GetLength(0); i++ )
            {
                for (int j = 0; j < mapArrayFromFile.GetLength(1); j++)
                {
                    // If it is not an obstacle...
                    if (textures[mapArrayFromFile[i, j]].Height == textures[0].Height)
                    {
                        // Put a new non-obstacle vertex in its place
                        vertexMatrix[i, j] = new Vertex(new Rectangle((int)(i * blockSize.X), (int)(j * blockSize.Y), (int)blockSize.X, (int)blockSize.Y), true);
                    }
                    else // otherwise...
                    {
                        // Put a new obstacle vertex in its place
                        vertexMatrix[i, j] = new Vertex(new Rectangle((int)(i * blockSize.X), (int)(j * blockSize.Y), (int)blockSize.X, (int)blockSize.Y), false);
                    }
                }
            }
        }

        public Vector2 GetPlayerStart()
        {
            float x = (playerSpawn.X * blockSize.X) + blockSize.X / 2;
            float y = (playerSpawn.Y * blockSize.Y) + blockSize.Y / 2;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Fills the "neighbors" lists. Must be done once and only once at the beginning
        /// </summary>
        public void SetUpNeighbors()
        {
            //Loop through the entire array
            for (int i = 0; i < vertexMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < vertexMatrix.GetLength(1); j++)
                {
                    //Assign neighbors, if they exist (make sure to avoid indexOutOfBounds exceptions)
                    //Up
                    if (j - 1 >= 0)
                    {
                        vertexMatrix[i, j].Neighbors.Add(vertexMatrix[i, j - 1]);
                    }
                    //Down
                    if (j + 1 < vertexMatrix.GetLength(1))
                    {
                        vertexMatrix[i, j].Neighbors.Add(vertexMatrix[i, j + 1]);
                    }
                    //Left
                    if (i - 1 >= 0)
                    {
                        vertexMatrix[i, j].Neighbors.Add(vertexMatrix[i - 1, j]);
                    }
                    //Right
                    if (i + 1 < vertexMatrix.GetLength(0))
                    {
                        vertexMatrix[i, j].Neighbors.Add(vertexMatrix[i + 1, j]);
                    }
                }
            }
        }

        public List<Vertex> AStar(Vertex start, Vertex end)
        {
//            Game1.debug = "Start: " + start.Loc + ", End: " + end.Loc;
            // Boolean to monitor pathfinding success
            bool success = true;
            // Closed vertex list
            List<Vertex> closed = new List<Vertex>();
            // Actual path list
            List<Vertex> path = new List<Vertex>();
            // Create current vertex variable
            Vertex current = start;
            // Add the vertex to the queue
            pq.Enqueue(current);
            // Start the loop
            while (pq.Peek() != end)
            {
                // If a path could not be found...
                if (pq.Peek() == null)
                {
                    // Give up
                    success = false;
                    break;
                }
                // Store and remove top vertex
                current = pq.Dequeue();
                // Add it to the closed list
                closed.Add(current);
                // Heuristic variable
                int diagonal = 0;
                // Queue up neighbors of top vertex
                foreach (Vertex v in current.Neighbors)
                {
                    int cost = current.DistFromStart + 1; // Here, "1" is the distance from the current vert to its neighbor. For this graph, it will always be 1 block away
                    // Remove far away vertices
                    if (cost < v.DistFromStart)
                    {
                        if (pq.Contains(v))
                            pq.Remove(v);
                        if (closed.Contains(v))
                            closed.Remove(v);
                    }
                    // Adding to the priority queue
                    if (!closed.Contains(v) && !pq.Contains(v) && v.Walkable)
                    {
                        v.DistFromStart = cost;
                        diagonal = (int)(Math.Sqrt(Math.Pow(end.Loc.X - v.Loc.X, 2) + Math.Pow(end.Loc.Y - v.Loc.Y, 2)));
                        v.Priority = v.DistFromStart + diagonal;
                        v.Parent = current;
                        pq.Enqueue(v);
                    }
                }
            }
            // Get the actual path list... if it exists
            if (success)
            {
                //Game1.debug = "victory";
                current = end;
                while (current != start) 
                {
                    current = current.Parent;
                    path.Add(current);
                }
                // Change it so that the path isn't backwards
                path.Reverse();
                // Return the path
                return path;
            }
            else
            {
                return null;
            }
        }// end of A*

        public Vertex GetVertexAtLocation(int x, int y)
        {
            // Be wary of bad casting
            return vertexMatrix[(int)(x / blockSize.X), (int)(y / blockSize.Y)];
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;

namespace LevelEditor
{
    class Map
    {
        Vector2 mapSize;                    // Grid size (int x int)
        Vector2 blockSize;                  // How many pixels across and down each block in grid is
        Vector2 windowSize;                 // Size of the game window
        List<GridSpace> mapDrawingArray;    // Holds each grid space for drawing, in drawing order, with data
        List<Texture2D> textures;           // Holds all textures, assigns number to each
        double scaler;                      // Scale of each space on map for fitting onscreen
        int[,] mapArray;                    // Array for holding map values;
        Vector2 mousedOver;                 // Which block the mouse is over
        Vector2 mousePos;                   // Where mouse is
        
        int xSizeOfList;                    // Width of list of pieces on left
        int selectedTile;                   // The tile that has been selected for placement
        int initialTile;                    // The tile that covers the map initially
       
        SpriteFont font;                    // Text
        int textOffset;                     // How far from top left corner of textbox text starts
        int fontSize;                       // Size of text
        int textStart;                      // Where the textbox starts (x loc)
        string[] fileList;

        Vector2 playerSpawn;
        List<Vector2> enemySpawn;
        List<String> spawnList;
        int enemyCount;

        enum placeWhat { PlaceObjects, PlaceSpawns };
        placeWhat action;

        public Map(int winW, int winH)
        {
            // Sets initial window size (for reference by positioning things),
            //  and initializes the List used for drawing the map
            //  and the List of Texture2Ds
            windowSize = new Vector2(winW, winH);
            mapDrawingArray = new List<GridSpace>();
            textures = new List<Texture2D>();

            xSizeOfList = 200;
            selectedTile = 4;
            initialTile = 3;

            textOffset = 5;
            textStart = (int)(windowSize.X - xSizeOfList);

            action = placeWhat.PlaceObjects;
            playerSpawn = new Vector2(10, 10);  // Default player spawn
            enemySpawn = new List<Vector2>();
            spawnList = new List<string>();

            spawnList.Add("Player Spawn");
            spawnList.Add("Enemy Spawn Area");
            spawnList.Add("Remove Enemy Spawn Area");
        }


        public Vector2 WindowSize
        {
            set
            {
                windowSize = value;
                //windowSize = new Vector2(windowSize.X, windowSize.Y);
            }
        }


        public Vector2 MapSize
        {
            set
            {
                mapSize = value;
            }
        }

        public int EnemyCount { set { enemyCount = value; } }

        #region Set Up Map
        public void SetUpMap(int x, int y)
        {
            double ratioMap = ((double)x * textures[0].Width) / ((double)y * textures[0].Height);
            double ratioWindow = (windowSize.X - xSizeOfList) / windowSize.Y;
            System.Diagnostics.Debug.Write(ratioMap + " vs. " + ratioWindow + "\n");
            int edge;

            if(ratioMap > ratioWindow)
            {
                // Fit to x edge
                edge = x * textures[0].Width;
                scaler = ((double)windowSize.X - (double)xSizeOfList) / (double)edge;

            }
            else
            {
                // Fit to y edge
                edge = y * textures[0].Height;
                scaler = (double)windowSize.Y / (double)edge;
            }

            mapArray = new int[x,y];
            mapSize = new Vector2(x, y);

            for (int n = 0; n < x; ++n)
            {
                for (int m = 0; m < y; ++m)
                {
                    mapArray[n, m] = initialTile;
                }
            }
        }



        public void LoadingInTextures(GraphicsDevice gd, SpriteFont f)
        {
            //texProjectile = this.Content.Load<Texture2D>("Shot.png");

            /*
            System.IO.Stream fileStream = TitleContainer.OpenStream("Content/basicGroundBlock.png");
            textures.Add(Texture2D.FromStream(
                gd,
                fileStream));

            fileStream = TitleContainer.OpenStream("Content/basicObstacleBlock.png");
            textures.Add(Texture2D.FromStream(
                gd,
                fileStream));

            fileStream.Close();
             */

            fileList = Directory.GetFiles("Content/MapTiles1(City)");

            System.IO.Stream fileStream;

            for (int n = 0; n < fileList.Length; ++n)
            {
                fileStream = TitleContainer.OpenStream(fileList[n]);
                textures.Add(Texture2D.FromStream(
                    gd,
                    fileStream));
                fileStream.Close();
            }

            font = f;
            fontSize = 10;  //Is this accurate?

            //System.Diagnostics.Debug.Write(textures.Count + " Textures\n");


            blockSize = new Vector2(textures[0].Width, textures[0].Height);
        }


        public void putGridOnScreen()
        {
            mapDrawingArray.Clear();                                // Empties the map drawing array

            for (int x = 0; x < mapSize.X; ++x)
            {
                for (int y = 0; y < mapSize.Y; ++y)
                {
                        int xPos = (int)((x * blockSize.X * scaler));
                        int yPos = (int)((y * blockSize.Y * scaler));
                        MapSetup(x, y, xPos, yPos);
                }
            }

            MarkEnemySpawns();
        }

        void MapSetup(int blockX, int blockY, int posX, int posY)
        {
            if (textures[mapArray[blockX, blockY]].Height <= textures[0].Height)
            {
                mapDrawingArray.Add(new GridSpace(new Vector2(blockX, blockY), new Vector2(posX, posY), textures[mapArray[blockX, blockY]], false, default(Vector2)));
            }

            if (textures[mapArray[blockX, blockY]].Height > textures[0].Height)
            {
                //Vector2 objectImgSize = new Vector2(textures[1].Width * (float)scaler, textures[1].Height * (float)scaler);
                float imgY = textures[mapArray[blockX, blockY]].Height * (float)scaler;
                //Vector2 bottomCorner = new Vector2(posX, (posY + (blockSize.Y * (float)scaler)));
                float bottomCornerY = posY + (blockSize.Y * (float)scaler);
                Vector2 imgDrawLocation = new Vector2(posX, (bottomCornerY - imgY));

                mapDrawingArray.Add(new GridSpace(new Vector2(blockX, blockY), imgDrawLocation, textures[mapArray[blockX, blockY]], true, default(Vector2)));
            }


        }
        #endregion

        public void WhereIsMouse(int x, int y)
        {
            mousePos = new Vector2(x, y);
            int xSpace = (int)(x / (blockSize.X * scaler));
            int ySpace = (int)(y / (blockSize.Y * scaler));
            mousedOver = new Vector2(xSpace, ySpace);
        }

        public void Click(bool leftClick)
        {
            if(action == placeWhat.PlaceObjects)
            {
                #region Place Objects
                if (mousedOver.X < mapSize.X && mousedOver.X >= 0 && mousedOver.Y < mapSize.Y && mousedOver.Y >= 0)
                {
                    if(leftClick)
                        mapArray[(int)mousedOver.X, (int)mousedOver.Y] = selectedTile;
                }

                if (mousePos.X > textStart)
                {
                    if (leftClick)
                    {
                        int selection = (int)(mousePos.Y / (float)(fontSize + textOffset));
                        if(selection < textures.Count)
                            selectedTile = selection;
                    }
                    else
                    {
                        int selection = (int)(mousePos.Y / (float)(fontSize + textOffset));
                        if (selection < textures.Count)
                        {
                            selectedTile = selection;
                            SetAllTiles(selection);
                        }
                    }

                }
                #endregion
            }
            else if(action == placeWhat.PlaceSpawns)
            {
                #region Place Spawns
                if (mousedOver.X < mapSize.X && mousedOver.X >= 0 && mousedOver.Y < mapSize.Y && mousedOver.Y >= 0)
                                                    // If moused over an actual tile
                {
                    if (leftClick)
                    {
                        if(selectedTile == 0)       // If choosing a player spawn point
                        {
                            playerSpawn = mousedOver;
                        }
                        else if(selectedTile == 1)   // If choosing an enemy spawn area
                        {
                            bool foundTile = false;

                            for (int n = 0; n < enemySpawn.Count; ++n)
                                            // Makes sure tile isn't already in list
                            {
                                if(enemySpawn[n].Equals(mousedOver))
                                {
                                    foundTile = true;
                                }
                            }

                            if (foundTile == false)
                                            // If it isn't already on the list
                            {
                                enemySpawn.Add(mousedOver);
                                //System.Diagnostics.Debug.Write("Added (" + mousedOver.X + ", " + mousedOver.Y + ")");
                                for(int m = 0; m < mapDrawingArray.Count; ++m)
                                {
                                    if(mapDrawingArray[m].BlockSpot == mousedOver)
                                    {
                                        mapDrawingArray[m].EnemySpawn = true;
                                        //System.Diagnostics.Debug.WriteLine("At (" + mapDrawingArray[m].BlockSpot.X + ", " + mapDrawingArray[m].BlockSpot.Y + ")");

                                        m = mapDrawingArray.Count;
                                    }
                                }
                                
                            }
                                    
                        }
                        else if(selectedTile == 2)  // If removing enemy spawn areas
                        {
                            for (int n = 0; n < enemySpawn.Count; ++n)
                            {
                                if (enemySpawn[n].Equals(mousedOver))
                                {
                                    enemySpawn.RemoveAt(n);

                                    for (int m = 0; m < mapDrawingArray.Count; ++m)
                                    {
                                        if (mapDrawingArray[m].BlockSpot == mousedOver)
                                        {
                                            mapDrawingArray[m].EnemySpawn = false;
                                            m = mapDrawingArray.Count;
                                        }
                                    }

                                    n = enemySpawn.Count;
                                }
                            }
                        }
                    }
                }

                if (mousePos.X > textStart)
                {
                    int selection = (int)(mousePos.Y / (float)(fontSize + textOffset));
                    if (selection < textures.Count)
                        selectedTile = selection;
                }
                #endregion
            }
        }

        public void SetAllTiles(int selection)
        {
            // Right click fills whole map
            for (int x = 0; x < mapSize.X; ++x)
            {
                for (int y = 0; y < mapSize.Y; ++y)
                {
                    mapArray[x, y] = selectedTile;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Color color = Color.White;
            
            // Draws each block's texture from the List of visible blocks
            for (int n = 0; n < mapDrawingArray.Count; ++n)
            {
                if (mousedOver == mapDrawingArray[n].BlockSpot)
                    color = Color.Red;
                else if (mapDrawingArray[n].EnemySpawn == true)
                    color = Color.MediumPurple;
                else if (mapDrawingArray[n].BlockSpot == playerSpawn)
                    color = Color.CornflowerBlue;

                sb.Draw(
                    mapDrawingArray[n].Image,
                    new Rectangle((int)mapDrawingArray[n].PositionOnScreen.X, (int)mapDrawingArray[n].PositionOnScreen.Y, (int)(mapDrawingArray[n].Image.Width * scaler), (int)(mapDrawingArray[n].Image.Height * scaler)),
                    color);

                color = Color.White;
            }

            if(action == placeWhat.PlaceObjects)
            {
                #region Draws list of titles
                // Draws the list of tiles
                for(int n = 0; n < textures.Count; ++n)
                {
                    if(n == selectedTile)
                    {
                        color = Color.Blue;
                    }

                    if (mousePos.X > textStart && 
                        mousePos.Y >= (n * (fontSize + textOffset)) &&
                        mousePos.Y < (n * (fontSize + textOffset) + fontSize))
                    {
                        color = Color.Red;
                        sb.Draw(
                            textures[n],
                            new Vector2(textStart - textures[0].Width, (n * (fontSize + textOffset))),
                            Color.White);
                    }

                    sb.DrawString(
                        font,
                        fileList[n].Substring(24),
                        new Vector2(textStart + textOffset, n * (fontSize + textOffset)),
                        color);

                    color = Color.White;

                }
                #endregion
            }
            else if (action == placeWhat.PlaceSpawns)
            {
                for (int n = 0; n < spawnList.Count; ++n)
                {
                    if (n == selectedTile)
                    {
                        color = Color.Blue;
                    }

                    /*
                    if (mousePos.X > textStart &&
                        mousePos.Y >= (n * (fontSize + textOffset)) &&
                        mousePos.Y < (n * (fontSize + textOffset) + fontSize))
                    {
                        color = Color.Red;
                        sb.Draw(
                            textures[n],
                            new Vector2(textStart - textures[0].Width, (n * (fontSize + textOffset))),
                            Color.White);
                    }
                     */

                    sb.DrawString(
                        font,
                        spawnList[n],
                        new Vector2(textStart + textOffset, n * (fontSize + textOffset)),
                        color);

                    color = Color.White;
                }
            }
        }

        void MarkEnemySpawns()
        {
            for (int n = 0; n < enemySpawn.Count; ++n)
            {
                for (int m = 0; m < mapDrawingArray.Count; ++m)
                {
                    if (mapDrawingArray[m].BlockSpot == enemySpawn[n])
                    {
                        mapDrawingArray[m].EnemySpawn = true;
                        m = mapDrawingArray.Count;
                    }
                }
            }
        }

        public void WriteToFile(String filename)
        {
            StreamWriter output = new StreamWriter(filename);

            output.WriteLine("Size");
            output.WriteLine(mapSize.X);
            output.WriteLine(mapSize.Y);
            output.WriteLine();

            output.WriteLine("Grid");

            for (int y = 0; y < mapSize.Y; ++y)
            {
                for (int x = 0; x < mapSize.X; ++x)
                {
                    output.Write(mapArray[x,y] + " ");
                }
                output.WriteLine();
            }
            output.WriteLine();

            output.WriteLine("Player Spawn");
            output.WriteLine(playerSpawn.X);
            output.WriteLine(playerSpawn.Y);
            output.WriteLine();

            output.WriteLine("Enemy Spawns");
            for (int n = 0; n < enemySpawn.Count; ++n)
            {
                output.WriteLine(enemySpawn[n].X);
                output.WriteLine(enemySpawn[n].Y);
                // Two lines per spawn location
            }
            output.WriteLine();

            output.WriteLine("Enemy Count");
            output.WriteLine(enemyCount);
            output.WriteLine();


                output.Close();
        }

        public void SwitchTileSpawn()
        {
            if (action == placeWhat.PlaceObjects)
            {
                action = placeWhat.PlaceSpawns;
                selectedTile = 0;
            }
                
            else if (action == placeWhat.PlaceSpawns)
                action = placeWhat.PlaceObjects;
        }

    }
}

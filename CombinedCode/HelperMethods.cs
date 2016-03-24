using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CombinedCode
{
    public class HelperMethods
    {
        public static Vector2 AngleToVector(double theta)
        {
            //Game1.debug = "theta = " + theta;
            double tempX = 0, tempY = 0;

            tempX = (Math.Cos(theta));
            tempY = (Math.Sin(theta));

            //And finally, store those values into the direction vector
            return new Vector2((float)tempX, (float)tempY);
        }

        /// <summary>
        /// Converts a local coordinate to a world coordinate. Used within the enemy shooting AI methods.
        /// </summary>
        /// <param name="thisLoc">Coordinate to be translated</param>
        /// <param name="screenLoc">Take it from mapData</param>
        /// <returns></returns>
        public static Vector2 ConvertLocalToWorld(Vector2 thisLoc, Vector2 screenLoc)
        {
            float newX = (screenLoc.X + thisLoc.X);
            float newY = (screenLoc.Y + thisLoc.Y);

            return new Vector2(newX, newY);
        }

        public static double GetDist(Vector2 loc1, Vector2 loc2)
        {
            double result = Math.Sqrt(Math.Pow(loc1.X - loc2.X, 2) + Math.Pow(loc1.Y - loc2.Y, 2));
            return result;
        }
    }
}

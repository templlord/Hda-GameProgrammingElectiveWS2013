using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;

namespace GridHighlighter
{
    public struct IntVector2
    {
        public int x;       //Index related to Tiles, not screen position!
        public int y;       //Index related to Tiles, not screen position!

        //Constructor
        public IntVector2(int pointX, int pointY)
        {
            x = pointX;
            y = pointY;
        }

        //Converts from tilearray indices to screen coordinates
        public Vector2 ConvertToScreenCoordinates(int gridSize)
        {
            return new Vector2(x * gridSize + gridSize * 0.5f, y * gridSize + gridSize * 0.5f);   // + gridSize/2 will be wrong later on when window size does not depend on gridSize --> tileSize variable or further calculation needed
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x,y);
        }

    }
}

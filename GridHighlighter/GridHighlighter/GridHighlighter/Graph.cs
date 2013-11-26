using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GridHighlighter
{
    public class Graph
    {
        public WayPointList waypoints;
        public List<Line> lines;


        //Constructor
        public Graph()
        {
            waypoints = new WayPointList();
            lines = new List<Line>();
        }

        public void AddWaypoint(int x, int y)
        {
            waypoints.list.Add(new IntVector2(x, y));
        }

        public void AddLine(IntVector2 a, IntVector2 b, int gridSize)
        {
            lines.Add(new Line(a, b, gridSize, Color.Orange, 1));
        }

        public bool IsValid()
        {
						return waypoints.HasValidLength();
        }
    }
}

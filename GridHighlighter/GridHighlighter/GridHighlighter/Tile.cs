using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GridHighlighter
{
	public enum ETileStatus {eTS_invalid = -1,eTS_first = 0, eTS_default = eTS_first, eTS_active, eTS_waypoint, eTS_occupied, eTS_last};
	public class Tile
	{
		private Texture2D texture;
		private Rectangle rectangle;
		private Color color;
		private ETileStatus status = ETileStatus.eTS_default;
		private bool active = false;		//To be obsolete
		private bool occupied = false;	//To be obsolete

		public Tile(int size, GraphicsDevice graphicsDevice)
		{
			texture = new Texture2D(graphicsDevice, 1, 1);
			texture.SetData(new Color[] { Color.White });

			rectangle.Width = size;
			rectangle.Height = size;
		}

		public ETileStatus getStatus()
		{
			return status;
		}

		public bool isActive()
		{
			return this.active;
		}

		public bool IsOccupied()
		{
			return this.occupied;
		}

		public Texture2D getTexture()
		{
			return texture;
		}

		public Rectangle getRectangle()
		{
			return rectangle;
		}

		public Color getColor()
		{
			return color;
		}

		public void setDefault() 
		{
			status = ETileStatus.eTS_default;
		}

		public void setActive()
		{
			status = ETileStatus.eTS_active;
		}

		public void setWaypoint()
		{
			status = ETileStatus.eTS_waypoint;
		}

		public void setOccupied()
		{
			status = ETileStatus.eTS_occupied;
		}

		public void setActive(bool a)	// to be obsolete
		{
			this.active = a;
		}

		public void setOccupied(bool o)	// to be obsolete
		{
			this.occupied = o;
		}

		public void setRectanglePosition(int x, int y)
		{
			this.rectangle.X = x;
			this.rectangle.Y = y;
		}

		public void setColor(Color c)
		{
			this.color = c;
		}
	}
}

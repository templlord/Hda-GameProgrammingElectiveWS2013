using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameLibrary;

namespace GridHighlighter
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		// constants
		const int GRID_PIXELSIZE	= 25;
		const int GRID_SIZE				= 25;

		//variables
		GraphicsDeviceManager		graphics;
		SpriteBatch							spriteBatch;

		Rectangle SCREEN_RECT = new Rectangle(0, 0, GRID_PIXELSIZE * GRID_SIZE, GRID_PIXELSIZE * GRID_SIZE);
		Tile[,] grid						= new Tile[GRID_PIXELSIZE, GRID_PIXELSIZE];

		// mouse input
		MouseState	mouseState					= Mouse.GetState();
		MouseState	previousMouseState	= Mouse.GetState();

		CAnimationHandler marioHandler = new CAnimationHandler();
		SAnimationInstance marioLeftClick;

		Graph route = new Graph();
		List<Enemy> Enemies = new List<Enemy>();
		List<Projectile> Projectiles = new List<Projectile>();

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferHeight = GRID_PIXELSIZE * GRID_PIXELSIZE;
			graphics.PreferredBackBufferWidth = GRID_PIXELSIZE * GRID_PIXELSIZE;
			IsMouseVisible = true;
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			for (int i = 0; i < grid.GetLength(0); ++i)
			{
				for (int j = 0; j < grid.GetLength(1); ++j)
				{
					grid[i, j] = new Tile(GRID_PIXELSIZE, GraphicsDevice);
				}
			}

			marioHandler = this.Content.Load<CAnimationHandler>(@"Animations");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			previousMouseState = mouseState;
			mouseState = Mouse.GetState();
			
			Tile curTile = null;
			int gridIndexLength = grid.GetLength(0);
			int gridEntryLength = grid.GetLength(1);


			for (int i = 0; i < gridIndexLength; ++i)
			{
				for (int j = 0; j < gridEntryLength; ++j)
				{
					curTile = grid[i, j];

					if (curTile.getStatus() != ETileStatus.eTS_waypoint && curTile.getStatus() != ETileStatus.eTS_occupied) 
					{
						curTile.setDefault();
					}
				}
			}
			OnMouseOver(mouseState.X / GRID_PIXELSIZE, mouseState.Y / GRID_PIXELSIZE);

			// enemy behavior
			for (int i = 0; i < Enemies.Count; ++i)
			{
				//Start shooting
				Enemy target = Enemies[i].FindEnemyInRange(Enemies);
				if (target != null)
				{
					if (Enemies[i].GetShotPossible())
					{
						Enemies[i].StartShotTimer();
						Projectile nextProjectile = new Projectile(Enemies[i].GetPosition(), target.GetPosition() - Enemies[i].GetPosition(), Color.Red, 2, 2, 1, Enemies[i]);
						Projectiles.Add(nextProjectile);
					}
				}

				//Check shot collisions
				for (int j = 0; j < Projectiles.Count; ++j)
				{
					if (Projectiles[j] != null)
					{
						if (Projectiles[j].GetShooter() != Enemies[i])
						{
							if (Enemies[i].CheckProjectileCollision(Projectiles[j]))
							{
								Projectiles[j] = null;
							}
						}
					}
				}
			}
			RemoveEnemies();        //Non-optimal performance -> objects deleting themselves?
			RemoveProjectiles();    // "

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();

			for (int i = 0; i < grid.GetLength(0); ++i)
			{
				for (int j = 0; j < grid.GetLength(1); ++j)
				{
					switch (grid[i, j].getStatus()) 
					{
						case ETileStatus.eTS_default:
							grid[i, j].setColor(Color.Black);
							break;
						case ETileStatus.eTS_active:
							grid[i, j].setColor(Color.White);
							break;
						case ETileStatus.eTS_waypoint:
							grid[i, j].setColor(Color.Yellow);
							break;
						case ETileStatus.eTS_occupied:
							grid[i, j].setColor(Color.Blue);
							break;
						case ETileStatus.eTS_invalid:
							grid[i, j].setColor(Color.Red);
							break;
					}
					grid[i, j].setRectanglePosition(i * GRID_PIXELSIZE, j * GRID_PIXELSIZE);
					spriteBatch.Draw(grid[i, j].getTexture(), grid[i, j].getRectangle(), grid[i, j].getColor());
				}
			}

			//Draw lines
			foreach (Line connection in route.lines)
			{
				//connection.Draw(spriteBatch, GRID_SIZE);
			}

			//Animate and draw enemies
			foreach (Enemy opponent in Enemies)
			{
				opponent.MoveAlongGraph(GRID_PIXELSIZE);
				spriteBatch.Draw(opponent.getAnimationInstance().getCurrentImage(marioHandler), new Rectangle((int)opponent.getPosition().X, (int)opponent.getPosition().Y, opponent.getAnimationInstance().getCurrentImage(marioHandler).Width, opponent.getAnimationInstance().getCurrentImage(marioHandler).Height), Color.White);
				opponent.animationNextImage(gameTime, marioHandler);
			}

			//Draw projectiles
			foreach (Projectile shot in Projectiles)
			{
				//shot.MoveAndDraw(spriteBatch, GRID_SIZE);
			}


			//Draw lines
			foreach (Line connection in route.lines)
			{
				//connection.Draw(spriteBatch, GRID_SIZE);
			}


			//Draw projectiles
			foreach (Projectile shot in Projectiles)
			{
				shot.Move(GRID_PIXELSIZE);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void RemoveEnemies()
		{
			for (int i = 0; i < Enemies.Count; ++i)
			{
				if (Enemies[i].GetCompletion() | !Enemies[i].GetAlive())
				{
					Enemies[i] = null;
				}
			}
			Enemies.RemoveAll(item => item == null);
		}

		//Removes projectiles that collided with an enemy    !!! also remove if colliding with something else
		public void RemoveProjectiles()
		{
			for (int i = 0; i < Projectiles.Count; ++i)
			{
				if (Projectiles[i] != null)
				{
					if (!Projectiles[i].CheckIfOnScreen(SCREEN_RECT))
					{
						Projectiles[i] = null;
					}
				}
			}
			Projectiles.RemoveAll(item => item == null);
		}
		private void OnMouseOver(int i, int j) 
		{
			// catch invalid input
			if (i >= grid.GetLength(0)|| j>=grid.GetLength(1)||i<0||j<0)
			{
				return;
			}
			if (grid[i, j].getStatus() != ETileStatus.eTS_waypoint && grid[i, j].getStatus() != ETileStatus.eTS_occupied)
			{
				grid[i, j].setActive();
			}
			//Add waypoints/lines on LMB click
			if (previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed & !(grid[i, j].getStatus()==ETileStatus.eTS_waypoint))
			{
				grid[i, j].setWaypoint();
				route.AddWaypoint(i, j);
				if (route.waypoints.HasValidLength())
				{
					route.lines.Add(new Line(route.waypoints.list[route.waypoints.list.Count - 2], route.waypoints.list[route.waypoints.list.Count - 1], GRID_PIXELSIZE, Color.Orange, 1));
				}
			}

			//Animates an object along the graph on RMB click
			if (previousMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed)
			{
				if (route.waypoints.HasValidLength())
				{
					int startNodeIndex = route.waypoints.FindNearestStartingWaypointIndex(i, j);
					Enemies.Add(new Enemy(route.waypoints.list[startNodeIndex].ConvertToScreenCoordinates(GRID_PIXELSIZE), new Rectangle(0, 0, 20, 20), 3, 1, 150, 1000, route, startNodeIndex, marioHandler));
				}
			}
		}
		private void OnClick()
		{

		}
	}
}

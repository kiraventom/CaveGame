using System;

namespace CaveGenerator
{
	public class Bomb
	{
		internal Bomb(uint ticks)
		{
			Counter = ticks;
		}

		internal Tile TileWithBomb { get; private set; }
		internal uint Counter { get; private set; }
		public bool IsActivated { get; private set; }

		internal event EventHandler Exploded;

		internal void Put(Tile tile, bool activate = true)
		{
			TileWithBomb = tile;
			TileWithBomb.Bomb = this;
			IsActivated = activate;
			ChangeTracker.ReportChange(tile.Location);
		}

		internal bool Tick()
		{
			if (IsActivated)
			{
				if (Counter == 0)
				{
					this.Explode();
					return true;
				}
				else
				{
					--Counter;
				}
			}

			return false;
		}
	
		private void Explode()
		{
			var locsToDestroy = new Geometry.Square(TileWithBomb.Location, 1).ToLocations();
			foreach (var loc in locsToDestroy)
			{
				var tile = GameEngine.Cave.Tiles[loc.X, loc.Y];
				tile.TryDestroyObstacle();
			}

			ChangeTracker.ReportChange(TileWithBomb.Location);
			TileWithBomb.Bomb = null;
			this.TileWithBomb = null;
			IsActivated = false;
			Exploded.Invoke(this, EventArgs.Empty);
		}
	}
}

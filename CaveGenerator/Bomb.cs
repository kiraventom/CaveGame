using System;

namespace CaveGenerator
{
	internal class Bomb
	{
		public Bomb(Location loc, uint ticks)
		{
			TileWithBomb = GameEngine.Cave.Tiles[loc.X, loc.Y];
			Counter = ticks;
			ChangeTracker.ReportChange(loc);
		}

		public Tile TileWithBomb { get; }
		public uint Counter { get; private set; }

		public event EventHandler Exploded;

		public bool Tick()
		{
			if (GameEngine.Cave.Tiles[TileWithBomb.X, TileWithBomb.Y].HasBomb)
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

			TileWithBomb.Bomb = null;
			ChangeTracker.ReportChange(TileWithBomb.Location);
			Exploded.Invoke(this, EventArgs.Empty);
		}
	}
}

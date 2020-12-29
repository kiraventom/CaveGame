using System.Collections.Generic;
using System.Linq;

namespace CaveGenerator
{
	public class Cave 
	{
		internal Cave(Size size, IEnumerable<Location> obstacles = null)
		{
			Size = size;
			Tiles = new Tile[Size.Width, Size.Height];
			GenerateBase();
			if (obstacles is null)
				CreateObstacles();
			else
				CreateObstacles(obstacles);

			Tile.ConnectAllTiles(this);
		}

		public Size Size { get; }
		public Tile[,] Tiles { get; }
																							
		private void GenerateBase()													
		{
			CreateEmpty();
			CreateBorders();
		}

		private void CreateEmpty()
		{
			for (uint x = 0; x < Size.Width; ++x)
			{
				for (uint y = 0; y < Size.Height; ++y)
				{
					Tiles[x, y] = new Tile(new Location(x, y));
				}
			}
		}
		
		private void CreateBorders()
		{
			for (uint x = 0; x < Size.Width; ++x)
			{
				for (uint y = 0; y < Size.Height; ++y)
				{
					if (x == 0 || x == Size.Width - 1 || y == 0 || y == Size.Height - 1)
					{
						Tile tile = new Tile(new Location(x, y), true);
						Tiles[x, y] = tile;
					}
				}
			}
		}

		private void CreateObstacles()
		{
			for (uint x = 1; x < Size.Width - 1; ++x)
			{
				for (uint y = 1; y < Size.Height - 1; ++y)
				{
					bool isObstacle = Generator.RND.Next(0, 4) == 0;
					if (isObstacle)
					{
						Tiles[x, y] = new Tile(new Location(x, y), false);
					}
				}
			}
		}

		private void CreateObstacles(IEnumerable<Location> obstacles)
		{
			foreach (var obstacle in obstacles)
			{
				Tiles[obstacle.X, obstacle.Y] = new Tile(new Location(obstacle.X, obstacle.Y), false);
			}
		}
	}																						
}

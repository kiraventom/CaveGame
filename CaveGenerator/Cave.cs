using System.Collections.Generic;

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

			Treasure = CreateTreasure();
			Tile.ConnectAllTiles(this);
		}

		public Size Size { get; }
		public Tile[,] Tiles { get; }
		public Tile Treasure { get; }
																							
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
					bool isObstacle = Generator.RND.Next(0, 2) == 0;
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

		private Tile CreateTreasure()
		{
			while (true)
			{
				int x = Generator.RND.Next(1, (int)Size.Width - 1);
				int y = Generator.RND.Next(1, (int)Size.Height - 1);
				if (Tiles[x, y].IsObstacle && !Tiles[x, y].IsBorder)
				{
					Tiles[x, y].IsTreasure = true;
					return Tiles[x, y];
				}
			}
		}
	}																						
}

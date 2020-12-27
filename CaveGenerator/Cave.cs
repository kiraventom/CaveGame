using System.Collections.Generic;
using System.Linq;

namespace CaveGenerator
{
	public class Cave 
	{
		internal Cave(Size size)
		{
			Size = size;
			Tiles = new Tile[Size.Width, Size.Height];
			GenerateTiles();
			Tile.ConnectAllTiles(this);
		}

		public Size Size { get; }
		public Tile[,] Tiles { get; }
																							
		private void GenerateTiles()													
		{
			CreateEmpty();
			CreateBorders();
			CreateObstacles();
		}

		private void CreateEmpty()
		{
			for (uint x = 0; x < Size.Width; ++x)
			{
				for (uint y = 0; y < Size.Height; ++y)
				{
					Tiles[x, y] = new EmptyTile() { Location = new Location(x, y) };
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
						Tile tile = new Border
						{
							Location = new Location(x, y)
						};
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
						Tiles[x, y] = new Obstacle() { Location = new Location(x, y) };
					}
				}
			}
		}
	}																						
}

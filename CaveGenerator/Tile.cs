using System;

namespace CaveGenerator
{
	public abstract class Tile
	{
		public uint X => this.Location.X;
		public uint Y => this.Location.Y;
		public Location Location { get; init; }

		public Tile Left { get; private set; }
		public Tile Up { get; private set; }
		public Tile Right { get; private set; }
		public Tile Bottom { get; private set; }

		internal static void ConnectAllTiles(Cave cave)
		{
			for (uint x = 0; x < cave.Size.Width; ++x)
			{
				for (uint y = 0; y < cave.Size.Height; ++y)
				{
					var tile = cave.Tiles[x, y];
					tile.Left = x == 0 ? null : cave.Tiles[x - 1, y];
					tile.Bottom = y == 0 ? null : cave.Tiles[x, y - 1];
					tile.Right = x == cave.Size.Width - 1 ? null : cave.Tiles[x + 1, y];
					tile.Up = y == cave.Size.Height - 1 ? null : cave.Tiles[x, y + 1];
				}
			}
		}
	}

	public class Obstacle : Tile
	{
	
	}

	public class Border : Obstacle
	{

	}

	public class EmptyTile : Tile
	{
		public Actor Occupier { get; set; }
		public bool IsOccupied => Occupier is not null;
	}

	public struct Location
	{
		public Location(uint x, uint y)
		{
			this.X = x;
			this.Y = y;
		}

		public uint X { get; }
		public uint Y { get; }

		public Location AsConsoleLocation(Size size, byte cr)
		{
			return new Location((uint)(this.X * cr), size.Height - 1 - this.Y);
		}
	}

	public struct Size
	{
		public Size(uint width, uint height)
		{
			this.Width = width;
			this.Height = height;
		}

		public uint Width { get; }
		public uint Height { get; }
	}
}

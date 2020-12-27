using System;

namespace CaveGenerator
{
	public class Tile
	{
		/// <summary>
		/// Empty tile
		/// </summary>
		/// <param name="occupier"></param>
		internal Tile(Actor occupier = null)
		{
			IsBorder = false;
			IsObstacle = false;
			Occupier = occupier;
		}

		/// <summary>
		/// Obstacle tile
		/// </summary>
		/// <param name="isBorder"></param>
		internal Tile(bool isBorder)
		{
			IsObstacle = true;
			IsBorder = isBorder;
		}

		private bool _isObstacle { get; set; }
		public bool IsObstacle
		{
			get => _isObstacle;
			set
			{
				if (this.IsBorder)
					throw new InvalidOperationException("Cannot change border");
				
				_isObstacle = value;
			}
		}

		public bool IsBorder { get; }

		public Actor Occupier { get; internal set; }
		public bool IsOccupied => Occupier is not null;

		public uint X => this.Location.X;
		public uint Y => this.Location.Y;
		public Location Location { get; init; }

		internal Tile Left { get; private set; }
		internal Tile Up { get; private set; }
		internal Tile Right { get; private set; }
		internal Tile Bottom { get; private set; }

		internal bool TryDestroyObstacle()
		{
			if (!this.IsObstacle || this.IsBorder)
				return false;

			this.IsObstacle = false;
			ChangeTracker.ReportChange(this.Location);
			return true;
		}

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

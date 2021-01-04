using System;
using System.Linq;
using System.Collections.Generic;

namespace CaveGenerator
{
	public class Tile
	{
		/// <summary>
		/// Empty tile
		/// </summary>
		/// <param name="occupier"></param>
		internal Tile(Location loc, Actor occupier = null)
		{
			Location = loc;
			IsBorder = false;
			IsObstacle = false;
			Occupier = occupier;
		}

		/// <summary>
		/// Obstacle tile
		/// </summary>
		/// <param name="isBorder"></param>
		internal Tile(Location loc, bool isBorder)
		{
			Location = loc;
			IsObstacle = true;
			IsBorder = isBorder;
		}

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
		public bool IsVisible { get; internal set; }
		public Actor Occupier { get; internal set; }
		public bool IsOccupied => Occupier is not null;

		public uint X => this.Location.X;
		public uint Y => this.Location.Y;
		public Location Location { get; }

		internal IEnumerable<Tile> Neighbours => new [] {this.Left, this.Top, this.Right, this.Bottom };

		private bool _isObstacle { get; set; }
		private Tile Left { get; set; }
		private Tile Top { get; set; }
		private Tile Right { get; set; }
		private Tile Bottom { get; set; }

		internal Tile GetNeighbour(Direction dir)
		{
			return dir switch
			{
				Direction.Left => this.Left,
				Direction.Up => this.Top,
				Direction.Right => this.Right,
				Direction.Down => this.Bottom,

				_ => throw new NotImplementedException()
			};
		}

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
					tile.Top = y == cave.Size.Height - 1 ? null : cave.Tiles[x, y + 1];
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

		public Location(int x, int y)
		{
			this.X = x < 0 ? 0 : x > GameEngine.Cave.Size.Width - 1 ? GameEngine.Cave.Size.Width - 1 : (uint)x;
			this.Y = y < 0 ? 0 : y > GameEngine.Cave.Size.Width - 1 ? GameEngine.Cave.Size.Width - 1 : (uint)y;
		}

		public uint X { get; }
		public uint Y { get; }

		public Location AsConsoleLocation(Size size, byte cr = 2)
		{
			// double the X to make field look square (usually console font ratio is 1/2)
			// reverse the Y because we want Y = 0 be on the bottom and console Y = 0 is on the top
			return new Location(this.X * cr, size.Height - 1 - this.Y);
		}

		public override bool Equals(object obj) => obj is Location location && this == location;
		public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

		public static bool operator ==(Location loc1, Location loc2) => loc1.X == loc2.X && loc1.Y == loc2.Y;
		public static bool operator !=(Location loc1, Location loc2) => loc1.X != loc2.X || loc1.Y != loc2.Y;
	}

	public struct LocationF
	{
		public LocationF(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public double X { get; }
		public double Y { get; }

		public Location AsConsoleLocation(Size size, byte cr = 2)
		{
			// double the X to make field look square (usually console font ratio is 1/2)
			// reverse the Y because we want Y = 0 be on the bottom and console Y = 0 is on the top
			return new Location((uint)(Math.Floor(this.X) * cr), (uint)(size.Height - 1 - Math.Floor(this.Y)));
		}

		public override bool Equals(object obj) => obj is LocationF locationf && this == locationf;
		public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

		public static bool operator ==(LocationF loc1, LocationF loc2) => loc1.X == loc2.X && loc1.Y == loc2.Y;
		public static bool operator !=(LocationF loc1, LocationF loc2) => loc1.X != loc2.X || loc1.Y != loc2.Y;
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



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
		public bool IsTreasure { get; internal set; }
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
}
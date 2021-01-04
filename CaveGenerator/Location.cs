using System;

namespace CaveGenerator
{
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
}



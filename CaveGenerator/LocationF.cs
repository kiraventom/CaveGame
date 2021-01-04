using System;

namespace CaveGenerator
{
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
}



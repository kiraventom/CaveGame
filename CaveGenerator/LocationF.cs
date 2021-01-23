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

		public override bool Equals(object obj) => obj is LocationF locationf && this == locationf;
		public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

		public static bool operator ==(LocationF loc1, LocationF loc2) => loc1.X == loc2.X && loc1.Y == loc2.Y;
		public static bool operator !=(LocationF loc1, LocationF loc2) => loc1.X != loc2.X || loc1.Y != loc2.Y;
	}
}



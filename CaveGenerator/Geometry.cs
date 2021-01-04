using System;
using System.Collections.Generic;
using System.Linq;

namespace CaveGenerator.Geometry
{
	public enum LineType { Horizontal, Vertical, Diagonal }

	public struct Line
	{
		public Line(Location a, Location b)
		{
			this.A = new LocationF(a.X + 0.5, a.Y + 0.5);
			this.B = new LocationF(b.X + 0.5, b.Y + 0.5);

			double diff_x = Math.Abs(A.X - B.X);
			double diff_y = Math.Abs(A.Y - B.Y);

			if (diff_x > diff_y)
				Type = LineType.Horizontal;
			else
			if (diff_x < diff_y)
				Type = LineType.Vertical;
			else
				Type = LineType.Diagonal;
		}

		public LocationF A { get; }
		public LocationF B { get; }
		public LineType Type { get; }

		internal IList<Location> ToLocations()
		{
			var points = GetPoints();
			HashSet<Location> locations = new HashSet<Location>();
			foreach (var point in points)
			{
				uint x;
				uint y;
				if (point.X < 0)
					x = 0;
				else
				if (point.X > GameEngine.Cave.Size.Width - 1)
					x = GameEngine.Cave.Size.Width - 1;
				else
					x = (uint)Math.Floor(point.X);

				if (point.Y < 0)
					y = 0;
				else
				if (point.Y > GameEngine.Cave.Size.Height - 1)
					y = GameEngine.Cave.Size.Height - 1;
				else
					y = (uint)Math.Floor(point.Y);

				locations.Add(new Location(x, y));
			}

			return locations.ToList();
		}

		private IList<LocationF> GetPoints(double step = 0.1)
		{
			List<LocationF> locationFs = new List<LocationF>();

			Func<double, double, double, double> formula;
			double k, b;
			switch (Type)
			{
				case LineType.Horizontal:
				case LineType.Diagonal:
					// Необходимо получить уравнение прямой типа y = kx + b
					k = (B.Y - A.Y) / (B.X - A.X);   // k = (y2 - y1) / (x2 - x1)
					b = A.Y - k * A.X;               // b = y1 - k * x1

					formula = static (x, k, b) => k * x + b;
					bool leftToRight = A.X < B.X;
					if (leftToRight)
					{
						for (double x = A.X; x < B.X; x += step)
						{
							double y = formula.Invoke(x, k, b);
							locationFs.Add(new LocationF(x, y));
						}
					}
					else
					{
						for (double x = A.X - step; x >= B.X; x -= step)
						{
							double y = formula.Invoke(x, k, b);
							locationFs.Add(new LocationF(x, y));
						}
					}
					
					break;

				case LineType.Vertical:
					k = (B.X - A.X) / (B.Y - A.Y);
					b = A.X - k * A.Y;

					formula = static (y, k, b) => k * y + b;
					bool bottomToTop = A.Y < B.Y;
					if (bottomToTop)
					{
						for (double y = A.Y; y < B.Y; y += step)
						{
							double x = formula.Invoke(y, k, b);
							locationFs.Add(new LocationF(x, y));
						}
					}
					else
					{
						for (double y = A.Y - step; y >= B.Y; y -= step)
						{
							double x = formula.Invoke(y, k, b);
							locationFs.Add(new LocationF(x, y));
						}
					}
					break;
			}

			return locationFs;
		}
	}

	public struct Square
	{
		public Square(Location center, uint halfSide)
		{
			int x_left = (int)center.X - (int)halfSide;
			int x_right = (int)center.X + (int)halfSide;
			int y_top = (int)center.Y + (int)halfSide;
			int y_bottom = (int)center.Y - (int)halfSide;

			Top = new Line(new Location(x_left, y_top), new Location(x_right, y_top));
			Left = new Line(new Location(x_left, y_bottom), new Location(x_left, y_top));
			Bottom = new Line(new Location(x_left, y_bottom), new Location(x_right, y_bottom));
			Right = new Line(new Location(x_right, y_bottom), new Location(x_right, y_top));
		}

		public Line Top { get; }
		public Line Left { get; }
		public Line Bottom { get; }
		public Line Right { get; }

		public IEnumerable<Line> GetLines() => new[] { Top, Left, Bottom, Right };

		internal IList<Location> ToLocations()
		{
			HashSet<Location> locations = new HashSet<Location>();
			foreach (var line in GetLines())
				foreach (var loc in line.ToLocations())
					locations.Add(loc);

			return locations.ToList();
		}
	}
}
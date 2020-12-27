using System.Collections.Generic;

namespace CaveGenerator
{
	public static class ChangeTracker
	{
		public static List<Location> GetUpdates()
		{
			var list = new List<Location>(Changes);
			Changes.Clear();
			return list;
		}

		internal static void ReportChange(Location loc)
		{
			Changes.Add(loc);
		}

		private static readonly List<Location> Changes = new List<Location>();
	}
}

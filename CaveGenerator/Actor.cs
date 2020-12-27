using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveGenerator
{
	public abstract class Actor
	{
		public Location Location { get; }
		public EmptyTile OccupiedTile { get; internal set; }

		public bool Move(Tile moveTo)
		{
			if (moveTo is null || moveTo is not EmptyTile emptyMoveTo || emptyMoveTo.IsOccupied)
				return false;

			var before = this.OccupiedTile;
			this.OccupiedTile.Occupier = null;
			this.OccupiedTile = emptyMoveTo;
			this.OccupiedTile.Occupier = this;

			ChangeTracker.ReportChange(before.Location);
			ChangeTracker.ReportChange(moveTo.Location);

			return true;
		}
	}

	public class Player : Actor
	{
	}

	public class Enemy : Actor
	{
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveGenerator
{
	public enum Direction { None, Left, Up, Right, Down };

	public abstract class Actor
	{
		public Location Location { get; }
		internal Tile OccupiedTile { get; set; }

		internal bool MoveTo(Tile moveTo)
		{
			if (moveTo is null || moveTo.IsOccupied)
				return false;

			if (!moveTo.IsObstacle)
			{
				var before = this.OccupiedTile;
				this.OccupiedTile.Occupier = null;
				this.OccupiedTile = moveTo;
				this.OccupiedTile.Occupier = this;

				ChangeTracker.ReportChange(before.Location);
				ChangeTracker.ReportChange(moveTo.Location);

				return true;
			}
			else
			{
				moveTo.TryDestroyObstacle();
				return false;
			}
		}
	}

	public class Player : Actor
	{
	}

	public class Enemy : Actor
	{
	}
}

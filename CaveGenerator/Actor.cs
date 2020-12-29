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
		internal Tile OccupiedTile { get; set; }

		internal virtual bool MoveTo(Tile moveTo)
		{
			if (moveTo is null || moveTo.IsOccupied || moveTo.IsObstacle)
				return false;

			var before = this.OccupiedTile;
			this.OccupiedTile.Occupier = null;
			this.OccupiedTile = moveTo;
			this.OccupiedTile.Occupier = this;

			ChangeTracker.ReportChange(before.Location);
			ChangeTracker.ReportChange(moveTo.Location);

			return true;
		}
	}

	public class Player : Actor
	{
		internal override bool MoveTo(Tile moveTo)
		{
			bool didMove = base.MoveTo(moveTo);
			if (!didMove && moveTo.IsObstacle)
			{
				moveTo.TryDestroyObstacle();
			}

			return didMove;
		}
	}

	public class Enemy : Actor
	{
		public Enemy() => Intellect = new Intellect(this);

		internal Intellect Intellect { get; }
	}
}

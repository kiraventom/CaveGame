﻿using System.Collections.Generic;
using System.Linq;

namespace CaveGenerator
{
	public enum Direction { None, Left, Up, Right, Down };

	public abstract class Actor
	{
		virtual public Tile OccupiedTile { get; internal set; }
		protected abstract uint ViewDistance { get; }

		internal virtual bool MoveTo(Tile moveTo)
		{
			if (moveTo is null || moveTo.IsOccupied || moveTo.IsObstacle || moveTo.HasBomb && moveTo.Bomb.IsActivated)
				return false;

			var before = this.OccupiedTile;
			this.OccupiedTile.Occupier = null;
			this.OccupiedTile = moveTo;
			this.OccupiedTile.Occupier = this;

			ChangeTracker.ReportChange(before.Location);
			ChangeTracker.ReportChange(moveTo.Location);

			return true;
		}

		internal IEnumerable<Tile> GetVisibleTiles()
		{
			var square = new Geometry.Square(this.OccupiedTile.Location, ViewDistance);
			var borderLocs = square.Border.ToLocations();
			List<Geometry.Line> visionLines = new List<Geometry.Line>();
			foreach (var squareLoc in borderLocs)
			{
				var line = new Geometry.Line(this.OccupiedTile.Location, squareLoc);
				visionLines.Add(line);
			}

			HashSet<Tile> visibleTiles = new HashSet<Tile>();
			foreach (var visionLine in visionLines)
			{
				IList<Location> visionLineLocs = visionLine.ToLocations();
				for (int i = 0; i < visionLineLocs.Count; ++i)
				{
					var tile = GameEngine.Cave.Tiles[visionLineLocs[i].X, visionLineLocs[i].Y];
					visibleTiles.Add(tile);

					if (i != 0)
					{
						var currentLoc = visionLineLocs[i];
						var prevLoc = visionLineLocs[i - 1];
						var currentTile = GameEngine.Cave.Tiles[currentLoc.X, currentLoc.Y];
						var prevTile = GameEngine.Cave.Tiles[prevLoc.X, prevLoc.Y];
						var commonNeighbours = currentTile.Neighbours.Intersect(prevTile.Neighbours);
						if (commonNeighbours.Count() == 2 && commonNeighbours.All(n => n.IsObstacle))
						{
							visibleTiles.Remove(currentTile);
							break;
						}
					}

					if (tile.IsObstacle)
						break;
				}
			}

			return visibleTiles;
		}
	}

	public class Player : Actor
	{
		public Player()
		{
			_inventory = new List<Bomb>()
			{
				new Bomb(2),
				new Bomb(2),
			};
		}

		public IReadOnlyList<Bomb> Inventory => _inventory;
		private List<Bomb> _inventory { get; }

		public override Tile OccupiedTile
		{
			get => base.OccupiedTile;
			internal set
			{
				var wasNull = base.OccupiedTile is null;
				base.OccupiedTile = value;
				if (wasNull)
					UpdateFog();
			}
		}

		protected override uint ViewDistance => 4;

		internal override bool MoveTo(Tile moveTo)
		{
			bool didMove = base.MoveTo(moveTo);
			if (moveTo.HasBomb && !moveTo.Bomb.IsActivated)
			{
				this._inventory.Add(moveTo.Bomb);
				moveTo.Bomb = null;
			}

			if (moveTo.HasTreasure)
			{
				moveTo.HasTreasure = false;
			}

			UpdateFog();
			return didMove;
		}

		internal bool TryPlaceBombAt(Tile placeBombTo)
		{
			bool result = false;
			if (!placeBombTo.IsObstacle && !placeBombTo.IsOccupied && !placeBombTo.HasTreasure)
			{
				Bomb bomb = null;
				if (Inventory.Count > 0)
					bomb = Inventory[0];
				else
					return result;

				_inventory.Remove(bomb);
				bomb.Exploded += (_, _) => this.UpdateFog();
				bomb.Put(placeBombTo);
				result = true;
			}

			return result;
		}

		private void UpdateFog()
		{
			var visibleTiles = GetVisibleTiles();

			foreach (var tile in visibleTiles)
			{
				if (!tile.IsVisible)
				{
					tile.IsVisible = true;
					ChangeTracker.ReportChange(tile.Location);
				}
			}
		}
	}

	public class Enemy : Actor
	{
		public Enemy() => Intellect = new Intellect(this);

		protected override uint ViewDistance => 3;

		internal Intellect Intellect { get; }
	}
}

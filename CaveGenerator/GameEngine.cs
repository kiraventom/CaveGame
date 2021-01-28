using System.Collections.Generic;
using System.Linq;

namespace CaveGenerator
{
	public static class GameEngine
	{
		static GameEngine() { }

		public static int Seed => Generator.Seed;

		public static void Initialize()
		{
			Cave = Generator.CreateCave(new Size(30, 30));
			Player = Generator.CreateActor<Player>();
			Enemies = Generator.CreateActors<Enemy>(0);
			Bombs = new HashSet<Bomb>();
		}

		public static void HandleMoveRequest(Direction direction, bool putBomb = false)
		{
			if (direction != Direction.None)
			{
				Tile current = Player.OccupiedTile;
				var moveTo = current.GetNeighbour(direction);

				if (moveTo is not null)
				{
					if (putBomb)
					{
						bool didPlace = Player.TryPlaceBombAt(moveTo);
						if (didPlace)
						{
							Bombs.Add(moveTo.Bomb);
							moveTo.Bomb.Exploded += (s, _) => Bombs.Remove(s as Bomb);
						}
					}
					else
					{
						bool didMove = Player.MoveTo(moveTo);
					}
				}
			}

			Tick();
		}

		public static void Easter_OpenMap()
		{
			for (uint x = 0; x < Cave.Size.Width; ++x)
			{
				for (uint y = 0; y < Cave.Size.Height; ++y)
				{
					Cave.Tiles[x, y].IsVisible = true;
					ChangeTracker.ReportChange(new Location(x, y));
				}
			}
		}

		public static void Easter_GetBombs()
		{
			foreach (var neighbour in Player.OccupiedTile.Neighbours)
			{
				if (!neighbour.IsObstacle && !neighbour.IsOccupied && !neighbour.HasBomb && !neighbour.HasTreasure)
				{
					Bomb bomb = new Bomb(2);
					bomb.Put(neighbour, false);
				}
			}
		}

		private static void Tick()
		{
			foreach (var enemy in Enemies)
			{
				var tile = enemy.Intellect.GetTileToMoveTo();

				enemy.MoveTo(tile);
			}

			foreach (var bomb in Bombs)
			{
				bomb.Tick();
			}	
		}

		public static Cave Cave { get; private set; }
		public static Player Player { get; private set; }
		public static IEnumerable<Enemy> Enemies { get; private set; }
		private static HashSet<Bomb> Bombs { get; set; }
		public static bool DidWin => !Cave?.Treasure?.HasTreasure ?? false;
		public static bool DidLose
		{
			get
			{
				// если у игрока есть бомбы
				if (Player.Inventory.Any()) 
					return false;

				// если есть несобранная бомба в зоне видимости
				if (Bombs.Any(b => b.TileWithBomb.IsVisible))
					return false;

				// если есть неоткрытая пустая клетка рядом с открытой
				for (int x = 1; x < Cave.Size.Width - 1; ++x)
				{
					for (int y = 1; y < Cave.Size.Height - 1; ++y)
					{
						Tile tile = Cave.Tiles[x, y];
						if (tile.IsVisible && !tile.IsObstacle && tile.Neighbours.Any(n => !n.IsVisible))
							return false;
					}
				}
				
				return true;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveGenerator
{
	public static class GameEngine
	{
		static GameEngine() { }

		public static void Initialize()
		{
			Cave = Generator.CreateCave(new Size(30, 30));
			Player = Generator.CreateActor<Player>();
			Enemies = Generator.CreateActors<Enemy>(1);
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

		public static void ActivateEgg()
		{
			for (uint x = 0; x < Cave.Size.Width; ++x)
			{
				for (uint y = 0; y < Cave.Size.Height; ++y)
				{
					Cave.Tiles[x, y].IsVisible = !Cave.Tiles[x, y].IsVisible;
					ChangeTracker.ReportChange(new Location(x, y));
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
		public static bool IsTreasureFound => !Cave.Treasure.IsObstacle;
	}
}

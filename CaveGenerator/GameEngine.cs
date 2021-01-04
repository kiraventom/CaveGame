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
			Enemies = Generator.CreateActors<Enemy>(0);
			IsTreasureFound = false;
		}

		public static void HandleMoveRequest(Direction direction, bool shouldBreak = false)
		{
			if (direction != Direction.None)
			{
				Tile current = Player.OccupiedTile;
				var moveTo = current.GetNeighbour(direction);

				if (moveTo is not null)
				{
					if (shouldBreak)
					{
						bool didDestroy = Player.TryDestroyAt(moveTo);
						if (didDestroy && moveTo.IsTreasure)
						{
							IsTreasureFound = true;
							return;
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

		private static void Tick()
		{
			foreach (var enemy in Enemies)
			{
				var tile = enemy.Intellect.GetTileToMoveTo();

				enemy.MoveTo(tile);
			}
		}

		public static Cave Cave { get; private set; }
		public static Player Player { get; private set; }
		public static IEnumerable<Enemy> Enemies { get; private set; }
		public static bool IsTreasureFound { get; private set; }
	}
}

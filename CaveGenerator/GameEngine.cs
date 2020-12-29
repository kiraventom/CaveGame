using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveGenerator
{
	public static class GameEngine
	{
		static GameEngine()
		{
			Cave = Generator.CreateCave(new Size(30, 30));
			Player = Generator.CreateActor<Player>();
			Enemies = Generator.CreateActors<Enemy>(2);
		}

		public static void HandleMoveRequest(Direction direction)
		{
			if (direction != Direction.None)
			{
				Tile current = Player.OccupiedTile;
				var moveTo = current.GetNeighbour(direction);

				if (moveTo is not null)
				{
					bool didMove = Player.MoveTo(moveTo);
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

		public static Cave Cave { get; }
		public static Player Player { get; }
		public static IEnumerable<Enemy> Enemies { get; }
	}
}

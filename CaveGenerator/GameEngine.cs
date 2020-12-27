using System;
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
			Player = Generator.CreatePlayer(Cave);
		}

		public static void HandleRequest(Direction direction)
		{
			if (direction != Direction.None)
			{
				Tile current = Player.OccupiedTile;
				var moveTo = direction switch
				{
					Direction.Left => current.Left,
					Direction.Up => current.Up,
					Direction.Right => current.Right,
					Direction.Down => current.Bottom,

					_ => throw new NotImplementedException()
				};

				if (moveTo is not null)
				{
					bool didMove = Player.MoveTo(moveTo);
				}
			}
		}

		public static Cave Cave { get; }
		public static Player Player { get; }
	}
}

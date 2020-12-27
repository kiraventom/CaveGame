using System;
using CaveGenerator;

namespace ConsoleUI
{
	class Program
	{
		const int cr = 2; // console ratio

		static Cave Cave => GameEngine.Cave;
		static Player Player => GameEngine.Player;

		static void Main(string[] args)
		{
			Console.CursorVisible = false;

			DrawCave(Cave);
			StartGameCycle();

			return;
		}

		static void StartGameCycle()
		{
			while (true)
			{
				var cki = Console.ReadKey(true);
			    HandlePlayerMovement(cki);

				var updates = ChangeTracker.GetUpdates();
				foreach (var update in updates)
				{
					var tile = Cave.Tiles[update.X, update.Y];
					DrawTile(tile);
				}
			}
		}

		enum Direction { None, Left, Up, Right, Down };

		static bool HandlePlayerMovement(ConsoleKeyInfo cki)
		{
			var dir = cki.Key switch
			{
				ConsoleKey.W => Direction.Up,
				ConsoleKey.A => Direction.Left,
				ConsoleKey.S => Direction.Down,
				ConsoleKey.D => Direction.Right,

				_ => Direction.None
			};

			if (dir != Direction.None)
			{
				Tile current = Player.OccupiedTile;
				var moveTo = dir switch
				{
					Direction.Left => current.Left,
					Direction.Up => current.Up,
					Direction.Right => current.Right,
					Direction.Down => current.Bottom,

					_ => throw new NotImplementedException()
				};

				if (moveTo is not null)
				{
					bool didMove = Player.Move(moveTo);
				}
			}

			return dir != Direction.None;
		}

		static void DrawCave(Cave cave)
		{
			for (int i = 0; i < cave.Size.Width; ++i)
			{
				for (int j = 0; j < cave.Size.Height; ++j)
				{
					var tile = cave.Tiles[i, j];
					DrawTile(tile);
				}
			}
		}

		static void DrawTile(Tile tile)
		{
			char tileChar = CharTable.GetTileChar(tile);
			string tileStr = new string(tileChar, cr);

			var consoleLoc = tile.Location.AsConsoleLocation(GameEngine.Cave.Size, 2);
			Console.SetCursorPosition((int)consoleLoc.X, (int)consoleLoc.Y);
			Console.Write(tileStr);
		}
	}

}

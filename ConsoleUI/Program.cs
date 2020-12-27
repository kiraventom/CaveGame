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
			    var dir = HandleControls(cki);
				GameEngine.HandleRequest(dir);

				var updates = ChangeTracker.GetUpdates();
				foreach (var update in updates)
				{
					var tile = Cave.Tiles[update.X, update.Y];
					DrawTile(tile);
				}
			}
		}

		static Direction HandleControls(ConsoleKeyInfo cki)
		{
			var dir = cki.Key switch
			{
				ConsoleKey.W => Direction.Up,
				ConsoleKey.A => Direction.Left,
				ConsoleKey.S => Direction.Down,
				ConsoleKey.D => Direction.Right,

				_ => Direction.None
			};

			return dir;
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

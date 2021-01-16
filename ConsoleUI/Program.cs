using System;
using System.Threading;
using System.Threading.Tasks;
using CaveGenerator;

namespace ConsoleUI
{
	class Program
	{
		const int cr = 2; // console ratio
		static int easterEggCounter = 0;

		static Cave Cave => GameEngine.Cave;

		static void Main(string[] args)
		{
			Console.CursorVisible = false;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.White;
			Console.WindowWidth = 65;
			Console.WindowHeight = 32;
			Console.Title = GameEngine.Seed.ToString();

			while (true)
			{
				GameEngine.Initialize();
				DrawCave(Cave);
				StartGameCycle();
			}
		}

		static void StartGameCycle()
		{
			while (true)
			{
				ConsoleKeyInfo cki = new ConsoleKeyInfo();
				if (Console.KeyAvailable)
				{
					cki = Console.ReadKey(true);
					if (cki.Key == ConsoleKey.R)
					{
						return;
					}

					// easter egg
					if (cki.Key == ConsoleKey.F9)
					{
						++easterEggCounter;
						if (easterEggCounter == 3)
						{
							easterEggCounter = 0;
							GameEngine.ActivateEgg();
						}
					}
				}

				if (!GameEngine.IsTreasureFound)
				{
					var dir = HandleControls(cki);
					GameEngine.HandleMoveRequest(dir, cki.Modifiers.HasFlag(ConsoleModifiers.Shift));

					var updates = ChangeTracker.GetUpdates();
					foreach (var update in updates)
					{
						var tile = Cave.Tiles[update.X, update.Y];
						DrawTile(tile);
					}
				}

				if (GameEngine.IsTreasureFound)
				{
					string congratsMsg = "You won!";
					Console.SetCursorPosition(30 - congratsMsg.Length / 2, 15);
					Console.Write(congratsMsg);
				}

				Thread.Sleep(100);
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
			var color = CharTable.GetTileColor(tile);
			string tileStr = new string(tileChar, cr);

			var consoleLoc = tile.Location.AsConsoleLocation(GameEngine.Cave.Size, 2);
			Console.ForegroundColor = color;
			Console.SetCursorPosition((int)consoleLoc.X, (int)consoleLoc.Y);
			Console.Write(tileStr);
		}
	}

}

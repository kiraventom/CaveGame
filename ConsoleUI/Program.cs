using CaveGenerator;
using System;
using System.Threading;

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
					Console.Title = "Bombs: " + GameEngine.Player.Inventory.Count.ToString();
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

			var consoleLoc = ToConsoleLocation(tile.Location, GameEngine.Cave.Size, 2);
			Console.ForegroundColor = color;
			Console.SetCursorPosition((int)consoleLoc.X, (int)consoleLoc.Y);
			Console.Write(tileStr);
		}

		static Location ToConsoleLocation(Location loc, Size size, byte cr = 2)
		{
			// double the X to make field look square (usually console font ratio is 1/2)
			// reverse the Y because we want Y = 0 be on the bottom and console Y = 0 is on the top
			return new Location(loc.X * cr, size.Height - 1 - loc.Y);
		}

		static Location ToConsoleLocation(LocationF loc, Size size, byte cr = 2)
		{
			// double the X to make field look square (usually console font ratio is 1/2)
			// reverse the Y because we want Y = 0 be on the bottom and console Y = 0 is on the top
			return new Location((uint)(Math.Floor(loc.X) * cr), (uint)(size.Height - 1 - Math.Floor(loc.Y)));
		}
	}

}

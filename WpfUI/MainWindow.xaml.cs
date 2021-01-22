using CaveGenerator;
using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace WpfUI
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.Loaded += this.MainWindow_Loaded;
			this.MainView.PaintSurface += this.MainView_PaintSurface;
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(GameTick), true);
			this.MainView.MouseDown += this.MainView_MouseDown;
		}

		Cave Cave => GameEngine.Cave;

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			GameEngine.Initialize();
			var windowSize = ToWindowSize(Cave.Size);
			this.Width = windowSize.Width;
			this.Height = windowSize.Height;

			DrawCave(Cave);
		}

		private void MainView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			var updates = ChangeTracker.GetUpdates();
			foreach (var update in updates)
			{
				var tile = Cave.Tiles[update.X, update.Y];
				DrawTile(tile, e);
			}

			if (GameEngine.IsTreasureFound)
			{
				var canvas = e.Surface.Canvas;
				using SKPaint paint = new SKPaint { Color = SKColors.White, TextSize = 80, IsStroke = false, TextAlign = SKTextAlign.Center };
				canvas.DrawText("You won!", (float)MainView.ActualWidth / 2, (float)MainView.ActualHeight / 2, paint);
			}
		}

		private void MainView_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var point = e.MouseDevice.GetPosition(this.MainView);
			int tileX = (int)Math.Floor(point.X / TileSize.Width);
			int tileY = (int)Math.Floor((MainView.ActualHeight - point.Y) / TileSize.Height) - 1;

			Tile tile = Cave.Tiles[tileX, tileY];

			var dirs = new[] { Direction.Left, Direction.Up, Direction.Right, Direction.Down };
			foreach (var dir in dirs)
			{
				var neighbour = GameEngine.Player.OccupiedTile.GetNeighbour(dir);
				if (neighbour == tile)
				{
					GameEngine.HandleMoveRequest(dir, true);
					this.MainView.InvalidateVisual();
					return;
				}
			}
		}

		void GameTick(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.R)
			{
				GameEngine.Initialize();
				DrawCave(Cave);
				return;
			}

			if (!GameEngine.IsTreasureFound)
			{
				var dir = HandleControls(e.Key);
				GameEngine.HandleMoveRequest(dir, false);
				this.Title = "Bombs: " + GameEngine.Player.Inventory.Count.ToString();

				this.MainView.InvalidateVisual();
			}
		}

		Direction HandleControls(Key key)
		{
			var dir = key switch
			{
				Key.W => Direction.Up,
				Key.A => Direction.Left,
				Key.S => Direction.Down,
				Key.D => Direction.Right,

				_ => Direction.None
			};

			return dir;
		}

		void DrawCave(Cave cave)
		{
			for (int i = 0; i < cave.Size.Width; ++i)
			{
				for (int j = 0; j < cave.Size.Height; ++j)
				{
					ChangeTracker.ReportChange(new Location(i, j));
				}
			}

			this.MainView.InvalidateVisual();
		}

		void DrawTile(Tile tile, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			var layers = TileTable.GetTileLayers(tile);
			var windowLoc = ToWindowLocation(tile.Location, Cave.Size);

			var canvas = e.Surface.Canvas;
			while (layers.Any())
			{
				var layer = layers.Dequeue();
				canvas.DrawBitmap(layer, new SKRect(windowLoc.X, windowLoc.Y, windowLoc.X + TileSize.Width, windowLoc.Y + TileSize.Height));
			}
		}

		static Location ToWindowLocation(Location loc, CaveGenerator.Size size)
		{
			return new Location(loc.X * TileSize.Width, (size.Height - loc.Y - 1) * TileSize.Height);
		}

		static CaveGenerator.Size ToWindowSize(CaveGenerator.Size size)
		{
			return new CaveGenerator.Size(size.Width * (TileSize.Width + 2), size.Height * (TileSize.Height + 2));
		}

		static CaveGenerator.Size TileSize = new CaveGenerator.Size(30, 30);
	}
}

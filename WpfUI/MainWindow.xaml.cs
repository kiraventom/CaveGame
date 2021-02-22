using CaveGenerator;
using SkiaSharp;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

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

			this.MainView.MouseMove += this.MainView_MouseMove;

			uint tileSide = TileSize.Width;
		}

		[DllImport("User32.dll")]
		private static extern uint GetDpiForWindow(IntPtr hwnd);

		internal static CaveGenerator.Size TileSize = new CaveGenerator.Size(30, 30);
		internal static Tile hoveredTile { get; private set; }
		internal static bool facingRight = true;

		private static double scale = 1.0;
		private Cave Cave => GameEngine.Cave;
		private uint easter_openMapCounter = 0;
		private uint easter_getBombsCounter = 0;

		private void MainView_MouseMove(object sender, MouseEventArgs e)
		{
			var point = e.MouseDevice.GetPosition(this.MainView);
			point = new Point(point.X * scale, point.Y * scale);
			int tileX = (int)Math.Floor(point.X / TileSize.Width);
			int tileY = (int)Math.Floor((Cave.Size.Height * TileSize.Height - point.Y) / TileSize.Height);

			if (tileX >= Cave.Size.Width || tileX < 0 || tileY >= Cave.Size.Height || tileY < 0)
				return;

			if (hoveredTile != Cave.Tiles[tileX, tileY])
			{
				if (hoveredTile is not null)
					ChangeTracker.ReportChange(hoveredTile.Location);
				ChangeTracker.ReportChange(Cave.Tiles[tileX, tileY].Location);
				hoveredTile = Cave.Tiles[tileX, tileY];
				this.MainView.InvalidateVisual();
			}
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var wih = new WindowInteropHelper(GetWindow(this));
			IntPtr hWnd = wih.Handle;
			uint dpi = GetDpiForWindow(hWnd);
			scale = dpi switch
			{
				96 => 1.0,
				120 => 1.25,
				144 => 1.50,
				168 => 1.75,
				0 => throw new Exception("GetDpiForWindow returned 0"),

				_ => 1.0
			};

			GameEngine.Initialize();
			var windowSize = ToWindowSize(Cave.Size);
			this.Width = windowSize.Width;
			this.Height = windowSize.Height;

			DrawCave(Cave);

			this.Top = 0;
		}

		private void MainView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			var updates = ChangeTracker.GetUpdates();
			foreach (var update in updates)
			{
				var tile = Cave.Tiles[update.X, update.Y];
				DrawTile(tile, e);
			}

			if (GameEngine.DidWin)
			{
				var canvas = e.Surface.Canvas;
				using SKPaint paint = new SKPaint { Color = SKColors.White, TextSize = 80, IsStroke = false, TextAlign = SKTextAlign.Center };
				canvas.DrawText("You won!", (float)MainView.ActualWidth / 2, (float)MainView.ActualHeight / 2, paint);
			}
			else
			if (GameEngine.DidLose)
			{
				var canvas = e.Surface.Canvas;
				using SKPaint paint = new SKPaint { Color = SKColors.White, TextSize = 80, IsStroke = false, TextAlign = SKTextAlign.Center };
				canvas.DrawText("You lost!", (float)MainView.ActualWidth / 2, (float)MainView.ActualHeight / 2, paint);
			}
		}

		private void MainView_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Tile tile = hoveredTile;

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

			if (e.Key == Key.F9)
			{
				++easter_openMapCounter;
				if (easter_openMapCounter == 3)
				{
					GameEngine.Easter_OpenMap();
					easter_openMapCounter = 0;
				}
			}

			if (e.Key == Key.F8)
			{
				++easter_getBombsCounter;
				if (easter_getBombsCounter == 3)
				{
					GameEngine.Easter_GetBombs();
					easter_getBombsCounter = 0;
				}
			}

			if (!GameEngine.DidWin && !GameEngine.DidLose)
			{
				var dir = HandleControls(e.Key);
				if (dir == Direction.Right || dir == Direction.Left)
					facingRight = dir == Direction.Right;

				GameEngine.HandleMoveRequest(dir, false);
				this.Title = "Bombs: " + GameEngine.Player.Inventory.Count.ToString();
			}

			this.MainView.InvalidateVisual();
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
			return new CaveGenerator.Size((uint)(size.Width * (TileSize.Width + 1) / scale), (uint)(size.Height * (TileSize.Height + 2) / scale));
		}
	}
}

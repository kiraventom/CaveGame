using CaveGenerator;
using SkiaSharp;
using System.Collections.Generic;

namespace WpfUI
{
	static class TileTable
	{
		static TileTable()
		{
			// create highlighter
			for (int x = 0; x < highlighter.Width; ++x)
				for (int y = 0; y < highlighter.Height; ++y)
					highlighter.SetPixel(x, y, new SKColor(255, 255, 255, 64));
		}

		readonly static SKBitmap fog			= SKBitmap.Decode(Properties.Resources.FogTile);
		readonly static SKBitmap obstacle		= SKBitmap.Decode(Properties.Resources.ObstacleTile);
		readonly static SKBitmap bombOff		= SKBitmap.Decode(Properties.Resources.BombOffTile);
		readonly static SKBitmap bombOn			= SKBitmap.Decode(Properties.Resources.BombOnTile);
		readonly static SKBitmap free			= SKBitmap.Decode(Properties.Resources.FreeTile);
		readonly static SKBitmap playerLeft		= SKBitmap.Decode(Properties.Resources.PlayerLeftTile);
		readonly static SKBitmap playerRight	= SKBitmap.Decode(Properties.Resources.PlayerRightTile);
		readonly static SKBitmap treasure		= SKBitmap.Decode(Properties.Resources.TreasureTile);
		readonly static SKBitmap highlighter	= new SKBitmap((int)MainWindow.TileSize.Width, (int)MainWindow.TileSize.Height);

		internal static Queue<SKBitmap> GetTileLayers(Tile tile)
		{
			Queue<SKBitmap> layers = new Queue<SKBitmap>();
			layers.Enqueue(free);
			if (tile.IsObstacle)
				layers.Enqueue(obstacle);
			else if (tile.HasBomb)
			{
				if (tile.Bomb.IsActivated)
					layers.Enqueue(bombOn);
				else
					layers.Enqueue(bombOff);
			}
			else if (tile.IsOccupied && tile.Occupier is Player)
			{
				if (MainWindow.facingRight)
					layers.Enqueue(playerRight);
				else
					layers.Enqueue(playerLeft);
			}
				
			else if (tile.HasTreasure)
				layers.Enqueue(treasure);

			if (!tile.IsVisible)
				layers.Enqueue(fog);

			if (tile == MainWindow.hoveredTile)
				layers.Enqueue(highlighter);

			return layers;
		}
	}
}

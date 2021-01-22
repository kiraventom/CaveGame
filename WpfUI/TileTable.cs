using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaveGenerator;
using SkiaSharp;

namespace WpfUI
{
	static class TileTable
	{
		readonly static SKBitmap fog = SKBitmap.Decode(Properties.Resources.FogTile);
		readonly static SKBitmap obstacle = SKBitmap.Decode(Properties.Resources.ObstacleTile);
		readonly static SKBitmap bombOff = SKBitmap.Decode(Properties.Resources.BombOffTile);
		readonly static SKBitmap bombOn = SKBitmap.Decode(Properties.Resources.BombOnTile);
		readonly static SKBitmap free = SKBitmap.Decode(Properties.Resources.FreeTile);
		readonly static SKBitmap player = SKBitmap.Decode(Properties.Resources.PlayerTile);
		readonly static SKBitmap treasure = SKBitmap.Decode(Properties.Resources.TreasureTile);

		public static Queue<SKBitmap> GetTileLayers(Tile tile)
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
				layers.Enqueue(player);
			else if (tile.HasTreasure)
				layers.Enqueue(treasure);

			if (!tile.IsVisible)
				layers.Enqueue(fog);

			return layers;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CaveGenerator
{
	internal static class Generator
	{
		internal static Cave CreateCave(Size size) => new Cave(size);

		internal static Random RND = new Random();

		internal static Player CreatePlayer(Cave cave)
		{
			EmptyTile playerTile = null;
			for (int x = 0; x < cave.Size.Width; ++x)
			{
				for (int y = 0; y < cave.Size.Height; ++y)
				{
					var tile = cave.Tiles[x, y];
					if (tile is EmptyTile et && !et.IsOccupied)
					{
						playerTile = et;
						break;
					}
				}

				if (playerTile is not null)
					break;
			}

			if (playerTile is null)
			{
				throw new ArgumentException("Not enough empty tiles in cave");
			}

			Player player = new Player() { OccupiedTile = playerTile };
			player.OccupiedTile.Occupier = player;
			
			return player;
		}
	}
}

using CaveGenerator;
using System;

namespace ConsoleUI
{
	static class CharTable
	{
		public static char GetTileChar(Tile tile)
		{
			return tile switch
			{
				Tile t when !t.IsVisible => '\u2592',
				Tile t when t.IsObstacle => '\u2588',
				Tile t when t.HasBomb => 'X',
				Tile t when !t.IsOccupied => ' ',
				Tile t when t.Occupier is Player => 'O',
				Tile t when t.Occupier is Enemy => '8',

				_ => throw new NotImplementedException()
			};
		}

		public static ConsoleColor GetTileColor(Tile tile)
		{
			return tile switch
			{
				Tile t when !t.IsVisible => ConsoleColor.Black,
				Tile t when t.IsTreasure => ConsoleColor.Yellow,
				Tile t when t.HasBomb && t.Bomb.IsActivated => ConsoleColor.Red,

				_ => ConsoleColor.Black
			};
		}
	}

}

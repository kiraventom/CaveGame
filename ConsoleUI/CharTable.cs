using System;
using CaveGenerator;

namespace ConsoleUI
{
	static class CharTable
	{
		public static char GetTileChar(Tile tile)
		{
			return tile switch
			{
				Obstacle => '\u2588',
				EmptyTile et when !et.IsOccupied => ' ',
				EmptyTile et when et.Occupier is Player => 'O',
				EmptyTile et when et.Occupier is Enemy => '8',

				_ => throw new NotImplementedException()
			};
		}
	}

}

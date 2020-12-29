using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveGenerator
{
	internal class Intellect
	{
		internal Intellect(Enemy attachedTo) => AttachedTo = attachedTo;

		internal Enemy AttachedTo { get; }
		internal List<Location> PathToPlayer { get; } = new List<Location>();
		private Location LastPlayerLocation;
		private bool PlayerMoved => LastPlayerLocation != GameEngine.Player.OccupiedTile.Location;

		private static bool shouldProceed;

		internal Tile GetTileToMoveTo()
		{
			if (PlayerMoved || !PathToPlayer.Any())
			{
				CalculatePath();
				LastPlayerLocation = GameEngine.Player.OccupiedTile.Location;
			}

			if (PathToPlayer.Any())
			{
				var tile = PathToPlayer[0];
				PathToPlayer.RemoveAt(0);
				return GameEngine.Cave.Tiles[tile.X, tile.Y];
			}
			else
			{
				// проверяем, рядом ли игрок
				var tile = AttachedTo.OccupiedTile.Neighbours.FirstOrDefault(t => t == GameEngine.Player.OccupiedTile);
				// если рядом, атакуем его, если нет -- двигаемся на рандомную соседнюю клетку
				return tile ?? this.AttachedTo.OccupiedTile.Neighbours.ElementAt(Generator.RND.Next(4));
			}
		}

		private void CalculatePath()
		{
			var thisTile = AttachedTo.OccupiedTile;
			var playerLoc = GameEngine.Player.OccupiedTile.Location;
			List<Location> noObstaclePath = new List<Location>();

			// "включаем" рекурсивный метод
			shouldProceed = true;
			// находим кратчайший путь к игроку, если бы не было препятствий
			RecursiveFindPath(thisTile, playerLoc, noObstaclePath, GameEngine.Cave.Size.Width * GameEngine.Cave.Size.Height, true); 
			// устанавливаем (его длину * 1.5) как верхнюю границу - если длина пути в рекурсивном методе будет больше, метод остановится
			uint estimatedMaxPathLength = (uint)Math.Ceiling(noObstaclePath.Count * 1.5);

			// "включаем" рекурсивный метод
			shouldProceed = true;
			// сохраняем последний корректно построенный путь
			var lastCorrectPath = new List<Location>(PathToPlayer);
			PathToPlayer.Clear();
			// Пытаемся построить путь короче, чем верхняя граница
			bool didFind = RecursiveFindPath(thisTile, playerLoc, PathToPlayer, estimatedMaxPathLength);

			// если не получилось построить путь, перебираем клетки последнего успешно построенного пути,
			// начиная с ближайшей к игроку, и пытаемся построить путь от каждой
			if (!didFind)
			{
				List<Location> restOfThePath = new List<Location>();
				for (int i = lastCorrectPath.Count - 1; i >= 0; --i)
				{
					restOfThePath.Clear();
					var tile = GameEngine.Cave.Tiles[lastCorrectPath[i].X, lastCorrectPath[i].Y];

					// "включаем" рекурсивный метод
					shouldProceed = true;
					// Пытаемся построить оставшуюся часть пути от текущей клетки
					bool didFindRestOfPath = RecursiveFindPath(tile, GameEngine.Player.OccupiedTile.Location, restOfThePath, estimatedMaxPathLength);

					// если путь построился, удаляем все клетки после этой в исходном пути и добавляем построенное
					if (didFindRestOfPath)
					{
						lastCorrectPath.RemoveRange(i, lastCorrectPath.Count - i);
						lastCorrectPath.AddRange(restOfThePath);
						PathToPlayer.AddRange(lastCorrectPath);
						if (PathToPlayer.Any())
							PathToPlayer.RemoveAt(0);
						return;
					}
				}

				// путь невозможно построить -- очень специфическая позиция либо физическая недоступность игрока
			}
			else
			{
				if (PathToPlayer.Any())
					PathToPlayer.RemoveAt(0);
			}
		}

		private static bool RecursiveFindPath(Tile startingFrom, Location playerLoc, List<Location> path, uint maxLength, bool ignoreObstacles = false)
		{
			if (!shouldProceed)
			{
				return false;
			}

			if (path.Contains(startingFrom.Location) || !ignoreObstacles && startingFrom.IsObstacle)
			{
				return false;
			}

			if (startingFrom.IsOccupied && startingFrom.Occupier is Player)
			{
				return startingFrom.Occupier is Player;
			}

			if (path.Count > maxLength)
			{
				shouldProceed = false;
				return false;
			}

			path.Add(startingFrom.Location);

			Direction[] directions = GetDirections(startingFrom, playerLoc);

			for (int i = 0; i < directions.Length; ++i)
			{
				var dir = directions[i];
				bool tryDir = RecursiveFindPath(startingFrom.GetNeighbour(dir), playerLoc, path, maxLength, ignoreObstacles);
				if (tryDir)
				{
					return true;
				}
			}

			path.Remove(startingFrom.Location);
			return false;
		}

		// переписать покрасивее
		private static Direction[] GetDirections(Tile tile, Location playerLoc)
		{
			Direction[] directions = new Direction[4];
			var thisLoc = tile.Location;
			int diff_x = (int)thisLoc.X - (int)playerLoc.X;
			int diff_y = (int)thisLoc.Y - (int)playerLoc.Y;
			if (diff_x > 0)
			{
				if (diff_y > 0)
				{
					if (Math.Abs(diff_x) > Math.Abs(diff_y))
					{
						directions[0] = Direction.Left;
						directions[1] = Direction.Down;
						directions[2] = Direction.Up;
						directions[3] = Direction.Right;
					}
					else
					{
						directions[0] = Direction.Down;
						directions[1] = Direction.Left;
						directions[2] = Direction.Right;
						directions[3] = Direction.Up;
					}
				}
				else
				{
					if (Math.Abs(diff_x) > Math.Abs(diff_y))
					{
						directions[0] = Direction.Left;
						directions[1] = Direction.Up;
						directions[2] = Direction.Down;
						directions[3] = Direction.Right;
					}
					else
					{
						directions[0] = Direction.Up;
						directions[1] = Direction.Left;
						directions[2] = Direction.Right;
						directions[3] = Direction.Down;
					}
				}
			}
			else
			{
				if (diff_y > 0)
				{
					if (Math.Abs(diff_x) > Math.Abs(diff_y))
					{
						directions[0] = Direction.Right;
						directions[1] = Direction.Down;
						directions[2] = Direction.Up;
						directions[3] = Direction.Left;
					}
					else
					{
						directions[0] = Direction.Down;
						directions[1] = Direction.Right;
						directions[2] = Direction.Left;
						directions[3] = Direction.Up;
					}
				}
				else
				{
					if (Math.Abs(diff_x) > Math.Abs(diff_y))
					{
						directions[0] = Direction.Right;
						directions[1] = Direction.Up;
						directions[2] = Direction.Down;
						directions[3] = Direction.Left;
					}
					else
					{
						directions[0] = Direction.Up;
						directions[1] = Direction.Right;
						directions[2] = Direction.Left;
						directions[3] = Direction.Down;
					}
				}
			}

			return directions;
		}
	}
}

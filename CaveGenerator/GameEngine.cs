using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveGenerator
{
	public static class GameEngine
	{
		static GameEngine()
		{
			Cave = Generator.CreateCave(new Size(30, 30));
			Player = Generator.CreatePlayer(Cave);
		}

		public static Cave Cave { get; }
		public static Player Player { get; }
	}
}

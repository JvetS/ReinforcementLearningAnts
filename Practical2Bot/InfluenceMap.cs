using System;
using System.Collections.Generic;
using System.Linq;
using Ants;

namespace YourBot
{
	public class InfluenceMap
	{
		private Location[,] map;

        private float decay = 0.5f;
        public float Decay { get { return decay; } set { decay = value; } }

        private float m0 = 0.5f;
        private float m1 = 0.5f;
		public float Momentum { get { return m0; } set {  if (value >= 0.0f && value <= 1.0f) { m0 = value; m1 = 1.0f - value; } else throw new ArgumentException(); } }

		public float[,] influence;

		public float this[int row, int col] { get { return influence[row, col]; } set { influence[row, col] = value; } }

		public InfluenceMap(Location[,] map)
		{
			this.map = map;
			influence = new float[map.GetLength (0), map.GetLength (1)];
		}

		public void AddInfluence(Location loc, float influence)
		{
            if (map[loc.Row, loc.Col].Value != Tile.Water)
			    this.influence[loc.Row, loc.Col] = m1 * this.influence[loc.Row, loc.Col] + m0 * influence;
		}

        // Influence is almost fully decayed after ln(e * influence) / decay tiles, not taking into account the momentum.
		public void UpdateInfluence()
		{
			float[,] updatedInfluence = new float[map.GetLength(0), map.GetLength(1)];
			foreach (Location loc in map)
			{
				if (loc.Value == Tile.Water)
					continue;

                float max = loc.Neighbors.Max(x => influence[x.Row, x.Col] * decay);

				updatedInfluence[loc.Row, loc.Col] = m1 * influence[loc.Row, loc.Col] + m0 * max;
			}

			influence = updatedInfluence;
		}

		public void UpdateInfluence(int times)
		{
			for (int i = 0; i < times; ++i)
				UpdateInfluence();
			/*float[,] updatedInfluence = new float[map.GetLength(0), map.GetLength(1)];
			foreach (Location loc in map)
			{
				float max = loc.Neighbors.SelectMany( (l, i) =>
					{

						Location[] locs = new Location[spread];
						Location ll = l;
						for (int j = 0; j < spread; ++j)
						{
							locs[j] = ll;
							ll = ll.Neighbors[i];
						}
						return locs;
					}
				).Max(x => influence[x.Row, x.Col] * Decay);

				updatedInfluence[loc.Row, loc.Col] = m1 * influence[loc.Row, loc.Col] + m0 * max;
			}
			influence = updatedInfluence;*/
		}
	}
}


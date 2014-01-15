using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    [Serializable()]
    public class QState
    {
        List<Location> MyAnts;
        List<Location> Food, EnemyAnts;
        int visibleTiles, visitedTiles, Reward;

        public QState(GameState state)
        {
            MyAnts = CopyList(state.MyAnts);
            Food = CopyList(state.FoodTiles);
            EnemyAnts = CopyList(state.EnemyAnts);

            foreach (Location l in state.map)
            {
               
                if (state.GetIsVisible(l))
                    visibleTiles++;
                if (l.Visited)
                    visitedTiles++;
            }

            CalculateReward(state);
        }

		public override int GetHashCode()
		{
			StringBuilder sb = new StringBuilder();
			foreach (Location l in MyAnts)
				sb.AppendFormat("{0}", l.GetHashCode());

			foreach (Location l in Food)
				sb.AppendFormat("{0}", l.GetHashCode());

			foreach (Location l in EnemyAnts)
				sb.AppendFormat("{0}", l.GetHashCode());

			sb.AppendFormat("{0}", visibleTiles);
			sb.AppendFormat("{0}", visitedTiles);

			return sb.ToString().GetHashCode();
		}

        public List<Location> CopyList(List<Location> original)
        {
            List<Location> result = new List<Location>(original.Count);

            for (int i = 0; i < original.Count; i++)
            {
                Location originalLoc = original[i];
                result.Add(new Location(originalLoc.Row, originalLoc.Col));
            }

            return result;
        }

        private void CalculateReward(GameState state)
        {
            int result = 0;
            result += MyAnts.Count * 100;
            result += visibleTiles + visitedTiles;

            float foodClaimed = 0f;
            float longestDistanceToFood = 0f;

            foreach (Location food in Food)
            {
                if (foodClaimed >= MyAnts.Count)
                    break;

                int leastDistance = int.MaxValue;
                Location closestAnt = null;

                foreach (Location ant in MyAnts)
                {
                    int distance = Globals.pathFinder.FindRoute(ant, food).GetDistance;

                    if (distance < leastDistance)
                    {
                        leastDistance = distance;
                        closestAnt = ant;
                    }
                }

                foodClaimed++;

                if (longestDistanceToFood < leastDistance)
                    longestDistanceToFood = leastDistance;
            }

            int expectedAntGain = (int)Math.Ceiling(foodClaimed / longestDistanceToFood);

            result += expectedAntGain * 10;

            Reward = result;

        }

        public int GetReward
        {
            get
            {
                return Reward;
            }
        }
    }
}

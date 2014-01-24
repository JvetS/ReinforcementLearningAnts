using Ants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourBot
{
    [Serializable()]
    class AnyRaze : QAction
    {
        public AnyRaze()
            : base("AnyRaze", 0)
        { }

        public override bool Apllicable(Ants.GameState state)
        {
            foreach (float f in Globals.hillInfluence.influence)
            {
                if (f > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Make ants home in and raze a hill.
        /// </summary>
        /// <param name="state"></param>
        public override void DoAction(Ants.GameState state, int hashcode)
        {
            base.DoAction(state, hashcode);

            // Make all ants move towards the closest enemy hill.
            foreach (AntData ant in allAnts)
            {
                // Move towards the unoccupied/passable tile closest to the hill.
            	Location next = ant.CurrentLocation.Neighbors[0];
				float max =  Globals.hillInfluence[next.Row, next.Col];
                foreach (Location l in ant.CurrentLocation.Neighbors)
                    if (Globals.hillInfluence[l.Row, l.Col] > max && Globals.state.GetIsUnoccupied(l) && Globals.state.GetIsPassable(l))
					{
						max = Globals.hillInfluence[l.Row, l.Col];
						next = l;
					}
                // Perform move.
                ant.AntRoute = new Route(ant.CurrentLocation, next, new Location[] { ant.CurrentLocation, next });
                ant.AdvancePath(this);
            }
        }
    }
}

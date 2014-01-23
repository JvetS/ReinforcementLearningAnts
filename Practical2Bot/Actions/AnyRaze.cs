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

        public override void DoAction(Ants.GameState state, int hashcode)
        {
            base.DoAction(state, hashcode);

            foreach (AntData ant in allAnts)
            {
            	Location next = ant.CurrentLocation.Neighbors[0];
				float max =  Globals.hillInfluence[next.Row, next.Col];
                foreach (Location l in ant.CurrentLocation.Neighbors)
                    if (Globals.hillInfluence[l.Row, l.Col] > max && Globals.state.GetIsUnoccupied(l) && Globals.state.GetIsPassable(l))
					{
						max = Globals.hillInfluence[l.Row, l.Col];
						next = l;
					}

                ant.AntRoute = new Route(ant.CurrentLocation, next, new Location[] { ant.CurrentLocation, next });
                ant.AdvancePath(this);
            }
        }
    }
}

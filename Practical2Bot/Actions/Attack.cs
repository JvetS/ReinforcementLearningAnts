using System;
using System.Linq;

using Ants;
using System.Collections.Generic;

namespace YourBot
{
    [Serializable()]
	public class Attack : QAction
    {
		public Attack() : base("attack", 0)
        {
        }

		public override bool Apllicable(GameState state)
		{
            foreach (Location loc in state.MyAnts)
            {
                if (Globals.friendlyInfluence[loc.Row, loc.Col] >= Globals.enemyInfluence[loc.Row, loc.Col])
                    return true;
            }
            return false;
			//return state.MyAnts.Any(l => Globals.friendlyInfluence[l.Row, l.Col] > Globals.enemyInfluence[l.Row, l.Col]);
		}

		public override void DoAction(GameState state, int hashcode)
		{
			base.DoAction(state, hashcode);

			foreach (AntData ant in allAnts)
			{
                Location next = ant.CurrentLocation.Neighbors[0];
				float max =  Globals.enemyInfluence[next.Row, next.Col];
                foreach (Location l in ant.CurrentLocation.Neighbors)
                {
                    if (Globals.enemyInfluence[l.Row, l.Col] > max && Globals.state.GetIsUnoccupied(l) && Globals.state.GetIsPassable(l))
                    {
                        max = Globals.enemyInfluence[l.Row, l.Col];
                        next = l;
                    }
                }

                ant.AntRoute = new Route(ant.CurrentLocation, next, new Location[] { ant.CurrentLocation, next });
				ant.AdvancePath(this);
			}
		}
    }
}


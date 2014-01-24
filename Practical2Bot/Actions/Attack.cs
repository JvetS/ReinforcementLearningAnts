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
            return state.MyAnts.Count > state.EnemyAnts.Count;
		}

        /// <summary>
        /// Ants will move and cluster towards enemies, dead ants add a fraction to the enemy influence.
        /// </summary>
        /// <param name="state"></param>
		public override void DoAction(GameState state, int hashcode)
		{
			base.DoAction(state, hashcode);

            // Make all ants attack, or at least close in on the enemy presence.
			foreach (AntData ant in allAnts)
			{
                // Select unoccupied/passable tile with maximum influence.
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
                //Perform move.
                ant.AntRoute = new Route(ant.CurrentLocation, next, new Location[] { ant.CurrentLocation, next });
				ant.AdvancePath(this);
			}
		}
    }
}


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
			return state.MyAnts.Any(l => Globals.friendlyInfluence[l.Row, l.Col] > Globals.enemyInfluence[l.Row, l.Col]);
		}

		public override void DoAction(GameState state, int hashcode)
		{
			base.DoAction(state, hashcode);

			foreach (AntData ant in allAnts)
			{
				Location cur = ant.CurrentLocation;
				if (Globals.friendlyInfluence[cur.Row, cur.Col] > Globals.enemyInfluence[cur.Row, cur.Col])
				{
					Location next = cur.Neighbors[0];
					float max =  Globals.enemyInfluence[next.Row, next.Col];
					foreach (Location l in cur.Neighbors)
						if (Globals.enemyInfluence[l.Row, l.Col] > max)
						{
							max = Globals.enemyInfluence[l.Row, l.Col];
							next = l;
						}

					ant.AntRole = Role.Attack;
					ant.AntRoute = new Route(cur, next, new Location[] { cur, next });
					ant.AdvancePath(this);
				}
			}
		}
    }
}


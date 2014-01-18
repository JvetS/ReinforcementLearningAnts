using System;
using System.Linq;

using Ants;
using System.Collections.Generic;

namespace YourBot
{
	public class Attack : QAction
    {
		public Attack() : base("attack", 0)
        {
        }

		public override bool Apllicable(GameState state)
		{
			return state.MyAnts.Any(l => Globals.friendlyInfluence[l.Row, l.Col] > Globals.enemyInfluence[l.Row, l.Col]);
		}

		public override void DoAction(GameState state, QState qstate)
		{
			base.DoAction(state, qstate);

			foreach (AntData ant in allAnts)
			{
				Location cur = ant.CurrentLocation;
				if (ant.AntRole == Role.Explore && Globals.friendlyInfluence[cur.Row, cur.Col] > Globals.enemyInfluence[cur.Row, cur.Col])
				{
					Location next = cur.Neighbors.Max(l => Globals.enemyInfluence[l.Row, l.Col]);

					ant.AntRole = Role.Attack;
					ant.AntRoute = new Route(cur, next, new Location[] { cur, next });
					ant.AdvancePath(this);
				}
			}
		}
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    class AllExplore : QAction
    {
        public AllExplore()
            : base("All explore", 0)
        { }

        public override bool Apllicable(GameState state)
        {
            return true;
        }

        public override void DoAction(GameState state, QState qstate)
        {
            base.DoAction(state, qstate);

            //to guarantee the same actions regardless of the particular run
            Random random = new Random(qstate.GetHashCode());

            foreach (AntData ant in allAnts)
            {
                Location lowerLeft = new Location(Globals.state.Height - 1, 0);
                Location upperRight = new Location(0, Globals.state.Width - 1);
                Location goal = SelectRandomLocation(lowerLeft, upperRight, random);

                ant.AntRoute = Globals.pathFinder.FindRoute(ant.CurrentLocation, goal);
                ant.AdvancePath(this);
            }
        }

        public Location SelectRandomLocation(Location lowerLeft, Location upperRight, Random random)
        {
            Location goal;
            do
            {
                goal = new Location(random.Next(upperRight.Row, lowerLeft.Row), random.Next(lowerLeft.Col, upperRight.Col));
            } while (goal == null && !Globals.state.MyHills.Contains(goal) && Globals.state.GetIsPassable(goal));

            return goal;
        }
    }
}

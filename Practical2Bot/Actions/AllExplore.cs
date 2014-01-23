using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    [Serializable()]
    class AllExplore : QAction
    {
        public AllExplore()
            : base("All explore", 0)
        { }

        public override bool Apllicable(GameState state)
        {
            return true;
        }

        public override void DoAction(GameState state, int hashcode)
        {
            base.DoAction(state, hashcode);

            //to guarantee the same actions regardless of the particular run
            Random random = new Random(hashcode);

            foreach (AntData ant in allAnts)
            {
                Location goal = SelectRandomNeighbor(ant.CurrentLocation, random);

                ant.AntRoute = new Route(ant.CurrentLocation, goal, new Location[] { ant.CurrentLocation, goal });

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

        public Location SelectRandomNeighbor(Location loc, Random random)
        {
            Location goal = null;
            while (!Globals.state.GetIsPassable((goal = loc.Neighbors[random.Next(4)])) && Globals.state.MyHills.Contains(goal) && !Globals.state.GetIsUnoccupied(goal))
                continue;

            return goal;
        }
    }
}

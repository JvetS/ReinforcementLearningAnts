using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    [Serializable()]
    class MoveAllToFood : QAction
    {
        public MoveAllToFood()
            : base("all food", 0)
        { }

        public override bool Apllicable(GameState state)
        {
            return state.FoodTiles.Count > 0;
        }

        /// <summary>
        /// Move ants towards the nearest food source.
        /// </summary>
        /// <param name="state"></param>
        public override void DoAction(GameState state, int hashcode)
        {
            base.DoAction(state, hashcode);

            // We moved to influence based food gathering after some issues with the path planner.

            // Move all ants towards their closest food piece.
            foreach (AntData ant in allAnts)
            {
                // Select the unoccupied/passable tile in the direction of the closest food piece.
                Location next = ant.CurrentLocation.Neighbors[0];
                float max = Globals.foodInfluence[next.Row, next.Col];
                foreach (Location l in ant.CurrentLocation.Neighbors)
                    if (Globals.foodInfluence[l.Row, l.Col] > max && Globals.state.GetIsUnoccupied(l) && Globals.state.GetIsPassable(l))
                    {
                        max = Globals.foodInfluence[l.Row, l.Col];
                        next = l;
                    }
                // Perform move.
                ant.AntRoute = new Route(ant.CurrentLocation, next, new Location[] { ant.CurrentLocation, next });
                ant.AdvancePath(this);
            }
        }
    }
}

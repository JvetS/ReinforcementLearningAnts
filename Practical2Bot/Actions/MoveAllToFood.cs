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
        /// move an equal amount of ants to each food tile
        /// </summary>
        /// <param name="state"></param>
        public override void DoAction(GameState state, int hashcode)
        {
            base.DoAction(state, hashcode);

            foreach (AntData ant in allAnts)
            {
                Location next = ant.CurrentLocation.Neighbors[0];
                float max = Globals.foodInfluence[next.Row, next.Col];
                foreach (Location l in ant.CurrentLocation.Neighbors)
                    if (Globals.foodInfluence[l.Row, l.Col] > max && Globals.state.GetIsUnoccupied(l) && Globals.state.GetIsPassable(l))
                    {
                        max = Globals.foodInfluence[l.Row, l.Col];
                        next = l;
                    }

                ant.AntRoute = new Route(ant.CurrentLocation, next, new Location[] { ant.CurrentLocation, next });
                ant.AdvancePath(this);
            }
        }
    }
}

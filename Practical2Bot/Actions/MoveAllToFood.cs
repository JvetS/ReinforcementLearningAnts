﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
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
        public override void DoAction(GameState state, QState qstate)
        {
            base.DoAction(state, qstate);

            int foodCount = state.FoodTiles.Count;
            int ants = allAnts.Count;
            int antPerFood = foodCount / ants;

            foreach (Location food in state.FoodTiles)
            {
                AnonymousMinHeap<AntData> antHeap = new AnonymousMinHeap<AntData>();

                foreach(AntData ant in allAnts)
                {
                    antHeap.Add(ant, state.GetDistance(ant.CurrentLocation,food));
                }

                int assignedAnts = 0;

                while (assignedAnts < antPerFood)
                {
                    AntData ant = antHeap.ExtractMin();

                    if (ant.AntRole != Role.Gather)
                    {
                        ant.AntRole = Role.Gather;
                        ant.AntRoute = Globals.pathFinder.FindRoute(ant.CurrentLocation, food);
                        ant.AdvancePath(this);
                        assignedAnts++;
                    }
                }
            }
        }
    }
}
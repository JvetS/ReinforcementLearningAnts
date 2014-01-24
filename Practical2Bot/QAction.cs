using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    [Serializable()]
    public abstract class QAction//this class provides all basic methods to move ants around the field, action classes use these methods to construct behaviour. 
    {
        public string ID { get; private set;}
        public float QValue;

        //to prevent ants occupying same tile
        protected Dictionary<Location, Location> orders = new Dictionary<Location, Location>();
        protected HashSet<AntData> allAnts = new HashSet<AntData>();
        public Dictionary<Location, AntData> antLocations = new Dictionary<Location, AntData>();

        public QAction(string id, float q)
        {
            ID = id;
            QValue = q;
        }

        public abstract bool Apllicable(GameState state);

        protected MoveType doMoveDirection(Location ant, Direction direction)
        {
            Location newLoc = Globals.state.GetDestination(ant, direction);

            if (Globals.state.GetIsUnoccupied(newLoc) && !orders.ContainsKey(newLoc))
            {
                Bot.IssueOrder(ant, direction);
                orders.Add(newLoc, ant);
                return MoveType.Success;
            }
            else
            {
                if (!Globals.state.GetIsPassable(newLoc))
                    return MoveType.FailTerrain;
                if (orders.ContainsKey(newLoc))
                    return MoveType.FailAnt;
                if (!Globals.state.GetIsUnoccupied(newLoc))
                    return MoveType.Fail;
            }

                return MoveType.Fail;
        }

        /// <summary>
        /// second in line when issueing orders, called from AntData.AdvancePath
        /// </summary>
        /// <param name="ant"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public MoveType MoveForLocation(Location ant, Location destination)
        {
            ICollection<Direction> direction = Globals.state.GetDirections(ant, destination);
            return doMoveDirection(ant, new List<Direction>(direction)[0]);
        }


        protected void CreatAntData(GameState state)
        {
            foreach (Location loc in state.MyAnts)
            {
                if (!antLocations.ContainsKey(loc))
                {
                    AntData newAnt = new AntData(loc);
                    antLocations.Add(loc, newAnt);
                    allAnts.Add(newAnt);
                }
            }
        }

        //turn locations into antData objects for compatibility with reused methods from previous practical
        public virtual void DoAction(GameState state, int hashcode)
        {
            //actions are serialised at the end, make sure to start fresh by clearing all the ant related data
            orders.Clear();
            allAnts.Clear();
            antLocations.Clear();
            CreatAntData(state);
        }

    }
}

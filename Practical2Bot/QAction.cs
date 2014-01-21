using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    [Serializable()]
    public abstract class QAction
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

        public virtual void DoAction(GameState state, int hashcode)
        {
            orders.Clear();
            allAnts.Clear();
            antLocations.Clear();
            CreatAntData(state);
        }

    }
}

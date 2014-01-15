using System;
using Ants;

namespace YourBot
{
    public enum Role { None, Explore, Gather, Defend, Attack }

    public class AntData
    {
        private static int TotalAnts;//counter to automatically ensire unique ID per ant
        public int ID { get; private set; }
        public Role AntRole {get; set;}
        public Route AntRoute;
        public Location CurrentLocation { get; private set; }

        public AntData(Location loc)
        {
            TotalAnts++;
            ID = TotalAnts;
            CurrentLocation = loc;
        }
        
        /// <summary>
        /// method to mave ant along its path, only place where position of ant can be modified
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public MoveType AdvancePath(QAction action)
        {
            Location target = this.AntRoute.GetNext;

            if (Globals.state.GetDistance(this.CurrentLocation, AntRoute.GetEnd) < 1)
            {
                return MoveType.Arrived;
            }

            if (target == null)
            {
                return MoveType.Arrived;
            }

            //her starts line to get order to framework
            MoveType move = action.MoveForLocation(CurrentLocation, target);

            //the move has been approved and forwarded to framwerk
            if (move == MoveType.Success)
            {
                
                action.antLocations.Remove(CurrentLocation);
                CurrentLocation = target;
                action.antLocations.Add(CurrentLocation, this);
                AntRoute.PathIndex++;

            }

            return move;
        }
        
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}

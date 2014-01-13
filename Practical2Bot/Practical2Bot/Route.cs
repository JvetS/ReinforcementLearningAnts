using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    public class Route : IComparable<Route>
    {
        private Location start;
        private Location end;
        private Location[] Path;

        private int distance, currentIndex;

        public Route(Location first, Location last, Location[] path)
        {
            start = first;
            end = last;
            distance = path.Length;
            currentIndex = 0;
            Path = path;
        }

        /// <summary>
        /// resyricts path storage from going out of bounds
        /// </summary>
        public int PathIndex
        {
            get { return currentIndex; }
            set
            {
                if (value >= 0 && value < distance)
                    currentIndex = value;
            }
        }

        public Location[] GetPath
        {
            get { return Path; }
        }

        /// <summary>
        /// gets next location withou going out of bounds
        /// </summary>
        public Location GetNext
        {
            get
            {
                if (currentIndex + 1 < distance)
                {
                    return Path[currentIndex+1];
                }
                else
                    return null;
            }
        }

        public Location GetStart
        {
            get { return start; }
        }

        public Location GetEnd
        {
            get { return end; }
        }

        public int GetDistance
        {
            get { return distance; }
        }

        public int CompareTo(Route other)
        {
            return distance - other.GetDistance;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool equals = false;
            Route other;

            if ((other = obj as Route) != null)
            {
                equals = start.Equals(other.GetStart) && end.Equals(other.GetEnd) && Path.Equals(other.Path);
            }

            return equals;
        }
    }
}

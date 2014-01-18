using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    class Pathfinder
    {
        private class MapTile : IHeapItem
        {
            int Index;
            float h, f;//a*vars
            int g;//a* var
            MapTile Pi;//pointer to previous tile on path
            Location Position;
            bool InOpen;

            public MapTile(int col, int row)
            {
                Reset();
				Position = Globals.state[row, col];
            }

            public void Reset()
            {
                h = 10000;
                g = 10000;
                f = 10000;
                InOpen = false;
                Pi = null;
            }

            public bool InOpenSet
            {
                get { return InOpen; }
                set { InOpen = value; }
            }

            public float CostEstimate
            {
                get { return f; }
                set { f = value; }
            }

            public int CostKnown
            {
                get { return g; }
                set { g = value; }
            }

            public float Heuristic
            {
                get { return h; }
                set { h = value; }
            }

            public Location GetLocation
            {
                get { return Position; }
            }

            public MapTile Parent
            {
                get { return Pi; }
                set { Pi = value; }
            }

            //methods to implement heap interface
            public float GetKey()
            {
                return f;
            }

            public void SetKey(float key)
            {
                f = key;
            }

            public int GetIndex()
            {
                return Index;
            }

            public void SetIndex(int index)
            {
                Index = index;
            }
        }


        MapTile[,] Map;
        public Pathfinder(int width, int height)
        {
            //use our own tiles independent of that game state
            Map = new MapTile[height, width];

            for (int col = 0; col < width; col++)
                for (int row = 0; row < height; row++)
                    Map[row, col] = new MapTile(col, row);
        }

        /// <summary>
        /// relaxes the edge between two nodes, this needs to happen for all neighbours of source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="neighbour"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private bool Relax(MapTile source, MapTile neighbour, MapTile destination)
        {
            float heuristic = Globals.state.GetDistance(neighbour.GetLocation, destination.GetLocation);//manhattan distance as heuristic

            bool betterRoute = neighbour.CostEstimate > source.CostKnown + 1 + heuristic;
            bool passable = Globals.state.GetIsPassable(neighbour.GetLocation);
            if (betterRoute && passable)
            {
                neighbour.CostKnown = source.CostKnown + 1;
                neighbour.CostEstimate = neighbour.CostKnown + heuristic;
                neighbour.Heuristic = heuristic;
                neighbour.Parent = source;

                return true;
            }

            return false;
        }

        /// <summary>
        /// when the pathfinding has terminated this builds the path, returns the Route object and readies the map for the next pathfinding call
        /// </summary>
        /// <param name="end"></param>
        /// <param name="toReset"></param>
        /// <returns></returns>
        private Route BuildRoute(MapTile end, HashSet<MapTile> toReset)
        {
            //pointers are end -> begin so the path comes in reverse

            Location[] Path = new Location[end.CostKnown+1];//tiles in a path eqaul to the cost + 1 (hops between tile +1), use this to avoid having to reverse the path in O(n)

            MapTile current = end;

            while(current != null)
            {
                Path[current.CostKnown] = current.GetLocation;
                current = current.Parent;
            }

            Location first = Path[0];

            foreach (MapTile t in toReset)//ready the map for next pathfinding call
                t.Reset();

            return new Route(first, end.GetLocation, Path);
        }

        /// <summary>
        /// call this to plan a path between points
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Route FindRoute(Location begin, Location end)
        {
            HashSet<MapTile> tilesToReset = new HashSet<MapTile>();//all tiles that need to be reset

            MapTile mapEnd = Map[end.Row, end.Col];
            MapTile mapBegin = Map[begin.Row, begin.Col];
            tilesToReset.Add(mapBegin);

            mapBegin.CostKnown = 0;
            mapBegin.Heuristic = Globals.state.GetDistance(begin, end);
            mapBegin.CostEstimate = mapBegin.CostKnown + mapBegin.Heuristic;


            MinHeap<MapTile> OpenSet = new MinHeap<MapTile>();
            OpenSet.Add(mapBegin);

            while (!OpenSet.IsEmpty)
            {
                MapTile current = OpenSet.ExtractMin();

                if (current == mapEnd)
                    return BuildRoute(mapEnd, tilesToReset);//reset after the oath is build, we need those pi pointers

                foreach (Direction d in (Direction[])Enum.GetValues(typeof(Direction)))
                {
                    Location tile = Globals.state.GetDestination(current.GetLocation, d);
                    MapTile neighbour = Map[tile.Row, tile.Col];

                    bool succesful = Relax(current, neighbour, mapEnd);
                    tilesToReset.Add(neighbour);//hashset will not contain duplicates

                    if (!neighbour.InOpenSet && succesful)
                    {
                        OpenSet.Add(neighbour);
                        neighbour.InOpenSet = true;//openset is a min heap, no O(1) lookup so store this in the tile
                    }
                    else
                    {

                        if (neighbour.InOpenSet && succesful)
                        {
                            OpenSet.ChangeKey(neighbour, neighbour.CostEstimate);
                        }
                    }
                }
            }

            foreach (MapTile t in tilesToReset)//no route found, still need to reset
                t.Reset();

            return null;
        }
    }
}

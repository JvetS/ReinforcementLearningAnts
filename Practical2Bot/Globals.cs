using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourBot;

namespace Ants
{
    static class Globals
    {
        public static GameState state;
        public static Random random;
        public static Pathfinder pathFinder;
		public static InfluenceMap enemyInfluence;
		public static InfluenceMap friendlyInfluence;
        public static InfluenceMap hillInfluence;
        public static InfluenceMap foodInfluence;
        public static string QLearnerFolder;
    }
}

using System;
using System.Collections.Generic;

namespace Ants {
	
	public class GameState : IGameState {
		
		public int Width { get; private set; }
		public int Height { get; private set; }
		
		public int LoadTime { get; private set; }
		public int TurnTime { get; private set; }
		
		private DateTime turnStart;
		public int TimeRemaining {
			get {
				TimeSpan timeSpent = DateTime.Now - turnStart;
				return TurnTime - (int)timeSpent.TotalMilliseconds;
			}
		}

        public int ViewRadius { get; private set; }
        public int AttackRadius { get; private set; }
		public int ViewRadius2 { get; private set; }
		public int AttackRadius2 { get; private set; }
		public int SpawnRadius2 { get; private set; }
        public int PlayerSeed { get; private set; }

		public List<Location> MyAnts { get; private set; }
		public List<Location> MyHills { get; private set; }
		public List<Location> EnemyAnts { get; private set; }
		public List<Location> EnemyHills { get; private set; }
		public List<Location> DeadTiles { get; private set; }
		public List<Location> FoodTiles { get; private set; }

		public Location this[int row, int col] {
			get { return this.map[row, col]; }
		}
		
        public Location[,] map;
		
		public GameState (int width, int height, 
		                  int turntime, int loadtime, 
		                  int viewradius2, int attackradius2, int spawnradius2, int seed) {
			
			Width = width;
			Height = height;
			
			LoadTime = loadtime;
			TurnTime = turntime;
            PlayerSeed = seed;
			

			ViewRadius2 = viewradius2;
			AttackRadius2 = attackradius2;
            ViewRadius = (int)Math.Sqrt(viewradius2);
            AttackRadius = (int)Math.Sqrt(attackradius2);
			SpawnRadius2 = spawnradius2;
			
			MyAnts = new List<Location>();
			MyHills = new List<Location>();
			EnemyAnts = new List<Location>();
			EnemyHills = new List<Location>();
			DeadTiles = new List<Location>();
			FoodTiles = new List<Location>();

			map = new Location[height, width];
			for (int i = 0; i < map.GetLength(0); ++i)
			{
				for (int j = 0; j < map.GetLength(1); ++j)
				{
					map[i, j] = new Location(i, j);
				}
			}

			foreach (Location l in map)
			{
				int col = (l.Col - 1) % width;
				if (col < 0)
					col += width;
				int row = (l.Row - 1) % height;
				if (row < 0)
					row += height;

				l.Neighbors[0] = map[row, l.Col];
				l.Neighbors[3] = map[l.Row, col];

				row = (l.Row + 1) % height;
				col = (l.Col + 1) % width;

				l.Neighbors[1] = map[row, l.Col];
				l.Neighbors[2] = map[l.Row, col];
			}
		}
        #region Utility
        public Location WrapCoordinates(Location input)
        {
            int col = input.Col % Width;
            if (col < 0)
                col += Width;
            int row = input.Row % Height;
            if (row < 0)
                row += Height;

            return new Location(row, col);
        }
        #endregion

        #region State mutators
        public void StartNewTurn () {
			// start timer
			turnStart = DateTime.Now;
			
			// clear ant data
			foreach (Location loc in MyAnts) map[loc.Row, loc.Col].ResetLand();
			foreach (Location loc in MyHills) map[loc.Row, loc.Col].ResetLand();
			foreach (Location loc in EnemyAnts) map[loc.Row, loc.Col].ResetLand();
			foreach (Location loc in EnemyHills) map[loc.Row, loc.Col].ResetLand();
			foreach (Location loc in DeadTiles) map[loc.Row, loc.Col].ResetLand();
			
			MyHills.Clear();
			MyAnts.Clear();
			EnemyHills.Clear();
			EnemyAnts.Clear();
			DeadTiles.Clear();
			
			// set all known food to unseen
			foreach (Location loc in FoodTiles) map[loc.Row, loc.Col].Value = Tile.Land;
			FoodTiles.Clear();
		}

		public void AddAnt (int row, int col, int team) {
			Location loc = map[row, col];
			loc.Value = Tile.Ant;

			loc.Team = team;
			if (team == 0) {
				MyAnts.Add(loc);
                map[row, col].Visited = true;
			} else {
				EnemyAnts.Add(loc);
			}
		}

		public void AddFood (int row, int col) {
			map[row, col].Value = Tile.Food;
			FoodTiles.Add(map[row, col]);
		}

		public void RemoveFood (int row, int col) {
			// an ant could move into a spot where a food just was
			// don't overwrite the space unless it is food
			Location loc = map[row, col];
			if (loc.Value == Tile.Food) {
				loc.Value = Tile.Land;
			}
			FoodTiles.Remove(loc);
		}

		public void AddWater (int row, int col) {
			map[row, col].Value = Tile.Water;
		}

		public void DeadAnt (int row, int col) {
			// food could spawn on a spot where an ant just died
			// don't overwrite the space unless it is land
			Location loc = map[row, col];
			if (loc.Value == Tile.Land) {
				loc.Value = Tile.Dead;
			}
			
			// but always add to the dead list
			DeadTiles.Add(loc);
		}

		public void AntHill (int row, int col, int team) {
			Location loc = map[row, col];
			if (loc.Value == Tile.Land) {
				loc.Value = Tile.Hill;
				loc.Team = team;
			}

			if (team == 0)
				MyHills.Add (loc);
			else
				EnemyHills.Add (loc);
		}
		#endregion

		/// <summary>
		/// Gets whether <paramref name="location"/> is passable or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is not water, <c>false</c> otherwise.</returns>
		/// <seealso cref="GetIsUnoccupied"/>
		public bool GetIsPassable (Location location) {
			return map[location.Row, location.Col].Value != Tile.Water;
		}
		
		/// <summary>
		/// Gets whether <paramref name="location"/> is occupied or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is passable and does not contain an ant, <c>false</c> otherwise.</returns>
		public bool GetIsUnoccupied (Location location) {
			return GetIsPassable(location) && map[location.Row, location.Col].Value != Tile.Ant && map[location.Row, location.Col].Value!=Tile.Food;
		}
		
		/// <summary>
		/// Gets the destination if an ant at <paramref name="location"/> goes in <paramref name="direction"/>, accounting for wrap around.
		/// </summary>
		/// <param name="location">The starting location.</param>
		/// <param name="direction">The direction to move.</param>
		/// <returns>The new location, accounting for wrap around.</returns>
		public Location GetDestination (Location location, Direction direction) {
			return location.Neighbors[(int)direction];
		}

		/// <summary>
		/// Gets the distance between <paramref name="loc1"/> and <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The first location to measure with.</param>
		/// <param name="loc2">The second location to measure with.</param>
		/// <returns>The distance between <paramref name="loc1"/> and <paramref name="loc2"/></returns>
		public int GetDistance (Location loc1, Location loc2) {
			int d_row = Math.Abs(loc1.Row - loc2.Row);
			d_row = Math.Min(d_row, Height - d_row);
			
			int d_col = Math.Abs(loc1.Col - loc2.Col);
			d_col = Math.Min(d_col, Width - d_col);
			
			return d_row + d_col;
		}

		/// <summary>
		/// Gets the closest directions to get from <paramref name="loc1"/> to <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The location to start from.</param>
		/// <param name="loc2">The location to determine directions towards.</param>
		/// <returns>The 1 or 2 closest directions from <paramref name="loc1"/> to <paramref name="loc2"/></returns>
		public ICollection<Direction> GetDirections (Location loc1, Location loc2) {
			List<Direction> directions = new List<Direction>();
           
                if (loc1.Row < loc2.Row)
                {
                    if (loc2.Row - loc1.Row >= Height / 2)
                        directions.Add(Direction.North);
                    if (loc2.Row - loc1.Row <= Height / 2)
                        directions.Add(Direction.South);
                }
                if (loc2.Row < loc1.Row)
                {
                    if (loc1.Row - loc2.Row >= Height / 2)
                        directions.Add(Direction.South);
                    if (loc1.Row - loc2.Row <= Height / 2)
                        directions.Add(Direction.North);
                }

                if (loc1.Col < loc2.Col)
                {
                    if (loc2.Col - loc1.Col >= Width / 2)
                        directions.Add(Direction.West);
                    if (loc2.Col - loc1.Col <= Width / 2)
                        directions.Add(Direction.East);
                }
                if (loc2.Col < loc1.Col)
                {
                    if (loc1.Col - loc2.Col >= Width / 2)
                        directions.Add(Direction.East);
                    if (loc1.Col - loc2.Col <= Width / 2)
                        directions.Add(Direction.West);
                }

                return directions;
		}
		
		public bool GetIsVisible(Location loc)
		{
			List<Location> offsets = new List<Location>();
			int squares = (int)Math.Floor(Math.Sqrt(this.ViewRadius2));
			for (int r = -1 * squares; r <= squares; ++r)
			{
				for (int c = -1 * squares; c <= squares; ++c)
				{
					int square = r * r + c * c;
					if (square < this.ViewRadius2)
					{
                        Location wrap = this.WrapCoordinates(new Location(r, c));
                        r = wrap.Row;
                        c = wrap.Col;
						offsets.Add(map[r, c]);
					}
				}
			}
			foreach (Location ant in this.MyAnts)
			{
				foreach (Location offset in offsets)
				{
					if ((ant.Col + offset.Col) == loc.Col &&
						(ant.Row + offset.Row) == loc.Row)
					{
								 return true;
					}
				}
			}
			return false;
		}

	}
}


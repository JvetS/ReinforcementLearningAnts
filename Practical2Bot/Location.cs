using System;
using System.Collections.Generic;

namespace Ants {
    [Serializable()]
	public class Location : IEquatable<Location> {

		/// <summary>
		/// Gets the row of this location.
		/// </summary>
		public int Row { get; private set; }

		/// <summary>
		/// Gets the column of this location.
		/// </summary>
		public int Col { get; private set; }

        public int Turn { get; set; }

        public bool Visited { get; set; }

		/// <summary>
		/// Gets the underlying tile value.
		/// </summary>
		private Tile value = Tile.Unseen;
		public Tile Value { get { return this.value; } set{ this.value = value; } }

		private int team = -1;
		public int Team { get { return this.team; } set { this.team = value; } }

		/// <summary>
		/// The neighboring locations of this location.
		/// Stored as { North, South, East, West }.
		/// </summary>
		private Location[] neighbors = new Location[4];
		public Location[] Neighbors { get{ return this.neighbors; } }

		/// <summary>
		/// Gets the next location in a planned path given its absolute turn number.
		/// A path may be mapped by adding the next location from the current location in the path to the dictionary, with its absolute turn number as the key.
		/// </summary>
		private Dictionary<Location, Location> nextLocationsInPath = new Dictionary<Location, Location> (10);
		public Dictionary<Location, Location> NextLocationsInPath { get { return this.nextLocationsInPath; } }

		public Location (int row, int col) {
			this.Row = row;
			this.Col = col;
		}

		public Location (int row, int col, Tile value) : this(row, col) {
			this.Value = value;
		}

		public void ResetLand()
		{
			this.value = Tile.Land;
			this.team = -1;
		}

		public override bool Equals (object obj) {
			if (ReferenceEquals (null, obj))
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType() != typeof (Location))
				return false;

			return Equals ((Location) obj);
		}

		public bool Equals (Location other) {
			if (ReferenceEquals (null, other))
				return false;
			if (ReferenceEquals (this, other))
				return true;

			return other.Row == this.Row && other.Col == this.Col;
		}

		public override int GetHashCode()
		{
			unchecked {
				return (this.Row * 397) ^ this.Col;
			}
		}
	}

}


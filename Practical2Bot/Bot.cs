using System;

namespace Ants {
	public abstract class Bot {

		public abstract void DoTurn(GameState state);

        public abstract void Initialise(GameState state);

        public abstract void LastTurn(GameState state, bool won);

		public static void IssueOrder(Location loc, Direction direction) {
			System.Console.Out.WriteLine("o {0} {1} {2}", loc.Row, loc.Col, direction.ToChar());
		}
	}
}
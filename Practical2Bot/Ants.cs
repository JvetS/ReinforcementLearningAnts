using System;
using System.IO;
using System.Collections.Generic;

namespace Ants {

	public class Ants {
		
		public static readonly Location North = new Location(-1, 0);
		public static readonly Location South = new Location(1, 0);
		public static readonly Location West = new Location(0, -1);
		public static readonly Location East = new Location(0, 1);
		
		public static IDictionary<Direction, Location> Aim = new Dictionary<Direction, Location> {
			{ Direction.North, North},
			{ Direction.East, East},
			{ Direction.South, South},
			{ Direction.West, West}
		};
		
		private const string READY = "ready";
		private const string GO = "go";
		private const string END = "end";
		
		private GameState state;
        private bool LastState = false;


		public void PlayGame(Bot bot) {
            StreamWriter writer = new StreamWriter("botInput.txt",false);
			List<string> input = new List<string>();
            bool endParse = false;
			
			
			while (!endParse) {
				string line = System.Console.In.ReadLine().Trim().ToLower();
                writer.WriteLine(line);
                writer.Flush();

				switch (line)
				{
				case READY:
					ParseSetup(input);
                    bot.Initialise(state);
					FinishTurn();
					input.Clear();
					break;
				case GO:
                    if (!LastState)
                    {
                        state.StartNewTurn();
                        ParseUpdate(input);
                        bot.DoTurn(state);
                        FinishTurn();
                        input.Clear();
                    }
                    else
                    {
                        string[] scores = input[1].Remove(0, 6).Split(); ;//de "scores<spatie>"er af slopen en dan splitten

                        int myScore = int.Parse(scores[0]);//we gaan er vanuit de wij bot 1 zijn
                        bool won = false;
                        for (int i = 1; i < scores.GetLength(0); i++)
                        {
                            won = myScore > int.Parse(scores[i]) && won;
                        }

                        input.RemoveRange(0, 4);//alle informatie voor de laatste state er af slopen

                        ParseUpdate(input);

                        bot.LastTurn(state, won);

                    }
					break;
				case END:
                    LastState = true;
                    break;
				default:
					input.Add(line);
					break;
					
			    }
            //catch (Exception e) {
            //    #if DEBUG
            //        FileStream fs = new FileStream("debug.log", System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //        StreamWriter sw = new StreamWriter(fs);
            //        sw.WriteLine(e);
            //        sw.Close();
            //        fs.Close();
            //    #endif
			}

            writer.Close();
		}
		
		// parse initial input and setup starting game state
		private void ParseSetup(List<string> input) {
			int width = 0, height = 0;
			int turntime = 0, loadtime = 0;
            int playerSeed = 0;
			int viewradius2 = 0, attackradius2 = 0, spawnradius2 = 0;
			
			foreach (string line in input) {
				if (string.IsNullOrEmpty (line))
					continue;

				string[] tokens = line.Split();
				string key = tokens[0];

				switch (key)
				{
				case "cols":
					width = int.Parse(tokens[1]);
                    break;
				case "rows":
					height = int.Parse(tokens[1]);
                    break;
				case "player_seed":
                    playerSeed = int.Parse(tokens[1]);
					break;
				case "turntime":
					turntime = int.Parse(tokens[1]);
					break;
				case "loadtime":
					loadtime = int.Parse(tokens[1]);
					break;
				case "viewradius2":
					viewradius2 = int.Parse(tokens[1]);
					break;
				case "attackradius2":
					attackradius2 = int.Parse(tokens[1]);
					break;
				case "spawnradius2":
					spawnradius2 = int.Parse(tokens[1]);
					break;
				}
			}
			
			this.state = new GameState(width, height, 
			                           turntime, loadtime, 
			                           viewradius2, attackradius2, spawnradius2, playerSeed);
		}
		
		// parse engine input and update the game state
		private void ParseUpdate(List<string> input) {
			// do some stuff first
			
			foreach (string line in input) {
				if (string.IsNullOrEmpty (line))
					continue;
								
				string[] tokens = line.Split();
				
				if (tokens.Length >=3) {
					int row = int.Parse(tokens[1]);
					int col = int.Parse(tokens[2]);

					switch (tokens [0])
					{
						case "a":
							state.AddAnt(row, col, int.Parse(tokens[3]));
							break;
						case "f":
							state.AddFood(row, col);
							break;
						case "r":
							state.RemoveFood(row, col);
							break;
						case "w":
							state.AddWater(row, col);
							break;
						case "d":
							state.DeadAnt(row, col);
							break;
						case "h":
							state.AntHill (row, col, int.Parse(tokens[3]));
							break;
					}
				}
			}
		}

		private void FinishTurn () {
			System.Console.Out.WriteLine(GO);
		}
		
	}
}
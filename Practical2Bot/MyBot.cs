using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Linq;
using YourBot;
using System.Runtime.Serialization;

namespace Ants
{

    public enum MoveType { Success, FailTerrain, FailAnt, Fail, Arrived }
    enum BotState { Early, Mid, Late }

    public class MyBot : Bot
    {
        QLearner Learner;

        public override void DoTurn(GameState state)
        {
            Learner.LearnPolicy(state);
        }

        public void LastTurn(GameState state)
        {
        }

        public override void Initialise(GameState state)
        {
            Globals.state = state;
            Globals.random = new Random(state.PlayerSeed);
        }

        public static void Main(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif
            new Ants().PlayGame(new MyBot());
        }
    }

}
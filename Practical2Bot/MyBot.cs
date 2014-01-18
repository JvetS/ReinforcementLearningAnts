using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Linq;
using YourBot;
using System.Runtime.Serialization.Formatters.Binary;
//GIT
namespace Ants
{

    public enum MoveType { Success, FailTerrain, FailAnt, Fail, Arrived }
    enum BotState { Early, Mid, Late }

    public class MyBot : Bot
    {
        QLearner Learner;

        public override void DoTurn(GameState state)
        {
            Learner.LearnPolicy(state, false);
        }

        public override void LastTurn(GameState state, bool won)
        {
            //TO DO laatse beurt afhandelen
            Learner.LearnPolicy(state, won);
            Learner.PrepareForSerialisation();
            BinaryFormatter formatter = new BinaryFormatter();
            Stream learnerStream = new FileStream("QData.Q", FileMode.OpenOrCreate);
            formatter.Serialize(learnerStream, Learner);
        }

        public override void Initialise(GameState state)
        {
            Globals.state = state;
            Globals.random = new Random(state.PlayerSeed);
            Globals.pathFinder = new Pathfinder(state.Width, state.Height);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Stream learnerStream = new FileStream("QData.Q", FileMode.Open);
                Learner = (QLearner)formatter.Deserialize(learnerStream);
            }
            catch
            {
                Learner = new QLearner(0.9f, 0.8f, 0.9f, state.PlayerSeed);
            }
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
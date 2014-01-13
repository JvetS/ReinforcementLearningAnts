using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Linq;
using YourBot;
using System.Runtime.Serialization.Formatters.Binary;

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
            //TO DO laatse beurt afhandelen
            BinaryFormatter formatter = new BinaryFormatter();
            Stream learnerStream = new FileStream("QData.Q", FileMode.OpenOrCreate);
            formatter.Serialize(learnerStream, Learner);
        }

        public override void Initialise(GameState state)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Stream learnerStream = new FileStream("QData.Q", FileMode.Open);
                Learner = (QLearner)formatter.Deserialize(learnerStream);
            }
            catch
            {
                Learner = new QLearner(0.9f,0.8f,0.9f);
            }
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
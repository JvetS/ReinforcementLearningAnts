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
			foreach (Location enm in state.EnemyAnts)
				Globals.enemyInfluence.AddInfluence(enm, 10.0f);

            foreach (Location enm in state.EnemyHills)
            {
                Globals.enemyInfluence.AddInfluence(enm, 1.0f);
                Globals.hillInfluence.AddInfluence(enm, 15.0f);
            }

			foreach (Location fnd in state.MyAnts)
				Globals.friendlyInfluence.AddInfluence(fnd, 10.0f);

			foreach (Location fnd in state.MyHills)
				Globals.friendlyInfluence.AddInfluence(fnd, 1.0f);

            foreach (Location ded in state.DeadTiles)
                Globals.enemyInfluence.AddInfluence(ded, 10.0f);

            foreach (Location loc in state.FoodTiles)
                Globals.foodInfluence.AddInfluence(loc, 10.0f);

			Globals.enemyInfluence.UpdateInfluence();
			Globals.friendlyInfluence.UpdateInfluence();
            Globals.hillInfluence.UpdateInfluence();
            Globals.foodInfluence.UpdateInfluence();

#if DEBUG
            
            Learner.LearnPolicy(state, false);         
#endif

#if RELEASE
            Learner.ExecutePolicy(state,false);
#endif
        }   

        public override void LastTurn(GameState state, bool won)
        {  
            Learner.LearnPolicy(state, won);
            Learner.PrepareForSerialisation();
            BinaryFormatter formatter = new BinaryFormatter();
            Stream learnerStream = new FileStream(Globals.QLearnerFolder + "\\" + "QData.Q", FileMode.OpenOrCreate);
            formatter.Serialize(learnerStream, Learner);
        }

        public override void Initialise(GameState state)
        {
            Globals.state = state;
            Globals.QLearnerFolder = FindQLearnerFolder();
            Globals.random = new Random(state.PlayerSeed);
            Globals.pathFinder = new Pathfinder(state.Width, state.Height);
			Globals.enemyInfluence = new InfluenceMap(state.map);
			Globals.friendlyInfluence = new InfluenceMap(state.map);
            Globals.hillInfluence = new InfluenceMap(state.map);
            Globals.foodInfluence = new InfluenceMap(state.map);

            string backupFile = Globals.QLearnerFolder + "\\" + "Qbackup.Q";
            string originalFile = Globals.QLearnerFolder + "\\" + "QData.Q";

            if(File.Exists(originalFile))//previously run, there is learning data
            {

                BinaryFormatter formatter = new BinaryFormatter();
                Stream learnerStream = new FileStream(originalFile, FileMode.Open);
                Learner = (QLearner)formatter.Deserialize(learnerStream);
                learnerStream.Close();

                if (File.Exists(backupFile))
                {
                    FileInfo originalInfo = new FileInfo(originalFile);
                    FileInfo backupInfo = new FileInfo(backupFile);

                    if (originalInfo.Length > backupInfo.Length)//the more the bots learn the bigger this should get, if it is suddenly smaller there has been a serialisation error, do not backup
                        System.IO.File.Copy(originalFile, backupFile, true);//in case of deserialisation error, your progress is not lost
                }
                else
                {
                    System.IO.File.Copy(originalFile, backupFile, true);//in case of deserialisation error, your progress is not lost
                }

                Learner.GamesPlayed++;
            }
            else
            {
                Learner = new QLearner(0.9f, 0.8f, 0.9f, state.PlayerSeed);//very first game, no learning done previously
            }
        }

        public static string FindQLearnerFolder()
        {
            bool contains = false;
            string current = Directory.GetCurrentDirectory();
            string parent = Directory.GetParent(current).FullName;
            while (!contains)//go up the file tree until you reach a folder that contains the QLearners folder
            {
                current = parent;
                parent = Directory.GetParent(current).FullName;
                string[] test = Directory.GetDirectories(parent);

                contains = test.Contains<string>(parent + "\\QLearners");
            }

            return parent + "\\QLearners";
        }

        public static void Main(string[] args)
        {
#if DEBUG
           //Debugger.Launch();
#endif
            new Ants().PlayGame(new MyBot());
        }
    }

}
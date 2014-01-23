using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;
using System.IO;    

namespace YourBot
{
    //deze klasse kan q waarden updaten en een policy uitvoeren
    [Serializable()]
    class QLearner
    {
        public float Gamma, Alpha, ExplorationChance;
        private Pair<QAction, QNode> PreviousAction;
        private QNode PreviousNode;
        private Dictionary<int, QNode> Nodes;
        private Random Random;
        private int Seed;
        private int Recognised, New;
        public int GamesPlayed;

        //exploration moet tussen 1 en 0
        public QLearner(float alpha, float gamma, float exploration, int seed)
        {
            Nodes = new Dictionary<int, QNode>();
            Gamma = gamma;
            Alpha = alpha;
            ExplorationChance = exploration;
            Random = new Random(seed);
            Seed = seed;
        }

        public void ExecutePolicy(GameState realState, bool win)
        {
            QState state = new QState(realState);
            int hashCode = state.GetHashCode();

            if (Nodes.ContainsKey(hashCode))
            {
                QNode current = Nodes[hashCode];

                Pair<QAction, QNode> chosenAction = current.BestAction();
                chosenAction.Item1.DoAction(realState, current.GetHashCode());
                PreviousAction = chosenAction;
                PreviousNode = current;
            }
            else
            {
                QNode newNode = InitialiseNewNode(state, realState, win);
                Nodes.Add(state.GetHashCode(), newNode);
                HandleExistingNode(state, realState);
            }
        }

        public void LearnPolicy(GameState realState, bool win)
        {
            QState state = new QState(realState);
            int hashCode = state.GetHashCode();

            //TO DO: hashcode van QState implementeren
            if (Nodes.ContainsKey(hashCode))
            {
                HandleExistingNode(state, realState);
                Recognised++;
            }
            else
            {
                QNode newNode = InitialiseNewNode(state, realState, win);
                Nodes.Add(state.GetHashCode(), newNode);
                HandleExistingNode(state, realState);
                New++;
            }
        }

        //zorgt ervoor dat bij de volgende run alles weer klaar staat voor de eerste state
        public void PrepareForSerialisation()
        {
            PreviousAction = null;
            PreviousNode = null;

            StreamWriter writer = new StreamWriter("Qlog2.txt", true);
            writer.WriteLine("recognised {0}, new {1}",Recognised,New);
            writer.Flush();
            writer.Close();

            Recognised = 0;
            New = 0;
            Random = new Random(Seed);//zodat random gedrag niet afhangt van hoeveel runs er gedaan zijn
        }

        private void HandleExistingNode(QState state, GameState realState)
        {
            QNode current = Nodes[state.GetHashCode()];

            if (PreviousAction != null && PreviousNode!= null && PreviousAction.Item2 == null)
            {
                PreviousAction.Item2 = current;

                float maxQ = current.MaxQValue();
                float reward = current.GetReward - PreviousNode.GetReward;

                PreviousAction.Item1.QValue = (1 - Alpha) * PreviousAction.Item1.QValue + Alpha * (reward + maxQ);
            }

            List<Pair<QAction, QNode>> actions = current.Actions;

            if (Random.NextDouble() < ExplorationChance)
            {
                Pair<QAction, QNode> chosenAction = actions[Random.Next(0, actions.Count)];

                if (chosenAction.Item2 != null)
                {
                    float maxQ = chosenAction.Item2.MaxQValue();
                    float reward = chosenAction.Item2.GetReward - state.GetReward;
                    chosenAction.Item1.QValue = (1 - Alpha) * chosenAction.Item1.QValue + Alpha * (reward + maxQ);//Q(s,a) = (1-a) * Q(s,a) + a(r+g max[Q(s', a')]
                }

                chosenAction.Item1.DoAction(realState, current.GetHashCode());
                PreviousAction = chosenAction;
            }
            else
            {
                Pair<QAction, QNode> chosenAction = current.BestAction();
                chosenAction.Item1.DoAction(realState, current.GetHashCode());
                PreviousAction = chosenAction;
            }

            PreviousNode = current;
        }

        private QNode InitialiseNewNode(QState state, GameState realState, bool win)
        {
            //to do, actions aan node toevoegen
            List<Pair<QAction, QNode>> actions = new List<Pair<QAction, QNode>>();

            QAction foodAction = new MoveAllToFood();
            if (foodAction.Apllicable(realState))
            {
                actions.Add(new Pair<QAction, QNode>(foodAction, null));
            }

            QAction exploreAction = new AllExplore();
            if (exploreAction.Apllicable(realState))
            {
                actions.Add(new Pair<QAction, QNode>(exploreAction, null));
            }

            QAction attackAction = new Attack();
            if (attackAction.Apllicable(realState))
            {
                actions.Add(new Pair<QAction, QNode>(attackAction, null));
            }

            QAction razeAction = new AnyRaze();
            if (razeAction.Apllicable(realState))
            {
                actions.Add(new Pair<QAction, QNode>(razeAction, null));
            }
            
            QNode newNode = new QNode(state, actions, win);

            return newNode;
        }

        public void PrintData()
        {
            StreamWriter writer = new StreamWriter("Qlog.txt");

            foreach (QNode node in Nodes.Values)
            {
                writer.WriteLine(node.PrintData());
                writer.WriteLine(" ");
            }

            writer.Flush();
            writer.Close();
        }
    }

    //deze classe vult de rol van één vak in de grid met alle pijlen naar andere states (zie slide 30 van reinforcement learning)
    [Serializable()]
    class QNode
    {
        public float Reward { get; private set; }
        public int HashCode;
        public List<Pair<QAction, QNode>> Actions { get; private set; }
        public bool Win;

        //Q(s,a) s is de QState, de lijst bevat de actions (a) gekoppeld aan de QNode waar de action je heen brengt
        public QNode(QState state, List<Pair<QAction, QNode>> actions, bool win)
        {
            HashCode = state.GetHashCode();
            Reward = state.GetReward;
            Actions = actions;
            Win = win;
        }

        public float GetReward
        {
            get { if (!Win) 
                    return Reward; 
                  else 
                    return int.MaxValue; 
            }//misschien wat extreem;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public float MaxQValue()
        {
            float maxQ = int.MinValue;
            foreach (Pair<QAction, QNode> action in Actions)
            {
                if (action.Item1.QValue > maxQ)
                    maxQ = action.Item1.QValue;
            }

            return maxQ;
        }

        public Pair<QAction, QNode> BestAction()
        {
            float maxQ = int.MinValue;
            Pair<QAction, QNode> maxAction = null;
            foreach (Pair<QAction, QNode> action in Actions)
            {
                if (action.Item1.QValue > maxQ)
                {
                    maxQ = action.Item1.QValue;
                    maxAction = action;
                }
            }

            return maxAction;
        }

        public string PrintData()
        {
            string result = "";
            foreach(Pair<QAction,QNode> pair in Actions)
            {
                result += pair.Item1.ID + " " + pair.Item1.QValue + " " + "\n";
            }

            return result;
        }
    }

    [Serializable()]
    class Pair<T, Y>
    {
        public T Item1;
        public Y Item2;
        public Pair(T item1, Y item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}

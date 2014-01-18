using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

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

        public void LearnPolicy(GameState realState, bool win)
        {
            QState state = new QState(realState);
            int hashCode = state.GetHashCode();

            //TO DO: hashcode van QState implementeren
            if (Nodes.ContainsKey(hashCode))
            {
                HandleExistingNode(state, realState);
            }
            else
            {
                QNode newNode = InitialiseNewNode(state, realState, win);
                Nodes.Add(state.GetHashCode(), newNode);
                HandleExistingNode(state, realState);
            }
        }

        //zorgt ervoor dat bij de volgende run alles weer klaar staat voor de eerste state
        public void PrepareForSerialisation()
        {
            PreviousAction = null;
            PreviousNode = null;
            Random = new Random(Seed);//zodat random gedrag niet afhangt van hoeveel runs er gedaan zijn
        }

        private void HandleExistingNode(QState state, GameState realState)
        {
            QNode current = Nodes[state.GetHashCode()];

            if (PreviousAction != null && PreviousNode!= null && PreviousAction.Item2 == null)
            {
                PreviousAction.Item2 = current;
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

                chosenAction.Item1.DoAction(realState, current.State);
                PreviousAction = chosenAction;
            }
            else
            {
                Pair<QAction, QNode> chosenAction = current.BestAction();
                chosenAction.Item1.DoAction(realState, current.State);
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


            QNode newNode = new QNode(state, actions, win);

            return newNode;
        }


    }

    //deze classe vult de rol van één vak in de grid met alle pijlen naar andere states (zie slide 30 van reinforcement learning)
    [Serializable()]
    class QNode
    {
        public QState State { get; private set; }
        public List<Pair<QAction, QNode>> Actions { get; private set; }
        public bool Win;

        //Q(s,a) s is de QState, de lijst bevat de actions (a) gekoppeld aan de QNode waar de action je heen brengt
        public QNode(QState state, List<Pair<QAction, QNode>> actions, bool win)
        {
            State = state;
            Actions = actions;
            Win = win;
        }

        public float GetReward
        {
            get { if (!Win) 
                    return State.GetReward; 
                  else 
                    return int.MaxValue; 
            }//misschien wat extreem;
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

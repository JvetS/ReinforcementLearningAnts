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
        private Dictionary<QState, QNode> Nodes;

        //exploration moet tussen 1 en 0
        public QLearner(float alpha, float gamma, float exploration)
        {
            Nodes = new Dictionary<QState, QNode>();
            Gamma = gamma;
            Alpha = alpha;
            ExplorationChance = exploration;
        }

        public void LearnPolicy(GameState realState)
        {
            QState state = new QState(realState);

            //TO DO: hashcode van QState implementeren
            if (Nodes.ContainsKey(state))
            {
                HandleExistingNode(state, realState);   
            }
            else
            {
                //TO DO: QNode initialiseren met actions en state
            }
        }

        private void HandleExistingNode(QState state, GameState realState)
        {
            QNode current = Nodes[state];

            
            if (PreviousAction.Item2 == null)
            {
                PreviousAction.Item2 = current;
            }

            List<Pair<QAction, QNode>> actions = current.Actions;

            Random random = new Random();

            if (random.NextDouble() < ExplorationChance)
            {
                Pair<QAction, QNode> chosenAction = actions[random.Next(0, 4)];

                if (chosenAction.Item2 != null)
                {
                    float maxQ = chosenAction.Item2.MaxQValue();
                    float reward = chosenAction.Item2.GetReward - state.GetReward;
                    chosenAction.Item1.QValue = (1 - Alpha) * chosenAction.Item1.QValue + Alpha * (reward + maxQ);//Q(s,a) = (1-a) * Q(s,a) + a(r+g max[Q(s', a')]
                }

                chosenAction.Item1.DoAction(realState);
                PreviousAction = chosenAction;
            }
            else
            {
                Pair<QAction, QNode> chosenAction = current.BestAction();
                chosenAction.Item1.DoAction(realState);
                PreviousAction = chosenAction;
            }

            PreviousNode = current;
        }

        private void InitialiseNewNode(QState state, GameState realState)
        {
            //to do, actions aan node toevoegen
            QNode newNode = new QNode(state, null);

            throw new NotImplementedException();
        }


    }

    //deze classe vult de rol van één vak in de grid met alle pijlen naar andere states (zie slide 30 van reinforcement learning)
    [Serializable()]
    class QNode
    {
        QState State;
        public List<Pair<QAction, QNode>> Actions { get; private set; }

        //Q(s,a) s is de QState, de lijst bevat de actions (a) gekoppeld aan de QNode waar de action je heen brengt
        public QNode(QState state, List<Pair<QAction,QNode>> actions)
        {
            State = state;
            Actions = actions;
        }

        public float GetReward
        {
            get { return State.GetReward; }
        }

        public float MaxQValue()
        {
            float maxQ = int.MinValue;
            foreach (Pair<QAction,QNode> action in Actions)
            {
                if (action.Item1.QValue > maxQ)
                    maxQ = action.Item1.QValue;
            }

            return maxQ;
        }

        public Pair<QAction,QNode> BestAction()
        {
            float maxQ = int.MinValue;
            Pair<QAction,QNode> maxAction = null;
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

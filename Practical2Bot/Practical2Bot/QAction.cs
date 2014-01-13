using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    [Serializable()]
    abstract class QAction
    {
        public string ID { get; private set;}
        public float QValue;

        public QAction(string id, float q)
        {
            ID = id;
            QValue = q;
        }

        public abstract void DoAction(GameState state);

    }
}

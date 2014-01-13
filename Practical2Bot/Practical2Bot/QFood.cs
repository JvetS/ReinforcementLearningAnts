using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants;

namespace YourBot
{
    class QFood : QAction
    {
        public QFood(string id = "food", int startQ = 0)
            :base(id,startQ)
        { }

        public override void DoAction(GameState state)
        {
            
        }
    }
}

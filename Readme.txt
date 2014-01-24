Jan van Dieren (3861171)
Steven van Rossum (3584518)
Job de Jong (3617610)

How to train the bot:
Build the bot in debug mode to have it learn a policy. Once a game starts the bot will look for the file QData.Q in the QLearners map, it will make one if nothing is found.
This Qdata.Q file will be used to serialise and deserialise our QLearner class.

The bot will also look for QBackup.Q in the same folder. Before a game starts it will copy the existing QData.Q over to this file to recover from any serialisation erorrs that may occur.

We will include some QData files we made, just change the name from QData<map>.Q to QData.Q and run it on the specified map.

We have included our own play_many_games.cmd (paths may not be right for you) which we used to train, it will automatically run a specified amount of games. Look for the --rounds option. 
this cmd file will not open a browser.

How to let the bot play a real game
Build the bot in release mode to have it execute the learned policy. It will also look for the QData.Q and QBackup.Q files in the QLearner folder. 
Using some of our QData files can be done the same way as above.

We have included the play_one_game.cmd file we use to let our bot play, this one will open a browser.

Debugging:
we have commented out the debugger.launch line in MyBot.cs line 131. you can uncomment it if you want.

what to find where:
all classes have their own .cs file except for
QNode -> found in QLearner.cs under the QLearner class
Pair -> found in QLearner.cs under the QNode class

Method that implements the learning formulas
QLearner.HandleExistingNode. QLearner .cs line 95

backwards q learning formula -> QLearner.cs line 106
regular q learning formula -> QLearner.cs line 119
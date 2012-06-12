using System;
using FervorLibrary;
using ObsidianFunctions;
using AIMLbot;

namespace CodeCompile
{
class Program
{

static void Main(string[] args)
{
AIMLbot.Bot chatBot;
AIMLbot.User chatUser;
string channel = args[0];
string rnick = args[1];
string rmsg = args[2];
try
{
    string query = rmsg.Remove(0, 9);
        ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
        chatBot = new AIMLbot.Bot();
        chatBot.loadSettings();
        chatUser = new AIMLbot.User(rnick, chatBot);
        chatBot.loadAIMLFromFiles();
        chatBot.isAcceptingUserInput = true;
        AIMLbot.Request r = new AIMLbot.Request(query, chatUser, chatBot);
        AIMLbot.Result res = chatBot.Chat(r);
        Console.WriteLine("PRIVMSG " + rnick + " :" + res.Output);
}
catch (Exception ex)
{
    Console.WriteLine("PRIVMSG " + rnick + " :" + ex.ToString()); 
}
}
}
}
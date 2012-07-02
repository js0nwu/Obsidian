using System;
using FervorLibrary;
using ObsidianFunctions;
using System.Collections.Generic; 

namespace CodeCompile
{
class Program
{
static void Main(string[] args)
{
string channel = args[0];
string rnick = args[1];
string rmsg = args[2];
try
{
string sender = rnick;
string query = rmsg.Remove(0, 9);
string[] parsenick = query.Split('>');
string recipient = parsenick[0];
string message = parsenick[1].Replace("~", "");
ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
bool nickonline = ObsidFunc.isOnline(recipient);
if (nickonline == true)
{
Console.WriteLine("PRIVMSG " + recipient + " :" + message);
}
else if (nickonline == false)
{
Console.WriteLine("PRIVMSG " + sender + " :" + ObsidFunc.addMessage(sender, recipient, message));
}
}
catch (Exception ex)
{
Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
}
}
}
}
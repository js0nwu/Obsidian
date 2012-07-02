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
//try
//{
ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
bool nickuser = ObsidFunc.isActiveUser(rnick);
if (nickuser == true)
{
System.IO.StreamReader sr = new System.IO.StreamReader("messages.bin");
string messageread = sr.ReadToEnd();
sr.Close();
Console.WriteLine("PRIVMSG " + rnick + " :" + messageread.Trim()); 
}
else
{
Console.WriteLine("PRIVMSG " + channel + " :Insufficient permissions!");
}
//}
//catch (Exception ex)
//{
//Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
//}
}
}
}
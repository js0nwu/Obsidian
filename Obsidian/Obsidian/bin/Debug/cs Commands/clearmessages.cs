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
ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
bool nickuser = ObsidFunc.isActiveUser(rnick); 
if (nickuser == true)
{
if (System.IO.File.Exists("messages.bin") == true)
{
System.IO.File.Delete("messages.bin"); 
System.IO.StreamWriter sw = new System.IO.StreamWriter("messages.bin");
sw.Write("|");
sw.Close();
Console.WriteLine("PRIVMSG " + channel + " :Messages cleared!"); 
}
else
{
Console.WriteLine("PRIVMSG " + channel + " :Message file doesn't exist!"); 
}
}
else
{
Console.WriteLine("PRIVMSG " + channel + " :Insufficient permissions!"); 
}
}
catch (Exception ex)
{
Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
}
}
}
}
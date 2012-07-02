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
Console.WriteLine("NAMES " + channel); 
}
catch (Exception ex)
{
Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
}
}
}
}
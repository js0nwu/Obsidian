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
    string query = rmsg.Remove(0, 14);
    string[] blacklist = ObsidFunc.addBlacklist(query);
    string newlist = String.Join(":", blacklist);
    Console.WriteLine("PRIVMSG " + channel + " :" + newlist);
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
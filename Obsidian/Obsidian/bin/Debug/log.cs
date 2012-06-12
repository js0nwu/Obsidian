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
				string query = rmsg.Remove(0, 5);
				if (query == "start")
				{
				ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
				bool nickuser = ObsidFunc.isActiveUser(rnick);
				if (nickuser == true)
				{
					ObsidFunc.logTrue();
					Console.WriteLine("PRIVMSG " + channel + " :Log = true");
				}
				else
				{
					Console.WriteLine("PRIVMSG " + channel + " :Insufficient permissions!");
				}
				}
				else if (query == "stop")
				{
				ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
				bool nickuser = ObsidFunc.isActiveUser(rnick);
				if (nickuser == true)
				{
				ObsidFunc.logFalse();
				Console.WriteLine("PRIVMSG " + channel + " :Log = false");
				}
				else
				{
				Console.WriteLine("PRIVMSG " + channel + " :Insufficient permissions!"); 
				}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
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
                            if (rnick == ObsidFunc.ownernick() && ObsidFunc.isActiveUser(rnick) == true)
                            {
                                string query = rmsg.Remove(0, 9).Trim();
                                string[] qSplit = query.Split(' ');
                                string file = qSplit[0].Remove(0, 1) + ".exe"; 
                                Console.WriteLine(ObsidFunc.exeExec(file, channel, rnick, query));
                            }
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
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
				string query = rmsg.Remove(0, 7);
                            bool nickuser = ObsidFunc.isActiveUser(rnick);
                            if (rnick == ObsidFunc.ownernick() && nickuser == true)
                            {
                                ObsidFunc.batch(query);
                                Console.WriteLine("PRIVMSG " + channel + " :Success!");
                            }
                            else
                            {
                                Console.WriteLine("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
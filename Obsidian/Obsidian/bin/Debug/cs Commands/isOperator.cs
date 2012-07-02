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
			ObsidianFunctions.Functions Obsid = new ObsidianFunctions.Functions();
            try
			{
			string query = rmsg.Remove(0, 12);
			if (query == "true")
			{
			
			bool nickuser = Obsid.isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                ObsidFunc.opTrue();
                                Console.WriteLine("PRIVMSG " + channel + " :isOperator = true");
                            }
                            else
                            {
                                Console.WriteLine("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
			}
			else if (query == "false")
			{
			bool nickuser = Obsid.isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                ObsidFunc.opFalse();
                                Console.WriteLine("PRIVMSG " + channel + " :isOperator = false");
                            }
                            else
                            {
                                Console.WriteLine("PRIVMSG " + channel + " :Insufficient Permissions!");
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
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
					FervorLibrary.Library FervLib = new FervorLibrary.Library(); 
					string query = rmsg.Remove(0, 10); 
					if (query == "true")
					{
						FervLib.greetTrue();
						Console.WriteLine("PRIVMSG " + channel + " :Success!");
					}
					else if (query == "false")
					{
						FervLib.greetFalse();
						Console.WriteLine("PRIVMSG " + channel + " :Success!");
					}
					else
					{
						Console.WriteLine("PRIVMSG " + channel + " :Invalid Input");
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
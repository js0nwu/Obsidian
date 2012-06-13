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
				string query = rmsg.Remove(0, 13);
				if (nickuser == true)
				{
				
					if (query.Contains("true"))
					{
						ObsidFunc.spamTrue();
						Console.WriteLine("PRIVMSG " + channel + " :spamControl = true"); 
					}
					else if (query.Contains("false"))
					{
						ObsidFunc.spamFalse();
						Console.WriteLine("PRIVMSG " + channel + " :spamControl = false");
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
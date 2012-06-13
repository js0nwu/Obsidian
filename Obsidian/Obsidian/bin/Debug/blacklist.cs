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
string stringblacklist = String.Join(":", ObsidFunc.blacklist());
if (stringblacklist == "" || stringblacklist == null)
{
stringblacklist = "None";
}
Console.WriteLine("PRIVMSG " + channel + " :" + stringblacklist); 
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
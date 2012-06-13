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
			string query = rmsg.Remove(0, 8);
ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
string hash = ObsidFunc.sha256calc(query);
Console.WriteLine("PRIVMSG " + channel + " :" + hash);
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
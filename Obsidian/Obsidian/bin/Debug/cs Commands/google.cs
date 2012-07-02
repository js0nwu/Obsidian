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
				string query = rmsg.Remove(0, 8).Replace(" ", "+");
				string googleURL = "http://google.com/search?q=" + query;
				Console.WriteLine("PRIVMSG " + channel + " :" + googleURL); 
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
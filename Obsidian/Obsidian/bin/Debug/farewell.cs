using System;
using FervorLibrary;
using ObsidianFunctions;

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
				string query = rmsg.Remove(0, 10);
				int queryindex = Int32.Parse(query);
				FervorLibrary.Library Farewell = new FervorLibrary.Library();
				string returnfarewell = Farewell.farewell(queryindex);
				string response = "PRIVMSG " + channel + " :" + returnfarewell;
				Console.WriteLine(response);
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
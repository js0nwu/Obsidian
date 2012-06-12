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
				string query = rmsg.Remove(0, 5);
				ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
				string md5encrypt = ObsidFunc.md5calc(query.ToString()).ToLower();
				string response = "PRIVMSG " + channel + " :" + md5encrypt;
				Console.WriteLine(response);
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
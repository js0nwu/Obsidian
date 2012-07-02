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
				string query = rmsg.Remove(0, 9);
				ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
				string definition = ObsidFunc.uDefine(query); 
				Console.WriteLine("PRIVMSG " + channel + " :" + definition); 
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
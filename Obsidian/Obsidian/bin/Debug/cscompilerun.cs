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
string csfile = args[3];
            try
			{
			ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
Console.WriteLine(ObsidFunc.CSCompileRun(command, channel, rnick, rmsg)); 
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}
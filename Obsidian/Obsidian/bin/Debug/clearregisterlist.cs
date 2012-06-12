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
				ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
				bool nickuser = ObsidFunc.isActiveUser(rnick);
				if (nickuser == true)
				{
					System.IO.File.Delete("registers.bin");
					System.IO.StreamWriter sw = new System.IO.StreamWriter("registers.bin");
					sw.Write("|");
					sw.Close();
					Console.WriteLine("PRIVMSG " + channel + " :Cleared!");
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
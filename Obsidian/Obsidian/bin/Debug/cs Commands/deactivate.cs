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
                System.IO.StreamReader sr = new System.IO.StreamReader(".activeusers");
                string[] temp = sr.ReadToEnd().Split(':');
                List<string> activelist = new List<string>(temp);
                sr.Close();
                foreach (string x in activelist)
                {
                    if (x == rnick)
                    {
                        string[] listofactiveusers = activelist.ToArray();
                        string listuser = String.Join(":", listofactiveusers) + ":";
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(".activeusers");
                        sw.Write(listuser);
                        sw.Close();
                        Console.Write("PRIVMSG " + rnick + " :User has been deactivated!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString());
            }
        }
    }
}
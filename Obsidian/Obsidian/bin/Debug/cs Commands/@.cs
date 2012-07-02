using System;
using FervorLibrary;
using ObsidianFunctions;
using System.Collections.Generic; 
using System.Diagnostics; 
using System.Text.RegularExpressions; 

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
string query = rmsg.Remove(0, 1); 
Process apriProcess = new Process();
apriProcess.StartInfo.FileName = "apricmd.exe";
apriProcess.StartInfo.Arguments = channel + " " + rnick + " " + "\"" + query + "\"";
apriProcess.StartInfo.UseShellExecute = false;
apriProcess.StartInfo.RedirectStandardOutput = true; 
apriProcess.StartInfo.CreateNoWindow = true;
apriProcess.Start();
apriProcess.WaitForExit(); 
string output = apriProcess.StandardOutput.ReadToEnd(); 
string[] splitLine = Regex.Split(output, "\n");
foreach (string x in splitLine)
{
Console.WriteLine("PRIVMSG " + channel + " :" + x); 
}
}
catch (Exception ex)
{
Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
}
}
}
}
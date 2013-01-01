/*
 * Created by SharpDevelop.
 * User: pilly
 * Date: 1/1/2013
 * Time: 1:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;
using ObsidianFunctions; 
namespace Obsidian_batchexec
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
	string query = rmsg.Remove(0, 11).Replace("\0", "").Trim();
	bool nickuser = ObsidFunc.isActiveUser(rnick); 
	if (rnick == ObsidFunc.ownernick() && nickuser == true)
	{
		string[] findFind = Regex.Split(query, "find(");
		string findStrings = findFind[1].Split(')')[0];
		string[] find = findStrings.Split(';');
		string[] replaceFind = Regex.Split(query, "replace(");
		string replaceStrings = replaceFind[1].Split(')')[0];
		string[] replace = replaceStrings.Split(';');
		string[] batchFind = Regex.Split(query, "batch(");
		string batchStrings = batchFind[1].Split(')')[0];
		Console.WriteLine("PRIVMSG " + channel + " :" + ObsidFunc.batchExec(batchStrings, find, replace));
		
	}
	else
	{
		Console.WriteLine("PRIVMSG " + channel + " :Insufficient Permissions!");
	}
}
catch (Exception ex)
{
	Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString());
}
}
}
}
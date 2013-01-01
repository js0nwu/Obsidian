using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace ObsidianFunctions
{
    public class Functions
    {
        public string addMessage(string sender, string recipient, string message)
        {
            try
            {
                message += "<" + sender + ">"; 
                StreamReader sr = new StreamReader("messages.bin");
                string[] messageread = sr.ReadToEnd().Split('|');
                sr.Close();
                string newrecipients = messageread[0] + recipient.Trim().ToLower() + "~";
                string newmessages = messageread[1] + message.Trim() + "~";
                StreamWriter sw = new StreamWriter("messages.bin");
                sw.Write(newrecipients + "|" + newmessages);
                sw.Close();
                return "Message added!"; 
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string[] sayMessages(string recipient)
        {
                StreamReader sr = new StreamReader("messages.bin");
                string[] messageread = sr.ReadToEnd().Split('|');
                sr.Close();
                string[] messageusers = messageread[0].Split('~');
                string[] messages = messageread[1].Split('~');
                List<string> returnList = new List<string>();
                foreach (string x in messageusers)
                {
                    if (x.ToLower() == recipient.ToLower())
                    {
                        int recipindex = Array.IndexOf(messageusers, recipient.ToLower());
                        string returnstring = "PRIVMSG " + recipient.ToLower() + " :" + messages[recipindex];
                        returnList.Add(returnstring);
                        messages = messages.Where(val => val != messages[recipindex]).ToArray();
                    }
                }
                messageusers = messageusers.Where(val => val != recipient).ToArray();
                string newuserlist = String.Join("~", messageusers);
                string newmessagelist = String.Join("~", messages);
                StreamWriter sw = new StreamWriter("messages.bin");
                sw.Write(newuserlist + "|" + newmessagelist);
                sw.Close();
                return returnList.ToArray();
        }
        public bool hasMessages(string nickname)
        {
            StreamReader sr = new StreamReader("messages.bin");
            string[] messageread = sr.ReadToEnd().Split('|');
            sr.Close();
            string[] messageusers = messageread[0].Split('~');
            bool isRecipient = false;
            foreach (string x in messageusers)
            {
                if (x.ToLower() == nickname.ToLower())
                {
                    isRecipient = true; 
                }
            }
            return isRecipient; 
        }
        public string setNames(string message)
        {
            StreamWriter sw = new StreamWriter("channelnames.bin");
            sw.Write(String.Join(":", message.Trim().Replace("@", "").Replace("+", "").Replace("~", "").Replace("&", "").Replace("%", "").Split(' ')));
            sw.Close();
            return String.Join(":", message.Trim().Replace("@", "").Replace("+", "").Replace("~", "").Replace("&", "").Replace("%", "").Split(' '));
        }
        public bool isOnline(string rnick)
        {
            StreamReader sr = new StreamReader("channelnames.bin");
            string[] srread = sr.ReadToEnd().Split(':');
            sr.Close();
            bool nickone = false; 
            foreach (string x in srread)
            {
                if (x.ToLower() == rnick.ToLower())
                {
                    nickone = true; 
                }
            }
            return nickone; 
        }
        public string channelNames()
        {
            if (File.Exists("channelnames.bin"))
            {
                StreamReader sr = new StreamReader("channelnames.bin");
                string channelnames = sr.ReadToEnd();
                sr.Close();
                return channelnames;
            }
            else
            {
                return "Channel names file does not exist!"; 
            }
        }
        public bool controlSpam()
        {
            if (File.Exists("spam"))
            {
                return true;
            }
            else
            {
                return false; 
            }
        }
        public void spamTrue()
        {
            StreamWriter sw = new StreamWriter("spam");
            sw.Close(); 
        }
        public void spamFalse()
        {
            if (File.Exists("spam"))
            {
                File.Delete("spam");
            }
        }
        public bool isOperator()
        {
            if (System.IO.File.Exists("ops"))
            {
                return true;
            }
            else
            {
                return false; 
            }
        }
        public void settalkingTo(string nickname)
        {
            StreamWriter sw = new StreamWriter("talkingTo.bin");
            sw.Write(nickname);
            sw.Close();
        }
        public string talkingTo()
        {
            if (System.IO.File.Exists("talkingTo.bin"))
            {
                StreamReader sr = new StreamReader("talkingTo.bin");
                string name = sr.ReadToEnd().Trim();
                sr.Close();
                return name;
            }
            else
            {
                return "nobody";
            }
        }
        public void cleartalkingTo()
        {
            if (File.Exists("talkingTo.bin"))
            {
                File.Delete("talkingTo.bin");
            }
        }
        public string exeExec(string filename, string channel, string rnick, string rmsg)
        {
            try
            {
                Process exeProcess = new Process();
                exeProcess.StartInfo.FileName = filename;
                exeProcess.StartInfo.Arguments = channel + " " + rnick + " " + "\"" + rmsg + "\"" ;
                exeProcess.StartInfo.UseShellExecute = false;
                exeProcess.StartInfo.RedirectStandardOutput = true;
                exeProcess.StartInfo.CreateNoWindow = true; 
                exeProcess.Start();
                exeProcess.WaitForExit();
                return exeProcess.StandardOutput.ReadToEnd().Trim() ;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string classExec(string filename, string channel, string rnick, string rmsg)
        {
            try
            {
                Process javaProcess = new Process();
                javaProcess.StartInfo.FileName = "ikvm.exe";
                javaProcess.StartInfo.Arguments = filename + " " + channel + " " + rnick + " " + "\"" + rmsg + "\"";
                javaProcess.StartInfo.UseShellExecute = false;
                javaProcess.StartInfo.RedirectStandardOutput = true;
                javaProcess.StartInfo.CreateNoWindow = true; 
                javaProcess.Start();
                javaProcess.WaitForExit();
                return javaProcess.StandardOutput.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }
        public string jarExec(string filename, string channel, string rnick, string rmsg)
        {
            try
            {
                Process javaProcess = new Process();
                javaProcess.StartInfo.FileName = "ikvm.exe";
                javaProcess.StartInfo.Arguments = "-jar " + filename + " " + channel + " " + rnick + " " + "\"" + rmsg + "\"";
                javaProcess.StartInfo.UseShellExecute = false;
                javaProcess.StartInfo.RedirectStandardOutput = true;
                javaProcess.StartInfo.CreateNoWindow = true; 
                javaProcess.Start();
                javaProcess.WaitForExit();
                return javaProcess.StandardOutput.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public void opTrue()
        {
            StreamWriter sw = new StreamWriter("ops");
            sw.Close(); 
        }
        public void opFalse()
        {
            if (System.IO.File.Exists("ops"))
            {
                System.IO.File.Delete("ops");
            }
        }
        public bool isLogging()
        {
            if (System.IO.File.Exists("log"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void logTrue()
        {
            StreamWriter sw = new StreamWriter("log");
            sw.Close();
        }
        public void logFalse()
        {
            if (System.IO.File.Exists("log"))
            {
                System.IO.File.Delete("log");
            }
        }
        public string ownernick()
        {
            StreamReader sr = new StreamReader("owner.bin");
            string owner = sr.ReadToEnd().Trim();
            sr.Close();
            return owner; 
        }
        public string md5calc(string input)
        {
            input = input.Replace("\0", "").Trim();
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
        public string sha1calc(string input)
        {
            input = input.Replace("\0", "").Trim();
            SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = sha1.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString().ToLower(); 
        }
        public string sha256calc(string input)
        {
            input = input.Replace("\0", "").Trim();
            SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = sha256.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
        public string sha384calc(string input)
        {
            input = input.Replace("\0", "").Trim();
            SHA384 sha384 = SHA384.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = sha384.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
        public string sha512calc(string input)
        {
            input = input.Replace("\0", "").Trim();
            SHA512 sha512 = SHA512.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = sha512.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
        public string reqreguser(string nickname, string password)
        {
            nickname = nickname.Replace("\0", "").Trim();
            password = password.Replace("\0", "").Trim();
            System.IO.StreamReader sr = new System.IO.StreamReader("registers.bin");
            string[] old = sr.ReadToEnd().Split('|');
            sr.Close();
            string users = old[0];
            string pass = old[1];
            users = users + nickname + ":";
            pass = pass + md5calc(password) + ":";
            System.IO.StreamWriter sw = new System.IO.StreamWriter("registers.bin");
            sw.Write(users + "|" + pass);
            sw.Close();
            return md5calc(password);
        }
        public bool isVerified(int index, string password)
        {
            password = password.Replace("\0", "").Trim();
            password = md5calc(password);
            bool iscorrect;
            StreamReader sr = new StreamReader("passwords.bin");
            string[] allpass = sr.ReadToEnd().Split(':');
            sr.Close();
            string verifypass = allpass[index];
            if (verifypass == password)
            {
                iscorrect = true;
            }
            else
            {
                iscorrect = false; 
            }
            return iscorrect;
        }
        public string addUser(string nickname)
        {
            nickname = nickname.Replace("\0", "").Trim();
            StreamReader sr = new StreamReader("registers.bin");
            string[] registereduserlist = sr.ReadToEnd().Split('|');
            string[] registeredusers = registereduserlist[0].Split(':');
            string[] registeredpasswords = registereduserlist[1].Split(':');
            sr.Close();
            StreamReader sru = new StreamReader("users.bin");
            StreamReader srp = new StreamReader("passwords.bin");
            string oldusers = sru.ReadToEnd();
            string oldpasswords = srp.ReadToEnd();
            sru.Close();
            srp.Close();
            
            foreach (string x in registeredusers)
            {
                if (x.Contains(nickname))
                {
                    int userregIndex = Array.IndexOf(registeredusers, nickname);
                    StreamWriter sw = new StreamWriter("users.bin");
                    sw.Write(oldusers + registeredusers[userregIndex] + ":");
                    StreamWriter sw2 = new StreamWriter("passwords.bin");
                    sw2.Write(oldpasswords + registeredpasswords[userregIndex] + ":");
                    sw.Close();
                    sw2.Close();
                    registeredusers = registeredusers.Where(val => val != nickname).ToArray();
                    registeredpasswords = registeredpasswords.Where(val => val != registeredpasswords[userregIndex]).ToArray();
                    string newuserlist = String.Join(":", registeredusers);
                    string newpasslist = string.Join(":", registeredpasswords);
                    StreamWriter newwrite = new StreamWriter("registers.bin");
                    newwrite.Write(newuserlist + "|" + newpasslist);
                    newwrite.Close();
                }

            }
            StreamReader sr3 = new StreamReader("users.bin");
            string newregusers = sr3.ReadToEnd();
            sr3.Close();
            return newregusers;
        }
        public string[] addBlacklist(string nickname)
        {
            nickname = nickname.Replace("\0", "").Trim();
            StreamReader sr = new StreamReader("blacklist.bin");
            string blacklist = sr.ReadToEnd();
            sr.Close();
            StreamWriter sw = new StreamWriter("blacklist.bin");
            sw.Write(blacklist + nickname + ":");
            sw.Close();
            StreamReader sr2 = new StreamReader("blacklist.bin");
            string[] newblacklist = sr2.ReadToEnd().Split(':');
            sr2.Close();
            return newblacklist;
        }
        public string[] removeBlacklist(string nickname)
        {
            string[] returnvalue = {"None"};
            nickname = nickname.Replace("\0", "").Trim();
            StreamReader sr = new StreamReader("blacklist.bin");
            string blacklist = sr.ReadToEnd();
            sr.Close();
            if (blacklist != null)
            {
                string[] blacklistedusers = blacklist.Split(':');
                foreach (string x in blacklistedusers)
                {
                    if (x.Contains(nickname))
                    {
                        blacklistedusers = blacklistedusers.Where(val => val != nickname).ToArray();
                        string newblacklist = String.Join(":", blacklistedusers);
                        StreamWriter sw5 = new StreamWriter("blacklist.bin");
                        sw5.Write(newblacklist);
                        sw5.Close();
                        StreamReader sr5 = new StreamReader("blacklist.bin");
                        returnvalue = sr5.ReadToEnd().Split(':');
                        sr5.Close();
                    }
                }
            }
            return returnvalue; 
        }
        public string[] blacklist()
        {
            StreamReader sr = new StreamReader("blacklist.bin");
            string[] returnvalue = sr.ReadToEnd().Split(':');
            sr.Close();
            return returnvalue; 
        }
        public string removeUser(string nickname)
        {
            nickname = nickname.Replace("\0", "").Trim();
            StreamReader sr = new StreamReader("users.bin");
            string[] userlist = sr.ReadToEnd().Split(':');
            sr.Close();
            StreamReader sr2 = new StreamReader("passwords.bin");
            string[] passwordlist = sr2.ReadToEnd().Split(':');
            sr2.Close();
            foreach (string x in userlist)
            {
                if (x.Contains(nickname))
                {
                    int userregIndex = Array.IndexOf(userlist, nickname);
                    userlist = userlist.Where(val => val != nickname).ToArray();
                    passwordlist = passwordlist.Where(val => val != passwordlist[userregIndex]).ToArray();
                    string newuserlist = String.Join(":", userlist);
                    string newpasslist = string.Join(":", passwordlist);
                    StreamWriter newsw1 = new StreamWriter("users.bin");
                    newsw1.Write(newuserlist);
                    newsw1.Close();
                    StreamWriter newsw2 = new StreamWriter("passwords.bin");
                    newsw2.Write(newpasslist);
                    newsw2.Close();
                    
                }
            }
            StreamReader newuserread = new StreamReader("users.bin");
            string updateuser = newuserread.ReadToEnd();
            newuserread.Close();
            return updateuser;
        }
        public void batch(string command)
        {
            StreamWriter sw = new StreamWriter("command.bat");
            sw.Write(command);
            sw.Close();
            System.Diagnostics.Process.Start("command.bat");
        }
        public string userlist()
        {
            StreamReader sr = new StreamReader("users.bin");
            string userlist = sr.ReadLine();
            sr.Close();
            if (userlist == "" || userlist == null)
            {
                userlist = "None";
            }
            return userlist;
        }
        public void logMsg(string message)
        {
            if (System.IO.File.Exists("log.bin") == false)
            {
                StreamWriter sw = new StreamWriter("log.bin");
                sw.Close(); 
            }
            GZipStream CompressRead = new GZipStream(File.OpenRead("log.bin"), CompressionMode.Decompress);
            StreamReader decompress = new StreamReader(CompressRead);
            string oldlog = decompress.ReadToEnd();
            decompress.Close();
            CompressRead.Close();
            GZipStream CompressWrite = new GZipStream(File.Create("log.bin"), CompressionMode.Compress);
            StreamWriter compress = new StreamWriter(CompressWrite);
            compress.Write(oldlog + message + "\n");
            compress.Close();
            CompressWrite.Close();
        }
        public float calc(string equation)
        {
            try
            {
                string[] rawsplit = equation.Split(')');
                string sfirstterm = rawsplit[0].Remove(0, 1);
                string ssecondterm = rawsplit[1].Remove(0, 2);
                string soperator = rawsplit[1].Substring(0, 1);
                float answer;
                float firstterm = float.Parse(sfirstterm);
                float secondterm = float.Parse(ssecondterm);
                char operation = soperator[0];
                if (operation == '+')
                {
                    answer = firstterm + secondterm;
                }
                else if (operation == '-')
                {
                    answer = firstterm - secondterm;
                }
                else if (operation == '*' || operation == 'x')
                {
                    answer = firstterm * secondterm;
                }
                else if (operation == '/')
                {
                    answer = firstterm / secondterm;
                }
                else
                {
                    answer = 9001.0F;
                }
                return answer;
            }
            catch (Exception ex)
            {
                return 9001F; 
            }
        }
        public string uDefine(string term)
        {
            try
            {
                term = term.Replace(" ", "+");
                string uDictionaryURL = "http://www.urbandictionary.com/define.php?term=" + term;
                System.Net.WebClient webClient = new System.Net.WebClient();
                string webSource = webClient.DownloadString(uDictionaryURL);
                webClient.Dispose();
                webSource = webSource.Trim().Replace("\0", "").Replace("\n", "");
                string firstDelimiter = "<div class=\"definition\">";
                string[] firstSplit = webSource.Split(new string[] { firstDelimiter }, StringSplitOptions.None);
                string secondDelimiter = "</div>";
                string[] secondSplit = firstSplit[1].Split(new string[] { secondDelimiter }, StringSplitOptions.None);
                return System.Text.RegularExpressions.Regex.Replace(secondSplit[0], @"<[^>]*>", "");
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }
        public string dDefine(string term)
        {
            try
            {
                term = term.Replace(" ", "+");
                string dDictionaryURL = "http://www.dictionary.reference.com/browse/" + term;
                System.Net.WebClient webClient = new System.Net.WebClient();
                string webSource = webClient.DownloadString(dDictionaryURL);
                webClient.Dispose();
                webSource = webSource.Trim().Replace("\0", "").Replace("\n", "");
                string firstDelimiter = "<div class=\"luna-Ent\"><span class=\"dnindex\">";
                string[] firstSplit = webSource.Split(new string[] { firstDelimiter }, StringSplitOptions.None);
                string secondDelimiter = "<div class=\"luna-Ent\"><span class=\"dnindex\">";
                string[] secondSplit = firstSplit[1].Split(new string[] { secondDelimiter }, StringSplitOptions.None);
                return System.Text.RegularExpressions.Regex.Replace(secondSplit[0], @"<[^>]*>", "");
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }
        public string wDefine(string term)
        {
            try
            {
                term = term.Replace(" ", "_");
                string wDictionaryURL = "http://en.wikipedia.org/wiki/" + term;
                System.Net.WebClient webClient = new System.Net.WebClient();
                webClient.Headers.Add("user-agent", "ObsidianBot");
                string webSource = webClient.DownloadString(wDictionaryURL);
                webClient.Dispose();
                webSource = webSource.Trim().Replace("\0", "").Replace("\n", "");
                string firstDelimiter = "</div><p>";
                string[] firstSplit = webSource.Split(new string[] { firstDelimiter }, StringSplitOptions.None);
                string secondDelimiter = ".";
                string[] secondSplit = firstSplit[1].Split(new string[] { secondDelimiter }, StringSplitOptions.None);
                string definition = System.Text.RegularExpressions.Regex.Replace(secondSplit[0], @"<[^>]*>", "");
                return definition;
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }
        public string CSCompile(string csfile)
        {
            try
            {
                CompilerParameters Params = new CompilerParameters();
                Params.GenerateExecutable = true;
                Params.ReferencedAssemblies.Add("System.dll");
                Params.ReferencedAssemblies.Add("FervorLibrary.dll");
                Params.ReferencedAssemblies.Add("ObsidianFunctions.dll");
                Params.ReferencedAssemblies.Add("AIMLBot.dll");
                Params.OutputAssembly = csfile.Replace(".cs", ".exe");
                StreamReader sr = new StreamReader(csfile);
                string cscode = sr.ReadToEnd();
                sr.Close();
                CompilerResults Results = new CSharpCodeProvider().CompileAssemblyFromSource(Params, cscode);
                if (Results.Errors.Count > 0)
                {
                    return String.Join(",", Results.Errors.ToString());
                }
                else
                {
                    return "Compile success!";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }
        
        public string batchExec(string batchFile, string[] findStrings, string[] replaceStrings)
        {
        	try
        	{
	        	StreamReader sr = new StreamReader(batchFile);
	        	string batchRead = sr.ReadToEnd(); 
	        	sr.Close();
	        	for (int i = 0; i < findStrings.Length; i++)
	        	{
	        		batchFile.Replace(findStrings[i], replaceStrings[i]);
	        	}
	        	Process batchProcess = new Process();
	        	batchProcess.StartInfo.FileName = batchFile;
	        	batchProcess.StartInfo.UseShellExecute = false;
	        	batchProcess.StartInfo.RedirectStandardOutput = true;
	        	batchProcess.StartInfo.CreateNoWindow = true;
	        	batchProcess.Start();
	        	batchProcess.WaitForExit();
	        	return batchProcess.StandardOutput.ReadToEnd(); 
        	}
        	catch (Exception ex)
        	{
        		return ex.ToString();
        	}
        	
        }
        public string JavaCompile(string javafile)
        {
            try
            {
                Process javaProcess = new Process();
                javaProcess.StartInfo.FileName = "ikvm.exe";
                javaProcess.StartInfo.Arguments = "-jar " + "ecj.jar " + javafile;
                javaProcess.StartInfo.UseShellExecute = false;
                javaProcess.StartInfo.RedirectStandardOutput = true;
                javaProcess.StartInfo.CreateNoWindow = true;
                javaProcess.Start();
                javaProcess.WaitForExit();
                string output = javaProcess.StandardOutput.ReadToEnd();
                return "Compile success!"; 
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string JavaCompileRun(string javafile, string channel, string rnick, string rmsg)
        {
            try
            {
                Process javaProcess = new Process();
                javaProcess.StartInfo.FileName = "ikvm.exe";
                javaProcess.StartInfo.Arguments = "-jar " + "ecj.jar " + javafile;
                javaProcess.StartInfo.UseShellExecute = false;
                javaProcess.StartInfo.RedirectStandardOutput = true;
                javaProcess.StartInfo.CreateNoWindow = true;
                javaProcess.Start();
                javaProcess.WaitForExit();
                string output = javaProcess.StandardOutput.ReadToEnd();
                Process javaexeProcess = new Process();
                javaexeProcess.StartInfo.FileName = "ikvm.exe";
                javaexeProcess.StartInfo.Arguments = javafile.Replace(".java", "") + " " + channel + " " + rnick + " \"" + rmsg + "\"";
                javaexeProcess.StartInfo.UseShellExecute = false;
                javaexeProcess.StartInfo.RedirectStandardOutput = true;
                javaexeProcess.StartInfo.CreateNoWindow = true;
                javaexeProcess.Start();
                javaexeProcess.WaitForExit();
                string runoutput = javaexeProcess.StandardOutput.ReadToEnd();
                return runoutput; 
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string CSCompileRun(string csfile, string channel, string rnick, string rmsg)
        {
            try
            {
                CompilerParameters Params = new CompilerParameters();
                Params.GenerateExecutable = true;
                Params.ReferencedAssemblies.Add("System.dll");
                Params.ReferencedAssemblies.Add("FervorLibrary.dll");
                Params.ReferencedAssemblies.Add("ObsidianFunctions.dll");
                Params.ReferencedAssemblies.Add("AIMLBot.dll");
                Params.OutputAssembly = csfile.Replace(".cs", ".exe");
                StreamReader sr = new StreamReader(csfile);
                string cscode = sr.ReadToEnd();
                sr.Close();
                CompilerResults Results = new CSharpCodeProvider().CompileAssemblyFromSource(Params, cscode);
                if (Results.Errors.Count > 0)
                {
                    return String.Join(",", Results.Errors.ToString());
                }
                else
                {
                    Process csProcess = new Process();
                    csProcess.StartInfo.FileName = csfile.Replace(".cs", ".exe");
                    csProcess.StartInfo.Arguments = channel + " " + rnick + " " + "\"" + rmsg + "\"";
                    csProcess.StartInfo.UseShellExecute = false;
                    csProcess.StartInfo.RedirectStandardOutput = true;
                    csProcess.StartInfo.CreateNoWindow = true; 
                    csProcess.Start();
                    csProcess.WaitForExit();
                    return csProcess.StandardOutput.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public bool isActiveUser(string nickname)
        {
            StreamReader sr = new StreamReader(".activeusers");
            string[] listofactiveusers = sr.ReadToEnd().Split(':');
            sr.Close();
            bool userbool = false;
            foreach (string x in listofactiveusers)
            {
                if (x == nickname)
                {
                    userbool = true;
                }
            }
            return userbool;
        }
        public string active(string rnick, string rmsg)
        {
            try
            {
                System.IO.StreamReader sr2 = new System.IO.StreamReader(".activeusers");
                string old = sr2.ReadToEnd();
                sr2.Close();
                System.IO.StreamReader sr = new System.IO.StreamReader("users.bin");
                string[] registeredusers = sr.ReadToEnd().Split(':');
                sr.Close();
                string returnvalue = "Log in failed!";
                foreach (string x in registeredusers)
                {
                    if (x == rnick)
                    {
                        string query = rmsg.Remove(0, 8);
                        int indexuser = Array.IndexOf(registeredusers, rnick);
                        ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                        bool passcorrect = ObsidFunc.isVerified(indexuser, query);
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(".activeusers");
                        if (passcorrect == true)
                        {
                            sw.Write(old + rnick + ":");
                            returnvalue = "Success! You are logged in!";
                        }
                        sw.Close();
                    }
                }
                return returnvalue; 
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string deactivate(string rnick, string rmsg)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(".activeusers");
                string[] temp = sr.ReadToEnd().Split(':');
                List<string> activelist = new List<string>(temp);
                sr.Close();
                string returnvalue = "";
                foreach (string x in activelist)
                {
                    if (x == rnick)
                    {
                        string[] listofactiveusers = activelist.ToArray();
                        string listuser = String.Join(":", listofactiveusers) + ":";
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(".activeusers");
                        sw.Write(listuser);
                        sw.Close();
                        returnvalue = "User has been deactivated!";
                    }
                }
                return returnvalue; 
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }

}

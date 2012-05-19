using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;

namespace ObsidianFunctions
{
    public class Functions
    {
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
        public string uDefine(string term)
        {
            try
            {
                term = term.Replace(" ", "+");
                string uDictionaryURL = "http://www.urbandictionary.com/define.php?term=" + term;
                System.Net.WebClient webClient = new System.Net.WebClient();
                string webSource = webClient.DownloadString(uDictionaryURL);
                webClient.Dispose();
                webSource = webSource.Trim().Replace("\0", "");
                StreamWriter sw = new StreamWriter("htmltest.bin");
                sw.Write(webSource);
                sw.Close();
                string firstDelimiter = "<div class=\"definition\">";
                string[] firstSplit = webSource.Split(new string[] { firstDelimiter }, StringSplitOptions.None);
                string secondDelimiter = "</div>";
                string[] secondSplit = firstSplit[1].Split(new string[] { secondDelimiter }, StringSplitOptions.None);
                StreamWriter sw2 = new StreamWriter("htmltestresult.bin");
                sw2.Write(secondSplit[0]);
                sw2.Close();
                return secondSplit[0];
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }
        
    }
}

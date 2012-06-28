using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using ObsidianFunctions;
using FervorLibrary;
using AIMLbot;
using System.Text.RegularExpressions; 


namespace Obsidian
{
    public partial class Form1 : Form
    {
        int port;
        string buf;
        string nick;
        string server;
        string channel;
        System.Net.Sockets.Socket sock;
        string mail;
        string rmsg;
        string password;
        bool isregistered;
        string rnick;
        string old;
        string oldmsg;
        int spamcount;
        bool isLogging;
        Bot chatBot;
        User chatUser;
        System.Timers.Timer updatetmr;
        bool canGreet;
        string[] blacklist;
        Thread updatethread;
        string ircuserlist;
        string owner;
        HashSet<string> executableCommands = new HashSet<string>();
        HashSet<string> compileCommands = new HashSet<string>();
        HashSet<string> classCommands = new HashSet<string>();
        HashSet<string> jarCommands = new HashSet<string>(); 
        HashSet<string> javaCommands = new HashSet<string>(); 

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string IRCInfo = textBox1.Text + "|" + textBox2.Text + "|" + textBox3.Text + "|" + textBox4.Text + "|" + textBox6.Text;
            System.IO.StreamWriter IRCInfoWrite = new System.IO.StreamWriter("IRCInfo.bin");
            IRCInfoWrite.Write(IRCInfo);
            IRCInfoWrite.Close();
            port = Int32.Parse(textBox2.Text);
            server = textBox1.Text;
            channel = textBox3.Text;
            nick = textBox4.Text;
            password = textBox6.Text;
            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions(); 
            owner = "Obsidian";
            System.Net.IPHostEntry ipHostInfo = System.Net.Dns.GetHostEntry(server);
            System.Net.IPEndPoint EP = new System.Net.IPEndPoint(ipHostInfo.AddressList[0], port);
            sock = new System.Net.Sockets.Socket(EP.Address.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            sock.Connect(server, port);
            send("NICK " + nick);
            send("USER " + nick + " 0 * :ObsidianBot");
            send("JOIN " + channel);
            send("MODE " + nick + " +B");
            startupdateTmr(); 
            timer2.Enabled = true;
            if (textBox6.Text != null)
            {
                timer3.Enabled = true;
            }
            string channeltext;
            if (textBox3.Text != "")
            {
                channeltext = channel;
            }
            else
            {
                channeltext = "<channel>";
            }
            textBox7.Text = "PRIVMSG " + channeltext + " :";
            timer1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            spamcount = 0;
            loadIRCInfo();
            Thread configGreet = new Thread(GreetConfig);
            configGreet.Start();
            Thread configFarewell = new Thread(FarewellConfig);
            configFarewell.Start();
            if (System.IO.File.Exists("users.bin") == false | System.IO.File.Exists("passwords.bin") == false | System.IO.File.Exists("registers.bin") == false | System.IO.File.Exists(".activeusers") == false)
            {
                Thread configUser = new Thread(startUserList);
                configUser.Start();
            }
            if (System.IO.File.Exists("log.bin") == false)
            {
                StreamWriter swlog = new StreamWriter("log.bin");
                swlog.Close();
            }
            StreamWriter sw = new StreamWriter(".activeusers");
            sw.Close();
            if (System.IO.File.Exists("owner.bin") == false)
            {
                ownerConfiguration();
            }
            if (System.IO.File.Exists("blacklist.bin") == false)
            {
                StreamWriter swbl = new StreamWriter("blacklist.bin");
                swbl.Close();
            }
            isLogging = false;
            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
            ObsidFunc.settalkingTo("nobody"); 
            updatetmr = new System.Timers.Timer(300);
            updatetmr.Elapsed += new System.Timers.ElapsedEventHandler(updateTmrWork);
            updatetmr.Interval = 300;
            canGreet = false;
            if (System.IO.File.Exists("messages.bin") == false)
            {
                StreamWriter swmg = new StreamWriter("messages.bin");
                swmg.Write("|"); 
                swmg.Close(); 
            }
            configHashSet(); 
        }
        public void nullHashSet()
        {
            compileCommands = null;
            executableCommands = null;
            classCommands = null;
            jarCommands = null; 
        }
        public void configHashSet()
        {
            string[] directoryFiles = Directory.GetFiles(Directory.GetCurrentDirectory());
            foreach (string x in directoryFiles)
            {
                if (x.EndsWith(".cs"))
                {
                    string[] filepath = x.Split('\\');
                    string filename = filepath[filepath.Length - 1];
                    compileCommands.Add(filename);
                }
                else if (x.EndsWith(".exe"))
                {
                    string[] filename = x.Split('\\');
                    string executable = filename[filename.Length - 1];
                    executableCommands.Add(executable.Replace(".exe", ""));
                }
                else if (x.EndsWith(".class"))
                {
                    string[] filepath = x.Split('\\');
                    string filename = filepath[filepath.Length - 1];
                    classCommands.Add(filename.Replace(".class", ""));
                }
                else if (x.EndsWith(".jar"))
                {
                    string[] filepath = x.Split('\\');
                    string filename = filepath[filepath.Length - 1];
                    jarCommands.Add(filename);
                }
                else if (x.EndsWith(".java"))
                {
                    string[] filepath = x.Split('\\');
                    string filename = filepath[filepath.Length - 1];
                    javaCommands.Add(filename); 
                }
            }
        }
        public void send(string msg)
        {
            msg += "\r\n";
            Byte[] data = System.Text.ASCIIEncoding.UTF8.GetBytes(msg);
            sock.Send(data, msg.Length, System.Net.Sockets.SocketFlags.None);
        }
        public string recv()
        {
            byte[] data = new byte[2048];
            sock.Receive(data, 2048, System.Net.Sockets.SocketFlags.None);
            mail = System.Text.ASCIIEncoding.UTF8.GetString(data).Replace("\0", "");
            if (mail.Contains(" "))
            {
                if (mail.Substring(0, 4) == "PING")
                {
                    string pserv = mail.Substring(mail.IndexOf(":"), mail.Length - mail.IndexOf(":"));
                    pserv = pserv.TrimEnd((char)0);
                    mail = "PING from " + pserv + "\r" + "PONG to " + pserv;
                    send("PONG " + pserv);
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 7) == "PRIVMSG")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    rnick = tmparr[0];
                    tmparr = mail.Split(':');
                    rmsg = tmparr[1].Trim() ;
                    string sayingto = Regex.Split(mail, "PRIVMSG ")[1].Split(':')[0].Substring(0, Regex.Split(mail, "PRIVMSG ")[1].Split(':')[0].Length - 1);

                    bool nickblacklisted = isBlacklisted(rnick);
                    ObsidianFunctions.Functions ObsidBot = new ObsidianFunctions.Functions();
                    //MessageBox.Show("privmsg"); 
                    if (nickblacklisted == false)
                    {
                        foreach (string command in executableCommands)
                        {
                            if (rmsg.Contains("!" + command) == true)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                send(ObsidFunc.exeExec(command + ".exe", channel, rnick, rmsg)); 
                            }
                        }
                        foreach (string command in jarCommands)
                        {
                            if (rmsg.Contains("!" + command.Replace(".jar", "")))
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                send(ObsidFunc.jarExec(command, channel, rnick, rmsg));
                            }
                        }
                        foreach (string command in classCommands)
                        {
                            if (rmsg.Contains("!" + command) == true)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                send(ObsidFunc.classExec(command, channel, rnick, rmsg));
                            }
                        }
                        foreach (string command in compileCommands)
                        {
                            if (rmsg.Contains("!" + command.Replace(".cs", "")) && executableCommands.Contains(command.Replace(".cs", ".exe")) == false)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                send(ObsidFunc.CSCompileRun(command, channel, rnick, rmsg));
                                configHashSet(); 
                            }
                        }
                        foreach (string command in javaCommands)
                        {
                            if (rmsg.Contains("!" + command.Replace(".java", "")) && classCommands.Contains(command.Replace(".java", ".class")) == false)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                send(ObsidFunc.JavaCompileRun(command, channel, rnick, rmsg));
                                configHashSet(); 
                            }
                        }
                        //add AIML
                        if (sayingto == nick && rnick != nick && rmsg.StartsWith("!") == false)
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            send(ObsidFunc.exeExec("botchat.exe", channel, rnick, rmsg));
                        }
                    }
                    
                    if (ObsidBot.controlSpam() == true)
                    {
                        if (rmsg == oldmsg)
                        {
                            increaseSpamCount();
                            if (spamcount >= 4)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                if (ObsidFunc.isOperator() == true)
                                {
                                    send("KICK " + channel + " " + rnick + " No Spamming or Repeating one's self");
                                    spamcount = 0;
                                }
                                else
                                {
                                    send("PRIVMSG " + channel + " :Try not to spam or excessively repeat yourself");
                                    spamcount = 0;
                                }
                            }


                        }
                    }
                      
                    ObsidianFunctions.Functions Obsid = new ObsidianFunctions.Functions();
                    if (Obsid.isLogging() == true)
                    {
                        logMsg(); 
                    }
                    detectLang();
                    bool newMessages = Obsid.hasMessages(rnick);
                    if (newMessages == true)
                    {
                        Obsid.sayMessages(rnick);
                    }
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "JOIN")
                {

                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    rnick = tmparr[0];
                    FervorLibrary.Library Greetings = new FervorLibrary.Library();
                    if (Greetings.canGreet() == true)
                    {
                        
                        Random rand = new Random();
                        int indexgreet = rand.Next(0, Greetings.greetnumber());
                        say(nick, "!greet " + indexgreet.ToString());
                    }
                    ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                    bool newMessages = ObsidFunc.hasMessages(rnick);
                    if (newMessages == true)
                    {
                        say(rnick, "!You have messages"); 
                        string[] usermessages = ObsidFunc.sayMessages(rnick);
                        foreach (string x in usermessages)
                        {
                            send(x); 
                        }
                    }
                    say(nick, "!names"); 
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "PART" || mail.Substring(mail.IndexOf(" ") + 1, 4) == "QUIT")
                {
                    ObsidianFunctions.Functions ObsidBot = new ObsidianFunctions.Functions();
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    rnick = tmparr[0];
                    FervorLibrary.Library FervLib = new FervorLibrary.Library();
                    if (FervLib.canGreet() == true)
                    {

                        Random rand = new Random();
                        
                        int indexfarewell = rand.Next(0, FervLib.farewellnumber());
                        say(nick, "!farewell " + indexfarewell.ToString()); 
                    }
                    ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                    say(rnick, ObsidFunc.deactivate(rnick, rmsg));
                    if (rnick == ObsidBot.talkingTo())
                    {
                        ObsidBot.settalkingTo("nobody"); 
                    }
                    say(nick, "!names"); 
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "MODE")
                {
                    mail = mail.Replace("\0", "").Trim();
                    int nameopslength = nick.Length + 3;
                    string action = mail.Substring(mail.Length - nameopslength);
                    if (action.StartsWith("+o") | action.StartsWith("+r") | action.StartsWith("+h"))
                    {
                        ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                        ObsidFunc.opTrue(); 
                        send("PRIVMSG " + channel + " :isOperator = true");

                    }
                    else if (action.StartsWith("-o") | action.StartsWith("-r") | action.StartsWith("-h"))
                    {
                        ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                        ObsidFunc.opFalse(); 
                        send("PRIVMSG " + channel + " :isOperator = false");
                    }

                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "KICK")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    rnick = tmparr[0];
                    tmparr = mail.Split(':');
                    rmsg = tmparr[1];
                    ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                    say(rnick, ObsidFunc.deactivate(rnick, rmsg)) ;
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 3) == "353")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split(':');
                    rmsg = tmparr[1];
                    ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                    say(nick, ObsidFunc.setNames(rmsg)); 
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "NICK")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    rnick = tmparr[0];
                    tmparr = mail.Split(':');
                    string newnick = tmparr[1];
                    ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                    say(rnick, ObsidFunc.deactivate(rnick, rmsg));
                    bool newMessages = ObsidFunc.hasMessages(newnick);
                    if (newMessages == true)
                    {
                        ObsidFunc.sayMessages(newnick);
                    }
                }
            }
            oldMsg();
            return mail;

        }

        public void GreetConfig()
        {
            if (System.IO.File.Exists("greet.bin") == false)
            {
                System.IO.StreamWriter greetwrite = new System.IO.StreamWriter("greet.bin");
                greetwrite.Write("Hallo,Ahalan,Ni Hao,Ahoj,Goddag,Goede dag,Hello,Bonjour,Guten Tag,Aloha,Shalom,Namaste,Dia dhuit,Ciao,Kon-nichiwa,Zdravstvuyte,Hola,Hej,Sawubona~");
                greetwrite.Write("Afrikaans,Arabic,Chinese,Czech,Danish,Dutch,English,French,German,Hawaiian,Hebrew,Hindi,Irish,Italian,Japanese,Russian,Spanish,Swedish,Zulu");
                greetwrite.Close();
                System.IO.StreamReader greetread = new System.IO.StreamReader("greet.bin");
                string greetraw = greetread.ReadToEnd();
                string[] split = greetraw.Split(',');
                FervorLibrary.Library FervLib = new FervorLibrary.Library();
                FervLib.setgreetnumber(split.Length / 2);
            }
            else if (System.IO.File.Exists("greet.bin") == true)
            {
                System.IO.StreamReader greetread = new System.IO.StreamReader("greet.bin");
                string greetraw = greetread.ReadToEnd();
                string[] split = greetraw.Split(',');
                FervorLibrary.Library FervLib = new FervorLibrary.Library();
                FervLib.setgreetnumber(split.Length / 2);
            }
        }
        public void FarewellConfig()
        {
            if (System.IO.File.Exists("farewell.bin") == false)
            {
                System.IO.StreamWriter farewellwrite = new System.IO.StreamWriter("farewell.bin");
                farewellwrite.Write("Tosiens,Ma'a as-salaama,Zai Jian,Sbohem,Farvel,Afscheid,Goodbye,Au revoir,Aurf Wiedersehen,Aloha,Kol Tuv,Ach-ha,Slan agat,Addio,Sayonara,Pa-ka,Adios,Adjo,Hamba kahle~");
                farewellwrite.Write("Afrikaans,Arabic,Chinese,Czech,Danish,Dutch,English,French,German,Hawaiian,Hebrew,Hindi,Irish,Italian,Japanese,Russian,Spanish,Swedish,Zulu");
                farewellwrite.Close();
                System.IO.StreamReader farewellread = new System.IO.StreamReader("farewell.bin");
                string farewellraw = farewellread.ReadToEnd();
                string[] split = farewellraw.Split(',');
                FervorLibrary.Library FervLib = new FervorLibrary.Library();
                FervLib.setfarewellnumber(split.Length / 2);
            }
            else if (System.IO.File.Exists("farewell.bin") == true)
            {
                System.IO.StreamReader farewellread = new System.IO.StreamReader("farewell.bin");
                string farewellraw = farewellread.ReadToEnd();
                string[] split = farewellraw.Split(',');
                FervorLibrary.Library FervLib = new FervorLibrary.Library();
                FervLib.setfarewellnumber(split.Length / 2);
            }
        }
        public void increaseSpamCount()
        {
            spamcount++;
        }
        public void oldMsg()
        {
            oldmsg = rmsg;
        }
        public void loadIRCInfo()
        {
            if (System.IO.File.Exists("IRCInfo.bin") == true)
            {
                System.IO.StreamReader IRCInfoRead = new System.IO.StreamReader("IRCInfo.bin");
                string IRCInfo = IRCInfoRead.ReadToEnd();
                string[] IRCInfoSplit = IRCInfo.Split('|');
                textBox1.Text = IRCInfoSplit[0];
                textBox2.Text = IRCInfoSplit[1];
                textBox3.Text = IRCInfoSplit[2];
                textBox4.Text = IRCInfoSplit[3];
                textBox6.Text = IRCInfoSplit[4];
                IRCInfoRead.Close();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            textBox5.Text = mail;
        }
        private void ircupdate()
        {
            mail = recv().Replace("\0", "");
            
        }
        public void startUserList()
        {
            StreamWriter sw1 = new StreamWriter("users.bin");
            sw1.Write(textBox4.Text + ":");
            sw1.Close();
            Functions ObsidFunc = new Functions();
            StreamWriter sw2 = new StreamWriter("passwords.bin");
            sw2.Write(ObsidFunc.md5calc(textBox6.Text) + ":");
            sw2.Close();
            StreamWriter sw3 = new StreamWriter("registers.bin");
            sw3.Write("|");
            sw3.Close();
            StreamWriter sw4 = new StreamWriter(".activeusers");
            sw4.Close();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            send("PRIVMSG " + "NickServ" + " :IDENTIFY " + textBox6.Text);
            isregistered = true;
            timer3.Enabled = false;
        }
       
        public void say(string saychannel, string message)
        {
            send("PRIVMSG " + saychannel + " :" + message);
        }
     
        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        public void ownerConfiguration()
        {
            ownerConfig form2 = new ownerConfig();
            form2.ShowDialog(); 
        }
        public void detectLang()
        {
            if (System.IO.File.Exists("badlang.bin") == true)
            {
                StreamReader sr = new StreamReader("badlang.bin");
                string[] badlist = sr.ReadToEnd().Trim().Split(':');
                sr.Close(); 
                foreach (string x in badlist)
                {
                    if (rmsg.ToLower().Contains(x.ToLower()) == true)
                    {
                        ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                        if (ObsidFunc.isOperator() == true)
                        {
                            send("KICK " + channel + " " + rnick + " No bad language allowed!");
                        }
                        else
                        {
                            send("PRIVMSG " + channel + " :" + rnick + ", I must request for you to change your word choice.");
                        }
                    }
                }
            }
        }
        public void logMsg()
        {
            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
            ObsidFunc.logMsg(mail);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            send(textBox7.Text);
            textBox7.Text = "PRIVMSG " + channel + " :";
        }
        public bool isBlacklisted(string name)
        {
            bool returnbool = false;
            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
            string[] blacklistbin = ObsidFunc.blacklist();
            foreach (string x in blacklistbin)
            {
                if (x == name)
                {
                    returnbool = true;
                }
            }
            return returnbool;
        }
        public void startupdateTmr()
        {
            updatetmr.Enabled = true;
        }
        public void updateTmrWork(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                updatetmr.Stop();
                Thread.Sleep(500);
                ircupdate();
            }
            finally
            {
                updatetmr.Start(); 
            }
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                say(nick, "!active " + textBox6.Text);
                say(nick, "!names");
                mail = recv().Replace("\0", "").Trim();
                if (System.IO.File.Exists(channel + ".txt"))
                {
                    StreamReader sr = new StreamReader(channel + ".txt");
                    string[] settings = sr.ReadToEnd().Split('|');
                    foreach (string x in settings)
                    {
                        send(x);
                        mail = recv().Replace("\0", "");
                    }
                }
            }
            catch (Exception ex)
            {
                say(channel, ex.ToString());
            }
            timer1.Enabled = false; 
        }
        
    }

}

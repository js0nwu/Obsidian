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
            //timer1.Enabled = true;
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
            botChat();
            setBlacklist();
            updatetmr = new System.Timers.Timer(500);
            updatetmr.Elapsed += new System.Timers.ElapsedEventHandler(updateTmrWork);
            updatetmr.Interval = 500;
            canGreet = false;
            if (System.IO.File.Exists("messages.bin") == false)
            {
                StreamWriter swmg = new StreamWriter("messages.bin");
                swmg.Write("|"); 
                swmg.Close(); 
            }
            configHashSet(); 
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
                            if (rmsg.Contains("!" + command.Replace(".cs", "")) && File.Exists(command.Replace(".cs", "") + ".exe") == false)
                            {
                                send("PRIVMSG " + channel + " :Command not compiled! Compiling...");
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                send(ObsidFunc.CSCompileRun(command, channel, rnick, rmsg));
                                configHashSet(); 
                            }
                        }
                        /*
                        if (rmsg.Contains("!respond") == true)
                        {
                            string response = "PRIVMSG " + channel + " :Response";
                            send(response);
                        }
                        else if (rmsg.Contains("!classexec ") == true)
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (rnick == ObsidFunc.ownernick() && ObsidFunc.isActiveUser(rnick) == true)
                            {
                                string query = rmsg.Remove(0, 10).Trim();
                                send(ObsidFunc.classExec(query, channel, rnick, rmsg).Trim());
                            }
                        }
                        else if (rmsg.Contains("!jarexec ") == true)
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (rnick == ObsidFunc.ownernick() && ObsidFunc.isActiveUser(rnick) == true)
                            {
                                string query = rmsg.Remove(0, 9).Trim();
                                send(ObsidFunc.jarExec(query, channel, rnick, rmsg));
                            }
                        }
                        else if (rmsg.Contains("!exeexec ") == true)
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (rnick == ObsidFunc.ownernick() && ObsidFunc.isActiveUser(rnick) == true)
                            {
                                string query = rmsg.Remove(0, 9).Trim();
                                string[] qSplit = query.Split(' ');
                                string file = qSplit[0].Remove(0, 1) + ".exe";
                                send(ObsidFunc.exeExec(file, channel, rnick, query));
                            }
                        }
                        else if (rmsg.Contains("!greet "))
                        {
                            string query = rmsg.Remove(0, 7);
                            try
                            {
                                int queryindex = Int32.Parse(query);
                                FervorLibrary.Library FervLib = new FervorLibrary.Library();
                                if (queryindex <= FervLib.greetnumber)
                                {
                                    FervorLibrary.Library Greeting = new FervorLibrary.Library();
                                    string returngreet = Greeting.greet(queryindex);

                                    string response = "PRIVMSG " + channel + " :" + returngreet;
                                    send(response);
                                }
                                else
                                {
                                    send("PRIVMSG " + channel + " :I don't know that many languages!");
                                }
                            }
                            catch (Exception ex)
                            {
                                send("PRIVMSG " + channel + " :Something went wrong: " + ex);
                            }


                        }
                        else if (rmsg.Contains("!farewell "))
                        {
                            string query = rmsg.Remove(0, 10);
                            try
                            {
                                int queryindex = Int32.Parse(query);
                                FervorLibrary.Library FervLib = new FervorLibrary.Library();
                                if (queryindex <= FervLib.farewellnumber)
                                {
                                    FervorLibrary.Library Farewelling = new FervorLibrary.Library();
                                    string returnfarewell = Farewelling.farewell(queryindex);
                                    string response = "PRIVMSG " + channel + " :" + returnfarewell;
                                    send(response);
                                }
                                else
                                {
                                    send("PRIVMSG " + channel + " :I don't know that many languages!");
                                }
                            }
                            catch (Exception ex)
                            {
                                send("PRIVMSG " + channel + " :Something went wrong: " + ex);
                            }
                        }
                        else if (rmsg.Contains("!md5 "))
                        {
                            generateMD5message();
                        }
                        else if (rmsg.Contains("!register "))
                        {
                            reqreguser();
                        }
                        else if (rmsg.Contains("!registerlist"))
                        {
                            listregs();
                        }
                        else if (rmsg.Contains("!clearregisterlist"))
                        {
                            clearregs();
                        }
                        else if (rmsg.Contains("!active "))
                        {
                            activateUser();
                        }
                        else if (rmsg.Contains("!deactivate"))
                        {
                            deactivateUser();
                        }
                        else if (rmsg.Contains("!adduser "))
                        {
                            System.IO.StreamReader sr = new StreamReader(".activeusers");
                            string[] users = sr.ReadToEnd().Split(':');
                            sr.Close();
                            foreach (string x in users)
                            {
                                if (x.Contains(rnick))
                                {
                                    string query = rmsg.Remove(0, 9);
                                    ObsidianFunctions.Functions obsidfunc = new ObsidianFunctions.Functions();
                                    string list = obsidfunc.addUser(query);
                                    send("PRIVMSG " + channel + " :" + list);
                                }
                            }
                        }

                        else if (rmsg.Contains("!removeuser "))
                        {
                            System.IO.StreamReader sr = new StreamReader(".activeusers");
                            string[] users = sr.ReadToEnd().Split(':');
                            sr.Close();
                            foreach (string x in users)
                            {
                                if (x.Contains(rnick))
                                {
                                    string query = rmsg.Remove(0, 12);
                                    ObsidianFunctions.Functions obsidfunc = new ObsidianFunctions.Functions();
                                    string list = obsidfunc.removeUser(query);
                                    send("PRIVMSG " + channel + " :" + list);
                                }
                            }
                        }
                        else if (rmsg.Contains("!userlist"))
                        {
                            listUsers();
                        }
                        else if (rmsg.Contains("!botquit"))
                        {
                            bool nickisuser = isActiveUser(rnick);
                            if (nickisuser == true)
                            {
                                send("QUIT");
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient permissions!");
                            }
                        }
                        else if (rmsg.Contains("!addops "))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (ObsidFunc.isOperator() == true)
                            {
                                string query = rmsg.Remove(0, 8);
                                bool nickisuser = ObsidFunc.isActiveUser(rnick);
                                if (nickisuser == true)
                                {
                                    send("MODE " + channel + " +o " + query);
                                }
                                else
                                {
                                    send("PRIVMSG " + channel + " :Insufficient permissions!");
                                }
                            }
                        }
                        else if (rmsg.Contains("!removeops "))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (ObsidFunc.isOperator() == true)
                            {
                                string query = rmsg.Remove(0, 11);
                                bool nickisuser = isActiveUser(rnick);
                                if (nickisuser == true)
                                {
                                    send("MODE " + channel + " -o " + query);
                                }
                                else
                                {
                                    send("PRIVMSG " + channel + " :Insufficient permissions!");
                                }
                            }
                        }
                        else if (rmsg.Contains("!kick "))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (ObsidFunc.isOperator() == true)
                            {
                                string query = rmsg.Remove(0, 6);
                                bool nickuser = isActiveUser(rnick);
                                if (nickuser == true)
                                {
                                    send("KICK " + channel + " " + query + " User-requested kick");
                                }
                                else
                                {
                                    send("PRIVMSG " + channel + " :Insufficient permissions!");
                                }
                            }
                        }

                        else if (rmsg.Contains("!batch "))
                        {
                            string query = rmsg.Remove(0, 7);
                            bool nickuser = isActiveUser(rnick);
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (rnick == ObsidFunc.ownernick() && nickuser == true)
                            {
                                ObsidFunc.batch(query);
                                send("PRIVMSG " + channel + " :Success!");
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
                        }
                        else if (rmsg.Contains("!cscompile "))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            string query = rmsg.Remove(0, 11).Replace("\0", "").Trim();
                            bool nickuser = ObsidFunc.isActiveUser(rnick);
                            if (rnick == ObsidFunc.ownernick() && nickuser == true)
                            {
                                say(channel, ObsidFunc.CSCompile(query));

                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
                        }
                        else if (rmsg.Contains("!log start"))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidFunc.logTrue();
                                send("PRIVMSG " + channel + " :Success!");
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
                        }
                        else if (rmsg.Contains("!log stop"))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidFunc.logFalse();
                                send("PRIVMSG " + channel + " :Success!");
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
                        }
                        else if (rmsg.Contains("!calc "))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            string query = rmsg.Remove(0, 6);
                            float answer = ObsidFunc.calc(query);
                            send("PRIVMSG " + channel + " :" + answer.ToString());
                        }
                        else if (rmsg.Contains("!isOperator true"))
                        {
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                ObsidFunc.opTrue();
                                send("PRIVMSG " + channel + " :isOperator = true");
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
                        }
                        else if (rmsg.Contains("!isOperator false"))
                        {
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                                ObsidFunc.opFalse();
                                send("PRIVMSG " + channel + " :isOperator = false");
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
                        }
                        else if (rmsg.Contains("!botchat"))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (ObsidFunc.talkingTo() != "nobody")
                            {
                                send("PRIVMSG " + rnick + " :Sorry, I'm already talking with someone");
                            }
                            else
                            {
                                ObsidFunc.settalkingTo(rnick);
                                FervorLibrary.Library Greetings = new FervorLibrary.Library();
                                Random rand = new Random();
                                int indexgreet = rand.Next(0, Greetings.greetnumber);
                                string greeting = Greetings.Greeting(rnick, indexgreet);
                                send("PRIVMSG " + rnick + " :" + greeting);
                                botChat();
                            }

                        }
                        else if (rmsg.Contains("quit"))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            if (rnick == ObsidFunc.talkingTo())
                            {
                                ObsidFunc.settalkingTo("nobody");
                            }
                        }

                        else if (rnick == ObsidBot.talkingTo())
                        {

                            Request r = new Request(rmsg, chatUser, chatBot);
                            Result res = chatBot.Chat(r);
                            send("PRIVMSG " + ObsidBot.talkingTo() + " :" + res.Output);

                        }
                        else if (rmsg.Contains("!udefine "))
                        {
                            string query = rmsg.Remove(0, 9);
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            string definition = ObsidFunc.uDefine(query);
                            send("PRIVMSG " + channel + " :" + definition);
                        }
                        else if (rmsg.Contains("!ddefine "))
                        {
                            string query = rmsg.Remove(0, 9);
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            string definition = ObsidFunc.dDefine(query);
                            send("PRIVMSG " + channel + " :" + definition);
                        }
                        else if (rmsg.Contains("!canGreet true"))
                        {
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                canGreet = true;
                                send("PRIVMSG " + channel + " :Success!");
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient permissions!");
                            }
                        }
                        else if (rmsg.Contains("!canGreet false"))
                        {
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                canGreet = false;
                                send("PRIVMSG " + channel + " :Success!");
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient permissions!");
                            }
                        }
                        else if (rmsg.Contains("!wdefine "))
                        {
                            string query = rmsg.Remove(0, 9);
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            string definition = ObsidFunc.wDefine(query);
                            send("PRIVMSG " + channel + " :" + definition);
                        }
                        else if (rmsg.Contains("!sha1 "))
                        {
                            Thread sha1hash = new Thread(generatesha1message);
                            sha1hash.Start();
                        }
                        else if (rmsg.Contains("!sha256 "))
                        {
                            Thread sha256hash = new Thread(generatesha256message);
                            sha256hash.Start();
                        }
                        else if (rmsg.Contains("!sha384 "))
                        {
                            Thread sha384hash = new Thread(generatesha384message);
                            sha384hash.Start();
                        }
                        else if (rmsg.Contains("!sha512 "))
                        {
                            Thread sha512hash = new Thread(generatesha512message);
                            sha512hash.Start();
                        }
                        else if (rmsg.Contains("!blacklistadd "))
                        {
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                string query = rmsg.Remove(0, 14);
                                Functions ObsidFunc = new Functions();
                                blacklist = ObsidFunc.addBlacklist(query);
                                string newlist = String.Join(":", blacklist);
                                send("PRIVMSG " + channel + " :" + newlist);
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :Insufficient Permissions!");
                            }
                        }
                        else if (rmsg.Contains("!blacklistremove "))
                        {
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                string query = rmsg.Remove(0, 17);
                                Functions ObsidFunc = new Functions();
                                blacklist = ObsidFunc.removeBlacklist(query);
                                string newlist = String.Join(":", blacklist);
                                if (newlist == null || newlist == "")
                                {
                                    newlist = "None";
                                }
                                send("PRIVMSG " + channel + " :" + newlist);
                            }
                            else
                            {
                                send("PRIVMSG " + channel + ":Insufficient Permissions!");
                            }
                        }
                        else if (rmsg.Contains("!blacklist"))
                        {
                            listBlacklist();
                        }
                        else if (rmsg.Contains("!get "))
                        {
                            string query = rmsg.Remove(0, 5);
                            if (query.StartsWith("me "))
                            {
                                string query2 = query.Remove(0, 3);
                                query = rnick + " " + query2;
                            }
                            say(channel, "Let me get that for you!");
                            say(channel, "\u0001ACTION " + "gives " + query + "\u0001");
                            say(channel, "There you go!");
                        }
                        else if (rmsg.Contains("!google "))
                        {
                            string query = rmsg.Remove(0, 8);
                            string googleURL = "http://google.com/search?q=" + query;
                            say(channel, googleURL);
                        }
                        else if (rmsg.Contains("!ircuserlist"))
                        {
                            Thread listircusers = new Thread(channelUsers);
                            listircusers.Start();
                            Thread saylistircusers = new Thread(saychannelUsers);
                            saylistircusers.Start();
                        }
                        else if (rmsg.Contains("!message "))
                        {
                            try
                            {
                                string sender = rnick;
                                string query = rmsg.Remove(0, 9);
                                string[] parsenick = query.Split('>');
                                string recipient = parsenick[0];
                                string message = "<" + sender + ">" + parsenick[1].Replace("~", "");
                                bool nickOnline = isOnline(recipient);
                                if (nickOnline == true)
                                {
                                    say(recipient, message);
                                    say(sender, "Message sent!");
                                }
                                else if (nickOnline == false)
                                {
                                    say(sender, "I'll tell " + recipient + " when he or she is online.");
                                    addMessage(recipient, message);
                                }

                            }
                            catch (Exception ex)
                            {
                                say(rnick, ex.ToString());
                            }
                        }
                        else if (rmsg.Contains("!spamControl true"))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidFunc.spamTrue();
                                say(channel, "spamControl = true");
                            }
                            else
                            {
                                say(rnick, "Insufficient permissions!");
                            }
                        }
                        else if (rmsg.Contains("!spamControl false"))
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidFunc.spamFalse();
                                say(channel, "spamControl = false");
                            }
                            else
                            {
                                say(rnick, "Insufficient permissions!");
                            }
                        }
                        else if (rmsg.Contains("forcequit"))
                        {
                            bool nickuser = isActiveUser(rnick);
                            if (nickuser == true)
                            {
                                ObsidBot.settalkingTo("nobody");
                            }
                            else
                            {
                                say(rnick, "Insufficient permissions!");
                            }
                        }
                         */
                      //  commands end
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
                    bool newMessages = hasMessages(rnick);
                    if (newMessages == true)
                    {
                        sayMessages(rnick);
                    }
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "JOIN")
                {

                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    rnick = tmparr[0];
                    if (canGreet == true)
                    {
                        FervorLibrary.Library Greetings = new FervorLibrary.Library();
                        Random rand = new Random();
                        int indexgreet = rand.Next(0, Greetings.greetnumber);
                        string greeting = Greetings.Greeting(rnick, indexgreet);
                        string greetingmessage = "PRIVMSG " + channel + " :" + greeting;
                        send(greetingmessage);
                    }
                    bool newMessages = hasMessages(rnick);
                    if (newMessages == true)
                    {
                        sayMessages(rnick);
                    }
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "PART" | mail.Substring(mail.IndexOf(" ") + 1, 4) == "QUIT")
                {

                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    rnick = tmparr[0];
                    if (canGreet == true)
                    {
                        FervorLibrary.Library Farewells = new FervorLibrary.Library();
                        Random rand = new Random();
                        FervorLibrary.Library FervLib = new FervorLibrary.Library();
                        int indexfarewell = rand.Next(0, FervLib.farewellnumber);
                        string farewell = Farewells.Farewell(rnick, indexfarewell);
                        string farewellmessage = "PRIVMSG " + channel + " :" + farewell;
                        send(farewellmessage);
                    }
                    Thread deactive = new Thread(deactivateUser);
                    deactive.Start();
                    ObsidianFunctions.Functions ObsidBot = new ObsidianFunctions.Functions();
                    if (rnick == ObsidBot.talkingTo())
                    {
                        ObsidBot.settalkingTo("nobody"); 
                    }

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
                    deactivateUser();
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "NICK")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    rnick = tmparr[0];
                    tmparr = mail.Split(':');
                    string newnick = tmparr[1];
                    deactivateUser();
                    bool newMessages = hasMessages(newnick);
                    if (newMessages == true)
                    {
                        sayMessages(newnick);
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
                FervLib.greetnumber = split.Length / 2;
            }
            else if (System.IO.File.Exists("greet.bin") == true)
            {
                System.IO.StreamReader greetread = new System.IO.StreamReader("greet.bin");
                string greetraw = greetread.ReadToEnd();
                string[] split = greetraw.Split(',');
                FervorLibrary.Library FervLib = new FervorLibrary.Library();
                FervLib.greetnumber = split.Length / 2;
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
                FervLib.farewellnumber = split.Length / 2;
            }
            else if (System.IO.File.Exists("farewell.bin") == true)
            {
                System.IO.StreamReader farewellread = new System.IO.StreamReader("farewell.bin");
                string farewellraw = farewellread.ReadToEnd();
                string[] split = farewellraw.Split(',');
                FervorLibrary.Library FervLib = new FervorLibrary.Library();
                FervLib.farewellnumber = split.Length / 2;
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
        public void generateMD5message()
        {
            string query = rmsg.Remove(0, 5);
            try
            {
                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                string md5encrypt = ObsidFunc.md5calc(query.ToString()).ToLower();
                string response = "PRIVMSG " + channel + " :" + md5encrypt;
                send(response);
            }
            catch (Exception ex)
            {
                send("PRIVMSG " + channel + " :Something went wrong: " + ex);
            }
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
        public void reqreguser()
        {
            string query = rmsg.Remove(0, 10);
            ObsidianFunctions.Functions requestreg = new ObsidianFunctions.Functions();
            string pass = requestreg.reqreguser(rnick, query);
            send("PRIVMSG " + rnick + " :You have been requested registration - user=" + rnick + " password=" + pass + " (hashed)");
        }
        public void listregs()
        {
            bool nickuser = isActiveUser(rnick);
            if (nickuser == true)
            {
                StreamReader sr = new StreamReader("registers.bin");
                string[] regusersarray = sr.ReadToEnd().Split('|');
                string regusers = regusersarray[0];
                sr.Close();
                if (regusers == "" || regusers == null)
                {
                    regusers = "None";
                }
                say(channel, regusers);
            }
            else
            {
                say(channel, "Insufficient permissions!");
            }
        }
        public void say(string saychannel, string message)
        {
            send("PRIVMSG " + saychannel + " :" + message);
        }
        public void clearregs()
        {
            System.IO.StreamReader sr = new StreamReader(".activeusers");
            string[] users = sr.ReadToEnd().Split(':');
            sr.Close();
            foreach (string x in users)
            {
                if (x.Contains(rnick))
                {
                    System.IO.File.Delete("registers.bin");
                    System.IO.StreamWriter sw = new StreamWriter("registers.bin");
                    sw.Write("|");
                    sw.Close();
                    send("PRIVMSG " + channel + " :Cleared!");
                }

            }
        }
        public void activateUser()
        {
            StreamReader sr2 = new StreamReader(".activeusers");
            old = sr2.ReadToEnd();
            sr2.Close();
            System.IO.StreamReader sr = new StreamReader("users.bin");
            string[] registeredusers = sr.ReadToEnd().Split(':');
            sr.Close();
            foreach (string x in registeredusers)
            {
                if (x == rnick)
                {
                    string query = rmsg.Remove(0, 8);
                    int indexuser = Array.IndexOf(registeredusers, rnick);
                    ObsidianFunctions.Functions obsidfunc = new ObsidianFunctions.Functions();
                    bool passcorrect = obsidfunc.isVerified(indexuser, query);
                    StreamWriter sw = new StreamWriter(".activeusers");
                    if (passcorrect == true)
                    {
                        sw.Write(old + rnick + ":");
                        send("PRIVMSG " + rnick + " :Success! You are logged in!");
                    }

                    sw.Close();
                }
            }
        }
        public void deactivateUser()
        {
            StreamReader sr = new StreamReader(".activeusers");
            string[] listofactiveusers = sr.ReadToEnd().Split(':');
            sr.Close();
            foreach (string x in listofactiveusers)
            {
                if (x == rnick)
                {
                    listofactiveusers = listofactiveusers.Where(val => val != rnick).ToArray();
                    string listuser = String.Join(":", listofactiveusers) + ":";
                    StreamWriter sw = new StreamWriter(".activeusers");
                    sw.Write(listuser);
                    sw.Close();
                    send("PRIVMSG " + rnick + " :User has been deactivated!");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
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
                string[] badlist = sr.ReadToEnd().Split(':');
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

        public void listUsers()
        {
            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
            string listusers = ObsidFunc.userlist();
            send("PRIVMSG " + channel + " :" + listusers);
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
        public void botChat()
        {
            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
            chatBot = new Bot();
            chatBot.loadSettings();
            chatUser = new User(ObsidFunc.talkingTo(), chatBot);
            chatBot.loadAIMLFromFiles();
            chatBot.isAcceptingUserInput = true;
        }
        public void generatesha1message()
        {
            string query = rmsg.Remove(0, 6);
            Functions ObsidFunc = new Functions();
            string hash = ObsidFunc.sha1calc(query);
            send("PRIVMSG " + channel + " :" + hash);
        }
        public void generatesha256message()
        {
            string query = rmsg.Remove(0, 8);
            Functions ObsidFunc = new Functions();
            string hash = ObsidFunc.sha256calc(query);
            send("PRIVMSG " + channel + " :" + hash);
        }
        public void generatesha384message()
        {
            string query = rmsg.Remove(0, 8);
            Functions ObsidFunc = new Functions();
            string hash = ObsidFunc.sha384calc(query);
            send("PRIVMSG " + channel + " :" + hash);
        }
        public void generatesha512message()
        {
            string query = rmsg.Remove(0, 8);
            Functions ObsidFunc = new Functions();
            string hash = ObsidFunc.sha512calc(query);
            send("PRIVMSG " + channel + " :" + hash);
        }
        public bool isBlacklisted(string name)
        {
            bool returnbool = false;
            foreach (string x in blacklist)
            {
                if (x == name)
                {
                    returnbool = true;
                }
            }
            return returnbool;
        }
        public void setBlacklist()
        {
            StreamReader sr = new StreamReader("blacklist.bin");
            string[] blockedusers = sr.ReadToEnd().Split(':');
            sr.Close();
            blacklist = blockedusers;
        }
        public void listBlacklist()
        {
            string stringblacklist = String.Join(":", blacklist);
            if (stringblacklist == "" || stringblacklist == null)
            {
                stringblacklist = "None";
            }
            send("PRIVMSG " + channel + " :" + stringblacklist);
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
        public void channelUsers()
        {
            //start me on a new thread
            send("NAMES " + channel);
            mail = recv().Replace("\0", "");
            string rawrespond = mail;
            rawrespond = rawrespond.Remove(0, 1);
            string[] tmprrespond = rawrespond.Split(':');
            ircuserlist = tmprrespond[1].Trim().Replace("@", "").Replace("+", "").Replace("~", "").Replace("&", "").Replace("%", ""); 
        }
        public void saychannelUsers()
        {
            //start me on a new thread and at the same time as channelUsers() and make me sleep longer
            Thread.Sleep(1000);
            say(channel, ircuserlist);
        }
        public bool isOnline(string nickname)
        {
            send("NAMES " + channel);
            mail = recv().Replace("\0", "");
            string rawrespond = mail;
            rawrespond = rawrespond.Remove(0, 1);
            string[] tmprrespond = rawrespond.Split(':');
            ircuserlist = tmprrespond[1].Trim().Replace("@", "").Replace("+", "").Replace("~", "").Replace("&", "").Replace("%", "");
            string[] channeluserarray = ircuserlist.Split(' ');
            bool nickOnline = false; 
            foreach (string x in channeluserarray)
            {
                if (x == nickname)
                {
                    nickOnline = true; 
                }
            }
            return nickOnline; 
        }
        public void addMessage(string recipient, string message)
        {
            StreamReader sr = new StreamReader("messages.bin");
            string[] messageread = sr.ReadToEnd().Split('|');
            sr.Close();
            string newrecipients = messageread[0] + recipient.Trim() + "~";
            string newmessages = messageread[1] + message.Trim() + "~";
            StreamWriter sw = new StreamWriter("messages.bin");
            sw.Write(newrecipients + "|" + newmessages);
            sw.Close(); 
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
                if (x == nickname)
                {
                    isRecipient = true; 
                }
            }
            return isRecipient; 
        }
        public void sayMessages(string recipient)
        {
            StreamReader sr = new StreamReader("messages.bin");
            string[] messageread = sr.ReadToEnd().Split('|');
            sr.Close();
            string[] messageusers = messageread[0].Split('~');
            string[] messages = messageread[1].Split('~');
            foreach (string x in messageusers)
            {
                if (x == recipient)
                {
                    int recipindex = Array.IndexOf(messageusers, recipient);
                    say(recipient, messages[recipindex]);
                    messages = messages.Where(val => val != messages[recipindex]).ToArray();
                }
            }
            messageusers = messageusers.Where(val => val != recipient).ToArray();
            string newuserlist = String.Join("~", messageusers);
            string newmessagelist = String.Join("~", messages);
            StreamWriter sw = new StreamWriter("messages.bin");
            sw.Write(newuserlist + "|" + newmessagelist);
            sw.Close(); 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            say(nick, "!active " + textBox6.Text);
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
            timer1.Enabled = false; 
        }
        
    }
}

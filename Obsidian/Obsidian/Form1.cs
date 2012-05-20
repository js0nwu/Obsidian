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
        string owner;
        string server;
        string channel;
        System.Net.Sockets.Socket sock;
        string mail;
        int greetnumber;
        string rmsg;
        int farewellnumber;
        string password;
        bool isregistered;
        string rnick;
        string old;
        bool isOperator;
        string ownernick;
        string oldmail;
        int spamcount;
        bool isLogging;
        string talkingTo;
        Bot chatBot;
        User chatUser;
        System.Timers.Timer updatetmr;
        bool canGreet; 

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
            owner = "Obsidian";
            System.Net.IPHostEntry ipHostInfo = System.Net.Dns.GetHostEntry(server);
            System.Net.IPEndPoint EP = new System.Net.IPEndPoint(ipHostInfo.AddressList[0], port);
            sock = new System.Net.Sockets.Socket(EP.Address.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            sock.Connect(server, port);
            send("NICK " + nick);
            send("USER " + nick + " 0 * :ObsidianBot");
            send("JOIN " + channel);
            send("MODE " + nick + " +B");
            updatetmr.Enabled = true; 
            timer2.Enabled = true;
            if (textBox6.Text != null)
            {
                timer3.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            spamcount = 0;
            loadIRCInfo();
            isOperator = false;
            Thread configGreet = new Thread(GreetConfig);
            configGreet.Start();
            Thread configFarewell = new Thread(FarewellConfig);
            configFarewell.Start();
            if (System.IO.File.Exists("users.bin") == false | System.IO.File.Exists("passwords.bin") == false | System.IO.File.Exists("registers.bin") == false | System.IO.File.Exists(".activeusers") == false)
            {
                Thread configUser = new Thread(startUserList);
                configUser.Start();
            }
            StreamWriter sw = new StreamWriter(".activeusers");
            sw.Close();
            if (System.IO.File.Exists("owner.bin") == false)
            {
                ownerConfiguration();
            }
            else
            {
                setOwner();
            }
            isLogging = false;
            talkingTo = "nobody";
            botChat();
            updatetmr = new System.Timers.Timer(500);
            updatetmr.Elapsed += new System.Timers.ElapsedEventHandler(ircupdate);
            updatetmr.Interval = 500;
            canGreet = true; 
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
            string mail = System.Text.ASCIIEncoding.UTF8.GetString(data).Replace("\0", "");
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
                    rmsg = tmparr[1];
                    if (rmsg.Contains("!respond") == true)
                    {
                        string response = "PRIVMSG " + textBox3.Text + " :Response";
                        send(response);
                    }
                    else if (rmsg.Contains("!greet "))
                    {
                        string query = rmsg.Remove(0, 7);
                        try
                        {
                            int queryindex = Int32.Parse(query);
                            if (queryindex <= greetnumber)
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
                            if (queryindex <= greetnumber)
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
                        Thread md5calc = new Thread(generateMD5message);
                        md5calc.Start();
                    }
                    else if (rmsg.Contains("!register "))
                    {
                        Thread requestregistration = new Thread(reqreguser);
                        requestregistration.Start();
                    }
                    else if (rmsg.Contains("!registerlist"))
                    {
                        Thread listreg = new Thread(listregs);
                        listreg.Start();
                    }
                    else if (rmsg.Contains("!clearregisterlist"))
                    {
                        Thread clearreg = new Thread(clearregs);
                        clearreg.Start();
                    }
                    else if (rmsg.Contains("!active "))
                    {
                        Thread activate = new Thread(activateUser);
                        activate.Start();
                    }
                    else if (rmsg.Contains("!deactivate"))
                    {
                        Thread deactive = new Thread(deactivateUser);
                        deactive.Start();
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
                    else if (rmsg.Contains("userlist"))
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
                        if (isOperator == true)
                        {
                            string query = rmsg.Remove(0, 8);
                            bool nickisuser = isActiveUser(rnick);
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
                        if (isOperator == true)
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
                        if (isOperator == true)
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
                        if (rnick == ownernick && nickuser == true)
                        {
                            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                            ObsidFunc.batch(query);
                            send("PRIVMSG " + channel + " :Success!");
                        }
                        else
                        {
                            send("PRIVMSG " + channel + " :Insufficient Permissions!");
                        }
                    }
                    else if (rmsg.Contains("!log start"))
                    {
                        bool nickuser = isActiveUser(rnick);
                        if (nickuser == true)
                        {
                            isLogging = true;
                            send("PRIVMSG " + channel + " :Success!");
                        }
                        else
                        {
                            send("PRIVMSG " + channel + " :Insufficient Permissions!");
                        }
                    }
                    else if (rmsg.Contains("!log stop"))
                    {
                        bool nickuser = isActiveUser(rnick);
                        if (nickuser == true)
                        {
                            isLogging = false;
                            send("PRIVMSG " + channel + " :Success!");
                        }
                        else
                        {
                            send("PRIVMSG " + channel + " :Insufficient Permissions!");
                        }
                    }
                    else if (rmsg.Contains("!calc "))
                    {
                        ObsidianFunctions.Functions ObsidFunc= new ObsidianFunctions.Functions();
                        string query = rmsg.Remove(0, 6);
                        float answer = ObsidFunc.calc(query);
                        send("PRIVMSG " + channel + " :" + answer.ToString()); 
                    }
                    else if (rmsg.Contains("!isOperator true"))
                    {
                        bool nickuser = isActiveUser(rnick);
                        if (nickuser == true)
                        {
                            isOperator = true;
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
                            isOperator = false;
                            send("PRIVMSG " + channel + " :isOperator = false"); 
                        }
                        else
                        {
                            send("PRIVMSG " + channel + " :Insufficient Permissions!");
                        }
                    }
                    else if (rmsg.Contains("!botchat"))
                    {
                        if (talkingTo != "nobody")
                        {
                            send("PRIVMSG " + rnick + " :Sorry, I'm already talking with someone");
                        }
                        else
                        {
                            talkingTo = rnick; 
                            FervorLibrary.Library Greetings = new FervorLibrary.Library();
                            Random rand = new Random();
                            int indexgreet = rand.Next(0, greetnumber);
                            string greeting = Greetings.Greeting(rnick, indexgreet);
                            send("PRIVMSG " + rnick + " :" + greeting);
                            botChat(); 
                        }
                        
                    }
                    else if (rmsg.Contains("quit"))
                    {
                        if (rnick == talkingTo)
                        {
                            talkingTo = "nobody"; 
                        }
                    }
                    else if (rnick == talkingTo)
                    {

                        Request r = new Request(rmsg, chatUser, chatBot);
                        Result res = chatBot.Chat(r);
                        send("PRIVMSG " + talkingTo + " :" + res.Output);

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
                    detectLang();
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "JOIN")
                {
                    
                        string[] tmparr = null;
                        mail = mail.Remove(0, 1);
                        tmparr = mail.Split('!');
                        string rnick = tmparr[0];
                        if (canGreet == true)
                        {
                        FervorLibrary.Library Greetings = new FervorLibrary.Library();
                        Random rand = new Random();
                        int indexgreet = rand.Next(0, greetnumber);
                        string greeting = Greetings.Greeting(rnick, indexgreet);
                        string greetingmessage = "PRIVMSG " + textBox3.Text + " :" + greeting;
                        send(greetingmessage);
                    }
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "PART" | mail.Substring(mail.IndexOf(" ") + 1, 4) == "QUIT")
                {
                    
                        string[] tmparr = null;
                        mail = mail.Remove(0, 1);
                        tmparr = mail.Split('!');
                        string rnick = tmparr[0];
                    if (canGreet == true)
                    {
                        FervorLibrary.Library Farewells = new FervorLibrary.Library();
                        Random rand = new Random();
                        int indexfarewell = rand.Next(0, farewellnumber);
                        string farewell = Farewells.Farewell(rnick, indexfarewell);
                        string farewellmessage = "PRIVMSG " + textBox3.Text + " :" + farewell;
                        send(farewellmessage);
                    }
                        Thread deactive = new Thread(deactivateUser);
                        deactive.Start();
                        if (rnick == talkingTo)
                        {
                            talkingTo = "nobody";
                        }
                    
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "MODE")
                {
                    mail = mail.Replace("\0", "").Trim();
                    int nameopslength = nick.Length + 3;
                    string action = mail.Substring(mail.Length - nameopslength);
                    if (action.StartsWith("+o") | action.StartsWith("+r"))
                    {
                        isOperator = true;
                        send("PRIVMSG " + channel + " :isOperator = true");

                    }
                    else if (action.StartsWith("-o") | action.StartsWith("-r"))
                    {
                        isOperator = false;
                        send("PRIVMSG " + channel + " :isOperator = false");
                    }

                }
            }
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
                greetnumber = split.Length / 2;
            }
            else if (System.IO.File.Exists("greet.bin") == true)
            {
                System.IO.StreamReader greetread = new System.IO.StreamReader("greet.bin");
                string greetraw = greetread.ReadToEnd();
                string[] split = greetraw.Split(',');
                greetnumber = split.Length / 2;
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
                farewellnumber = split.Length / 2;
            }
            else if (System.IO.File.Exists("farewell.bin") == true)
            {
                System.IO.StreamReader farewellread = new System.IO.StreamReader("farewell.bin");
                string farewellraw = farewellread.ReadToEnd();
                string[] split = farewellraw.Split(',');
                farewellnumber = split.Length / 2;
            }
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
            if (isLogging == true)
            {
                Thread messagelog = new Thread(logMsg);
                messagelog.Start();
            }
            
        }
        public void ircupdate(object source, System.Timers.ElapsedEventArgs e)
        {
            mail = recv().Replace("\0", "").Trim();

            if (mail == oldmail)
            {
                spamcount++;
                if (spamcount >= 4)
                {
                    if (isOperator == true)
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
            oldmail = mail;
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
            sw1.Close();
            StreamWriter sw2 = new StreamWriter("passwords.bin");
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
            System.IO.StreamReader sr = new StreamReader(".activeusers");
            string[] users = sr.ReadToEnd().Split(':');
            sr.Close();
            foreach (string x in users)
            {
                if (x.Contains(rnick))
                {
                    StreamReader sr2 = new StreamReader("registers.bin");
                    string[] registered = sr2.ReadToEnd().Split('|');
                    string usernames = registered[0];
                    sr2.Close();
                    
                    send("PRIVMSG " + channel + " :" + usernames);
                    
                }
                
            }
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
                if (x.Contains(rnick))
                {
                    string query = rmsg.Remove(0, 8);
                    int indexuser = Array.IndexOf(registeredusers, rnick);
                    ObsidianFunctions.Functions obsidfunc = new ObsidianFunctions.Functions();
                    bool passcorrect = obsidfunc.isVerified(indexuser, query);
                    StreamWriter sw = new StreamWriter(".activeusers");
                    if (passcorrect == true)
                    {
                        sw.Write(old + rnick + ":");
                        send("PRIVMSG " + channel + " :Success! You are logged in!");
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
                if (x.Contains(rnick))
                {
                    listofactiveusers = listofactiveusers.Where(val => val != rnick).ToArray();
                    string listuser = null;
                    foreach (string y in listofactiveusers)
                    {
                        listuser = listuser + y + ":";
                    }
                    StreamWriter sw = new StreamWriter(".activeusers");
                    sw.Write(listuser);
                    sw.Close();
                    send("PRIVMSG " + channel + " :User has been deactivated!");
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
                if (x.Contains(nickname))
                {
                    userbool = true;
                }
            }
            return userbool;
        }
        public void ownerConfiguration()
        {
            ownerConfig form2 = new ownerConfig();
            form2.Show();
        }
        public void setOwner()
        {
            StreamReader sr = new StreamReader("owner.bin");
            ownernick = sr.ReadToEnd();

        }
        public void detectLang()
        {
            if (System.IO.File.Exists("badlang.bin") == true)
            {
                StreamReader sr = new StreamReader("badlang.bin");
                string[] badlist = sr.ReadToEnd().Split(':');
                foreach (string x in badlist)
                {
                    if (rmsg.Contains(x) == true)
                    {
                        if (isOperator == true)
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
            textBox7.Text = ""; 
        }
        public void botChat()
        {
            
            chatBot = new Bot();
            chatBot.loadSettings();
            chatUser = new User(talkingTo, chatBot);
            chatBot.loadAIMLFromFiles();
            chatBot.isAcceptingUserInput = true;
        }

    }
}

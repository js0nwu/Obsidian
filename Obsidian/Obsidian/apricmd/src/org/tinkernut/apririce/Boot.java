package org.tinkernut.apririce;

import java.io.BufferedReader;
import java.io.File;
import java.io.BufferedWriter;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.HashMap;
import java.util.LinkedList;
import java.lang.Class;
import org.tinkernut.apririce.commands.Command;
import org.tinkernut.apririce.textUtils.Parser;
import org.tinkernut.apririce.textUtils.TextBuffer;
import jerklib.ConnectionManager;
import jerklib.Profile;
import jerklib.events.IRCEvent;
import jerklib.events.JoinCompleteEvent;
import jerklib.events.JoinEvent;
import jerklib.events.MessageEvent;
import jerklib.events.QuitEvent;
import jerklib.events.IRCEvent.Type;
import jerklib.listeners.IRCEventListener;

public class Boot {

	static String channel = "";
	static String rnick = "";
	static String rmsg = ""; 
	public static HashMap<String, Command> commandsMap;
	static String CMD_START = "@"; 
	public static void main(String[] args) {
		String channel = args[0];
		String rnick = args[1];
		String rmsg = args[2]; 
		
		Boot(channel, rnick, rmsg);
		/*
		while (true) {
			int lastThreadCount = Thread.activeCount();

			for (int i = 0; i < 8; i++) {				
				if (Thread.activeCount() == i && lastThreadCount != i) {
					System.out.println("Total thread count changed to "+ i);
				}

			}
		}
		*/
	}
	public static void Boot(String channel2, String rnick2, String rmsg2) {
		// TODO Auto-generated method stub
		// Initialize globals		
				//System.out.println("test1"); 
				channel = channel2; 
				rnick = rnick2; 
				rmsg = rmsg2; 
				commandsMap = new HashMap<String, Command>();
				File commandsDirectory = new File("src/org/tinkernut/apririce/commands/");
				// If directory doesn't exist
				if (!commandsDirectory.exists()) {
				System.out.println("There are no commands installed. Create or get some commands and place them in src/org/tinkernut/apririce/commands\nExiting.");
				commandsDirectory.mkdirs();
				// Exist completely if doesn't exist
				System.exit(1);
				}
				// And if it does exist...
				int numberOfCommands = commandsDirectory.listFiles().length;
				// Each file into commandsFilesArray array
				File commandsFilesArray[] = new File("src/org/tinkernut/apririce/commands/").listFiles();
				String className = "";
				//System.out.println("test2"); 
				for (int i = 0; i < numberOfCommands; i++) {
				try {
				className = commandsFilesArray[i].getName().substring(0, commandsFilesArray[i].getName().indexOf("."));
				//System.out.println("test3"); 
				if (className.equals("Command")) {
					//System.out.println("test4"); 
				continue;
				}

				// File string to Command type
				@SuppressWarnings("rawtypes")
				
				Class c = Class.forName("org.tinkernut.apririce.commands." + className);
				//System.out.println(className); 
				//System.out.println("test5"); 
				if (className.contains("Command")) {	
					//System.out.println("test6"); 
					Command cmd = (Command) c.newInstance();
					//System.out.println("test7"); 
					String nameClass = className.substring(0, className.indexOf("Command")).toLowerCase(); 
					//System.out.println("test8"); 
				commandsMap.put(nameClass, cmd);
				//System.out.println("test9"); 
				}
				} catch (ClassNotFoundException e) {
				System.out.println("Internal error; class " + className +" not found.");
				e.printStackTrace();
				} 
				catch (InstantiationException e) {
				System.out.println("Internal error; class " + className +" not found.");
				e.printStackTrace();
				} catch (IllegalAccessException e) {
				System.out.println("Internal error; class " + className +" not found.");
				e.printStackTrace();
				} 
				
				catch (ClassCastException e) {
				System.out.println(className + " does not extend Command. Skipping.");
				continue;
				}
				}
				receiveEvent(channel, rnick, rmsg); 
	}
	public static void receiveEvent(String channel, String rnick, String rmsg) {
		
			String me = rmsg;
			if (me.startsWith(CMD_START)) {			
				//Check and execute any commands
				String commandString = Parser.stripCommand(me);
				MessageEvent meNull = null; 
				if (commandsMap.containsKey(commandString)) {
					Boot bootNull = null; 
					commandsMap.get(commandString).initPriv(Parser.stripArguments(me), meNull, bootNull);
					//System.out.println("PRIVMSG " + rnick + " :is a command.");
					//System.out.println("test10"); 
					Thread thread = new Thread(commandsMap.get(commandString));
					thread.start();
					//System.out.println("test11"); 
				}
				else {
					//me.getSession().sayPrivate(me.getNick(), "Not a command.");
					System.out.println("Not a command."); 
				}
			}
		//}
	}
}

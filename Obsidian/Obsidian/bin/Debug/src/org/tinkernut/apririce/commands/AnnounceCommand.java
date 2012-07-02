package org.tinkernut.apririce.commands;

import java.util.Timer;
import java.util.TimerTask;
import org.tinkernut.apririce.textUtils.Parser;
import org.tinkernut.apririce.textUtils.TextBuffer;

public class AnnounceCommand extends Command{
	int interval = 0;
	String announcement = "";
	Timer timer;
	@SuppressWarnings(value = { "unused" })
	final private String helpText = "\"|announce set (time in seconds) -------- \" +\r\n" + 
			"					\"|announce set (words for announcement) -------- \" +\r\n" + 
			"					\"|announce start -------- \" +\r\n" + 
			"					\"|announce stop\", me";

	public void exec() {
		//set argument
		if (params.startsWith("set")) {
			//check if required arguments are missing
			if (Parser.stripArguments(params).equalsIgnoreCase("")) {
				TextBuffer.addAndDisplay("Enter an interval or announcement to set.", me);
			}
			//set interval or announcement
			else {
				int number;
				String words;
				try {
					number = Integer.parseInt(Parser.stripArguments(params));
					set(number);
				} catch (NumberFormatException e) {
					words = Parser.stripArguments(params);
					set(words);
				}
			}
		}
		else if (params.toLowerCase().startsWith("help")) {
			TextBuffer.addAndDisplay(getHelpText(), me);
		}
		//start argument
		if (params.startsWith("start")) {
			start();
		}
		if (params.startsWith("stop")) {
			timer.cancel();
			TextBuffer.addAndDisplay("Announcement stopped.", me);
		}
	}

	public void execPriv() {
		
	}

	public void set(int interval) {
		this.interval = interval*1000;
		TextBuffer.addAndDisplay("Interval succsessfuly set to: " + interval + " seconds", me);
	}

	public void set(String announcment) {
		this.announcement = announcment;
		TextBuffer.addAndDisplay("Announcement succsessfuly set to: " + this.announcement, me);
	}

	public void start() {
		if (interval == 0) {
			TextBuffer.addAndDisplay("Set an interval in seconds. |announce set <interval here(eg. 100, 300, 134, etc,.)>", me);
		}else if (announcement.equals("")) {
			TextBuffer.addAndDisplay("Set an announcement. |announce set <message here>", me);
		}else {			
			timer = new Timer();
			timer.scheduleAtFixedRate(new TimerTask() {
				public void run() {
					TextBuffer.addAndDisplay(announcement, me);
				}
			}, 0, interval);
		}

	}
}
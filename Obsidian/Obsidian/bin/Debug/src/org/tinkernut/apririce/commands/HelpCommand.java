package org.tinkernut.apririce.commands;

import org.tinkernut.apririce.textUtils.TextBuffer;

public class HelpCommand extends Command {
	@SuppressWarnings(value = { "unused" })
	final private String helpText = "I'm sorry Dave. I'm afraid I can't let you do that.";
	
	public void exec() {
		TextBuffer.addAndDisplay(getHelpText(), me);
	}

	public void execPriv() {
		System.out.println("This is the private secotr!");
	}
}

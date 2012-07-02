package org.tinkernut.apririce.commands;

import org.tinkernut.apririce.textUtils.Parser;
import org.tinkernut.apririce.textUtils.TextBuffer;

public class NickCommand extends Command{
	
	String helpText = "Do you REALLY need help with this? And no, you can't set the nick as \"help\".";

	@Override
	void exec() {
		if (Parser.getArgument(params, 1).equalsIgnoreCase("help")) {
			TextBuffer.addAndDisplay(helpText, me);
			return;
		}
		me.getSession().changeNick(params);
	}

	@Override
	void execPriv() {
		if (Parser.getArgument(params, 1).equalsIgnoreCase("help")) {
			me.getSession().sayPrivate(me.getNick(), helpText);
			return;
		}
		me.getSession().changeNick(params);
	}
}

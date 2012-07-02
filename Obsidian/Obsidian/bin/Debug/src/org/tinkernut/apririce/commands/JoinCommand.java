package org.tinkernut.apririce.commands;

import org.tinkernut.apririce.textUtils.Parser;

public class JoinCommand extends Command{

	@Override
	void exec() {
		me.getChannel().say(Integer.toString(Parser.getArgCount(params)));
//		Thread thread = new Thread(new Bot(Parser.stripArguments(params.toLowerCase()), Parser.stripArguments(params.toLowerCase()), Parser.stripArguments(params.toLowerCase())));
//		thread.start();
	}

	@Override
	void execPriv() {
		exec();
	}

}

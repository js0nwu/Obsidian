package org.tinkernut.apririce.commands;

import org.tinkernut.apririce.Boot;
import jerklib.events.MessageEvent;

public abstract class Command implements Runnable{
	//TODO: Pass User to constructor, hence finish user tracking!
	protected String params;
	protected MessageEvent me;
	final protected String helpText = "Some help text.";
	
	public void init(String params, MessageEvent me, Boot bot) {
		this.params = params;
		this.me = me;
	}
	
	public void initPriv(String params, MessageEvent me, Boot bot) {
		this.params = params;
		this.me = me;
	}
	public void run() {
		//if (me.getChannel() == null) {
		//	execPriv();
		//}else {
		//	exec();
		//}
		exec(); 
	}
	
	public String getHelpText() {
		return helpText;
	}
	
	abstract void exec();

	abstract void execPriv();
}
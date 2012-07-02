package org.tinkernut.apririce.commands;

public class LeaveCommand  extends Command{

	@Override
	void exec() {
		if (params.equals("") || params.equals("")) {
			me.getSession().close("Closed by command.");
		}
	}

	@Override
	void execPriv() {
		exec();
	}

}

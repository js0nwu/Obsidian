package org.tinkernut.apririce.textUtils;

public class Parser {
	public static String stripCommand(String cmd) {
		if (cmd.contains(" ")) {
			return cmd.substring(1, cmd.indexOf(" "));
		}
		else {
			return cmd.substring(1);
		}
	}
	public static String stripArguments(String args) {
		if (args.contains(" ")) {
			return args.substring(args.indexOf(" ")+1);
		}
		else {
			return "";
		}
	}
	@Deprecated
	public static String getFirstArgument(String args) {
		if (args.contains(" ")) {			
			return args.substring(0, args.indexOf(' '));
		}else {
			return args;
		}
	}

	public static String getArgument(String args, int argc) {
		String buffer = args;
		for (int i = 0; i < argc - 1; i++) {
			buffer = stripArguments(buffer);
		}
		if (buffer.contains(" ")) {
			buffer = buffer.substring(0, buffer.indexOf(" "));
		}
		return buffer;
	}

	@Deprecated
	public static int getArgCount(String args) {
		String buffer = args;
		int argC = 0;

		while (buffer.contains(" ")) {
			buffer = buffer.substring(buffer.indexOf(" ") + 1);
			argC++;
		}

		if (buffer.equals("") || buffer.equals(" ")) {
			return 0;
		}else {			
			return argC + 1;
		}
	}
}
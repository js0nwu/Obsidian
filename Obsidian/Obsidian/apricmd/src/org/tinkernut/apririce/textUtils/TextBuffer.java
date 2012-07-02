package org.tinkernut.apririce.textUtils;

import jerklib.events.MessageEvent;

public class TextBuffer {
	//TODO: Overload display() function as to take a User, as to display private messages.

	private static String tBuffer = "";

	public static final void add(final String text) {
		tBuffer += text;
	}
	public static final void addAndDisplay(final String text, MessageEvent me) {
		tBuffer += text;
		display();
	}
	public static final void addArray(final String[] textArray) {
		for (int i = 0; i < textArray.length; i++) {
			tBuffer += textArray[i] + " ";
		}
	}
	public static final void display() {
		//me.getChannel().say(tBuffer);
		System.out.println(tBuffer); 
		tBuffer = "";
	}
}
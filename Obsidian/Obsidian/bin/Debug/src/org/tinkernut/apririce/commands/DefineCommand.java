package org.tinkernut.apririce.commands;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.util.HashMap;
import java.util.Map;
import org.tinkernut.apririce.commands.Command;
import org.tinkernut.apririce.commands.website.characterReplacement;
import org.tinkernut.apririce.textUtils.Parser;
import org.tinkernut.apririce.textUtils.TextBuffer;

public class DefineCommand extends Command {
	URLConnection urlConnection;

	public void exec() {		
		Map<String, website> urlMap = new HashMap<String, website>();
		try {
			urlMap.put("urban", new website("<div class=\"definition\">", "</div><div class=\"example\">", characterReplacement.PERCENT, new URL("http://www.urbandictionary.com/define.php?term=")));
			urlMap.put("dic", new website("<div class=\"dndata\">", "</div></div><div class=\"luna-Ent\"><span class=\"dnindex\">", characterReplacement.PLUS, new URL("http://dictionary.reference.com/browse/")));
			urlMap.put("wiki", new website("</div><p>", ".", characterReplacement.PLUS, new URL("http://en.wikipedia.org/wiki/")));
		} catch (MalformedURLException e1) {
			TextBuffer.addAndDisplay("Malformed URL.", me);
			return;
		}

		if (params.contains(" ")) {			
			try {

				if (urlMap.containsKey(Parser.getArgument(params.toLowerCase(), 1))) {
					//Replace special characters in to be defined String
					String urlAddon = Parser.stripArguments(params);
					if (urlMap.get(Parser.getArgument(params, 1)).charReplacement.equals(characterReplacement.PERCENT)) {
						urlAddon = urlAddon.replace(" ", "%20");
						urlAddon = urlAddon.replace(",", "%2C");
					}if (urlMap.get(Parser.getArgument(params, 1)).charReplacement.equals(characterReplacement.PLUS)) {
						urlAddon = urlAddon.replace(" ", "+");
					}

					//Append definition String to url
					urlMap.get(Parser.getArgument(params, 1)).url = new URL(urlMap.get(Parser.getArgument(params, 1)).url.toString() + urlAddon); 

					//Establish connection and download HTML source
					urlConnection = urlMap.get(Parser.getArgument(params, 1)).url.openConnection();
					BufferedReader bReader = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
					TextBuffer.addAndDisplay("Connection successful.", me);

					//Put whole page source into a single line string
					String HTMLSource = "";

					while (!HTMLSource.contains("</html>")) {
						HTMLSource += bReader.readLine();
					}

					// Remove everything from HTMLSource except definition with unique HTML tags.
					String unusedStart = "";
					try {
						unusedStart = HTMLSource.substring(0, HTMLSource.indexOf(urlMap.get(Parser.getArgument(params, 1)).startingTag));						
					} catch (StringIndexOutOfBoundsException e) {
						TextBuffer.addAndDisplay("Definition doesn't exist, or error.", me);
						return;
					}

					String unusedEnd = HTMLSource.substring(HTMLSource.indexOf(urlMap.get(Parser.getArgument(params, 1)).endingTag, HTMLSource.indexOf(urlMap.get(Parser.getArgument(params, 1)).startingTag)));

					HTMLSource = HTMLSource.replace(unusedStart, "");
					HTMLSource = HTMLSource.replace(unusedEnd, "");
					HTMLSource = HTMLSource.replace("&quot;", "\"");
					HTMLSource = HTMLSource.replace("\n\t", " ");
					HTMLSource = HTMLSource.replace("<br/>", " ");
					HTMLSource = HTMLSource.replace("&amp;", "&");
					HTMLSource = HTMLSource.replace("  ", " ");
					if (!HTMLSource.endsWith(".")) {
						HTMLSource += ".";
					}

					String tagReplace = "";
					while (!tagReplace.equals(null)) {
						try {
							int startTag = HTMLSource.indexOf("<");							
							int endTag = HTMLSource.indexOf(">") + 1;
							tagReplace = HTMLSource.substring(startTag, endTag);
							HTMLSource = HTMLSource.replace(tagReplace, "");						
						} catch (IndexOutOfBoundsException e) {
							break;
						}
					}

					//Extract definition
					//					int start = 0;
					//					int end = HTMLSource.indexOf(urlMap.get(Parser.getFirstArgument(params)).endingTag, start);

					String definition = HTMLSource;
					TextBuffer.addAndDisplay(definition, me);
				}else {
					TextBuffer.addAndDisplay("Invalid site.", me);
				}
			} catch (IOException e) {
				TextBuffer.addAndDisplay("Unable to establish connection.", me);
			}
		}else if(!urlMap.containsKey(Parser.getArgument(params, 1))){			
			TextBuffer.addAndDisplay("Invalid site.", me);
		}else {			
			TextBuffer.addAndDisplay("Input something to define.", me);
		}
	}

	public void execPriv() {

	}
}

class website{
	String startingTag;
	String endingTag;
	characterReplacement charReplacement;

	URL url;

	public website(String startingTag, String endingTag, characterReplacement charReplacement, URL url) {
		this.startingTag = startingTag;
		this.endingTag = endingTag;
		this.charReplacement = charReplacement;
		this.url = url;
	}

	public enum characterReplacement{
		PERCENT, PLUS;
	}
}

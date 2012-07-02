package org.tinkernut.apririce.commands;

import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;
import java.net.URL;
import java.net.URLEncoder;

import org.tinkernut.apririce.commands.GoogleResults;
import org.tinkernut.apririce.textUtils.TextBuffer;


import com.google.gson.Gson;

public class GooglefirstCommand extends Command {

	@Override
	void exec() {
		String googlesearch = params;
			String google = "http://ajax.googleapis.com/ajax/services/search/web?v=1.0&q=";
		    String search = googlesearch;
		    String charset = "UTF-8";

		    URL url;
		    Reader reader;
			try {
				url = new URL(google + URLEncoder.encode(search, charset));
				reader = new InputStreamReader(url.openStream(), charset);
				
				GoogleResults results = new Gson().fromJson(reader, GoogleResults.class);
				
				// Show title and URL of 1st result.
				String str = results.getResponseData().getResults().get(0).getTitle() + " - " + results.getResponseData().getResults().get(0).getUrl();
				String str2 = str.replace("<b>", "");
				String searchresult = str2.replace("</b>", "");
				TextBuffer.addAndDisplay(searchresult, me);
			} catch (IOException e) {
				e.printStackTrace();
			}
	}

	@Override
	void execPriv() {
		System.out.println("Done!");
	}

}
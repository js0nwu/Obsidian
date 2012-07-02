package org.tinkernut.apririce.commands;

import java.math.BigInteger;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

import org.tinkernut.apririce.textUtils.Parser;
import org.tinkernut.apririce.textUtils.TextBuffer;

public class HashCommand extends Command{
	private final String helpText = "Available hashing algorithms: MD2, MD5, SHA-1, SHA-256, SHA-384, SHA-512. Format like so : |hash (algorithm) (text to hash) --- eg. |hash sha1 help";
	private MessageDigest mDigest;

	@Override
	void exec() {
		if (Parser.stripArguments(params).equals("")) {
			TextBuffer.addAndDisplay(helpText, me);
		}
		// MD2
		else if (Parser.getArgument(params, 1).equalsIgnoreCase("md2")) {
			try {
				mDigest = MessageDigest.getInstance("MD2");
				mDigest.update(Parser.stripArguments(params).getBytes());
				TextBuffer.addAndDisplay(new BigInteger(1, mDigest.digest()).toString(16), me);
			} catch (NoSuchAlgorithmException e) {
				System.out.println("Internal error, no such algorithm \"MD2\"");
				TextBuffer.addAndDisplay("Internal error, no such algorithm \"MD2\"", me);
				e.printStackTrace();
			}
		}
		// MD5
		else if (Parser.getArgument(params, 1).equalsIgnoreCase("md5")) {			
			try {
				mDigest = MessageDigest.getInstance("MD5");
				mDigest.update(Parser.stripArguments(params).getBytes());
				TextBuffer.addAndDisplay(new BigInteger(1, mDigest.digest()).toString(16), me);
			} catch (NoSuchAlgorithmException e) {
				System.out.println("Internal error, no such algorithm \"MD5\"");
				TextBuffer.addAndDisplay("Internal error, no such algorithm \"MD5\"", me);
			}
		}
		// SHA-1
		else if (Parser.getArgument(params, 1).equalsIgnoreCase("sha1") ||
				Parser.getArgument(params, 1).equalsIgnoreCase("sha-1")) {
			try {
				mDigest = MessageDigest.getInstance("SHA-1");
				mDigest.update(Parser.stripArguments(params).getBytes());
				TextBuffer.addAndDisplay(new BigInteger(1, mDigest.digest()).toString(16), me);
			} catch (NoSuchAlgorithmException e) {
				System.out.println("Internal error, no such algorithm \"SHA1\"");
				TextBuffer.addAndDisplay("Internal error, no such algorithm \"SHA1\"", me);
			}
		}
		// SHA-256
		else if (Parser.getArgument(params, 1).equalsIgnoreCase("sha256") ||
				Parser.getArgument(params, 1).equalsIgnoreCase("sha-256")) {
			try {
				mDigest = MessageDigest.getInstance("SHA-256");
				mDigest.update(Parser.stripArguments(params).getBytes());
				TextBuffer.addAndDisplay(new BigInteger(1, mDigest.digest()).toString(16), me);
			} catch (NoSuchAlgorithmException e) {
				System.out.println("Internal error, no such algorithm \"SHA-256\"");
				TextBuffer.addAndDisplay("Internal error, no such algorithm \"SHA-256\"", me);
			}
		}
		// SHA-384
		else if (Parser.getArgument(params, 1).equalsIgnoreCase("sha384") ||
				Parser.getArgument(params, 1).equalsIgnoreCase("sha-384")) {
			try {
				mDigest = MessageDigest.getInstance("SHA-384");
				mDigest.update(Parser.stripArguments(params).getBytes());
				TextBuffer.addAndDisplay(new BigInteger(1, mDigest.digest()).toString(16), me);
			} catch (NoSuchAlgorithmException e) {
				System.out.println("Internal error, no such algorithm \"SHA-384\"");
				TextBuffer.addAndDisplay("Internal error, no such algorithm \"SHA-384\"", me);
			}
		}
		// SHA-512
		else if (Parser.getArgument(params, 1).equalsIgnoreCase("sha512") ||
				Parser.getArgument(params, 1).equalsIgnoreCase("sha-512")) {
			try {
				mDigest = MessageDigest.getInstance("SHA-512");
				mDigest.update(Parser.stripArguments(params).getBytes());
				TextBuffer.addAndDisplay(new BigInteger(1, mDigest.digest()).toString(16), me);
			} catch (NoSuchAlgorithmException e) {
				System.out.println("Internal error, no such algorithm \"SHA-512\"");
				TextBuffer.addAndDisplay("Internal error, no such algorithm \"SHA-512\"", me);
			}
		}
		// Default case
		else {
			TextBuffer.addAndDisplay("No such algorithm is implemented (yet).", me);
		}
	}

	@Override
	void execPriv() {
	}

}

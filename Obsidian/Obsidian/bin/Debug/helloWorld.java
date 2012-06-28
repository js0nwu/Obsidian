

class helloWorld{
	public static void main(String[] args)
	{
	String channel = args[0];
	String rnick = args[1];
	String rmsg = args[2];
	System.out.println("PRIVMSG " + channel + " :Helloworld from a java!"); 
	}
}
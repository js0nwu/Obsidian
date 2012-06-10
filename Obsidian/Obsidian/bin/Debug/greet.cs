namespace CodeCompile
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine(args.Length);
            System.Console.WriteLine("CodeDOM test");
            System.Console.WriteLine("Line 2");
            ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
            System.Console.WriteLine(ObsidFunc.md5calc("test"));
            //System.Console.WriteLine(args[0]);
            FervorLibrary.Library FervLib = new FervorLibrary.Library();
            System.Console.WriteLine(FervLib.greet(0)); 
        }
    }
}
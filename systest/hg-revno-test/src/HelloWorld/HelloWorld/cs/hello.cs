using System.Reflection;

public static class Hello
{
	public static int Main(string[] args) 
	{
		System.Console.WriteLine("Test executable running");

	    return typeof (Hello).Assembly.GetName().Version.Revision;
	}
}
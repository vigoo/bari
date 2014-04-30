using System;
using System.IO;
using System.Reflection;

public static class Hello
{
	public static int Main(string[] args)
	{
	    var reader1 = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("HelloWorld.res1.txt"));
        var reader2 = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("HelloWorld.Subdir.res2.txt"));

        Console.WriteLine(reader1.ReadToEnd() + reader2.ReadToEnd());

		return 11;
	}
}
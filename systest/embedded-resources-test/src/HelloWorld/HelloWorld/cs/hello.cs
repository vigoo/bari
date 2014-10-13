using System;
using System.IO;
using System.Reflection;
using System.Windows;

public static class Hello
{
	public static int Main(string[] args)
	{
	    var reader1 = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("HelloWorld.res1.txt"));
        var reader2 = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("HelloWorld.Subdir.res2.txt"));

        Console.WriteLine(reader1.ReadToEnd() + reader2.ReadToEnd());

	    var reader3 = new StreamReader(Application.GetResourceStream(new Uri("HelloWorld;component/res3.txt", UriKind.Relative)).Stream);
        var reader4 = new StreamReader(Application.GetResourceStream(new Uri("HelloWorld;component/Inner/res4.txt", UriKind.Relative)).Stream);

        Console.WriteLine(reader3.ReadToEnd() + reader4.ReadToEnd());

		return 11;
	}
}
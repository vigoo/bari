using System;
using System.IO;
using HelloWorld.Dep;

public static class Hello
{
	public static int Main(string[] args) 
	{
		Console.WriteLine(dep.TEST);

		return 11;
	}
}
using System;
using System.IO;

public static class Hello
{
	public static int Main(string[] args) 
	{
		Console.WriteLine(File.ReadAllText(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "generated.txt")));		

		return 11;
	}
}
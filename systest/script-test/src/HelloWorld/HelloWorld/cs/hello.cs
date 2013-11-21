using System;
using System.IO;

public static class Hello
{
	public static int Main(string[] args) 
	{
		Console.WriteLine(File.ReadAllText(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "message.txt")));

		return 11;
	}
}
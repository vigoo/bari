public static class Hello
{
	public static int Main(string[] args) 
	{
		if (System.Environment.Is64BitProcess)
		{
			System.Console.WriteLine("64 bit");
			return 64;
		}
		else
		{
			System.Console.WriteLine("32 bit");	
			return 32;
		}
	}
}
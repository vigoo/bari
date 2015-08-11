using System;

namespace systests
{
    public class Program
    {
        public static int Main(string[] argv)
        {
            try
            {
                var test1 = new SysTests();
                test1.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("FAIL: " + ex.Message);
                return 1;
            }
        }
    }
}


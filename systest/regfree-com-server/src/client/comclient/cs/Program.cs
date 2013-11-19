using System;
using RegFreeComServerWrapper;

namespace ComClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new TestClassClass();
            Console.WriteLine("Hello {0}", server.Name);
        }
    }
}

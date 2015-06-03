namespace Proj11
{
    public static class Hello
    {
        public static int Main(string[] args)
        {
            System.Console.WriteLine("1{0}{1}", Proj21.dep.Prop, Proj22.dep.Prop);
            return 10;
        }
    }
}
using System.IO;
using System.Linq;
using System.Reflection;

public static class Hello
{
	public static int Main(string[] args)
	{
	    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
	    var dllpath = Path.Combine(path, "RuntimeDep.dll");

	    if (!File.Exists(dllpath))
	    {
	        System.Console.WriteLine("Runtime dependency is missing ({0})!", dllpath);
	        return 1;
	    }

        if (Assembly.GetExecutingAssembly().GetReferencedAssemblies().Any(a => a.Name == "RuntimeDep"))
        {
            System.Console.WriteLine("Dependency is not runtime only!");
            return 2;
        }

	    return 0;
	}
}
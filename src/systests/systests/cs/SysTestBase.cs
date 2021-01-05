using System;
using System.IO;
using System.Diagnostics;
using System.Linq;


namespace systests
{
    public class SysTestBase
    {
        protected readonly string root;
        private readonly string logs;
        private readonly string tmp;
        protected readonly bool isRunningOnMono = (Type.GetType ("Mono.Runtime") != null);


        public SysTestBase (string root)
        {
            this.root = root;
            logs = Path.Combine(root, "logs");
            tmp = Path.Combine(root, "tmp");
        }        

        protected void Log(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        protected void Steps(Action[] steps)
        {
            int failureCount = 0;
            foreach (var step in steps)
            {
                try
                {
                    step();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("FAIL: " + ex.Message);
                    failureCount++;
                } 
            }

            if (failureCount > 0)
                throw new SysTestException(failureCount + " tests failed");
        }

        protected void Initialize()
        {
            Log("..initializing..");

            if (Directory.Exists(logs))
                Directory.Delete(logs, true);
            if (Directory.Exists(tmp))
                Directory.Delete(tmp, true);

            Directory.CreateDirectory(logs);
            Directory.CreateDirectory(tmp);

            Log("done!\n");
        }

        protected void SimpleExeBuild(string name, string exePath, int expectedExitCode, string expectedOutput)
        {
            Log(".."+name+"..");
            Clean(name);
            Build(name);
            CheckExe(name, exePath, expectedExitCode, expectedOutput);
        }

        protected void ExeProductBuild(string name, string product, string exePath, int expectedExitCode, string expectedOutput)
        {
            Log(".."+name+"..");
            Clean(name);
            BuildProduct(name, product);
            CheckExe(name, exePath, expectedExitCode, expectedOutput);
        }

        protected void ExeBuildWithGoal(string name, string goal, string exePath, int expectedExitCode, string expectedOutput)
        {
            Log(".."+name+"..");
            CleanWithGoal(name, goal);
            BuildWithGoal(name, goal);
            CheckExe(name, exePath, expectedExitCode, expectedOutput);
        }

        protected void Clean(string name, string logPrefix = "")
        {
            RunBari(name, logPrefix + name + ".clean.log", new [] { "-v", "clean" });
        }

        protected void CleanWithGoal(string name, string goal, string logPrefix = "")
        {
            RunBari(name, logPrefix + name + ".clean.log", new [] { "-v", "--target", goal, "clean" });
        }

        protected void Build(string name, string logPrefix = "")
        {
            RunBari(name, logPrefix + name + ".build.log", new [] { "-v", "build" });
        }

        protected void BuildWithGoal(string name, string goal, string logPrefix = "")
        {
            RunBari(name, logPrefix + name + ".build.log", new [] { "-v", "--target", goal, "build" });
        }

        protected void BuildProduct(string name, string product, string logPrefix = "")
        {
            RunBari(name, logPrefix + name + ".build.log", new [] { "-v", "build", product });
        }

        protected void InternalCheckExe(string name, string exePath, int expectedExitCode, string expectedOutput)
        {
            var fullExePath = Path.Combine(root, name, exePath);
            var psi = new ProcessStartInfo {
                FileName = isRunningOnMono ? "mono" : fullExePath,
                Arguments = isRunningOnMono ? fullExePath : "",
                CreateNoWindow = true,
                WorkingDirectory = Path.Combine(root, name),
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (var process = Process.Start(psi))
            using (var outputStream = process.StandardOutput)
            {
                process.WaitForExit();
                string output = outputStream.ReadToEnd();               
                int exitCode = process.ExitCode;

                if (exitCode != expectedExitCode)
                {
                    throw new SysTestException("Exit code was: " + exitCode);
                }

                if (output != expectedOutput)
                {
                    throw new SysTestException("Output was: " + output);
                }
            }
        }

        protected void CheckExe(string name, string exePath, int expectedExitCode, string expectedOutput)
        {
            InternalCheckExe(name, exePath, expectedExitCode, expectedOutput);
            Log("OK\n");
        }

        protected int RunBari(string name, string logName, string[] args)
        {            
            var bariPath = Path.Combine(root, "..", "target", "full", "bari.exe");
            var psi = new ProcessStartInfo {
                FileName = isRunningOnMono ? "mono" : bariPath,
                Arguments = String.Join(" ", isRunningOnMono ? new [] { bariPath }.Concat(args) : args),
                CreateNoWindow = true,
                WorkingDirectory = Path.Combine(root, name),
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (var process = Process.Start(psi))
            using (var output = process.StandardOutput)
            {
                string text = output.ReadToEnd();
                process.WaitForExit();
                File.WriteAllText(Path.Combine(logs, logName), text);

                return process.ExitCode;
            }
        }
    }
}


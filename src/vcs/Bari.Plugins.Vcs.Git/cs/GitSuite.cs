using System.Diagnostics;
using System.IO;
using Bari.Core.Generic;

namespace Bari.Plugins.Vcs.Git
{
    public class GitSuite
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GitSuite));

        private readonly IFileSystemDirectory suiteRoot;
        private readonly IEnvironmentVariableContext environmentVariableContext;

        public GitSuite([SuiteRoot] IFileSystemDirectory suiteRoot, IEnvironmentVariableContext environmentVariableContext)
        {
            this.suiteRoot = suiteRoot;
            this.environmentVariableContext = environmentVariableContext;
        }

        public bool IsAvailable
        {
            get
            {
                var localRoot = suiteRoot as LocalFileSystemDirectory;
                if (localRoot != null)
                {
                    if (Directory.Exists(Path.Combine(localRoot.AbsolutePath, ".git")))
                    {
                        if (IsGitAvailable())
                        {
                            log.InfoFormat("Git support initialized");
                            return true;
                        }
                        else
                        {
                            log.WarnFormat("Suite seems to be in a git repository but git is not available");
                        }
                    }
                }

                return false;
            }
        }

        private static bool IsGitAvailable()
        {
            var output = RunGit("", "--version");
            return output != null && output.StartsWith("git version ");
        }

        private static string RunGit(string root, string arguments)
        {
            using (var process = Process.Start(
                new ProcessStartInfo("git", arguments)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = root
                }))
            {
                if (process != null)
                {
                    process.WaitForExit();
                    return process.StandardOutput.ReadToEnd();
                }
                else
                {
                    return null;
                }
            }
        }

        public void AddEnvironmentVariables()
        {
            var localRoot = suiteRoot as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                var output = RunGit(localRoot.AbsolutePath, "describe");
                log.Debug(output);
                if (output != null)
                {
                    var parts = output.Split('-');
                    if (parts.Length >= 2)
                    {
                        environmentVariableContext.Define("GIT_TAG", parts[0]);
                        environmentVariableContext.Define("GIT_REVNO", parts[1]);
                    }
                }
            }
        }
    }
}
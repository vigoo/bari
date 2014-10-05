﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.UI;
using Bari.Plugins.PythonScripts.Exceptions;
using IronPython.Compiler;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Bari.Plugins.PythonScripts.Scripting
{
    public class ProjectBuildScriptRunner : ScriptRunnerBase, IProjectBuildScriptRunner
    {
        private readonly IUserOutput output;

        public ProjectBuildScriptRunner([TargetRoot] IFileSystemDirectory targetRoot, IReferenceBuilderFactory referenceBuilderFactory, IBuildContextFactory buildContextFactory, IUserOutput output)
            : base(targetRoot, referenceBuilderFactory, buildContextFactory)
        {
            this.output = output;
        }

        /// <summary>
        /// Executes the given build script on the given project, returning the set of
        /// target files the script generated.
        /// 
        /// <para>
        /// The script's global scope has the following predefined variables set:
        /// - <c>project</c> refers to the project being built
        /// - <c>sourceSet</c> is the project's source set associated with the build script
        /// - <c>targetRoot</c> is the root target directory where all the files are generated
        /// - <c>targetDir</c> is where the project's output should be built
        /// </para>
        /// <para>
        /// A special global function <c>get_tool</c> is available which accepts a reference URI and
        /// a file name. 
        /// The same protocols are supported as for references in the suite definition. The 
        /// function's return value is the absolute path of the given file, where the tool
        /// has been deployed.
        /// </para>
        /// </summary>
        /// <param name="project">The input project to build</param>
        /// <param name="buildScript">The build script to execute</param>
        /// <returns>Returns the set of files generated by the script. They have to be
        /// indicated in the script's <c>results</c> variable, relative to <c>targetDir</c>.</returns>
        public ISet<TargetRelativePath> Run(Project project, IBuildScript buildScript)
        {
            var engine = CreateEngine();
            
            var runtime = engine.Runtime;
            try
            {
                var scope = runtime.CreateScope();

                scope.SetVariable("project", project);
                scope.SetVariable("sourceSet",
                                  project.GetSourceSet(buildScript.SourceSetName)
                                         .Files.Select(srp => (string) srp)
                                         .ToList());
                AddGetToolToScope(scope, project);

                var targetDir = TargetRoot.GetChildDirectory(project.Module.Name, createIfMissing: true);
                var localTargetDir = targetDir as LocalFileSystemDirectory;
                var localTargetRoot = TargetRoot as LocalFileSystemDirectory;
                if (localTargetDir != null && localTargetRoot != null)
                {
                    scope.SetVariable("targetRoot", localTargetRoot.AbsolutePath);
                    scope.SetVariable("targetDir", localTargetDir.AbsolutePath);

                    var pco = (PythonCompilerOptions)engine.GetCompilerOptions();
                    pco.Module |= ModuleOptions.Optimized;

                    try
                    {
                        var script = engine.CreateScriptSourceFromString(buildScript.Source, SourceCodeKind.File);
                        script.Compile(pco);
                        script.Execute(scope);

                        return new HashSet<TargetRelativePath>(
                            scope.GetVariable<IList<object>>("results")
                                .Cast<string>()
                                .Select(t => GetTargetRelativePath(targetDir, t)));
                    }
                    catch (Exception ex)
                    {
                        var eo = engine.GetService<ExceptionOperations>();

                        string msg, typeName;
                        eo.GetExceptionMessage(ex, out msg, out typeName);

                        foreach (var line in msg.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            output.Error(line);
                        }

                        output.Error("Call stack:");
                        foreach (var frame in eo.GetStackFrames(ex))
                        {
                            var line = frame.GetFileLineNumber();
                            var method = frame.GetMethodName();

                            output.Error(String.Format("Line {0} in {1}", line, method));
                        }

                        throw new ScriptException(buildScript);
                    }
                }
                else
                {
                    throw new NotSupportedException("Only local file system is supported for python scripts!");
                }
            }
            finally
            {
                runtime.Shutdown();
            }
        }
    }
}
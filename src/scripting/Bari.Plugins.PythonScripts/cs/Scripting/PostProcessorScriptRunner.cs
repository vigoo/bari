using System;
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
    public class PostProcessorScriptRunner : ScriptRunnerBase, IPostProcessorScriptRunner
    {
        private readonly IUserOutput output;
        private readonly IParameters parameters;

        public PostProcessorScriptRunner([TargetRoot] IFileSystemDirectory targetRoot, IReferenceBuilderFactory referenceBuilderFactory, IBuildContextFactory buildContextFactory, IUserOutput output, IParameters parameters) :
            base(targetRoot, referenceBuilderFactory, buildContextFactory)
        {
            this.output = output;
            this.parameters = parameters;
        }

        public ISet<TargetRelativePath> Run(IPostProcessorsHolder target, PostProcessDefinition definition, IPostProcessorScript postProcessorScript)
        {
            var engine = CreateEngine();
            var runtime = engine.Runtime;

            try
            {
                var scope = runtime.CreateScope();
                AddGetToolToScope(scope);
                scope.SetVariable("is_mono", parameters.UseMono);

                var targetDir = TargetRoot.GetChildDirectory(target.RelativeTargetPath, createIfMissing: true);
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
                        var script = engine.CreateScriptSourceFromString(postProcessorScript.Source, SourceCodeKind.File);
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

                        foreach (var line in msg.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries))
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

                        throw new ScriptException(postProcessorScript);
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
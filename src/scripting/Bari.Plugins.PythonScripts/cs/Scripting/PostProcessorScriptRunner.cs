using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using IronPython.Compiler;
using IronPython.Runtime;
using Microsoft.Scripting;

namespace Bari.Plugins.PythonScripts.Scripting
{
    public class PostProcessorScriptRunner : ScriptRunnerBase, IPostProcessorScriptRunner
    {
        public PostProcessorScriptRunner([TargetRoot] IFileSystemDirectory targetRoot, IReferenceBuilderFactory referenceBuilderFactory, IBuildContextFactory buildContextFactory) :
            base(targetRoot, referenceBuilderFactory, buildContextFactory)
        {
        }

        public ISet<TargetRelativePath> Run(IPostProcessorsHolder target, PostProcessDefinition definition, IPostProcessorScript postProcessorScript)
        {
            var engine = CreateEngine();
            var runtime = engine.Runtime;

            try
            {
                var scope = runtime.CreateScope();
                AddGetToolToScope(scope);

                var targetDir = TargetRoot.GetChildDirectory(target.RelativeTargetPath, createIfMissing: true);
                var localTargetDir = targetDir as LocalFileSystemDirectory;
                var localTargetRoot = TargetRoot as LocalFileSystemDirectory;
                if (localTargetDir != null && localTargetRoot != null)
                {
                    scope.SetVariable("targetRoot", localTargetRoot.AbsolutePath);
                    scope.SetVariable("targetDir", localTargetDir.AbsolutePath);
                    var pco = (PythonCompilerOptions)engine.GetCompilerOptions();
                    pco.Module |= ModuleOptions.Optimized;
                    var script = engine.CreateScriptSourceFromString(postProcessorScript.Source, SourceCodeKind.File);
                    script.Compile(pco);
                    script.Execute(scope);

                    return new HashSet<TargetRelativePath>(
                            scope.GetVariable<IList<object>>("results")
                                 .Cast<string>()
                                 .Select(t => GetTargetRelativePath(targetDir, t)));

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
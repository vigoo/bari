using System;
using System.IO;
using System.Linq;

namespace systests
{
    public class SysTests: SysTestBase
    {
        private readonly string debugOrDebugMono;
        
        public SysTests ()
            : base(Environment.CurrentDirectory)
        {
            var isMono = Type.GetType("Mono.Runtime") != null;
            debugOrDebugMono = isMono ? "debug-mono" : "debug";
        }

        public void Run()
        {
            Log("Executing system tests for bari\n");

            Steps(new Action[] 
            {
                () => Initialize(),
                () => ExeBuildWithGoal("single-cs-exe", debugOrDebugMono, Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 11, "Test executable running\n"),
                () => SimpleExeBuild("module-ref-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 10, "TEST\n"),
                () => SimpleExeBuild("module-ref-test-withrt", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 10, "TEST\n"),
                () => SimpleExeBuild("suite-ref-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 10, "TEST\n"),
                () => SimpleExeBuild("fsrepo-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 9, "Dependency acquired\n"),
                () => SimpleExeBuild("alias-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 9, "Dependency acquired\n"),
                () => ContentTest(),
                () => SimpleExeBuild("runtime-ref-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 0, ""),
                () => SimpleExeBuild("script-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 11, "Hello_base!!!\n\nHello_world!!!\n\n"),
                () => ExeProductBuild("postprocessor-script-test", "main", Path.Combine("target", "main", "HelloWorld.exe"), 11, "Hello_world!!!\n\n"),
                () => MultiSolutionTest()
            }.Concat(isRunningOnMono ? new Action[0] : new Action[] {
                () => SimpleExeBuild("embedded-resources-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 11, "Hello world!\nWPF hello world WPF!"),
                () => SimpleExeBuild("cpp-rc-support", Path.Combine("target", "Module1", "hello.exe"), 13, "Test C++ executable running"),
                () => SimpleExeBuild("mixed-cpp-cli", Path.Combine("target", "Module1", "hello.exe"), 11, "Hello world"),
                () => SimpleExeBuild("regfree-com-server", Path.Combine("target", "client", "comclient.exe"), 0, "Hello world"),
                () => SimpleExeBuild("single-cpp-exe", Path.Combine("target", "Module1", "hello.exe"), 13, "Test C++ executable running"),
                () => SimpleExeBuild("static-lib-test", Path.Combine("target", "test", "hello.exe"), 10, "Hello world!"),
                () => SimpleExeBuild("cpp-version", Path.Combine("target", "Module1", "hello.exe"), 11, "1.2.3.4\n1.2.3.4"),
                () => X86X64Test(),
                () => CppReleaseTest(),
                () => SimpleExeBuild("custom-plugin-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 11, "Hello base!!!\n\nHello world!!!\n"),
                // TODO: custom-plugin-test for mono
                () => SimpleExeBuild("single-fs-exe", Path.Combine("target", "Module", "Exe1.exe"), 12, "Test F# executable running\n"),
                // TODO: F# support with mono
            }).ToArray());
        }

        private void ContentTest()
        {
            Log("..content-test..");
            Clean("content-test");
            Build("content-test");
            InternalCheckExe("content-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 11, "Test executable running\n");

            var contentBeside = File.ReadAllText(Path.Combine(root, "content-test", "target", "HelloWorld", "content-beside-cs.txt"));
            if (contentBeside != "content-beside-cs")
                throw new SysTestException("Wrong content in content-beside-cs.txt: " + contentBeside);

            var additionalContent = File.ReadAllText(Path.Combine(root, "content-test", "target", "HelloWorld", "additional-content.txt"));
            if (additionalContent != "additional-content")
                throw new SysTestException("Wrong content in additional-content.txt: " + additionalContent);

            Log("OK\n");
        }

        private void X86X64Test()
        {
            Log("..x86-x64-test..");
            ExeBuildWithGoal("x86-x64-test", "debug-x86", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 32, "32 bit\n");
            ExeBuildWithGoal("x86-x64-test", "debug-x64", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 64, "64 bit\n");
            Log("OK\n");
        }

        private void CppReleaseTest()
        {
            Log("..cpp-release-test..");
            ExeBuildWithGoal("cpp-release-test", "custom-release", Path.Combine("target", "Module1", "hello.exe"), 13, "Test C++ executable running\n");
            Log("OK\n");
        }

        private void MultiSolutionTest()
        {
            Log("..multi-solution-test..");
            Clean("suite-ref-test", logPrefix: "multi-solution-test-1.1-");
            BuildProduct("suite-ref-test", "all", logPrefix: "multi-solution-test-1.1-");
            InternalCheckExe("suite-ref-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 10, "TEST\n");
            Directory.Delete(Path.Combine(root, "suite-ref-test", "target"), true);

            BuildProduct("suite-ref-test", "HelloWorld", logPrefix: "multi-solution-test-1.2-");
            InternalCheckExe("suite-ref-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 10, "TEST\n");
            Directory.Delete(Path.Combine(root, "suite-ref-test", "target"), true);

            BuildProduct("suite-ref-test", "all", logPrefix: "multi-solution-test-1.3-");
            InternalCheckExe("suite-ref-test", Path.Combine("target", "HelloWorld", "HelloWorld.exe"), 10, "TEST\n");

            Log("OK\n");
        }
    }
}


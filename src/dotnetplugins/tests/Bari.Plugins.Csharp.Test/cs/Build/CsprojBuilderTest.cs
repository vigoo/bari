using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Plugins.Csharp.Build;
using Bari.Plugins.VsCore.VisualStudio;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ninject;

namespace Bari.Plugins.Csharp.Test.Build
{
    [TestFixture]
    public class CsprojBuilderTest
    {
        private IKernel kernel;
        private Suite suite;
        private Project project;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("root")).WhenTargetHas
                <SuiteRootAttribute>();
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("target")).WhenTargetHas
                <TargetRootAttribute>();
            kernel.Bind<IProjectGuidManagement>().To<DefaultProjectGuidManagement>();

            suite = kernel.Get<Suite>();
            suite.Name = "test suite";

            var mod = suite.GetModule("testmod");
            project = mod.GetProject("test");

            kernel.Bind<Suite>().ToConstant(suite);
            kernel.Bind<Project>().ToConstant(project);
        }

        [TearDown]
        public void TearDown()
        {
            kernel.Dispose();
        }

        private class RefBuilder: IReferenceBuilder
        {
            private Reference reference;

            /// <summary>
            /// Dependencies required for running this builder
            /// </summary>
            public IDependencies Dependencies
            {
                get { return new NoDependencies(); }
            }

            /// <summary>
            /// Gets an unique identifier which can be used to identify cached results
            /// </summary>
            public string Uid
            {
                get { return reference.ToString(); }
            }

            /// <summary>
            /// Get the builders to be executed before this builder
            /// </summary>
            public IEnumerable<IBuilder> Prerequisites
            {
                get { return new IBuilder[0]; }
            }

            /// <summary>
            /// Runs this builder
            /// </summary>
            /// <param name="context">Current build context</param>
            /// <returns>Returns a set of generated files, in target relative paths</returns>
            public ISet<TargetRelativePath> Run(IBuildContext context)
            {
                return new HashSet<TargetRelativePath>();
            }

            public bool CanRun()
            {
                return true;
            }

            public Type BuilderType
            {
                get { return typeof (RefBuilder); }
            }

            /// <summary>
            /// Gets or sets the reference to be resolved
            /// </summary>
            public Reference Reference
            {
                get { return reference; }
                set { reference = value; }
            }

            /// <summary>
            /// If <c>false</c>, the reference builder can be ignored as an optimization
            /// </summary>
            public bool IsEffective
            {
                get { return true; }
            }
        }

        [Test]
        public void ChangeInReferenceListInvalidatesCsproj() // Issue #11
        {
            kernel.Bind<IReferenceBuilder>().To<RefBuilder>().Named("test");

            var context = new Mock<IBuildContext>();
            context.Setup(c => c.GetEffectiveBuilder(It.IsAny<IBuilder>())).Returns<IBuilder>(b => b);
            
            var builder1 = kernel.Get<CsprojBuilder>();
            project.AddReference(new Reference(new Uri("test://ref1"), ReferenceType.Build));
            var prereqs1 = builder1.Prerequisites.ToList(); // enforce lazy eval

            var fp0 = builder1.Dependencies.Fingerprint;

            project.AddReference(new Reference(new Uri("test://ref2"), ReferenceType.Build));
            var builder2 = kernel.Get<CsprojBuilder>();
            var prereqs2 = builder2.Prerequisites.ToList(); // enforce lazy eval

            var fp1 = builder2.Dependencies.Fingerprint;

            fp0.Should().NotBe(fp1);
        }
    }
}
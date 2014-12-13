using System;
using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Cache;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Test.Helper;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Cache
{
    [TestFixture]
    public class CachedBuilderTest
    {
        [Test]
        public void RunsBuilderOnlyOnceIfFingerprintRemains()
        {
            // Setting up the test
            var resultSet = new HashSet<TargetRelativePath>
                {
                    new TargetRelativePath(String.Empty, @"a\b\c"), 
                    new TargetRelativePath(String.Empty, @"c\d"), 
                    new TargetRelativePath(String.Empty, @"e")
                };

            var realBuilder = new Mock<IBuilder>();
            var realBuilderDeps = new Mock<IDependencies>();
            var initialFingerprint = new Mock<IDependencyFingerprint>();
            var buildContext = new Mock<IBuildContext>();
            var cache = new Mock<IBuildCache>();
            var targetDir = new TestFileSystemDirectory("target");

            realBuilderDeps.Setup(dep => dep.CreateFingerprint()).Returns(initialFingerprint.Object);
            
            realBuilder.Setup(b => b.Dependencies).Returns(realBuilderDeps.Object);
            realBuilder.Setup(b => b.Uid).Returns("");
            realBuilder.Setup(b => b.BuilderType).Returns(typeof(IBuilder));
            realBuilder.Setup(b => b.Run(buildContext.Object)).Returns(resultSet);
            realBuilder.Setup(b => b.CanRun()).Returns(true);

            // Creating the builder
            var cachedBuilder = new CachedBuilder(realBuilder.Object, cache.Object, targetDir);

            cachedBuilder.Dependencies.Should().Be(realBuilderDeps.Object);            
            
            // Running the builder for the first time
            var result1 = cachedBuilder.Run(buildContext.Object);

            // ..verifying
            result1.Should().BeEquivalentTo(resultSet);
            realBuilder.Verify(b => b.Run(buildContext.Object), Times.Once());

            // Modifying cache behavior
            cache.Setup(c => c.Contains(new BuildKey(realBuilder.Object.BuilderType, ""), initialFingerprint.Object)).Returns(true);
            cache.Setup(c => c.Restore(new BuildKey(realBuilder.Object.BuilderType, ""), targetDir, It.IsAny<bool>())).Returns(resultSet);

            // Running the builder for the second time
            var result2 = cachedBuilder.Run(buildContext.Object);

            // ..verifying
            result2.Should().BeEquivalentTo(resultSet);
            realBuilder.Verify(b => b.Run(buildContext.Object), Times.Once());
        }

        [Test]
        [ExpectedException(typeof (BuilderCantRunException))]
        public void ThrowsExceptionIfBuilderCannotRunAndNoCachedResults()
        {
            // Setting up the test
            var realBuilder = new Mock<IBuilder>();
            var realBuilderDeps = new Mock<IDependencies>();
            var initialFingerprint = new Mock<IDependencyFingerprint>();
            var buildContext = new Mock<IBuildContext>();
            var cache = new Mock<IBuildCache>();
            var targetDir = new TestFileSystemDirectory("target");

            realBuilderDeps.Setup(dep => dep.CreateFingerprint()).Returns(initialFingerprint.Object);

            realBuilder.Setup(b => b.Dependencies).Returns(realBuilderDeps.Object);
            realBuilder.Setup(b => b.Uid).Returns("");
            realBuilder.Setup(b => b.BuilderType).Returns(typeof(IBuilder));
            realBuilder.Setup(b => b.CanRun()).Returns(false);

            // Creating the builder
            var cachedBuilder = new CachedBuilder(realBuilder.Object, cache.Object, targetDir);

            // Running the builder for the first time
            cachedBuilder.Run(buildContext.Object);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowsExceptionIfBuilderThrowsExceptionAndNoCachedResults()
        {
            // Setting up the test
            var realBuilder = new Mock<IBuilder>();
            var realBuilderDeps = new Mock<IDependencies>();
            var initialFingerprint = new Mock<IDependencyFingerprint>();
            var buildContext = new Mock<IBuildContext>();
            var cache = new Mock<IBuildCache>();
            var targetDir = new TestFileSystemDirectory("target");

            realBuilderDeps.Setup(dep => dep.CreateFingerprint()).Returns(initialFingerprint.Object);

            realBuilder.Setup(b => b.Dependencies).Returns(realBuilderDeps.Object);
            realBuilder.Setup(b => b.Uid).Returns("");
            realBuilder.Setup(b => b.BuilderType).Returns(typeof(IBuilder));
            realBuilder.Setup(b => b.CanRun()).Returns(true);
            realBuilder.Setup(b => b.Run(It.IsAny<IBuildContext>())).Throws<InvalidOperationException>();

            // Creating the builder
            var cachedBuilder = new CachedBuilder(realBuilder.Object, cache.Object, targetDir);

            // Running the builder for the first time
            cachedBuilder.Run(buildContext.Object);
        }

        [Test]
        public void ReturnsCachedIfBuilderCannotRun()
        {
            // Setting up the test
            var resultSet = new HashSet<TargetRelativePath>
                {
                    new TargetRelativePath(String.Empty, @"a\b\c"), 
                    new TargetRelativePath(String.Empty, @"c\d"), 
                    new TargetRelativePath(String.Empty, @"e")
                };

            var realBuilder = new Mock<IBuilder>();
            var realBuilderDeps = new Mock<IDependencies>();
            var initialFingerprint = new Mock<IDependencyFingerprint>();
            var buildContext = new Mock<IBuildContext>();
            var cache = new Mock<IBuildCache>();
            var targetDir = new TestFileSystemDirectory("target");

            realBuilderDeps.Setup(dep => dep.CreateFingerprint()).Returns(initialFingerprint.Object);

            realBuilder.Setup(b => b.Dependencies).Returns(realBuilderDeps.Object);
            realBuilder.Setup(b => b.BuilderType).Returns(typeof (IBuilder));
            realBuilder.Setup(b => b.Uid).Returns("");
            realBuilder.Setup(b => b.Run(buildContext.Object)).Returns(resultSet);
            realBuilder.Setup(b => b.CanRun()).Returns(true);            

            // Creating the builder
            var cachedBuilder = new CachedBuilder(realBuilder.Object, cache.Object, targetDir);

            cache.Setup(c => c.ContainsAny(new BuildKey(realBuilder.Object.BuilderType, ""))).Returns(true);
            cache.Setup(c => c.Restore(new BuildKey(realBuilder.Object.BuilderType, ""), targetDir, It.IsAny<bool>())).Returns(resultSet);

            // Making it wrong
            realBuilder.Setup(b => b.CanRun()).Returns(false);
            realBuilder.Setup(b => b.Run(It.IsAny<IBuildContext>())).Throws<InvalidOperationException>();

            // And trying again:
            var result2 = cachedBuilder.Run(buildContext.Object);
            result2.Should().BeEquivalentTo(resultSet);
        }

        [Test]
        public void ReturnsCachedIfBuilderThrowsException()
        {
            // Setting up the test
            var resultSet = new HashSet<TargetRelativePath>
                {
                    new TargetRelativePath(String.Empty, @"a\b\c"), 
                    new TargetRelativePath(String.Empty, @"c\d"), 
                    new TargetRelativePath(String.Empty, @"e")
                };

            // Setting up the test
            var realBuilder = new Mock<IBuilder>();
            var realBuilderDeps = new Mock<IDependencies>();
            var initialFingerprint = new Mock<IDependencyFingerprint>();
            var buildContext = new Mock<IBuildContext>();
            var cache = new Mock<IBuildCache>();
            var targetDir = new TestFileSystemDirectory("target");

            realBuilderDeps.Setup(dep => dep.CreateFingerprint()).Returns(initialFingerprint.Object);

            realBuilder.Setup(b => b.Dependencies).Returns(realBuilderDeps.Object);
            realBuilder.Setup(b => b.Uid).Returns("");
            realBuilder.Setup(b => b.BuilderType).Returns(typeof(IBuilder));

            // Creating the builder
            var cachedBuilder = new CachedBuilder(realBuilder.Object, cache.Object, targetDir);

            cache.Setup(c => c.ContainsAny(new BuildKey(realBuilder.Object.BuilderType, ""))).Returns(true);
            cache.Setup(c => c.Restore(new BuildKey(realBuilder.Object.BuilderType, ""), targetDir, It.IsAny<bool>())).Returns(resultSet);

            // Making it wrong
            realBuilder.Setup(b => b.Run(It.IsAny<IBuildContext>())).Throws<InvalidOperationException>();

            // And trying again:
            var result2 = cachedBuilder.Run(buildContext.Object);
            result2.Should().BeEquivalentTo(resultSet);
        }
    }
}
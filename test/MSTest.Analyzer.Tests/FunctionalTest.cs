// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Analyzer.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Tests the <see cref="FakeValidTest"/> class to confirm that no
    /// warnings are found in the file, while using MSTest features.
    /// </summary>
    [TestClass]
    public class FunctionalTest : DiagnosticVerifier
    {
        /// <summary>
        /// Compiles, with the analyzers the code of the <see cref="FakeValidTest"/> class.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task FakeValidTest()
        {
            var diagnostics = await RunCompilationAsync("FakeValidTest.cs").ConfigureAwait(false);
            Assert.IsNotNull(diagnostics);
            if (diagnostics.Count() != 0)
            {
                string message = string.Join("\r\n", diagnostics.Select(d => d.GetMessage()));
                Assert.Fail("Some warnings have been raised:\r\n{0}", message);
            }
        }

        /// <summary>
        /// Compiles, with the analyzers the code of the <see cref="FakeInvalidTest"/> class.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task FakeInvalidTest()
        {
            var diagnostics = await RunCompilationAsync("FakeInvalidTest.cs").ConfigureAwait(false);
            Assert.IsNotNull(diagnostics);
            var actual = diagnostics.Select(d => d.GetMessage()).ToArray();

            var expected = new[]
            {
                "The method 'FakeInvalidTest.PublicAssemblyInitialize' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.PublicAssemblyCleanup' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.PublicClassInitialize' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.PublicClassCleanup' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.PublicTestInitialize' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.PublicTestCleanup' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.PublicTestMethod' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.PublicTestMethodWithCategory' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.PublicDataTestMethod' is public and should be marked with one of the test attribute",
                "The method 'FakeInvalidTest.ProtectedClassInitialize' should be public in order to be used as a test method",
                "The method 'FakeInvalidTest.ProtectedClassCleanup' should be public in order to be used as a test method",
                "The method 'FakeInvalidTest.ProtectedTestInitialize' should be public in order to be used as a test method",
                "The method 'FakeInvalidTest.ProtectedTestCleanup' should be public in order to be used as a test method",
                "The method 'FakeInvalidTest.ProtectedTestMethod' should be public in order to be used as a test method",
                "The method 'FakeInvalidTest.ProtectedDataTestMethod' should be public in order to be used as a test method",
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            throw new NotImplementedException();
        }

        private static async Task<IEnumerable<Diagnostic>> RunCompilationAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("message", nameof(fileName));
            }

            // Create the analyzers
            Type analyzerType = typeof(DiagnosticAnalyzer);
            var analyzers = typeof(MT1001Analyzer).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(analyzerType) && !t.IsAbstract)
                .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
                .ToArray();

            // Create the project
            string fileContent = File.ReadAllText(fileName);
            Project project = CreateProject(new[] { fileContent });

            // Execute the compilation
            var compilation = await project.GetCompilationAsync();
            var withAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzers));
            var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync();

            return diagnostics;
        }
    }
}

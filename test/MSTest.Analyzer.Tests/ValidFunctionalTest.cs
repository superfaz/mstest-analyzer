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
using System.Text;
using System.Threading.Tasks;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Tests the <see cref="FakeValidTest"/> class to confirm that no
    /// warnings are found in the file, while using MSTest features.
    /// </summary>
    [TestClass]
    public class ValidFunctionalTest : DiagnosticVerifier
    {
        /// <summary>
        /// Compiles, with the analyzers the code of the <see cref="FakeValidTest"/> class.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Compilation()
        {
            // Create the analyzers
            Type analyzerType = typeof(DiagnosticAnalyzer);
            var analyzers = typeof(MT1001Analyzer).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(analyzerType) && !t.IsAbstract)
                .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
                .ToArray();

            // Create the project
            string fileContent = File.ReadAllText("FakeValidTest.cs");
            Project project = CreateProject(new[] { fileContent });

            // Execute the compilation
            var compilation = await project.GetCompilationAsync();
            var withAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzers));
            var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync();

            Assert.IsNotNull(diagnostics);
            if (diagnostics.Length != 0)
            {
                string message = string.Join("\r\n", diagnostics.Select(d => d.GetMessage()));
                Assert.Fail("Some warnings have been raised:\r\n{0}", message);
            }
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            throw new NotImplementedException();
        }
    }
}

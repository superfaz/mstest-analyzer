// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MSTest.Analyzer.Helpers
{
    /// <summary>
    /// Class for turning strings into documents and getting the diagnostics on them
    /// All methods are static
    /// </summary>
    public abstract class DiagnosticVerifier
    {
        /// <summary>
        /// Default prefix used for the files created for the compilation.
        /// </summary>
        public static readonly string DefaultFilePathPrefix = "Test";

        /// <summary>
        /// Default extension for the files created for the compilation.
        /// </summary>
        public static readonly string DefaultFileExt = "cs";

        /// <summary>
        /// Name of the test project for the compilation.
        /// </summary>
        public static readonly string TestProjectName = "TestProject";

        /// <summary>
        /// Path to the standard/system dlls.
        /// </summary>
        private static readonly string AssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

        /// <summary>
        /// All the standard references that will be available for the compilation.
        /// </summary>
        private static readonly IEnumerable<MetadataReference> References = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(TestClassAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(AssemblyPath, "System.Runtime.dll")),
        };

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzer and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the documents</param>
        /// <param name="documents">The Documents that the analyzer will be run on</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        protected static Diagnostic[] GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer analyzer, Document[] documents)
        {
            var projects = new HashSet<Project>();
            foreach (var document in documents)
            {
                projects.Add(document.Project);
            }

            var diagnostics = new List<Diagnostic>();
            foreach (var project in projects)
            {
                var compilationWithAnalyzers = project.GetCompilationAsync().Result.WithAnalyzers(ImmutableArray.Create(analyzer));
                var diags = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
                foreach (var diag in diags)
                {
                    if (diag.Location == Location.None || diag.Location.IsInMetadata)
                    {
                        diagnostics.Add(diag);
                    }
                    else
                    {
                        for (int i = 0; i < documents.Length; i++)
                        {
                            var document = documents[i];
                            var tree = document.GetSyntaxTreeAsync().Result;
                            if (tree == diag.Location.SourceTree)
                            {
                                diagnostics.Add(diag);
                            }
                        }
                    }
                }
            }

            var results = SortDiagnostics(diagnostics);
            diagnostics.Clear();
            return results;
        }

        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string</param>
        /// <returns>A Document created from the source string</returns>
        protected static Document CreateDocument(string source)
        {
            return CreateProject(new[] { source }).Documents.First();
        }

        /// <summary>
        /// Creates a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <returns>A Project created out of the Documents created from the source strings</returns>
        protected static Project CreateProject(string[] sources)
        {
            string fileNamePrefix = DefaultFilePathPrefix;
            string fileExt = DefaultFileExt;

            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                .AddMetadataReferences(projectId, References);

            int count = 0;
            foreach (var source in sources)
            {
                var newFileName = fileNamePrefix + count + "." + fileExt;
                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            return solution.GetProject(projectId);
        }

        /// <summary>
        /// Get the CSharp analyzer being tested - to be implemented in non-abstract class
        /// </summary>
        /// <returns>
        /// The <see cref="DiagnosticAnalyzer"/> to be tested.
        /// </returns>
        protected abstract DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer();

        /// <summary>
        /// Called to test a C# DiagnosticAnalyzer when applied on the single inputted string as a source
        /// Note: input a DiagnosticResult for each Diagnostic expected
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on</param>
        /// <param name="expected"> DiagnosticResults that should appear after the analyzer is run on the source</param>
        protected void VerifyCSharpDiagnostic(string source, params DiagnosticResult[] expected)
        {
            this.VerifyCSharpDiagnostic(new[] { source }, expected);
        }

        /// <summary>
        /// Called to test a C# DiagnosticAnalyzer when applied on the inputted strings as a source
        /// Note: input a DiagnosticResult for each Diagnostic expected
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on</param>
        /// <param name="expected">DiagnosticResults that should appear after the analyzer is run on the sources</param>
        protected void VerifyCSharpDiagnostic(string[] sources, params DiagnosticResult[] expected)
        {
            var analyzer = this.GetCSharpDiagnosticAnalyzer();
            var diagnostics = GetSortedDiagnostics(sources, analyzer);
            VerifyDiagnosticResults(diagnostics, analyzer, expected);
        }

        /// <summary>
        /// Given classes in the form of strings, and an IDiagnosticAnalyzer to apply to it, return the diagnostics found in the string after converting it to a document.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="analyzer">The analyzer to be run on the sources</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        private static Diagnostic[] GetSortedDiagnostics(string[] sources, DiagnosticAnalyzer analyzer)
        {
            return GetSortedDiagnosticsFromDocuments(analyzer, GetDocuments(sources));
        }

        /// <summary>
        /// Sort diagnostics by location in source document
        /// </summary>
        /// <param name="diagnostics">The list of Diagnostics to be sorted</param>
        /// <returns>An IEnumerable containing the Diagnostics in order of Location</returns>
        private static Diagnostic[] SortDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }

        /// <summary>
        /// Given an array of strings as sources, turn them into a project and return the documents and spans of it.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <returns>A Tuple containing the Documents produced from the sources and their TextSpans if relevant</returns>
        private static Document[] GetDocuments(string[] sources)
        {
            var project = CreateProject(sources);
            var documents = project.Documents.ToArray();

            if (sources.Length != documents.Length)
            {
                throw new InvalidOperationException("Amount of sources did not match amount of Documents created");
            }

            return documents;
        }

        /// <summary>
        /// Checks each of the actual Diagnostics found and compares them with the corresponding DiagnosticResult in the array of expected results.
        /// Diagnostics are considered equal only if the DiagnosticResultLocation, Id, Severity, and Message of the DiagnosticResult match the actual diagnostic.
        /// </summary>
        /// <param name="actualResults">The Diagnostics found by the compiler after running the analyzer on the source code</param>
        /// <param name="analyzer">The analyzer that was being run on the sources</param>
        /// <param name="expectedResults">Diagnostic Results that should have appeared in the code</param>
        private static void VerifyDiagnosticResults(IEnumerable<Diagnostic> actualResults, DiagnosticAnalyzer analyzer, params DiagnosticResult[] expectedResults)
        {
            int expectedCount = expectedResults.Count();
            int actualCount = actualResults.Count();

            if (expectedCount != actualCount)
            {
                string diagnosticsOutput = actualResults.Any() ? FormatDiagnostics(analyzer, actualResults.ToArray()) : "    NONE.";

                Assert.Fail(
                    "Mismatch between number of diagnostics returned, expected \"{0}\" actual \"{1}\"\r\n\r\nDiagnostics:\r\n{2}\r\n",
                    expectedCount,
                    actualCount,
                    diagnosticsOutput);
            }

            for (int i = 0; i < expectedResults.Length; i++)
            {
                var actual = actualResults.ElementAt(i);
                var expected = expectedResults[i];

                if (expected.Location == null)
                {
                    if (actual.Location != Location.None)
                    {
                        Assert.Fail(
                            "Expected:\nA project diagnostic with No location\nActual:\n{0}",
                            FormatDiagnostics(analyzer, actual));
                    }
                }
                else
                {
                    var additionalLocations = actual.AdditionalLocations.ToArray();

                    if (additionalLocations.Length != expected.Locations.Count() - 1)
                    {
                        Assert.Fail(
                            "Expected {0} additional locations but got {1} for Diagnostic:\r\n    {2}\r\n",
                            expected.Locations.Count() - 1,
                            additionalLocations.Length,
                            FormatDiagnostics(analyzer, actual));
                    }

                    int j = 0;
                    foreach (var location in expected.Locations)
                    {
                        if (j == 0)
                        {
                            VerifyDiagnosticLocation(analyzer, actual, actual.Location, location);
                        }
                        else
                        {
                            VerifyDiagnosticLocation(analyzer, actual, additionalLocations[j - 1], location);
                        }

                        j++;
                    }
                }

                if (actual.Id != expected.Id)
                {
                    Assert.Fail(
                        "Expected diagnostic id to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Id,
                        actual.Id,
                        FormatDiagnostics(analyzer, actual));
                }

                if (actual.Severity != expected.Severity)
                {
                    Assert.Fail(
                        "Expected diagnostic severity to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Severity,
                        actual.Severity,
                        FormatDiagnostics(analyzer, actual));
                }

                if (actual.GetMessage() != expected.Message)
                {
                    Assert.Fail(
                        "Expected diagnostic message to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Message,
                        actual.GetMessage(),
                        FormatDiagnostics(analyzer, actual));
                }
            }
        }

        /// <summary>
        /// Helper method to VerifyDiagnosticResult that checks the location of a diagnostic and compares it with the location in the expected DiagnosticResult.
        /// </summary>
        /// <param name="analyzer">The analyzer that was being run on the sources</param>
        /// <param name="diagnostic">The diagnostic that was found in the code</param>
        /// <param name="actual">The Location of the Diagnostic found in the code</param>
        /// <param name="expected">The DiagnosticResultLocation that should have been found</param>
        private static void VerifyDiagnosticLocation(DiagnosticAnalyzer analyzer, Diagnostic diagnostic, Location actual, DiagnosticResultLocation expected)
        {
            var actualSpan = actual.GetLineSpan();

            Assert.IsTrue(
                actualSpan.Path == expected.Path || (actualSpan.Path != null && actualSpan.Path.Contains("Test0.") && expected.Path.Contains("Test.")),
                "Expected diagnostic to be in file \"{0}\" was actually in file \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                expected.Path,
                actualSpan.Path,
                FormatDiagnostics(analyzer, diagnostic));

            var actualLinePosition = actualSpan.StartLinePosition;

            // Only check line position if there is an actual line in the real diagnostic
            if (actualLinePosition.Line > 0)
            {
                if (actualLinePosition.Line + 1 != expected.Line)
                {
                    Assert.Fail(
                        "Expected diagnostic to be on line \"{0}\" was actually on line \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Line,
                        actualLinePosition.Line + 1,
                        FormatDiagnostics(analyzer, diagnostic));
                }
            }

            // Only check column position if there is an actual column position in the real diagnostic
            if (actualLinePosition.Character > 0)
            {
                if (actualLinePosition.Character + 1 != expected.Column)
                {
                    Assert.Fail(
                        "Expected diagnostic to start at column \"{0}\" was actually at column \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Column,
                        actualLinePosition.Character + 1,
                        FormatDiagnostics(analyzer, diagnostic));
                }
            }
        }

        /// <summary>
        /// Helper method to format a Diagnostic into an easily readable string
        /// </summary>
        /// <param name="analyzer">The analyzer that this verifier tests</param>
        /// <param name="diagnostics">The Diagnostics to be formatted</param>
        /// <returns>The Diagnostics formatted as a string</returns>
        private static string FormatDiagnostics(DiagnosticAnalyzer analyzer, params Diagnostic[] diagnostics)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < diagnostics.Length; ++i)
            {
                builder.AppendLine("// " + diagnostics[i].ToString());

                var analyzerType = analyzer.GetType();
                var rules = analyzer.SupportedDiagnostics;

                foreach (var rule in rules)
                {
                    if (rule != null && rule.Id == diagnostics[i].Id)
                    {
                        var location = diagnostics[i].Location;
                        if (location == Location.None)
                        {
                            builder.AppendFormat(CultureInfo.InvariantCulture, "GetGlobalResult({0}.{1})", analyzerType.Name, rule.Id);
                        }
                        else
                        {
                            Assert.IsTrue(
                                location.IsInSource,
                                $"Test base does not currently handle diagnostics in metadata locations. Diagnostic in metadata: {diagnostics[i]}\r\n");

                            string resultMethodName = "GetCSharpResultAt";
                            var linePosition = diagnostics[i].Location.GetLineSpan().StartLinePosition;

                            builder.AppendFormat(
                                CultureInfo.InvariantCulture,
                                "{0}({1}, {2}, {3}.{4})",
                                resultMethodName,
                                linePosition.Line + 1,
                                linePosition.Character + 1,
                                analyzerType.Name,
                                rule.Id);
                        }

                        if (i != diagnostics.Length - 1)
                        {
                            builder.Append(',');
                        }

                        builder.AppendLine();
                        break;
                    }
                }
            }

            return builder.ToString();
        }
    }
}

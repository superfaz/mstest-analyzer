// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Analyzer.Helpers;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Tests the <see cref="MT1004Analyzer"/> analyzer
    /// as well as the associated <see cref="MT1004CodeFixProvider"/> code fixer.
    /// </summary>
    [TestClass]
    public class MT1004UnitTest : CodeFixVerifier
    {
        /// <summary>
        /// Tests with an empty code.
        /// </summary>
        [TestMethod]
        public void EmptyCode()
        {
            var test = string.Empty;

            this.VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// Tests with a valid structure.
        /// </summary>
        [TestMethod]
        public void ValidCode()
        {
            var test = @"
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestPackage
{
    [TestClass]
    public class TypeName
    {
        internal void InternalMethodName()
        {
        }

        private void PrivateMethodName()
        {
        }
    }
}";

            this.VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// Tests with an invalid structure.
        /// </summary>
        [TestMethod]
        public void InvalidCode()
        {
            var test = @"
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestPackage
{
    [TestClass]
    public class TypeName
    {
        [TestMethod]
        internal void InternalMethodName()
        {
        }

        [TestMethod]
        private void PrivateMethodName()
        {
        }
    }
}";
            var expected1 = new DiagnosticResult
            {
                Id = "MT1004",
                Message = "The method 'TypeName.InternalMethodName' should be public in order to be used as a test method",
                Severity = DiagnosticSeverity.Warning,
                Location = new DiagnosticResultLocation("Test0.cs", 11, 23)
            };
            var expected2 = new DiagnosticResult
            {
                Id = "MT1004",
                Message = "The method 'TypeName.PrivateMethodName' should be public in order to be used as a test method",
                Severity = DiagnosticSeverity.Warning,
                Location = new DiagnosticResultLocation("Test0.cs", 16, 22)
            };

            this.VerifyCSharpDiagnostic(test, expected1, expected2);

            var fixtest = @"
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestPackage
{
    [TestClass]
    public class TypeName
    {
        internal void InternalMethodName()
        {
        }
        private void PrivateMethodName()
        {
        }
    }
}";
            this.VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Creates a new instance of the CSharp diagnostic analyzer begin tested.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="MT1004Analyzer"/>.
        /// </returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MT1004Analyzer();
        }

        /// <summary>
        /// Creates a new instance of the CSharp code fix begin tested.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="MT1004CodeFixProvider"/>.
        /// </returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MT1004CodeFixProvider();
        }
    }
}

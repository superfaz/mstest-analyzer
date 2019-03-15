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
    /// Tests the <see cref="MT1003Analyzer"/> analyzer
    /// as well as the associated <see cref="MT1003CodeFixProvider"/> code fixer.
    /// </summary>
    [TestClass]
    public class MT1003UnitTest : CodeFixVerifier
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
        [TestMethod]
        public void MethodName()
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

namespace TestPackage
{
    [TestClass]
    public class TypeName
    {
        public void MethodName()
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MT1003",
                Message = "The method 'TypeName.MethodName' is public and should be marked with one of the test attribute",
                Severity = DiagnosticSeverity.Warning,
                Location = new DiagnosticResultLocation("Test0.cs", 9, 21)
            };

            this.VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestPackage
{
    [TestClass]
    public class TypeName
    {
        [TestMethod]
        public void MethodName()
        {
        }
    }
}";
            this.VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Tests reformatting is correct with documentation blocks
        /// </summary>
        [TestMethod]
        public void ReformatsCorrectlyWithDocumentation()
        {
            var test = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyTest
{
    /// <summary>
    /// Tests the <see cref=""MyClass""/> class.
    /// </summary>
    [TestClass]
    public class MyClassTest
    {
        /// <summary>
        /// Tests the <see cref=""MyClass.Constructor""/> constructor.
        /// </summary>
        public void Constructor()
        {
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "MT1003",
                Message = "The method 'MyClassTest.Constructor' is public and should be marked with one of the test attribute",
                Severity = DiagnosticSeverity.Warning,
                Location = new DiagnosticResultLocation("Test0.cs", 15, 21)
            };

            this.VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyTest
{
    /// <summary>
    /// Tests the <see cref=""MyClass""/> class.
    /// </summary>
    [TestClass]
    public class MyClassTest
    {
        /// <summary>
        /// Tests the <see cref=""MyClass.Constructor""/> constructor.
        /// </summary>
        [TestMethod]
        public void Constructor()
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
        /// A new instance of <see cref="MT1003Analyzer"/>.
        /// </returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MT1003Analyzer();
        }

        /// <summary>
        /// Creates a new instance of the CSharp code fix begin tested.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="MT1003CodeFixProvider"/>.
        /// </returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MT1003CodeFixProvider();
        }
    }
}

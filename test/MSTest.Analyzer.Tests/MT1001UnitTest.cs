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
    /// Tests the <see cref="MT1001Analyzer"/> analyzer
    /// as well as the associated <see cref="MT1001CodeFixProvider"/> code fixer.
    /// </summary>
    [TestClass]
    public class MT1001UnitTest : CodeFixVerifier
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
    }
}";

            this.VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// Tests with a valid structure.
        /// </summary>
        [TestMethod]
        public void ValidCode_NoUsing()
        {
            var test = @"
using System;

namespace TestPackage
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class TypeName
    {
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
    public class TypeName
    {
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MT1001",
                Message = "The class 'TypeName' should be marked with the [TestClass] attribute",
                Severity = DiagnosticSeverity.Warning,
                Location = new DiagnosticResultLocation("Test0.cs", 6, 18)
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
    }
}";
            this.VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Tests with an invalid structure.
        /// </summary>
        [TestMethod]
        public void InvalidCode_WrongAttribute()
        {
            var test = @"
using System;

namespace TestPackage
{
    [TestClass]
    public class TypeName
    {
    }

    internal class TestClassAttribute : Attribute
    {
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MT1001",
                Message = "The class 'TypeName' should be marked with the [TestClass] attribute",
                Severity = DiagnosticSeverity.Warning,
                Location = new DiagnosticResultLocation("Test0.cs", 7, 18)
            };

            this.VerifyCSharpDiagnostic(test, expected);
        }

        /// <summary>
        /// Creates a new instance of the CSharp diagnostic analyzer begin tested.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="MT1001Analyzer"/>.
        /// </returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MT1001Analyzer();
        }

        /// <summary>
        /// Creates a new instance of the CSharp code fix begin tested.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="MT1001CodeFixProvider"/>.
        /// </returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MT1001CodeFixProvider();
        }
    }
}

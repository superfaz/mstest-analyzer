// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Provide a global test context in order to verify that the analyzers
    /// are working good on a 'normal' class.
    /// </summary>
    /// <remarks>
    /// This class is considered valid and no warnings, from the MSTest.Analyzer project are expected.
    /// </remarks>
    [TestClass]
    public class FakeValidTest
    {
        /// <summary>
        /// Initializes all tests within the project.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [AssemblyInitialize]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        public static void AssemblyInitialize(TestContext context)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Clean-ups after the execution of all tests within the project.
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Initializes all tests within this class.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [ClassInitialize]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        public static void ClassInitialize(TestContext context)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Clean-ups after the execution of all tests within this class.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Called before any test within this class.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Called after any test within this class.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Actual test within the test class.
        /// </summary>
        [TestMethod]
        public void TestMethod()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Actual test within the test class - that use initial test data set.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        public void DataTestMethod(int value)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// A non-public method that should be ignored by the analyzer.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        protected static void IgnoredStatic(TestContext context)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// A non-public method that should be ignored by the analyzer.
        /// </summary>
        protected void IgnoredInstance()
        {
            // Do nothing because it is only used to fake a not exposed method
        }

        [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Needed for testing purpose")]
        private class PrivateClass
        {
            public void NotUsedMethod()
            {
                // Do nothing because it is only used to fake a not exposed method
            }
        }
    }
}

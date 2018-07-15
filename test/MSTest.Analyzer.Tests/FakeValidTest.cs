// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public static void AssemblyInitialize(TestContext context)
        {
        }

        /// <summary>
        /// Clean-ups after the execution of all tests within the project.
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
        }

        /// <summary>
        /// Initializes all tests within this class.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }

        /// <summary>
        /// Clean-ups after the execution of all tests within this class.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        /// <summary>
        /// Called before any test within this class.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
        }

        /// <summary>
        /// Called after any test within this class.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
        }

        /// <summary>
        /// Actual test within the test class.
        /// </summary>
        [TestMethod]
        public void TestMethod()
        {
        }

        /// <summary>
        /// Actual test within the test class - that use initial test data set.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void DataTestMethod(int value)
        {
        }

        /// <summary>
        /// A non-public method that should be ignored by the analyzer.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        protected static void IgnoredStatic(TestContext context)
        {
        }

        /// <summary>
        /// A non-public method that should be ignored by the analyzer.
        /// </summary>
        protected void IgnoredInstance()
        {
        }
    }
}

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
    /// This class is considered invalid and all methods should trigger a warning from
    /// the MSTest.Analyzer analyzers.
    /// </remarks>
    [TestClass]
    public class FakeInvalidTest
    {
        /// <summary>
        /// Initializes all tests within the project.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        public static void PublicAssemblyInitialize(TestContext context)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Clean-ups after the execution of all tests within the project.
        /// </summary>
        public static void PublicAssemblyCleanup()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Initializes all tests within this class.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        public static void PublicClassInitialize(TestContext context)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Clean-ups after the execution of all tests within this class.
        /// </summary>
        public static void PublicClassCleanup()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Called before any test within this class.
        /// </summary>
        public void PublicTestInitialize()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Called after any test within this class.
        /// </summary>
        public void PublicTestCleanup()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Actual test within the test class.
        /// </summary>
        public void PublicTestMethod()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Actual test within the test class.
        /// </summary>
        [TestCategory("category")]
        public void PublicTestMethodWithCategory()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Actual test within the test class - that use initial test data set.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        public void PublicDataTestMethod(int value)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Wrong accessibility of the ClassInitialize.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [ClassInitialize]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        protected static void ProtectedClassInitialize(TestContext context)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Wrong accessibility of the ClassCleanup.
        /// </summary>
        [ClassCleanup]
        protected static void ProtectedClassCleanup()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Wrong accessibility of the TestInitialize.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [TestInitialize]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        protected void ProtectedTestInitialize(TestContext context)
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Wrong accessibility of the TestCleanup.
        /// </summary>
        [TestCleanup]
        protected void ProtectedTestCleanup()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Wrong accessibility of the TestMethod.
        /// </summary>
        [TestMethod]
        protected void ProtectedTestMethod()
        {
            // Do nothing because it is only used to fake an actual test method
        }

        /// <summary>
        /// Wrong accessibility of the DataTestMethod.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        [DataTestMethod]
        [DataRow(1)]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Needed for testing purpose")]
        protected void ProtectedDataTestMethod(int value)
        {
            // Do nothing because it is only used to fake an actual test method
        }
    }
}

// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

namespace MSTest.Analyzer
{
    /// <summary>
    /// Contains the contants that describes the MSTest framework.
    /// </summary>
    internal static class MSTestConstants
    {
        /// <summary>
        /// The namespace associated with the MSTest library.
        /// </summary>
        public const string Namespace = "Microsoft.VisualStudio.TestTools.UnitTesting";

        /// <summary>
        /// The name of the TestClass attribute.
        /// </summary>
        public const string TestClass = "TestClassAttribute";

        /// <summary>
        /// The name of the TestMethod attribute.
        /// </summary>
        public const string TestMethod = "TestMethodAttribute";

        /// <summary>
        /// The attributes that can be used to mark an instance method part of a unit test.
        /// </summary>
        public static readonly string[] InstanceTestAttributes = new[]
        {
            "TestMethodAttribute",
            "DataTestMethodAttribute",
            "TestInitializeAttribute",
            "TestCleanupAttribute"
        };

        /// <summary>
        /// The attributes that can be used to mark a static method part of a unit test.
        /// </summary>
        public static readonly string[] StaticTestAttributes = new[]
        {
            "ClassInitializeAttribute",
            "ClassCleanupAttribute",
            "AssemblyInitializeAttribute",
            "AssemblyCleanupAttribute"
        };
    }
}

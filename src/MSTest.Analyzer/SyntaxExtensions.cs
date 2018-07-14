// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Extensions methods for the SyntaxNode elements.
    /// </summary>
    internal static class SyntaxExtensions
    {
        /// <summary>
        /// Checks that an attribute is representing the [TestClass] attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns><c>true</c> if the attribute is [TestClass]; otherwise <c>false</c>.</returns>
        public static bool IsTestClassAttribute(this AttributeSyntax attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            return attribute.Name.ToString() == "TestClass"
                || attribute.Name.ToString() == "TestClassAttribute";
        }

        /// <summary>
        /// Checks that an attribute is representing the [TestMethod] attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns><c>true</c> if the attribute is [TestMethod]; otherwise <c>false</c>.</returns>
        public static bool IsTestMethodttribute(this AttributeSyntax attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            return attribute.Name.ToString() == "TestMethod"
                || attribute.Name.ToString() == "TestMethodAttribute";
        }
    }
}

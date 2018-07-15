// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Extension methods for the <see cref="ISymbol"/> instances.
    /// </summary>
    internal static class SymbolExtensions
    {
        /// <summary>
        /// Retrieves the namespace associated with a specific symbol.
        /// </summary>
        /// <param name="symbol">The reference.</param>
        /// <returns>The namespace of the symbol.</returns>
        public static string GetNamespace(this ISymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            ICollection<string> names = new LinkedList<string>();
            INamespaceSymbol @namespace = symbol.ContainingNamespace;
            while (!string.IsNullOrEmpty(@namespace.Name))
            {
                names.Add(@namespace.Name);
                @namespace = @namespace.ContainingNamespace;
            }

            return string.Join(".", names.Reverse());
        }

        /// <summary>
        /// Checks if a specific attribute represents one of the main MSTest attribute that
        /// can be applied to an instance method.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="symbol"/> represents a valid method attribute;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The expected attributes are:
        /// <list type="bullet">
        ///   <item>[TestMethod]</item>
        ///   <item>[DataTestMethod]</item>
        ///   <item>[TestInitialize]</item>
        ///   <item>[TestCleanup]</item>
        /// </list>
        /// </remarks>
        public static bool IsInstanceTestMethodAttribute(this ISymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (symbol.GetNamespace() != MSTestConstants.Namespace)
            {
                return false;
            }

            return MSTestConstants.InstanceTestAttributes.Contains(symbol.Name);
        }

        /// <summary>
        /// Checks if a specific attribute represents one of the main MSTest attribute that
        /// can be applied to a static method.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="symbol"/> represents a valid method attribute;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The expected attributes are:
        /// <list type="bullet">
        ///   <item>[ClassInitialize]</item>
        ///   <item>[ClassCleanup]</item>
        ///   <item>[AssemblyInitialize]</item>
        ///   <item>[AssemblyCleanup]</item>
        /// </list>
        /// </remarks>
        public static bool IsStaticTestMethodAttribute(this ISymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (symbol.GetNamespace() != MSTestConstants.Namespace)
            {
                return false;
            }

            return MSTestConstants.StaticTestAttributes.Contains(symbol.Name);
        }
    }
}

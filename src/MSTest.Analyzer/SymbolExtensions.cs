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
    }
}

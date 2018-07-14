// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Checks that all (concrete) public classes of a test project are marked with the [TestClass] attribute.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MT1001Analyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="MT1001Analyzer"/> analyzer.
        /// </summary>
        public static readonly string DiagnosticId = "MT1001";

        /// <summary>
        /// The title of the rule.
        /// </summary>
        private const string Title = "Public class should have a test attribute";

        /// <summary>
        /// The message format used by the rule.
        /// </summary>
        private const string MessageFormat = "The class '{0}' should be marked with the [TestClass] attribute";

        /// <summary>
        /// The category of the rule.
        /// </summary>
        private const string Category = "Maintainability";

        /// <summary>
        /// The descriptor associated with the rule.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule
            = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        /// <summary>
        /// Registers the actions in this analysis context.
        /// </summary>
        /// <param name="context">The context for the analysis.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(HandleNamedType, SymbolKind.NamedType);
        }

        /// <summary>
        /// Checks the classes definition.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        private static void HandleNamedType(SymbolAnalysisContext context)
        {
            if (context.Symbol.DeclaredAccessibility != Accessibility.Public
                || context.Symbol.IsStatic
                || context.Symbol.IsAbstract)
            {
                return;
            }

            if (!context.Symbol.GetAttributes().Any(
                a => a.AttributeClass.Name == MSTestConstants.TestClass
                && a.AttributeClass.GetNamespace() == MSTestConstants.Namespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Symbol.Locations[0], context.Symbol.Name));
            }
        }
    }
}

// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Checks that all public methods of a test project are marked with the [TestMethod] attribute.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MT1003Analyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="MT1003Analyzer"/> analyzer.
        /// </summary>
        public static readonly string DiagnosticId = "MT1003";

        /// <summary>
        /// The title of the rule.
        /// </summary>
        private const string Title = "Public method should have a test attribute";

        /// <summary>
        /// The message format used by the rule.
        /// </summary>
        private const string MessageFormat = "The method '{0}.{1}' is public and should be marked with one of the test attribute";

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
            context.RegisterSymbolAction(HandleMethod, SymbolKind.Method);
        }

        /// <summary>
        /// Checks the method definition.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        private static void HandleMethod(SymbolAnalysisContext context)
        {
            if (context.Symbol.DeclaredAccessibility != Accessibility.Public
             || context.Symbol.ContainingType.DeclaredAccessibility != Accessibility.Public)
            {
                // Ignore non exposed methods
                return;
            }

            if (!context.Symbol.IsStatic && context.Symbol.GetAttributes().All(
                a => !a.AttributeClass.IsInstanceTestMethodAttribute()))
            {
                // The instance method doesn't have a test attribute
                string type = context.Symbol.ContainingType.Name;
                string method = context.Symbol.Name;
                Diagnostic diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0], type, method);
                context.ReportDiagnostic(diagnostic);
            }

            if (context.Symbol.IsStatic && context.Symbol.GetAttributes().All(
                a => !a.AttributeClass.IsStaticTestMethodAttribute()))
            {
                // The instance method doesn't have a test attribute
                string type = context.Symbol.ContainingType.Name;
                string method = context.Symbol.Name;
                Diagnostic diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0], type, method);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}

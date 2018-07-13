// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Provide code fix for the <see cref="MT1001PublicClassShouldHaveTestAttribute"/> analyzer.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MT1001CodeFixProvider))]
    [Shared]
    public class MT1001CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The namespace associated with the MSTest library.
        /// </summary>
        public const string MSTestNamespace = "Microsoft.VisualStudio.TestTools.UnitTesting";

        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MT1001PublicClassShouldHaveTestAttribute.DiagnosticId); }
        }

        /// <summary>
        /// Returns the code fixer that can be used to fix multiple instance of the MT1001 issue.
        /// </summary>
        /// <returns>The code fixer.</returns>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc />
        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            string title = "Add [TestClass] attribute";

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                CodeAction action = CodeAction.Create(
                    title,
                    cancellationToken => AddTestClassAttributeAsync(context.Document, diagnostic, cancellationToken),
                    nameof(MT1001CodeFixProvider));
                context.RegisterCodeFix(action, diagnostic);
            }

            return Task.CompletedTask;
        }

        private static SyntaxNode AddTestUsingDirective(SyntaxNode root)
        {
            var compilationUnit = root.AncestorsAndSelf().OfType<CompilationUnitSyntax>().First();
            var updated = compilationUnit.AddUsings(
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(MSTestNamespace)));

            return root.ReplaceNode(compilationUnit, updated);
        }

        private static async Task<Document> AddTestClassAttributeAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            // Add the [TestClass] attribute
            var node = root.FindNode(diagnostic.Location.SourceSpan)
                .AncestorsAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .First();

            var attributes = node.AttributeLists.Add(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                        SyntaxFactory.Attribute(
                            SyntaxFactory.IdentifierName("TestClass"))))
                .WithTrailingTrivia(SyntaxFactory.Whitespace("\r\n")));
            root = root.ReplaceNode(node, node.WithAttributeLists(attributes));

            // Add the using directive
            root = AddTestUsingDirective(root);

            return document.WithSyntaxRoot(root);
        }
    }
}

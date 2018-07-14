// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MSTest.Analyzer
{
    /// <summary>
    /// Provide code fix for the <see cref="MT1002Analyzer"/> analyzer.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MT1002CodeFixProvider))]
    [Shared]
    public class MT1002CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MT1002Analyzer.DiagnosticId); }
        }

        /// <summary>
        /// Returns the code fixer that can be used to fix multiple instance of the MT1002 issue.
        /// </summary>
        /// <returns>The code fixer.</returns>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc />
        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            string title = "Remove [TestClass] attribute";

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                CodeAction action = CodeAction.Create(
                    title,
                    cancellationToken => RemoveTestClassAttributeAsync(context.Document, diagnostic, cancellationToken),
                    nameof(MT1002CodeFixProvider));
                context.RegisterCodeFix(action, diagnostic);
            }

            return Task.CompletedTask;
        }

        private static async Task<Document> RemoveTestClassAttributeAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            // Remove the [TestClass] attribute
            var node = root.FindNode(diagnostic.Location.SourceSpan)
                .AncestorsAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .First();

            List<SyntaxNode> nodesToRemove = new List<SyntaxNode>();
            foreach (var list in node.AttributeLists)
            {
                if (list.Attributes.Count == 1
                    && list.Attributes.First().IsTestClassAttribute())
                {
                    nodesToRemove.Add(list);
                }
                else if (list.Attributes.Count > 1)
                {
                    nodesToRemove.AddRange(list.Attributes.Where(a => a.IsTestClassAttribute()));
                }
            }

            root = root.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia);

            return document.WithSyntaxRoot(root);
        }
    }
}

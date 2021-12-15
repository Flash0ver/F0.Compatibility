using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using F0.Extensions;
using F0.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace F0.CodeAnalysis;

[Generator]
internal sealed class IsExternalInitGenerator : IIncrementalGenerator
{
	private const string HintName = "IsExternalInit.g.cs";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<SyntaxNode> declarations = context.SyntaxProvider.CreateSyntaxProvider(SyntaxProviderPredicate, SyntaxProviderTransform);

		IncrementalValueProvider<(Compilation, ImmutableArray<SyntaxNode>)> tuple = context.CompilationProvider.Combine(declarations.Collect());

		IncrementalValueProvider<(ParseOptions, (Compilation, ImmutableArray<SyntaxNode>))> source = context.ParseOptionsProvider.Combine(tuple);

		context.RegisterSourceOutput(source, SourceOutputAction);
	}

	private static bool SyntaxProviderPredicate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
	{
		return syntaxNode switch
		{
			AccessorDeclarationSyntax accessor when accessor.Keyword.IsKind(SyntaxKind.InitKeyword) => true,
			RecordDeclarationSyntax record when record is
			{
				ParameterList.Parameters.Count: > 0,
			} => RequiresIsExternalInit(record),
			_ => false,
		};

		static bool RequiresIsExternalInit(RecordDeclarationSyntax record)
		{
			if (record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword))
			{
				return record.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ReadOnlyKeyword));
			}

			Debug.Assert(record.ClassOrStructKeyword.IsKind(SyntaxKind.ClassKeyword) || record.ClassOrStructKeyword.IsKind(SyntaxKind.None), $"Unmatched value: {record}");
			return true;
		}
	}

	private static SyntaxNode SyntaxProviderTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		Debug.Assert(context.Node is AccessorDeclarationSyntax or RecordDeclarationSyntax);

		return context.Node;
	}

	private static void SourceOutputAction(SourceProductionContext context, (ParseOptions Options, (Compilation Compilation, ImmutableArray<SyntaxNode> Nodes) Tuple) source)
	{
		if (source.Tuple.Nodes.IsDefaultOrEmpty || HasIsExternalInit(source.Tuple.Compilation))
		{
			return;
		}

		if (!source.Options.TryGetCSharpLanguageVersion(out LanguageVersion langVersion))
		{
			return;
		}

		string text = GenerateSourceCode(langVersion);
		var sourceText = SourceText.From(text, Encodings.Utf8NoBom);
		context.AddSource(HintName, sourceText);
	}

	private static bool HasIsExternalInit(Compilation compilation)
	{
		INamedTypeSymbol? type = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.IsExternalInit");

		return type is not null;
	}

	private static string GenerateSourceCode(LanguageVersion langVersion)
	{
		bool hasFileScopedNamespace = langVersion >= LanguageVersion.CSharp10;

		using StringWriter writer = new(CultureInfo.InvariantCulture);
		using IndentedTextWriter sourceText = new(writer, Trivia.Tab);

		sourceText.WriteLine("// <auto-generated/>");
		sourceText.WriteLine("#nullable enable");
		sourceText.WriteLine();

		if (hasFileScopedNamespace)
		{
			sourceText.WriteLine("namespace System.Runtime.CompilerServices;");
			sourceText.WriteLine();
		}
		else
		{
			sourceText.WriteLine("namespace System.Runtime.CompilerServices");
			sourceText.WriteLine(Tokens.OpenBrace);
			sourceText.Indent++;
		}

		sourceText.WriteLine("internal static class IsExternalInit");
		sourceText.WriteLine(Tokens.OpenBrace);
		sourceText.WriteLine(Tokens.CloseBrace);

		if (!hasFileScopedNamespace)
		{
			sourceText.Indent--;
			sourceText.WriteLine(Tokens.CloseBrace);
		}

		Debug.Assert(sourceText.Indent == 0, $"Invalid {nameof(sourceText.Indent)}: {sourceText.Indent}");

		return writer.ToString();
	}
}

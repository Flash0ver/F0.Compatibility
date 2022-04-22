using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace F0.Tests.Testing;

internal static partial class CSharpIncrementalGeneratorVerifier<TIncrementalGenerator>
	where TIncrementalGenerator : IIncrementalGenerator, new()
{
	private static readonly UTF8Encoding encoding = new(false, true);

	internal static (string filename, string content)[] EmptyGeneratedSources { get; } = Array.Empty<(string filename, string content)>();

	public static DiagnosticResult Diagnostic()
		=> new();

	public static DiagnosticResult Diagnostic(string id, DiagnosticSeverity severity)
		=> new(id, severity);

	public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
		=> new(descriptor);

	public static Task VerifyGeneratorAsync(string source, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
		=> VerifyGeneratorAsync(source, DiagnosticResult.EmptyDiagnosticResults, EmptyGeneratedSources, refAssemblies, langVersion);

	public static Task VerifyGeneratorAsync(string source, (string filename, string content) generatedSource, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
		=> VerifyGeneratorAsync(source, DiagnosticResult.EmptyDiagnosticResults, new[] { generatedSource }, refAssemblies, langVersion);

	public static Task VerifyGeneratorAsync(string source, (string filename, string content)[] generatedSources, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
		=> VerifyGeneratorAsync(source, DiagnosticResult.EmptyDiagnosticResults, generatedSources, refAssemblies, langVersion);

	public static Task VerifyGeneratorAsync(string source, DiagnosticResult diagnostic, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
		=> VerifyGeneratorAsync(source, new[] { diagnostic }, EmptyGeneratedSources, refAssemblies, langVersion);

	public static Task VerifyGeneratorAsync(string source, DiagnosticResult[] diagnostics, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
		=> VerifyGeneratorAsync(source, diagnostics, EmptyGeneratedSources, refAssemblies, langVersion);

	public static Task VerifyGeneratorAsync(string source, DiagnosticResult diagnostic, (string filename, string content) generatedSource, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
		=> VerifyGeneratorAsync(source, new[] { diagnostic }, new[] { generatedSource }, refAssemblies, langVersion);

	public static Task VerifyGeneratorAsync(string source, DiagnosticResult[] diagnostics, (string filename, string content) generatedSource, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
		=> VerifyGeneratorAsync(source, diagnostics, new[] { generatedSource }, refAssemblies, langVersion);

	public static Task VerifyGeneratorAsync(string source, DiagnosticResult diagnostic, (string filename, string content)[] generatedSources, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
		=> VerifyGeneratorAsync(source, new[] { diagnostic }, generatedSources, refAssemblies, langVersion);

	public static async Task VerifyGeneratorAsync(string source, DiagnosticResult[] diagnostics, (string filename, string content)[] generatedSources, ReferenceAssemblies? refAssemblies = default, LanguageVersion? langVersion = default)
	{
		CSharpIncrementalGeneratorVerifier<TIncrementalGenerator>.Test test = new()
		{
			TestState =
			{
				Sources = { source },
			},
		};

		if (diagnostics.Length != 0)
		{
			test.ExpectedDiagnostics.AddRange(diagnostics);
		}

		foreach ((string filename, string content) in generatedSources)
		{
			var sourceText = SourceText.From(content, encoding);

			test.TestState.GeneratedSources.Add((typeof(TIncrementalGenerator), filename, sourceText));
		}

		if (refAssemblies is not null)
		{
			test.TestState.ReferenceAssemblies = refAssemblies;
		}

		if (langVersion.HasValue)
		{
			test.LanguageVersion = langVersion.Value;
		}

		await test.RunAsync(CancellationToken.None);
	}
}

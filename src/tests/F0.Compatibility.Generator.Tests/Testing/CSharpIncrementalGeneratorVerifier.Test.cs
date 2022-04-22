using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace F0.Tests.Testing;

internal static partial class CSharpIncrementalGeneratorVerifier<TIncrementalGenerator>
	where TIncrementalGenerator : IIncrementalGenerator, new()
{
	public sealed class Test : CSharpSourceGeneratorTest<EmptySourceGeneratorProvider, XUnitVerifier>
	{
		public Test()
		{
		}

		public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

		protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
		{
			ISourceGenerator generator = new TIncrementalGenerator().AsSourceGenerator();

			return new[] { generator };
		}

		protected override CompilationOptions CreateCompilationOptions()
		{
			CompilationOptions compilationOptions = base.CreateCompilationOptions();

			ImmutableDictionary<string, ReportDiagnostic> diagnosticSpecificOptions =
				compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings);
			compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(diagnosticSpecificOptions);

			return compilationOptions;
		}

		protected override ParseOptions CreateParseOptions()
		{
			var parseOptions = (CSharpParseOptions)base.CreateParseOptions();

			parseOptions = parseOptions.WithLanguageVersion(LanguageVersion);

			return parseOptions;
		}
	}
}

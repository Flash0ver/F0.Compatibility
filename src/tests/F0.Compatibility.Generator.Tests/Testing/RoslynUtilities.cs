using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace F0.Tests.Testing;

internal static class RoslynUtilities
{
	internal static void TestGenerator<TGenerator>(string inputSource)
		where TGenerator : IIncrementalGenerator, new()
	{
		Compilation compilation = RunGenerator<TGenerator>(inputSource, 1);

		SyntaxTree generated = compilation.SyntaxTrees.Last();
		string actualSource = generated.ToString();

		if (!actualSource.Equals(inputSource, StringComparison.Ordinal))
		{
			string message = "Generator should not have contributed to the output, but generated:" + Environment.NewLine + actualSource;
			Assert.Fail(message);
		}
	}

	internal static void TestGenerator<TGenerator>(string inputSource, string expectedSource)
		where TGenerator : IIncrementalGenerator, new()
	{
		Compilation compilation = RunGenerator<TGenerator>(inputSource, 2);

		SyntaxTree generated = compilation.SyntaxTrees.Last();
		string actualSource = generated.ToString();

		if (!actualSource.Equals(expectedSource, StringComparison.Ordinal))
		{
			string diff = Diff(expectedSource, actualSource);
			string message = "Expected and actual source text differ: " + Environment.NewLine + diff;
			Assert.Fail(message);
		}
	}

	private static Compilation RunGenerator<TGenerator>(string inputSource, int expectedSyntaxTreeCount)
		where TGenerator : IIncrementalGenerator, new()
	{
		Debug.Assert(expectedSyntaxTreeCount >= 1);
		int generatedSource = expectedSyntaxTreeCount - 1;

		var generator = new TGenerator();

		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

		Compilation inputCompilation = CreateCompilation(inputSource);

		driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics, CancellationToken.None);

		Assert.True(diagnostics.IsEmpty, $"Source generation failed with {diagnostics.Length} diagnostics: {FormatDiagnostics(diagnostics)}");
		Assert.True(outputCompilation.SyntaxTrees.Count() == expectedSyntaxTreeCount, $"Expected output compilation to have {expectedSyntaxTreeCount} syntax trees, but found {outputCompilation.SyntaxTrees.Count()}.");
		Assert.True(outputCompilation.GetDiagnostics().IsEmpty, $"Expected output compilation to have no diagnostics, but found {outputCompilation.GetDiagnostics().Length}: {FormatDiagnostics(outputCompilation.GetDiagnostics())}");

		GeneratorDriverRunResult runResult = driver.GetRunResult();
		Assert.True(runResult.GeneratedTrees.Length == generatedSource, $"Expected {generatedSource} generated syntax trees, but generated {runResult.GeneratedTrees.Length}.");
		Assert.True(runResult.Diagnostics.IsEmpty, $"Expected no run diagnostics, but found {runResult.Diagnostics.Length}: {FormatDiagnostics(runResult.Diagnostics)}");

		GeneratorRunResult generatorResult = runResult.Results[0];
		Assert.True(GetSourceGenerator(generatorResult.Generator).Equals(generator), $"Expected and actual generator are not the same.");
		Assert.True(generatorResult.Diagnostics.IsEmpty, $"Expected no run diagnostics, but found {generatorResult.Diagnostics.Length}: {FormatDiagnostics(generatorResult.Diagnostics)}");
		Assert.True(generatorResult.GeneratedSources.Length == generatedSource, $"Expected {generatedSource} generated sources, but found {generatorResult.GeneratedSources.Length}.");
		Assert.True(generatorResult.Exception is null, $"Expected no exception, but found: {generatorResult.Exception}");

		return outputCompilation;
	}

	private static Compilation CreateCompilation(string source)
		=> CSharpCompilation.Create("compilation",
			new[] { CSharpSyntaxTree.ParseText(source) },
			new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

	private static string FormatDiagnostics(ImmutableArray<Diagnostic> diagnostics)
	{
		StringBuilder text = new(Environment.NewLine);
		for (int i = 0; i < diagnostics.Length; i++)
		{
			string line = $"[{i}]: {diagnostics[i]}";
			_ = text.AppendLine(line);
		}
		return text.ToString();
	}

	private static IIncrementalGenerator GetSourceGenerator(ISourceGenerator generator)
	{
		Type type = generator.GetType();

		// https://sourceroslyn.io/#Microsoft.CodeAnalysis/SourceGeneration/IncrementalWrapper.cs
		Debug.Assert(type.FullName == "Microsoft.CodeAnalysis.IncrementalGeneratorWrapper");

		PropertyInfo? property = type.GetProperty("Generator", BindingFlags.Instance | BindingFlags.NonPublic);
		Debug.Assert(property is not null);
		object? wrapped = property.GetValue(generator);
		Debug.Assert(wrapped is not null);

		Debug.Assert(generator.GetGeneratorType() == wrapped.GetType());
		return (IIncrementalGenerator)wrapped;
	}

	private static string Diff(string original, string modified)
	{
		StringBuilder diffText = new();

		Differ differ = new();
		InlineDiffBuilder diffBuilder = new(differ);
		DiffPaneModel diffModel = diffBuilder.BuildDiffModel(original, modified, false);

		foreach (DiffPiece diffPiece in diffModel.Lines)
		{
			_ = diffPiece.Type switch
			{
				ChangeType.Inserted => diffText.Append('+'),
				ChangeType.Deleted => diffText.Append('-'),
				_ => diffText.Append(' '),
			};
			_ = diffText.AppendLine(diffPiece.Text);
		}

		return diffText.ToString();
	}
}

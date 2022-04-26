using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace F0.Extensions;

internal static class ParseOptionsExtensions
{
	internal static LanguageVersion GetCSharpLanguageVersion(this ParseOptions parseOptions)
	{
		if (parseOptions is not CSharpParseOptions cSharpParseOptions)
		{
			throw new ArgumentException($"{nameof(parseOptions.Language)} must be {LanguageNames.CSharp}, but was {parseOptions.Language}.", nameof(parseOptions));
		}

		Debug.Assert(parseOptions.Language.Equals(LanguageNames.CSharp, StringComparison.Ordinal));

		return cSharpParseOptions.LanguageVersion;
	}

	internal static bool TryGetCSharpLanguageVersion(this ParseOptions parseOptions, out LanguageVersion langVersion)
	{
		if (parseOptions is not CSharpParseOptions cSharpParseOptions)
		{
			langVersion = LanguageVersion.Default;
			return false;
		}

		Debug.Assert(parseOptions.Language.Equals(LanguageNames.CSharp, StringComparison.Ordinal));

		langVersion = cSharpParseOptions.LanguageVersion;
		return true;
	}
}

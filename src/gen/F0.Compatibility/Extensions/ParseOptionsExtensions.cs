using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace F0.Extensions;

internal static class ParseOptionsExtensions
{
	internal static bool TryGetCSharpLanguageVersion(this ParseOptions parseOptions, out LanguageVersion langVersion)
	{
		if (parseOptions.Language.Equals(LanguageNames.CSharp, StringComparison.Ordinal))
		{
			var cSharpParseOptions = parseOptions as CSharpParseOptions;
			Debug.Assert(cSharpParseOptions is not null);

			langVersion = cSharpParseOptions.LanguageVersion;
			return true;
		}

		langVersion = LanguageVersion.Default;
		return false;
	}
}

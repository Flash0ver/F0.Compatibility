using F0.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace F0.Tests.Extensions;

public class ParseOptionsExtensionsTests
{
	[Theory]
	[InlineData(LanguageVersion.CSharp7_3)]
	[InlineData(LanguageVersion.CSharp8)]
	[InlineData(LanguageVersion.CSharp9)]
	public void GetCSharpLanguageVersion_CSharp_Returns(LanguageVersion languageVersion)
	{
		ParseOptions parseOptions = new CSharpParseOptions(languageVersion);

		LanguageVersion langVersion = ParseOptionsExtensions.GetCSharpLanguageVersion(parseOptions);

		Assert.Equal(languageVersion, langVersion);
	}

	[Fact]
	public void GetCSharpLanguageVersion_VisualBasic_DoesNotReturn()
	{
		ParseOptions parseOptions = new Microsoft.CodeAnalysis.VisualBasic.VisualBasicParseOptions();

		Func<object> langVersion = () => ParseOptionsExtensions.GetCSharpLanguageVersion(parseOptions);

		Exception exception = Assert.Throws<ArgumentException>("parseOptions", langVersion);
		Assert.StartsWith("Language must be C#, but was Visual Basic.", exception.Message, StringComparison.Ordinal);
	}

	[Theory]
	[InlineData(LanguageVersion.CSharp7_3)]
	[InlineData(LanguageVersion.CSharp8)]
	[InlineData(LanguageVersion.CSharp9)]
	public void TryGetCSharpLanguageVersion_CSharp_ReturnTrue(LanguageVersion languageVersion)
	{
		ParseOptions parseOptions = new CSharpParseOptions(languageVersion);

		bool isCSharp = ParseOptionsExtensions.TryGetCSharpLanguageVersion(parseOptions, out LanguageVersion langVersion);

		Assert.True(isCSharp);
		Assert.Equal(languageVersion, langVersion);
	}

	[Fact]
	public void TryGetCSharpLanguageVersion_VisualBasic_ReturnFalse()
	{
		ParseOptions parseOptions = new Microsoft.CodeAnalysis.VisualBasic.VisualBasicParseOptions();

		bool isCSharp = ParseOptionsExtensions.TryGetCSharpLanguageVersion(parseOptions, out LanguageVersion langVersion);

		Assert.False(isCSharp);
		Assert.Equal(LanguageVersion.Default, langVersion);
	}
}

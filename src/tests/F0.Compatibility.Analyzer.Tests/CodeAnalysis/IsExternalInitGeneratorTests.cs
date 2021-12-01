using System.Reflection;

namespace F0.Tests.CodeAnalysis;

public class IsExternalInitGeneratorTests
{
	public object? InitOnlySetter { get; init; }

	[Fact]
	public void TypeDoesNotExist_Generate_TypeHasBeenGenerated()
	{
		Type type = typeof(System.Runtime.CompilerServices.IsExternalInit);
		Assembly actual = type.Assembly;

#if HAS_SYSTEM_RUNTIME_COMPILERSERVICES_ISEXTERNALINIT
		Assembly expected = typeof(object).Assembly;
#else
		Assembly expected = typeof(IsExternalInitGeneratorTests).Assembly;
#endif

		Assert.Equal(expected, actual);
	}
}

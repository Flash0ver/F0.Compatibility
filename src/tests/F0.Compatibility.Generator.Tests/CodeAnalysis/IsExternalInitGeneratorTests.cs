using F0.CodeAnalysis;
using F0.Tests.Testing;

namespace F0.Tests.CodeAnalysis;

public class IsExternalInitGeneratorTests
{
	[Theory]
	[MemberData(nameof(InitOnlySetter_TheoryData))]
	public void SingleInitOnlySetter_Compile_GenerateType(string code)
	{
#if HAS_SYSTEM_RUNTIME_COMPILERSERVICES_ISEXTERNALINIT
		RoslynUtilities.TestGenerator<IsExternalInitGenerator>(code);
#else
		RoslynUtilities.TestGenerator<IsExternalInitGenerator>(code, GetGenerated());
#endif
	}

	[Fact]
	public void MultipleInitOnlySetter_Compile_GenerateType()
	{
		string code = @"
public class Class { public string InitOnlySetter { get; init; } }
public struct Struct { public string InitOnlySetter { get; init; } }
public readonly struct ReadOnlyStruct { public string InitOnlySetter { get; init; } }
public record class ReferenceType(string InitOnlySetter);
public readonly record struct ValueType(string InitOnlySetter);
";

#if HAS_SYSTEM_RUNTIME_COMPILERSERVICES_ISEXTERNALINIT
		RoslynUtilities.TestGenerator<IsExternalInitGenerator>(code);
#else
		RoslynUtilities.TestGenerator<IsExternalInitGenerator>(code, GetGenerated());
#endif
	}

	[Fact]
	public void NoInitOnlySetter_Compile_DoNotGenerateType()
	{
		string code = @"
public class SetAccessor { public string InitOnlySetter { get; set; } }
public class NoSetAccessor { public string InitOnlySetter { get; } }
public record class Empty();
public record struct ValueType(string InitOnlySetter);
";

		RoslynUtilities.TestGenerator<IsExternalInitGenerator>(code);
	}

	private static TheoryData<string> InitOnlySetter_TheoryData()
	{
		TheoryData<string> data = new();
		data.Add("public class Class { public string InitOnlySetter { get; init; } }");
		data.Add("public struct Struct { public string InitOnlySetter { get; init; } }");
		data.Add("public readonly struct ReadOnlyStruct { public string InitOnlySetter { get; init; } }");
		data.Add("public record class ReferenceType(string InitOnlySetter);");
		data.Add("public readonly record struct ValueType(string InitOnlySetter);");
		return data;
	}

	private static string GetGenerated()
	{
		string generated = @"// <auto-generated/>
#nullable enable

namespace System.Runtime.CompilerServices
{
	internal static class IsExternalInit
	{
	}
}
";

		return generated;
	}
}

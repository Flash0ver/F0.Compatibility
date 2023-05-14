using System.Reflection;

namespace F0.Text;

internal static class Source
{
	private static readonly AssemblyName assemblyName = typeof(Source).Assembly.GetName();

	public static readonly string GeneratedCodeAttribute = $"""[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{assemblyName.Name}", "{assemblyName.Version}")]""";
}

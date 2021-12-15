#if DEBUG && NETFRAMEWORK
namespace F0.Tests.Diagnostics;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Debug.Assert on .NET Framework")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "Debug.Assert on .NET Framework")]
internal sealed class DebugAssertException : Exception
{
	public DebugAssertException(string? message)
		: base(message)
	{
	}
}
#endif

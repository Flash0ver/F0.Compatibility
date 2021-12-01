namespace F0.Examples;

internal static class Program
{
	private static void Main(string[] args)
	{
		WriteLine("F0.Compatibility");
		WriteLine(args);
		WriteLine();

		Record record = new(1);
		Record copy = record with { Property = 2 };
		WriteLine(copy);

		Type type = typeof(System.Runtime.CompilerServices.IsExternalInit);
		WriteLine(type.Assembly);
	}
}

public sealed record class Record(int Property);

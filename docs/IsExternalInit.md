# IsExternalInit
Generator: [IsExternalInitGenerator](../src/gen/F0.Compatibility/CodeAnalysis/IsExternalInitGenerator.cs)

|              |                       |
| ------------ | --------------------- |
| HintName     | `IsExternalInit.g.cs` |
| Language     | C# 9.0 or greater     |
| Available in | `[0.1.0,)`            |

## Summary
Enables _C# 9.0_'s _init-only setters_ (and _positional record types_, respectively) on non-`net5.0` compatible target frameworks (i.e., target frameworks that do not define [System.Runtime.CompilerServices.IsExternalInit][system-runtime-compilerservices-isexternalinit]).

## Examples
```xml
<Project>
  <PropertyGroup>
    <TargetFrameworks>netcoreapp3.1;net48;netstandard2.1</TargetFrameworks> <!--or less-->
    <LangVersion>9.0</LangVersion> <!--or greater-->
  </PropertyGroup>
</Project>
```

```csharp
public record Example(int Number) //record type with positional parameter
{
    public string Text { get; init; } //init-only setter
}
```

## Remarks
The target frameworks
- .NET Standard
- .NET Core
- .NET Framework

do not define the `class` [System.Runtime.CompilerServices.IsExternalInit][system-runtime-compilerservices-isexternalinit],
which causes

> Error [CS0518][cs0518]: Predefined type `System.Runtime.CompilerServices.IsExternalInit` is not defined or imported

when using _init-only setters_ explicitly, or implicitly via _positional parameters on record declarations_ (both _record classes_ and _readonly record structs_, however non-_readonly record structs_ synthesize _set accessors_).

That type is available in _.NET 5_ (`net5.0`) and later.
This generator adds the required type, if not found, to the compilation _internally_ when an `init` _accessor_ is either declared or synthesized by the compiler.

## Applies to
| Target Framework | Versions | TFMs                      |
| ---------------- | -------- | ------------------------- |
| .NET Standard    | all      | `netstandard2.1` or lower |
| .NET Core        | all      | `netcoreapp3.1` or lower  |
| .NET Framework   | all      | `net48` or lower          |

## Additional resources
- What's new in C# 9.0
  - [Init only setters](https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-9#init-only-setters)
  - [Record types](https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-9#record-types)
- What's new in C# 10
  - [Record structs](https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-10#record-structs)
- C# Language Proposals
  - [Init Only Setters](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/init.md)
  - [Records](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/records.md)
  - [Record structs](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/record-structs.md)
- .NET 5.0
  - [api-diff (netcoreapp3.1)](https://github.com/dotnet/core/blob/main/release-notes/5.0/api-diff/netcoreapp3.1/.NET/5.0_System.Runtime.CompilerServices.md)
  - [api-diff (netstandard2.1)](https://github.com/dotnet/core/blob/main/release-notes/5.0/api-diff/netstandard2.1/5.0_System.Runtime.CompilerServices.md)

## History
- [0.1.0](../CHANGELOG.md#v010-2021-12-15)

[system-runtime-compilerservices-isexternalinit]: https://docs.microsoft.com/dotnet/api/system.runtime.compilerservices.isexternalinit
[cs0518]: https://docs.microsoft.com/dotnet/csharp/language-reference/compiler-messages/cs0518

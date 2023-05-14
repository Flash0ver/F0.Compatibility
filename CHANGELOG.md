# F0.Compatibility
Changelog

## vNext
- Added `System.CodeDom.Compiler.GeneratedCodeAttribute` to type `System.Runtime.CompilerServices.IsExternalInit` emitted by `IsExternalInit`-Generator.

## v0.1.0 (2021-12-15)
- Added `IsExternalInit`-Generator, enabling _C# 9.0_'s _init-only setters_ (and also _positional record types_) on non-`net5.0` compatible target frameworks.

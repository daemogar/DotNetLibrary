#if NETSTANDARD2_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Runtime.CompilerServices;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Used to add the init setter forward compatibility with .net5 and greater.
/// </summary>
#pragma warning disable IDE1006 // Naming Styles
public interface IsExternalInit { }
#pragma warning restore IDE1006 // Naming Styles

#endif
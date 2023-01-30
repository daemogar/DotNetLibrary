#if NETSTANDARD2_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// Used to add the init setter forward compatibility with .net5 and greater.
/// </summary>
#pragma warning disable IDE1006 // Naming Styles
public interface IsExternalInit { }
#pragma warning restore IDE1006 // Naming Styles
#endif
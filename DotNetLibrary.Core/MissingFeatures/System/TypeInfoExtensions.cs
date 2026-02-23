#if NETSTANDARD2_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Reflection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <inheritdoc cref="TypeInfo"/>
public static class TypeInfoExtensions
{
	/// <inheritdoc cref="TypeInfo.IsAssignableFrom(TypeInfo)"/>
	public static bool IsAssignableTo(this TypeInfo thisType, Type targetType)
		=> targetType?.IsAssignableFrom(thisType) ?? false;
}
#endif
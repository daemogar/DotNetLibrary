#if NETSTANDARD2_0_OR_GREATER
namespace System.Reflection;

/// <inheritdoc cref="TypeInfo"/>
public static class TypeInfoExtensions
{
	/// <inheritdoc cref="TypeInfo.IsAssignableFrom(TypeInfo)"/>
	public static bool IsAssignableTo(this TypeInfo thisType, Type targetType)
		=> targetType?.IsAssignableFrom(thisType) ?? false;
}
#endif
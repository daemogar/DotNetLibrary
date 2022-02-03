namespace Microsoft.Extensions.Hosting;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal class UseTimeAttribute : Attribute {
	public bool UseHoursMinutes { get; }
	
	public UseTimeAttribute(bool useHoursMinutes)
	{
		UseHoursMinutes = useHoursMinutes;
	}
}
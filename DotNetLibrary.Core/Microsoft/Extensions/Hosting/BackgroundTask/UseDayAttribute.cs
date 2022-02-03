namespace Microsoft.Extensions.Hosting;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal class UseDayAttribute : Attribute {
	public bool AllowMultipleDays { get; }
	
	public UseDayAttribute(bool allowMultiple)
	{
		AllowMultipleDays = allowMultiple;
	}
}


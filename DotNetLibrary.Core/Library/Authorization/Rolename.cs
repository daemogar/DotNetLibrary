namespace DotNetLibrary.Authorization
{
	public class Rolename : IRolename
	{
		public string AccessCode { get; set; }
		public string? AccessKey { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		public Rolename() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		public Rolename(string rolename)
		{
			var parts = rolename.Split('.');

			AccessCode = parts[0];

			if (parts.Length > 1)
				AccessKey = parts[1];
		}

		public Rolename(string accessCode, string accessKey)
		{
			AccessCode = accessCode;
			AccessKey = accessKey;
		}

		public string GetValue()
			=> AccessKey == null
				? AccessCode
				: $"{AccessCode}.{AccessKey}";

		public override string ToString() => GetValue();
	}
}
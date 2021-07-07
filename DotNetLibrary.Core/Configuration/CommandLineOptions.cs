using CommandLine;

namespace DotNetLibrary.Configuration
{
	public record CommandLineOptions
	{
		public string[] Args { get; init; } = null!;
	
		[Option('p', "port", Default = -1, Required = false, HelpText = "The port the application should startup on if different then 80/443 in Live or 5001 in development.")]
		public int Port { get; set; }
	}
}

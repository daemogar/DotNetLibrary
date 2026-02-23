#if !NETSTANDARD2_0_OR_GREATER
using System.Text;
using System.Net.Mime;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Mvc.Formatters;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class StringInputFormatter : TextInputFormatter
{
	public StringInputFormatter()
	{
		SupportedEncodings.Add(UTF8EncodingWithoutBOM);
		SupportedEncodings.Add(UTF16EncodingLittleEndian);

		SupportedMediaTypes.Add(MediaTypeNames.Text.Plain);
	}

	public override async Task<InputFormatterResult> ReadRequestBodyAsync(
		InputFormatterContext context, Encoding encoding)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(nameof(context));

		using var streamReader = new StreamReader(
				context.HttpContext.Request.Body,
				encoding, true);

		var body = await streamReader.ReadToEndAsync();
		return await InputFormatterResult.SuccessAsync(body.Trim('"'));
	}
}
#endif
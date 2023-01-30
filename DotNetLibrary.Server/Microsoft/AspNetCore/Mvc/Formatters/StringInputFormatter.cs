using System.Text;
using System.Net.Mime;

namespace Microsoft.AspNetCore.Mvc.Formatters;

public class StringInputFormatter : TextInputFormatter
{
	public StringInputFormatter() : base()
	{
		SupportedEncodings.Add(UTF8EncodingWithoutBOM);
		SupportedEncodings.Add(UTF16EncodingLittleEndian);

		SupportedMediaTypes.Add(MediaTypeNames.Text.Plain);
	}

	public override async Task<InputFormatterResult> ReadRequestBodyAsync(
		InputFormatterContext context, Encoding encoding)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		using var streamReader = new StreamReader(
				context.HttpContext.Request.Body,
				encoding);

		var body = await streamReader.ReadToEndAsync();
		return await InputFormatterResult.SuccessAsync(body.Trim('"'));
	}
}
using Dictionary.Application.Common.Interfaces.Text;
using System.Globalization;

namespace Dictionary.Infrastructure.Text;
public class TurkishTextNormalizer : ITextNormalizer
{
    public string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return input
            .Trim()
            .ToLower(new CultureInfo("tr-TR"));
    }
}

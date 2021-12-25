using System.Globalization;
using System.Net;
using System.Windows.Controls;

namespace SyncOMatic.ValidationRules
{
    public class IPAddressValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse((string)value, out ipAddress))
                return ValidationResult.ValidResult;
            return new ValidationResult(false, "Adres IP jest nieprawidłowy.");
        }
    }
}

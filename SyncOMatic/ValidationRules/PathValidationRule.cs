using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SyncOMatic.ValidationRules
{
    public class PathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Directory.Exists((string)value))
                return ValidationResult.ValidResult;
            return new ValidationResult(false, "Podana ścieżka jest nieprawidłowa.");
        }
    }
}

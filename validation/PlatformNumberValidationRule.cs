using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace IpisCentralDisplayController.validation
{
    public class PlatformNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Platform number cannot be empty.");
            }

            string platformNumber = value.ToString();
            if (int.TryParse(platformNumber, out _))
            {
                return new ValidationResult(true, null);
            }
            else if (Regex.IsMatch(platformNumber, @"^\d+A$"))
            {
                return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Invalid platform number. Must be a number or in the format nA.");
        }
    }
}

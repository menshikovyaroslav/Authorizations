using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mail.ru.client.Classes.ValidationRules
{
    public class NullValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || value.ToString() == "")
            {
                return new ValidationResult(false, "Clear field !");
            }
            else
            {
                return new ValidationResult(true, "ok");
            }

        }
    }
}

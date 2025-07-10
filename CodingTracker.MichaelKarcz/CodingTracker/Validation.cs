using Spectre.Console;
using System.Globalization;

namespace CodingTracker
{
    internal static class Validation
    {
        internal static ValidationResult ValidateGenericTime(string time)
        {
            if (!DateTime.TryParseExact(time, "MM-dd-yyyy hh:mm tt", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                return ValidationResult.Error("Incorrect time format! Remember to format your entry as MM-dd-yyyy hh:mm tt, so '05-24-2025 05:22 PM' for example.\n");
            }

            DateTime timeDT = DateTime.ParseExact(time, "MM-dd-yyyy hh:mm tt", new CultureInfo("en-US"));
            if (timeDT > DateTime.Now)
            {
                return ValidationResult.Error("You cannot log a future session.");
            }

            return ValidationResult.Success();
        }

        internal static ValidationResult ValidateEndTime(string endTime, string startTime)
        {
            ValidationResult validateGenericTimeResult = ValidateGenericTime(endTime);

            if (!validateGenericTimeResult.Successful)
            {
                return validateGenericTimeResult;
            }

            DateTime endTimeDT = DateTime.ParseExact(endTime, "MM-dd-yyyy hh:mm tt", new CultureInfo("en-US"));
            DateTime startTimeDT = DateTime.ParseExact(startTime, "MM-dd-yyyy hh:mm tt", new CultureInfo("en-US"));

            if (endTimeDT < startTimeDT)
            {
                return ValidationResult.Error("You cannot have an end time earlier than the start time.\n");
            }

            return ValidationResult.Success();
        }
    }
}

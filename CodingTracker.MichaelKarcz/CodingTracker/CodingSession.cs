using System.Globalization;

namespace CodingTracker
{
    internal class CodingSession
    {
        internal int Id { get; set; }
        internal string StartTime { get; set; } // MM-dd-yyyy hh:mm tt
        internal string? EndTime { get; set; } // MM-dd-yyyy hh:mm tt
        internal string? Duration { get; set; }


        internal CodingSession()
        {
            StartTime = DateTime.Now.ToString("MM-dd-yyyy hh:mm tt", new CultureInfo("en-US"));
            EndTime = "";
            Duration = "";
        }

        internal void CalculateDuration()
        {
            if (!string.IsNullOrEmpty(EndTime) && !string.IsNullOrEmpty(StartTime))
            {
                DateTime endTimeDT = DateTime.ParseExact(EndTime, "MM-dd-yyyy hh:mm tt", new CultureInfo("en-US"));
                DateTime startTimeDT = DateTime.ParseExact(StartTime, "MM-dd-yyyy hh:mm tt", new CultureInfo("en-US"));

                Duration = (endTimeDT - startTimeDT).ToString();
            }
            else
            {
                Duration = "";
            }

        }

    }
}

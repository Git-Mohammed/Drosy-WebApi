using System.Globalization;

namespace Drosy.Domain.Shared.System.CalandeHelper
{
    /// <summary>
    /// Simple ISO‐8601 week helpers.
    /// </summary>
    public static class IsoWeekHelper
    {
        /// <summary>
        /// Returns the Monday and Sunday dates of the specified ISO week number in a year.
        /// </summary>
        public static (DateTime Monday, DateTime Sunday) GetWeekRange(int year, int week)
        {
            // .NET Core has ISOWeek but no Range helper
            var monday = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
            var sunday = monday.AddDays(6);
            return (monday, sunday);
        }

        /// <summary>
        /// Returns the Monday of the specified ISO week.
        /// </summary>
        public static DateTime GetFirstDateOfIsoWeek(int year, int week)
            => ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);

    }
}

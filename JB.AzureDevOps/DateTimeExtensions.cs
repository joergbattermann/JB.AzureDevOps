using System;
using System.Globalization;

namespace JB.AzureDevOps
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts the given <paramref name="dateTime"/> value to its wiql compatible string representation.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static string ToWiqlDateTimeString(this DateTime dateTime)
            => dateTime.ToUniversalTime().ToString(Microsoft.TeamFoundation.WorkItemTracking.WebApi.WitConstants.WorkItemTrackingWebConstants.InvariantUtcTimeFormat, CultureInfo.InvariantCulture);
    }
}
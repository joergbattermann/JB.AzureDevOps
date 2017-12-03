using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace JB.TeamFoundationServer.WorkItemTracking
{
    /// <summary>
    /// Extension Methods for <see cref="WorkItem"/> instances.
    /// </summary>
    public static class WorkItemExtensions
    {
        /// <summary>
        /// Tries to get the field value for the provided <paramref name="workItem"/> and <paramref name="fieldReferenceName"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="workItem">The work item.</param>
        /// <param name="fieldReferenceName">Name of the field reference.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static bool TryGetFieldValue<TValue>(this WorkItem workItem, string fieldReferenceName, out TValue value, TValue defaultValue = default(TValue))
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fieldReferenceName));

            if(workItem.Fields == null)
                throw new ArgumentException($"The {nameof(workItem)} must have been retrieved including its {nameof(workItem.Fields)} prior to retrieving a value.");

            if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValue) && workItemValue is TValue workItemValueAsRequestedType)
            {
                value = workItemValueAsRequestedType;
                return true;
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }
    }
}
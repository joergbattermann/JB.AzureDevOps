using System;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace JB.AzureDevOps.WorkItemTracking
{
    /// <summary>
    /// Extension Methods for <see cref="WorkItemUpdate"/> instances.
    /// </summary>
    public static class WorkItemUpdateExtensions
    {
        /// <summary>
        /// Determines whether the provided <paramref name="workItemUpdate"/> has any work item field updates.
        /// </summary>
        /// <param name="workItemUpdate">The work item update.</param>
        /// <returns>
        ///   <c>true</c> if fields were updated; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">workItemUpdate</exception>
        public static bool HasFieldUpdates(this WorkItemUpdate workItemUpdate)
        {
            if (workItemUpdate == null) throw new ArgumentNullException(nameof(workItemUpdate));
            
            return workItemUpdate.Fields?.Any() ?? false;
        }

        /// <summary>
        /// Determines whether the provided <paramref name="workItemUpdate"/> has any work item relation updates (added, updated or removed).
        /// </summary>
        /// <param name="workItemUpdate">The work item update.</param>
        /// <returns>
        ///   <c>true</c> if relates were added, updated or removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">workItemUpdate</exception>
        public static bool HasRelationUpdates(this WorkItemUpdate workItemUpdate)
        {
            if (workItemUpdate == null) throw new ArgumentNullException(nameof(workItemUpdate));

            return workItemUpdate.Relations != null;
        }
    }
}
using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace JB.TeamFoundationServer.WorkItemTracking
{
    /// <summary>
    /// Builds a <see cref="JsonPatchDocument"/> to be used in Work Item update methods.
    /// </summary>
    public class UpdateWorkItemPatchDocumentBuilder : WorkItemPatchDocumentBuilder
    {
        /// <summary>
        /// Gets the work item.
        /// </summary>
        /// <value>
        /// The work item.
        /// </value>
        public WorkItem WorkItem { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateWorkItemPatchDocumentBuilder"/> class.
        /// </summary>
        /// <exception cref="ArgumentException">title - title</exception>
        public UpdateWorkItemPatchDocumentBuilder(WorkItem workItem)
            : base()
        {
            WorkItem = workItem ?? throw new ArgumentNullException(nameof(workItem));
        }
    }
}
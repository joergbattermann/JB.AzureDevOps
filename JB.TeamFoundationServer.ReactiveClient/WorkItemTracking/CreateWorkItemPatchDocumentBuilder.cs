using System;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace JB.TeamFoundationServer.ReactiveClient.WorkItemTracking
{
    /// <summary>
    /// Builds a <see cref="JsonPatchDocument"/> to be used in Work Item creation methods.
    /// </summary>
    public class CreateWorkItemPatchDocumentBuilder : WorkItemPatchDocumentBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateWorkItemPatchDocumentBuilder"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <exception cref="ArgumentException">title - title</exception>
        public CreateWorkItemPatchDocumentBuilder(string title)
            :base()
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException($"Value for '{nameof(title)}' cannot be null or whitespace.", nameof(title));

            AddFieldValue(WellKnownWorkItemFieldReferenceNames.Title, title);
        }
    }
}
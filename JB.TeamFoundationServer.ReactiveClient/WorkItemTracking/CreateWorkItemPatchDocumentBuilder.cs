using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace JB.TeamFoundationServer.ReactiveClient.WorkItemTracking
{
    /// <summary>
    /// Builds a <see cref="JsonPatchDocument"/> to be used in Work Item creation methods.
    /// </summary>
    public class CreateWorkItemPatchDocumentBuilder : WorkItemPatchDocumentBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateWorkItemPatchDocumentBuilder" /> class.
        /// </summary>
        /// <param name="workItemType">Type of the work item.</param>
        /// <param name="title">The title.</param>
        /// <param name="fieldsAndValues">The additional fields and values to set.</param>
        /// <exception cref="ArgumentException">title - title</exception>
        public CreateWorkItemPatchDocumentBuilder(string workItemType, string title, params KeyValuePair<string, object>[] fieldsAndValues)
            :base()
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException($"Value for '{nameof(workItemType)}' cannot be null or whitespace.", nameof(workItemType));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException($"Value for '{nameof(title)}' cannot be null or whitespace.", nameof(title));

            AddFieldValue(WellKnownWorkItemFieldReferenceNames.System.WorkItemType, workItemType);
            AddFieldValue(WellKnownWorkItemFieldReferenceNames.System.Title, title);

            foreach (var fieldAndValue in fieldsAndValues ?? Enumerable.Empty<KeyValuePair<string, object>>())
            {
                AddFieldValue(fieldAndValue.Key, fieldAndValue.Value);
            }
        }
    }
}
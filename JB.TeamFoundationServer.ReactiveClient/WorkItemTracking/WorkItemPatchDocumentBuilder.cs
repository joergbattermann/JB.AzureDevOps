using System;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace JB.TeamFoundationServer.ReactiveClient.WorkItemTracking
{
    /// <summary>
    /// Builds a <see cref="JsonPatchDocument"/> to be used in Work Item Creation and Update methods.
    /// </summary>
    public class WorkItemPatchDocumentBuilder
    {
        // ToDo: these must be placed elsewhere / more centrally
        private const string FieldsBasePath = "/fields";
        private const string RelationsBasePath = "/relations";
        private const string RelationsReverseSuffix = "-reverse";
        private const string RelationsForwardSuffix = "-forward";

        private readonly JsonPatchDocument _patchDocument = new JsonPatchDocument();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"></see> class.
        /// </summary>
        public WorkItemPatchDocumentBuilder()
        {
            // for creation see https://github.com/Microsoft/vsts-dotnet-samples/blob/master/ClientLibrary/Snippets/Microsoft.TeamServices.Samples.Client/WorkItemTracking/WorkItemsSample.cs#L198
            // for update see https://github.com/Microsoft/vsts-dotnet-samples/blob/master/ClientLibrary/Snippets/Microsoft.TeamServices.Samples.Client/WorkItemTracking/WorkItemsSample.cs#L316
        }

        /// <summary>
        /// Adds a forward relation of the given <paramref name="workItemLinkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItemUrl" /> to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="targetWorkItemUrl">The target work item URL.</param>
        /// <param name="workItemLinkTypeReferenceName">Reference Name of the work item link type.</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddForwardRelation(string targetWorkItemUrl, string workItemLinkTypeReferenceName, string comment = "")
        {
            if (string.IsNullOrWhiteSpace(targetWorkItemUrl))
                throw new ArgumentException($"Value for '{nameof(targetWorkItemUrl)}' cannot be null or whitespace.", nameof(targetWorkItemUrl));

            if(string.IsNullOrWhiteSpace(workItemLinkTypeReferenceName))
                throw new ArgumentException($"Value for '{nameof(workItemLinkTypeReferenceName)}' cannot be null or whitespace.", nameof(workItemLinkTypeReferenceName));

            _patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"{RelationsBasePath}/-",
                    Value = new
                    {
                        rel = $"{workItemLinkTypeReferenceName}{RelationsForwardSuffix}",
                        url = targetWorkItemUrl,
                        attributes = new
                        {
                            comment = !string.IsNullOrWhiteSpace(comment)
                                ? comment
                                : string.Empty
                        }
                    }
                }
            );

            return this;
        }

        /// <summary>
        /// Adds a reverse relation of the given <paramref name="workItemLinkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItemUrl" /> to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="targetWorkItemUrl">The target work item URL.</param>
        /// <param name="workItemLinkTypeReferenceName">Reference Name of the work item link type.</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddReverseRelation(string targetWorkItemUrl, string workItemLinkTypeReferenceName, string comment = "")
        {
            if (string.IsNullOrWhiteSpace(targetWorkItemUrl))
                throw new ArgumentException($"Value for '{nameof(targetWorkItemUrl)}' cannot be null or whitespace.", nameof(targetWorkItemUrl));

            if (string.IsNullOrWhiteSpace(workItemLinkTypeReferenceName))
                throw new ArgumentException($"Value for '{nameof(workItemLinkTypeReferenceName)}' cannot be null or whitespace.", nameof(workItemLinkTypeReferenceName));

            _patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"{RelationsBasePath}/-",
                    Value = new
                    {
                        rel = $"{workItemLinkTypeReferenceName}{RelationsReverseSuffix}",
                        url = targetWorkItemUrl,
                        attributes = new
                        {
                            comment = !string.IsNullOrWhiteSpace(comment)
                                ? comment
                                : string.Empty
                        }
                    }
                }
            );

            return this;
        }

        /// <summary>
        /// Adds the field <paramref name="value"/> for the provided <paramref name="fieldReferenceName"/> to this <see cref="WorkItemPatchDocumentBuilder"/> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="fieldReferenceName">Reference Name of the work item field.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddFieldValue(string fieldReferenceName, object value)
        {
            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException($"Value for '{nameof(fieldReferenceName)}' cannot be null or whitespace.", nameof(fieldReferenceName));

            _patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = GetPathForFieldReferenceName(fieldReferenceName),
                    Value = value
                }
            );

            return this;
        }

        /// <summary>
        /// Removes the field values for the provided <paramref name="fieldReferenceName"/> from this <see cref="WorkItemPatchDocumentBuilder"/> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="fieldReferenceName">Reference Name of the work item field.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder RemoveFieldValue(string fieldReferenceName)
        {
            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException($"Value for '{nameof(fieldReferenceName)}' cannot be null or whitespace.", nameof(fieldReferenceName));

            _patchDocument.RemoveAll(operation =>
                string.Equals(operation.Path, fieldReferenceName, StringComparison.InvariantCultureIgnoreCase));

            return this;
        }

        /// <summary>
        /// Gets the <see cref="JsonPatchOperation.Path"/> for the provided <paramref name="fieldReferenceName"/>.
        /// </summary>
        /// <param name="fieldReferenceName">Work Item Field Reference name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        protected string GetPathForFieldReferenceName(string fieldReferenceName)
        {
            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException($"Value for '{nameof(fieldReferenceName)}' cannot be null or whitespace.", nameof(fieldReferenceName));

            return $"{FieldsBasePath}/{fieldReferenceName}";
        }

        /// <summary>
        /// Gets the <see cref="JsonPatchDocument"/> representing the current state of this <see cref="WorkItemPatchDocumentBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public virtual JsonPatchDocument ToJsonPatchDocument() => _patchDocument;

        /// <summary>
        /// Performs an implicit conversion from <see cref="WorkItemPatchDocumentBuilder"/> to <see cref="JsonPatchDocument"/>.
        /// </summary>
        /// <param name="workItemBuilder">The work item builder.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JsonPatchDocument(WorkItemPatchDocumentBuilder workItemBuilder) => workItemBuilder._patchDocument;
    }
}
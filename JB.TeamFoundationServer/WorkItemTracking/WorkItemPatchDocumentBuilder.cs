using System;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace JB.TeamFoundationServer.WorkItemTracking
{
    /// <summary>
    /// Builds a <see cref="JsonPatchDocument"/> to be used in Work Item Creation and Update methods.
    /// </summary>
    public abstract class WorkItemPatchDocumentBuilder
    {
        protected JsonPatchDocument PatchDocument { get; } = new JsonPatchDocument();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemPatchDocumentBuilder" /> class.
        /// </summary>
        protected WorkItemPatchDocumentBuilder()
        {
            // for creation see https://github.com/Microsoft/vsts-dotnet-samples/blob/master/ClientLibrary/Snippets/Microsoft.TeamServices.Samples.Client/WorkItemTracking/WorkItemsSample.cs#L198
            // for update see https://github.com/Microsoft/vsts-dotnet-samples/blob/master/ClientLibrary/Snippets/Microsoft.TeamServices.Samples.Client/WorkItemTracking/WorkItemsSample.cs#L316
        }

        /// <summary>
        /// Adds a relation of the given <paramref name="linkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItemUrl" />
        /// to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="targetWorkItemUrl">The target work item URL.</param>
        /// <param name="linkTypeReferenceName">The link type reference name (including direction '-forward' or '-reverse', if applicable).</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddRelation(string targetWorkItemUrl, string linkTypeReferenceName, string comment = "")
        {
            if (string.IsNullOrWhiteSpace(targetWorkItemUrl))
                throw new ArgumentException($"Value for '{nameof(targetWorkItemUrl)}' cannot be null or whitespace.", nameof(targetWorkItemUrl));

            if (string.IsNullOrWhiteSpace(linkTypeReferenceName))
                throw new ArgumentException($"Value for '{nameof(linkTypeReferenceName)}' cannot be null or whitespace.", nameof(linkTypeReferenceName));

            PatchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"{Constants.WorkItems.RelationsBasePath}/-",
                    Value = new
                    {
                        rel = $"{linkTypeReferenceName}",
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
        /// Adds a relation of the given <paramref name="linkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItem" />
        /// to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="targetWorkItem">The target work item.</param>
        /// <param name="linkTypeReferenceName">The link type reference name (including direction '-forward' or '-reverse', if applicable).</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddRelation(WorkItem targetWorkItem, string linkTypeReferenceName, string comment = "")
        {
            if (targetWorkItem == null) throw new ArgumentNullException(nameof(targetWorkItem));
            if (string.IsNullOrWhiteSpace(targetWorkItem.Url)) throw new ArgumentException($"The '{nameof(targetWorkItem)}' must have a valid '{nameof(WorkItem.Url)}' value", nameof(targetWorkItem));

            return AddRelation(targetWorkItem.Url, linkTypeReferenceName, comment);
        }

        /// <summary>
        /// Adds a forward relation of the given <paramref name="linkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItem" />
        /// to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation" />.
        /// </summary>
        /// <param name="targetWorkItem">The target work item.</param>
        /// <param name="linkTypeReferenceName">Reference Name of the work item link type.</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">targetWorkItem</exception>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddForwardRelation(WorkItem targetWorkItem, string linkTypeReferenceName, string comment = "")
        {
            if (targetWorkItem == null) throw new ArgumentNullException(nameof(targetWorkItem));
            if (string.IsNullOrWhiteSpace(targetWorkItem.Url)) throw new ArgumentException($"The '{nameof(targetWorkItem)}' must have a valid '{nameof(WorkItem.Url)}' value", nameof(targetWorkItem));

            return AddForwardRelation(targetWorkItem.Url, linkTypeReferenceName, comment);
        }

        /// <summary>
        /// Adds a reverse relation of the given <paramref name="linkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItem" />
        /// to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation" />.
        /// </summary>
        /// <param name="targetWorkItem">The target work item.</param>
        /// <param name="linkTypeReferenceName">Reference Name of the work item link type.</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">targetWorkItem</exception>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddReverseRelation(WorkItem targetWorkItem, string linkTypeReferenceName, string comment = "")
        {
            if (targetWorkItem == null) throw new ArgumentNullException(nameof(targetWorkItem));
            if (string.IsNullOrWhiteSpace(targetWorkItem.Url)) throw new ArgumentException($"The '{nameof(targetWorkItem)}' must have a valid '{nameof(WorkItem.Url)}' value", nameof(targetWorkItem));

            return AddReverseRelation(targetWorkItem.Url, linkTypeReferenceName, comment);
        }

        /// <summary>
        /// Adds a forward relation of the given <paramref name="linkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItemUrl" /> to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="targetWorkItemUrl">The target work item URL.</param>
        /// <param name="linkTypeReferenceName">Reference Name of the work item link type.</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddForwardRelation(string targetWorkItemUrl, string linkTypeReferenceName, string comment = "")
        {
            if (string.IsNullOrWhiteSpace(targetWorkItemUrl))
                throw new ArgumentException($"Value for '{nameof(targetWorkItemUrl)}' cannot be null or whitespace.", nameof(targetWorkItemUrl));

            if(string.IsNullOrWhiteSpace(linkTypeReferenceName))
                throw new ArgumentException($"Value for '{nameof(linkTypeReferenceName)}' cannot be null or whitespace.", nameof(linkTypeReferenceName));

            return AddRelation(
                targetWorkItemUrl,
                linkTypeReferenceName + Constants.WorkItems.RelationsForwardSuffix,
                comment);
        }

        /// <summary>
        /// Adds a reverse relation of the given <paramref name="linkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItemUrl" /> to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="targetWorkItemUrl">The target work item URL.</param>
        /// <param name="linkTypeReferenceName">Reference Name of the work item link type.</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddReverseRelation(string targetWorkItemUrl, string linkTypeReferenceName, string comment = "")
        {
            if (string.IsNullOrWhiteSpace(targetWorkItemUrl))
                throw new ArgumentException($"Value for '{nameof(targetWorkItemUrl)}' cannot be null or whitespace.", nameof(targetWorkItemUrl));

            if (string.IsNullOrWhiteSpace(linkTypeReferenceName))
                throw new ArgumentException($"Value for '{nameof(linkTypeReferenceName)}' cannot be null or whitespace.", nameof(linkTypeReferenceName));

            return AddRelation(
                targetWorkItemUrl,
                linkTypeReferenceName + Constants.WorkItems.RelationsReverseSuffix,
                comment);
        }

        /// <summary>
        /// Adds an attachment to a work item by using the <paramref name="attachmentReference"/> and optionally setting the attachment's <paramref name="comment"/>.
        /// </summary>
        /// <param name="attachmentReference">The attachment reference.</param>
        /// <param name="comment">The (optional) attachment comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">attachmentReference</exception>
        public virtual WorkItemPatchDocumentBuilder AddAttachment(AttachmentReference attachmentReference, string comment = "")
        {
            if (attachmentReference == null) throw new ArgumentNullException(nameof(attachmentReference));

            PatchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"{Constants.WorkItems.RelationsBasePath}/-",
                    Value = new
                    {
                        rel = Constants.WorkItems.RelationReferenceNameForAttachedFiles,
                        url = attachmentReference.Url,
                        attributes = new
                        {
                            comment = !string.IsNullOrWhiteSpace(comment)
                                ? comment
                                : string.Empty
                        }
                    }
                });

            return this;
        }

        /// <summary>
        /// Adds a hyperlink to a work item by using the <paramref name="hyperLink"/> and optionally setting the links's <paramref name="comment"/>.
        /// </summary>
        /// <param name="hyperLink">The hyperlink uri.</param>
        /// <param name="comment">The (optional) hyperlink comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">attachmentReference</exception>
        public virtual WorkItemPatchDocumentBuilder AddHyperlink(Uri hyperLink, string comment = "")
        {
            if (hyperLink == null) throw new ArgumentNullException(nameof(hyperLink));

            PatchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"{Constants.WorkItems.RelationsBasePath}/-",
                    Value = new
                    {
                        rel = Constants.WorkItems.RelationReferenceNameForHyperlinks,
                        url = hyperLink,
                        attributes = new
                        {
                            comment = !string.IsNullOrWhiteSpace(comment)
                                ? comment
                                : string.Empty
                        }
                    }
                });

            return this;
        }

        /// <summary>
        /// Adds an artifucat link to a work item by using the <paramref name="artifactLink"/> and optionally setting the links's <paramref name="comment"/>.
        /// </summary>
        /// <param name="artifactLink">The artifact link.</param>
        /// <param name="comment">The (optional) artifact link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">attachmentReference</exception>
        public virtual WorkItemPatchDocumentBuilder AddArtifactLink(ArtifactLink artifactLink, string comment = "")
        {
            if (artifactLink == null) throw new ArgumentNullException(nameof(artifactLink));

            PatchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"{Constants.WorkItems.RelationsBasePath}/-",
                    Value = new
                    {
                        rel = Constants.WorkItems.RelationReferenceNameForArtifactLinks,
                        url = artifactLink.ReferencedUri,
                        attributes = new
                        {
                            comment = !string.IsNullOrWhiteSpace(comment)
                                ? comment
                                : string.Empty
                        }
                    }
                });

            return this;
        }

        /// <summary>
        /// Adds the field <paramref name="value"/> for the provided <paramref name="fieldReferenceName"/> to this <see cref="WorkItemPatchDocumentBuilder"/> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="fieldReferenceName">Reference Name of the work item field.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public virtual WorkItemPatchDocumentBuilder AddOrUpdateFieldValue(string fieldReferenceName, object value)
        {
            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException($"Value for '{nameof(fieldReferenceName)}' cannot be null or whitespace.", nameof(fieldReferenceName));

            var pathForFieldReferenceName = GetPathForFieldReferenceName(fieldReferenceName);

            var existingAddOperationForField = PatchDocument.FirstOrDefault(patchOperation =>
                patchOperation.Operation == Operation.Add &&
                string.Equals(patchOperation.Path, pathForFieldReferenceName, StringComparison.OrdinalIgnoreCase));

            if (existingAddOperationForField != null)
            {
                existingAddOperationForField.Value = value;
            }
            else
            {
                PatchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = GetPathForFieldReferenceName(fieldReferenceName),
                        Value = value
                    }
                );
            }
            
            return this;
        }

        /// <summary>
        /// Adds a history entry for the provided <paramref name="comment"/>.
        /// </summary>
        /// <param name="comment">The comment to add.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">comment - comment</exception>
        public virtual WorkItemPatchDocumentBuilder AddComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException($"Value for '{nameof(comment)}' cannot be null or whitespace.", nameof(comment));

            return AddOrUpdateFieldValue(WellKnownWorkItemFieldReferenceNames.System.History, comment);
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

            PatchDocument.RemoveAll(operation =>
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

            return $"{Constants.WorkItems.FieldsBasePath}/{fieldReferenceName}";
        }

        /// <summary>
        /// Gets the <see cref="JsonPatchDocument"/> representing the current state of this <see cref="WorkItemPatchDocumentBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public virtual JsonPatchDocument ToJsonPatchDocument() => PatchDocument;

        /// <summary>
        /// Performs an implicit conversion from <see cref="WorkItemPatchDocumentBuilder"/> to <see cref="JsonPatchDocument"/>.
        /// </summary>
        /// <param name="workItemBuilder">The work item builder.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JsonPatchDocument(WorkItemPatchDocumentBuilder workItemBuilder) => workItemBuilder.PatchDocument;
    }

    public abstract class WorkItemPatchDocumentBuilder<T> : WorkItemPatchDocumentBuilder
        where T : WorkItemPatchDocumentBuilder
    {
        /// <summary>
        /// Adds the field <paramref name="value"/> for the provided <paramref name="fieldReferenceName"/> to this instance.
        /// </summary>
        /// <param name="fieldReferenceName">Reference Name of the work item field.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public new virtual T AddOrUpdateFieldValue(string fieldReferenceName, object value) => base.AddOrUpdateFieldValue(fieldReferenceName, value) as T;

        /// <summary>
        /// Adds a history entry for the provided <paramref name="comment"/>.
        /// </summary>
        /// <param name="comment">The comment to add.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">comment - comment</exception>
        public new virtual T AddComment(string comment) => base.AddComment(comment) as T;

        /// <summary>
        /// Adds a forward relation of the given <paramref name="workItemLinkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItemUrl" /> to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="targetWorkItemUrl">The target work item URL.</param>
        /// <param name="workItemLinkTypeReferenceName">Reference Name of the work item link type.</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public new virtual T AddForwardRelation(string targetWorkItemUrl, string workItemLinkTypeReferenceName,
            string comment = "") => base.AddForwardRelation(targetWorkItemUrl, workItemLinkTypeReferenceName, comment) as T;

        /// <summary>
        /// Adds a reverse relation of the given <paramref name="workItemLinkTypeReferenceName" /> type pointing to the provided <paramref name="targetWorkItemUrl" /> to this <see cref="WorkItemPatchDocumentBuilder" /> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="targetWorkItemUrl">The target work item URL.</param>
        /// <param name="workItemLinkTypeReferenceName">Reference Name of the work item link type.</param>
        /// <param name="comment">The (optional) relation/link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public new virtual T AddReverseRelation(string targetWorkItemUrl, string workItemLinkTypeReferenceName,
            string comment = "") => base.AddReverseRelation(targetWorkItemUrl, workItemLinkTypeReferenceName, comment) as T;

        /// <summary>
        /// Adds an attachment to a work item by using the <paramref name="attachmentReference"/> and optionally setting the attachment's <paramref name="comment"/>.
        /// </summary>
        /// <param name="attachmentReference">The attachment reference.</param>
        /// <param name="comment">The (optional) attachment comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">attachmentReference</exception>
        public new virtual T AddAttachment(AttachmentReference attachmentReference, string comment = "") => base.AddAttachment(attachmentReference, comment) as T;

        /// <summary>
        /// Adds a hyperlink to a work item by using the <paramref name="hyperLink"/> and optionally setting the links's <paramref name="comment"/>.
        /// </summary>
        /// <param name="hyperLink">The hyperlink uri.</param>
        /// <param name="comment">The (optional) attachment comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">attachmentReference</exception>
        public new virtual T AddHyperlink(Uri hyperLink, string comment = "") => base.AddHyperlink(hyperLink, comment) as T;

        /// <summary>
        /// Adds an artifucat link to a work item by using the <paramref name="artifactLink"/> and optionally setting the links's <paramref name="comment"/>.
        /// </summary>
        /// <param name="artifactLink">The artifact link.</param>
        /// <param name="comment">The (optional) artifact link comment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">attachmentReference</exception>
        public new virtual T AddArtifactLink(ArtifactLink artifactLink, string comment = "") => base.AddArtifactLink(artifactLink, comment) as T;

        /// <summary>
        /// Removes the field values for the provided <paramref name="fieldReferenceName"/> from this <see cref="WorkItemPatchDocumentBuilder"/> and ultimately the resulting <see cref="JsonPatchOperation"/>.
        /// </summary>
        /// <param name="fieldReferenceName">Reference Name of the work item field.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">fieldReferenceName - fieldReferenceName</exception>
        public new virtual T RemoveFieldValue(string fieldReferenceName) => base.RemoveFieldValue(fieldReferenceName) as T;
    }
}
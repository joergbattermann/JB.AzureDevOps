using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace JB.TeamFoundationServer.WorkItemTracking
{
    /// <summary>
    /// Builds a <see cref="JsonPatchDocument"/> to be used in Work Item update methods.
    /// </summary>
    public class UpdateWorkItemPatchDocumentBuilder : WorkItemPatchDocumentBuilder<UpdateWorkItemPatchDocumentBuilder>
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

        /// <summary>
        /// Changes the the work item type. Depending on the <paramref name="newWorkItemType"/> additional field value(s) might needed to be set.
        /// </summary>
        /// <param name="newWorkItemType">The new work item type name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">newWorkItemType - newWorkItemType</exception>
        public virtual UpdateWorkItemPatchDocumentBuilder ChangeWorkItemType(string newWorkItemType)
        {
            if (string.IsNullOrWhiteSpace(newWorkItemType))
                throw new ArgumentException($"Value for '{nameof(newWorkItemType)}' cannot be null or whitespace.", nameof(newWorkItemType));

            return AddOrUpdateFieldValue(WellKnownWorkItemFieldReferenceNames.System.WorkItemType, newWorkItemType);
        }

        /// <summary>
        /// Removes the relation for the given <paramref name="relationid"/>.
        /// </summary>
        /// <param name="relationid">The relation identifier.</param>
        /// <returns></returns>
        public virtual UpdateWorkItemPatchDocumentBuilder RemoveRelation(int relationid)
        {
            PatchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Remove,
                    Path = "/relations/" + relationid
                }
            );

            return this;
        }

        /// <summary>
        /// Removes the attachment relation for the given <paramref name="attachmentRelationId"/>.
        /// </summary>
        /// <param name="attachmentRelationId">The attachment relation identifier.</param>
        public virtual UpdateWorkItemPatchDocumentBuilder RemoveAttachmentRelation(int attachmentRelationId)
        {
            return RemoveRelation(attachmentRelationId);
        }

        /// <summary>
        /// Removes the hyperlink relation for the given <paramref name="hyperlinkRelationId"/>.
        /// </summary>
        /// <param name="hyperlinkRelationId">The hyperlink relation identifier.</param>
        public virtual UpdateWorkItemPatchDocumentBuilder RemoveHyperlinkRelation(int hyperlinkRelationId)
        {
            return RemoveRelation(hyperlinkRelationId);
        }

        /// <summary>
        /// Removes the artifact link relation for the given <paramref name="artifactLinkRelationId"/>.
        /// </summary>
        /// <param name="artifactLinkRelationId">The artifact link relation identifier.</param>
        public virtual UpdateWorkItemPatchDocumentBuilder RemoveArtifactLinkRelation(int artifactLinkRelationId)
        {
            return RemoveRelation(artifactLinkRelationId);
        }
    }
}
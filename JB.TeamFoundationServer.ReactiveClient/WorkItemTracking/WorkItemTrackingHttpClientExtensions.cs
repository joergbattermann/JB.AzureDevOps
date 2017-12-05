using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace JB.TeamFoundationServer.WorkItemTracking
{
    /// <summary>
    /// Extension Methods for <see cref="WorkItemTrackingHttpClient"/> instances.
    /// </summary>
    public static class WorkItemTrackingHttpClientExtensions
    {
        /// <summary>
        /// Creates an attachment for the provided <paramref name="uploadStream"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="uploadStream">The upload stream.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="uploadType">Type of the upload.</param>
        /// <param name="areaPath">The area path.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// workItemTrackingHttpClient
        /// or
        /// uploadStream
        /// </exception>
        public static IObservable<AttachmentReference> CreateAttachment(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Stream uploadStream, string fileName = null,
            string uploadType = null, string areaPath = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (uploadStream == null) throw new ArgumentNullException(nameof(uploadStream));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateAttachmentAsync(uploadStream, fileName, uploadType, areaPath, userState, token));
        }

        /// <summary>
        /// Creates or updates the classification node.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="postedNode">The posted node.</param>
        /// <param name="project">The project.</param>
        /// <param name="structureGroup">The structure group.</param>
        /// <param name="path">The path.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemClassificationNode> CreateOrUpdateClassificationNode(this WorkItemTrackingHttpClient workItemTrackingHttpClient, WorkItemClassificationNode postedNode, Guid project, TreeStructureGroup structureGroup, string path = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateOrUpdateClassificationNodeAsync(postedNode, project, structureGroup, path, userState, token));
        }

        /// <summary>
        /// Creates or moves a query.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="postedQuery">The query to create.</param>
        /// <param name="project">Project ID</param>
        /// <param name="parentQueryItemPath">The parent path for the query to create or move to.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<QueryHierarchyItem> CreateOrMoveQuery(this WorkItemTrackingHttpClient workItemTrackingHttpClient, QueryHierarchyItem postedQuery, Guid project, string parentQueryItemPath, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateQueryAsync(postedQuery, project, parentQueryItemPath, userState, token));
        }


        /// <summary>
        /// Creates a single work item for the provided <paramref name="creationDocument"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="creationDocument">The JSON Patch document representing the new work item.</param>
        /// <param name="project">Project ID</param>
        /// <param name="workItemType">The work item type of the work item to create</param>
        /// <param name="validateOnly">Indicate if you only want to validate the changes without saving the work item</param>
        /// <param name="bypassRules">Do not enforce the work item type rules on this update</param>
        /// <param name="suppressNotifications">Do not fire any notifications for this change</param>
        /// <param name="userState">The userState object.</param>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> CreateWorkItem(this WorkItemTrackingHttpClient workItemTrackingHttpClient, JsonPatchDocument creationDocument, string workItemType, Guid project,  bool? validateOnly = null, bool? bypassRules = null, bool? suppressNotifications = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (creationDocument == null) throw new ArgumentNullException(nameof(creationDocument));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateWorkItemAsync(creationDocument, project, workItemType, validateOnly, bypassRules, suppressNotifications, userState, token));
        }

        /// <summary>
        /// Creates a single work item for the provided <paramref name="title" /> and <paramref name="workItemType"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="title">The title of the work item to create.</param>
        /// <param name="project">Project ID</param>
        /// <param name="workItemType">The work item type of the work item to create</param>
        /// <param name="validateOnly">Indicate if you only want to validate the changes without saving the work item</param>
        /// <param name="bypassRules">Do not enforce the work item type rules on this update</param>
        /// <param name="suppressNotifications">Do not fire any notifications for this change</param>
        /// <param name="userState">The userState object.</param>
        /// <param name="fieldsAndValues">The additional fields and values to set.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        /// <exception cref="ArgumentException">
        /// Value cannot be null or whitespace. - title
        /// or
        /// Value cannot be null or whitespace. - workItemType
        /// </exception>
        public static IObservable<WorkItem> CreateWorkItem(this WorkItemTrackingHttpClient workItemTrackingHttpClient, string title, string workItemType, Guid project, bool? validateOnly = null, bool? bypassRules = null, bool? suppressNotifications = null, object userState = null, params KeyValuePair<string, object>[] fieldsAndValues)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
            if (string.IsNullOrWhiteSpace(workItemType))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(workItemType));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateWorkItemAsync(
                new CreateWorkItemPatchDocumentBuilder(workItemType, title, fieldsAndValues),
                project,
                workItemType,
                validateOnly,
                bypassRules, suppressNotifications, userState, token));
        }

        /// <summary>
        /// Updates a single work item for the provided <paramref name="updateDocument"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="updateDocument">The JSON Patch document representing the update</param>
        /// <param name="id">The id of the work item to update</param>
        /// <param name="validateOnly">Indicate if you only want to validate the changes without saving the work item</param>
        /// <param name="bypassRules">Do not enforce the work item type rules on this update</param>
        /// <param name="suppressNotifications">Do not fire any notifications for this change</param>
        /// <param name="userState">The userState object.</param>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> UpdateWorkItem(this WorkItemTrackingHttpClient workItemTrackingHttpClient, JsonPatchDocument updateDocument, int id, bool? validateOnly = null, bool? bypassRules = null, bool? suppressNotifications = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (updateDocument == null) throw new ArgumentNullException(nameof(updateDocument));

            return Observable.FromAsync(token => workItemTrackingHttpClient.UpdateWorkItemAsync(updateDocument, id, validateOnly, bypassRules, suppressNotifications, userState, token));
        }

        /// <summary>
        /// Updates a single work item for the provided <paramref name="updateDocument"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="updateDocument">The JSON Patch document representing the update</param>
        /// <param name="workItem">The work item to update</param>
        /// <param name="validateOnly">Indicate if you only want to validate the changes without saving the work item</param>
        /// <param name="bypassRules">Do not enforce the work item type rules on this update</param>
        /// <param name="suppressNotifications">Do not fire any notifications for this change</param>
        /// <param name="userState">The userState object.</param>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> UpdateWorkItem(this WorkItemTrackingHttpClient workItemTrackingHttpClient, JsonPatchDocument updateDocument, WorkItem workItem, bool? validateOnly = null, bool? bypassRules = null, bool? suppressNotifications = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (updateDocument == null) throw new ArgumentNullException(nameof(updateDocument));
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (workItem.Id == null) throw new ArgumentException($"The provided {nameof(workItem)} must have an {nameof(workItem.Id)}", nameof(workItem));

            return workItemTrackingHttpClient.UpdateWorkItem(updateDocument, workItem.Id.Value, validateOnly, bypassRules, suppressNotifications, userState);
        }

        /// <summary>
        /// Gets the work item for the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The <see cref="WorkItemTrackingHttpClient"/> to use.</param>
        /// <param name="id">The workitem identifier.</param>
        /// <param name="fields">The work item fields to retrieve.</param>
        /// <param name="asOf">The 'As of time' of the work item to retrieve.</param>
        /// <param name="expand">The <see cref="WorkItemExpand"/> to apply to the underlying client.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> GetWorkItem(this WorkItemTrackingHttpClient workItemTrackingHttpClient, int id, IEnumerable<string> fields = null, DateTime? asOf = null, WorkItemExpand? expand = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return id <= 0
                ? Observable.Empty<WorkItem>()
                : Observable.FromAsync(token => workItemTrackingHttpClient.GetWorkItemAsync(id, fields, asOf, expand, userState, token)); 
        }
        
        /// <summary>
        /// Gets the work items for the provided <paramref name="ids" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The <see cref="WorkItemTrackingHttpClient" /> to use.</param>
        /// <param name="ids">The workitem identifiers.</param>
        /// <param name="fields">The work item fields to retrieve.</param>
        /// <param name="asOf">The 'As of time' of the work item to retrieve.</param>
        /// <param name="expand">The <see cref="WorkItemExpand"/> to apply to the underlying client.</param>
        /// <param name="errorPolicy">The error policy.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> GetWorkItems(this WorkItemTrackingHttpClient workItemTrackingHttpClient, IEnumerable<int> ids, IEnumerable<string> fields = null,
            DateTime? asOf = null,
            WorkItemExpand? expand = null,
            WorkItemErrorPolicy? errorPolicy = null,
            object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            if (ids == null)
            {
                return Observable.Empty<WorkItem>();
            }

            // else
            return Observable.FromAsync(
                    token => workItemTrackingHttpClient.GetWorkItemsAsync(ids, fields, asOf, expand, errorPolicy, userState, token))
                .SelectMany(workItems => workItems)
                .OfType<WorkItem>(); // if errorPolicy is set to WorkItemErrorPolicy.Omit, all non-found ids / their workitems are returned by the VSTS .net Api as null (as of writing)
        }
    }
}
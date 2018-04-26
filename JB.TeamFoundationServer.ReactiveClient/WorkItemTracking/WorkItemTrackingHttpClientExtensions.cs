using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.TeamFoundation.Core.WebApi.Types;
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
        /// <param name="uploadType">Type of the upload. If defined it must be either 'simple' or 'chunked' according to <see href="https://www.visualstudio.com/en-us/docs/integrate/api/wit/attachments#upload-an-attachment">API documentation</see>.</param>
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
        /// Creates an attachment for the provided <paramref name="fileToUpload" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="fileToUpload">The file to upload.</param>
        /// <param name="uploadType">Type of the upload. If defined it must be either 'simple' or 'chunked' according to <see href="https://www.visualstudio.com/en-us/docs/integrate/api/wit/attachments#upload-an-attachment">API documentation</see>.</param>
        /// <param name="areaPath">The area path.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient
        /// or
        /// uploadStream</exception>
        public static IObservable<AttachmentReference> CreateAttachment(this WorkItemTrackingHttpClient workItemTrackingHttpClient, FileInfo fileToUpload,
            string uploadType = null, string areaPath = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (fileToUpload == null) throw new ArgumentNullException(nameof(fileToUpload));
            if (fileToUpload.Exists == false) throw new FileNotFoundException($"File '{fileToUpload.FullName}' cannot be found or process has insufficient permissions.");

            return Observable.FromAsync(async token =>
            {
                using (var fileStream = fileToUpload.Open(FileMode.Open, FileAccess.Read))
                {
                    return await workItemTrackingHttpClient.CreateAttachmentAsync(fileStream, fileToUpload.Name, uploadType, areaPath, userState, token);
                }
            });
        }

        /// <summary>
        /// Creates or updates the classification node.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="postedNode">The posted node.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="structureGroup">The structure group.</param>
        /// <param name="path">The path.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemClassificationNode> CreateOrUpdateClassificationNode(this WorkItemTrackingHttpClient workItemTrackingHttpClient, WorkItemClassificationNode postedNode, Guid projectId, TreeStructureGroup structureGroup, string path = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateOrUpdateClassificationNodeAsync(postedNode, projectId, structureGroup, path, userState, token));
        }

        /// <summary>
        /// Gets the classification node.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="structureGroup">The structure group.</param>
        /// <param name="path">Path of the classification node.</param>
        /// <param name="depth">Depth of children to retrieve.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemClassificationNode> GetClassificationNode(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid projectId, TreeStructureGroup structureGroup, string path = null, int? depth = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.GetClassificationNodeAsync(projectId, structureGroup, path, depth, userState, token));
        }

        /// <summary>
        /// Gets the area path node.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="path">Path of the classification node.</param>
        /// <param name="depth">Depth of children to retrieve.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemClassificationNode> GetAreaPathNode(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid projectId, string path = null, int? depth = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return workItemTrackingHttpClient.GetClassificationNode(projectId, TreeStructureGroup.Areas, path, depth, userState);
        }

        /// <summary>
        /// Gets the root area path node.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="project">The projectId.</param>
        /// <param name="depth">Depth of children to retrieve.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemClassificationNode> GetRootAreaPathNode(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid projectId, int? depth = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return workItemTrackingHttpClient.GetAreaPathNode(projectId, path: null, depth: depth, userState: userState);
        }

        /// <summary>
        /// Gets the iteration path node.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="path">Path of the classification node.</param>
        /// <param name="depth">Depth of children to retrieve.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemClassificationNode> GetIterationPathNode(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid projectId, string path = null, int? depth = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return workItemTrackingHttpClient.GetClassificationNode(projectId, TreeStructureGroup.Iterations, path, depth, userState);
        }

        /// <summary>
        /// Gets the root iteration path node.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="depth">Depth of children to retrieve.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemClassificationNode> GetRootIterationPathNode(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid projectId, int? depth = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return workItemTrackingHttpClient.GetIterationPathNode(projectId, path: null, depth: depth, userState: userState);
        }

        /// <summary>
        /// Gets the root classification (area and iteration) nodes.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="depth">Depth of children to retrieve.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemClassificationNode> GetRootClassificationNodes(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid projectId, int? depth = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.GetRootNodesAsync(projectId, depth, userState, token))
                .SelectMany(classificationNodes => classificationNodes);
        }

        /// <summary>
        /// Creates or moves a query.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="postedQuery">The query to create.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parentQueryItemPath">The parent path for the query to create or move to.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<QueryHierarchyItem> CreateOrMoveQuery(this WorkItemTrackingHttpClient workItemTrackingHttpClient, QueryHierarchyItem postedQuery, Guid projectId, string parentQueryItemPath, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateQueryAsync(postedQuery, projectId, parentQueryItemPath, userState, token));
        }

        /// <summary>
        /// Gets all availabile work item tracking artifact link types for the <paramref name="workItemTrackingHttpClient"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkArtifactLink> GetArtifactLinkTypes(this WorkItemTrackingHttpClient workItemTrackingHttpClient, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.GetWorkArtifactLinkTypesAsync(userState, token))
                .SelectMany(workArtifactLinks => workArtifactLinks);
        }

        /// <summary>
        /// Executes the provided batch <paramref name="requests"/> in a single call.
        /// See <see href="https://www.visualstudio.com/en-us/docs/integrate/api/wit/batch"/> for details.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="requests">The batch requests to execute</param>
        /// <param name="userState">The userState object.</param>
        /// <returns>The list of responses for each request.</returns>
        public static IObservable<WitBatchResponse> ExecuteBatchRequests(this WorkItemTrackingHttpClient workItemTrackingHttpClient, IEnumerable<WitBatchRequest> requests, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            
            return Observable.FromAsync(token => workItemTrackingHttpClient.ExecuteBatchRequest(requests, userState, token))
                .SelectMany(witBatchResponses => witBatchResponses);
        }

        /// <summary>
        /// Gets the work item references for the provided artifact uris in the <paramref name="artifactUriQuery"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="artifactUriQuery">The artifact URI query.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// workItemTrackingHttpClient
        /// or
        /// artifactUriQuery
        /// </exception>
        public static IObservable<(string ArtifactUri, IEnumerable<WorkItemReference> WorkItemReferences)> GetWorkItemReferencesForArtifactUris(this WorkItemTrackingHttpClient workItemTrackingHttpClient, ArtifactUriQuery artifactUriQuery, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (artifactUriQuery == null) throw new ArgumentNullException(nameof(artifactUriQuery));

            return Observable.FromAsync(token => workItemTrackingHttpClient.QueryWorkItemsForArtifactUrisAsync(artifactUriQuery, userState, token))
                .SelectMany(artifactUriQueryResult => artifactUriQueryResult.ArtifactUrisQueryResult)
                .Where(keyValuePair => !string.IsNullOrWhiteSpace(keyValuePair.Key))
                .Select(keyValuePair => (keyValuePair.Key, keyValuePair.Value ?? Enumerable.Empty<WorkItemReference>()));
        }

        /// <summary>
        /// Gets the work item references for the provided artifact uris in the <paramref name="artifactUris"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="artifactUris">The artifact uris to query for.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// workItemTrackingHttpClient
        /// or
        /// artifactUriQuery
        /// </exception>
        public static IObservable<(string ArtifactUri, IEnumerable<WorkItemReference> WorkItemReferences)> GetWorkItemReferencesForArtifactUris(this WorkItemTrackingHttpClient workItemTrackingHttpClient, IEnumerable<string> artifactUris, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (artifactUris == null) throw new ArgumentNullException(nameof(artifactUris));

            return workItemTrackingHttpClient.GetWorkItemReferencesForArtifactUris(new ArtifactUriQuery { ArtifactUris = artifactUris}, userState);
        }
        
        /// <summary>
        /// Dowloads the content of the attachment for the provided <paramref name="attachmentId"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="attachmentId">The attachment identifier.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<Stream> DownloadAttachmentContent(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid attachmentId, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.GetAttachmentContentAsync(attachmentId, userState: userState, cancellationToken: token));
        }

        /// <summary>
        /// Downloads the content of the attachment for the provided <paramref name="attachmentId" /> and writes it to the <paramref name="outputFile" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="attachmentId">The attachment identifier.</param>
        /// <param name="outputFile">The output / target file to write the content to.</param>
        /// <param name="overwriteExisting">if set to <c>true</c> [overwrite existing].</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<Unit> DownloadAttachmentToFile(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid attachmentId, FileInfo outputFile, bool overwriteExisting = false, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (outputFile == null) throw new ArgumentNullException(nameof(outputFile));

            return workItemTrackingHttpClient.DownloadAttachmentContent(attachmentId, userState)
                .SelectMany(async (contentStream, cancellationToken) =>
                {
                    using (var outputStream = outputFile.Open(overwriteExisting ? FileMode.OpenOrCreate : FileMode.Create, FileAccess.ReadWrite))
                    {
                        // ToDo: make the buffer value an optional parameter or use a BCL constant (if available) - this is dirty
                        await contentStream.CopyToAsync(outputStream, 81920, cancellationToken);
                    }
                    
                    return Unit.Default;
                });
        }

        /// <summary>
        /// Creates a single work item for the provided <paramref name="creationDocument" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="creationDocument">The JSON Patch document representing the new work item.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="workItemType">The work item type of the work item to create</param>
        /// <param name="validateOnly">Indicate if you only want to validate the changes without saving the work item</param>
        /// <param name="bypassRules">Do not enforce the work item type rules on this update</param>
        /// <param name="suppressNotifications">Do not fire any notifications for this change</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> CreateWorkItem(this WorkItemTrackingHttpClient workItemTrackingHttpClient,
            JsonPatchDocument creationDocument, Guid projectId, string workItemType, bool? validateOnly = null,
            bool? bypassRules = null, bool? suppressNotifications = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (creationDocument == null) throw new ArgumentNullException(nameof(creationDocument));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateWorkItemAsync(creationDocument, projectId, workItemType, validateOnly, bypassRules, suppressNotifications, userState, token));
        }

        /// <summary>
        /// Creates a single work item for the provided <paramref name="title" /> and <paramref name="workItemType" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="title">The title of the work item to create.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="workItemType">The work item type of the work item to create</param>
        /// <param name="validateOnly">Indicate if you only want to validate the changes without saving the work item</param>
        /// <param name="bypassRules">Do not enforce the work item type rules on this update</param>
        /// <param name="suppressNotifications">Do not fire any notifications for this change</param>
        /// <param name="userState">The userState object.</param>
        /// <param name="fieldsAndValues">The additional fields and values to set.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace. - title
        /// or
        /// Value cannot be null or whitespace. - workItemType</exception>
        public static IObservable<WorkItem> CreateWorkItem(this WorkItemTrackingHttpClient workItemTrackingHttpClient,
            string title, Guid projectId, string workItemType, bool? validateOnly = null, bool? bypassRules = null,
            bool? suppressNotifications = null, object userState = null,
            params KeyValuePair<string, object>[] fieldsAndValues)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
            if (string.IsNullOrWhiteSpace(workItemType))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(workItemType));

            return Observable.FromAsync(token => workItemTrackingHttpClient.CreateWorkItemAsync(
                new CreateWorkItemPatchDocumentBuilder(workItemType, title, fieldsAndValues),
                projectId,
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
        /// Updates multiple work items with the provided <paramref name="updateDocument"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="updateDocument">The JSON Patch document representing the update.</param>
        /// <param name="ids">The ids of the work item to update</param>
        /// <param name="bypassRules">Do not enforce the work item type rules on this update.</param>
        /// <param name="suppressNotifications">Do not fire any notifications for this change.</param>
        /// <param name="userState">The userState object.</param>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WitBatchResponse> UpdateWorkItems(this WorkItemTrackingHttpClient workItemTrackingHttpClient, JsonPatchDocument updateDocument, IEnumerable<int> ids, bool? bypassRules = null, bool? suppressNotifications = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (updateDocument == null) throw new ArgumentNullException(nameof(updateDocument));
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            return workItemTrackingHttpClient.ExecuteBatchRequests(
                ids.Select(id => workItemTrackingHttpClient.CreateWorkItemBatchRequest(id, updateDocument, bypassRules ?? false, suppressNotifications ?? false)),
                userState);
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

        /// <summary>
        /// Gets the work items for the provided <paramref name="workItemReferences" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The <see cref="WorkItemTrackingHttpClient" /> to use.</param>
        /// <param name="workItemReferences">The workitem references.</param>
        /// <param name="fields">The work item fields to retrieve.</param>
        /// <param name="asOf">The 'As of time' of the work item to retrieve.</param>
        /// <param name="expand">The <see cref="WorkItemExpand"/> to apply to the underlying client.</param>
        /// <param name="errorPolicy">The error policy.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> GetWorkItems(this WorkItemTrackingHttpClient workItemTrackingHttpClient, IEnumerable<WorkItemReference> workItemReferences, IEnumerable<string> fields = null,
            DateTime? asOf = null,
            WorkItemExpand? expand = null,
            WorkItemErrorPolicy? errorPolicy = null,
            object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            if (workItemReferences == null)
            {
                return Observable.Empty<WorkItem>();
            }

            // else
            return workItemTrackingHttpClient.GetWorkItems(
                workItemReferences
                    .Select(reference => reference.Id)
                    .Distinct(),
                fields,
                asOf,
                expand,
                errorPolicy,
                userState);
        }

        /// <summary>
        /// Gets the work item relation type for the provided <paramref name="workItemLinkTypeReferenceName"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="workItemLinkTypeReferenceName">Reference name of the work item link type.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemRelationType> GetWorkItemRelationType(this WorkItemTrackingHttpClient workItemTrackingHttpClient, string workItemLinkTypeReferenceName, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.GetRelationTypeAsync(workItemLinkTypeReferenceName, userState, token))
                .OfType<WorkItemRelationType>();
        }

        /// <summary>
        /// Gets the available work item relation types.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="userState">The userState object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemRelationType> GetWorkItemRelationTypes(this WorkItemTrackingHttpClient workItemTrackingHttpClient, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.GetRelationTypesAsync(userState, token))
                .SelectMany(workItemRelationTypes => workItemRelationTypes);
        }

        /// <summary>
        /// Gets the linked source work items.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="workItemLinks">The work item links.</param>
        /// <param name="workItemRelationTypeReferenceName">Name of the work item relation type reference.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="asOf">As of.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="errorPolicy">The error policy.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> GetLinkedSourceWorkItems(this WorkItemTrackingHttpClient workItemTrackingHttpClient,
            IEnumerable<WorkItemLink> workItemLinks,
            string workItemRelationTypeReferenceName = "",
            IEnumerable<string> fields = null,
            DateTime? asOf = null,
            WorkItemExpand? expand = null,
            WorkItemErrorPolicy? errorPolicy = null,
            object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            if (workItemLinks == null)
            {
                return Observable.Empty<WorkItem>();
            }

            // else
            return Observable.Create<WorkItem>(observer =>
            {
                return workItemTrackingHttpClient.GetWorkItems(
                        workItemLinks
                            .Where(workItemLink => !string.IsNullOrWhiteSpace(workItemLink.Rel)
                                                   && workItemLink.Source != null
                                                   && (string.IsNullOrWhiteSpace(workItemRelationTypeReferenceName) ||
                                                       string.Equals(workItemLink.Rel,
                                                           workItemRelationTypeReferenceName,
                                                           StringComparison.OrdinalIgnoreCase)))
                            .Select(workItemLink => workItemLink.Source.Id)
                            .Distinct(),
                        fields,
                        asOf,
                        expand,
                        errorPolicy,
                        userState)
                    .Subscribe(observer);
            });
        }

        /// <summary>
        /// Gets the linked source work items.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="workItemLinks">The work item links.</param>
        /// <param name="workItemRelationTypeReferenceName">Name of the work item relation type reference.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="asOf">As of.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="errorPolicy">The error policy.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> GetLinkedTargetWorkItems(this WorkItemTrackingHttpClient workItemTrackingHttpClient,
            IEnumerable<WorkItemLink> workItemLinks,
            string workItemRelationTypeReferenceName = "",
            IEnumerable<string> fields = null,
            DateTime? asOf = null,
            WorkItemExpand? expand = null,
            WorkItemErrorPolicy? errorPolicy = null,
            object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            if (workItemLinks == null)
            {
                return Observable.Empty<WorkItem>();
            }

            // else
            return Observable.Create<WorkItem>(observer =>
            {
                return workItemTrackingHttpClient.GetWorkItems(
                        workItemLinks
                            .Where(workItemLink => !string.IsNullOrWhiteSpace(workItemLink.Rel)
                                                   && workItemLink.Target != null
                                                   && (string.IsNullOrWhiteSpace(workItemRelationTypeReferenceName) ||
                                                       string.Equals(workItemLink.Rel,
                                                           workItemRelationTypeReferenceName,
                                                           StringComparison.OrdinalIgnoreCase)))
                            .Select(workItemLink => workItemLink.Target.Id)
                            .Distinct(),
                        fields,
                        asOf,
                        expand,
                        errorPolicy,
                        userState)
                    .Subscribe(observer);
            });
        }

        /// <summary>
        /// Gets the related work items for the <paramref name="workItem" /> of the given <paramref name="workItemRelationTypeReferenceName" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="workItem">The work item.</param>
        /// <param name="projectId">Project ID</param>
        /// <param name="workItemRelationTypeReferenceName">Name of the work item relation type reference. i.e. 'System.LinkTypes.Hierarchy-Forward'.</param>
        /// <param name="targetWorkItemTypeName">Name of the target work item type.</param>
        /// <param name="timePrecision">Whether or not to use time precision.</param>
        /// <param name="count">The max number of results to return.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// workItemTrackingHttpClient
        /// or
        /// workItem
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">workItemRelationTypeReferenceName</exception>
        public static IObservable<WorkItem> GetRelatedWorkItems(this WorkItemTrackingHttpClient workItemTrackingHttpClient, WorkItem workItem, Guid projectId, string workItemRelationTypeReferenceName, string targetWorkItemTypeName = "", bool? timePrecision = null, int? count = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (string.IsNullOrWhiteSpace(workItemRelationTypeReferenceName)) throw new ArgumentOutOfRangeException(nameof(workItemRelationTypeReferenceName));

            return Observable.FromAsync(
                    token => workItemTrackingHttpClient
                        .QueryByWiqlAsync(
                            new Wiql()
                            {
                                Query = $"SELECT [System.Id] FROM workitemLinks WHERE ([Source].[System.Id] = {workItem.Id ?? 0}) AND ([System.Links.LinkType] = '{workItemRelationTypeReferenceName}') {(!string.IsNullOrWhiteSpace(targetWorkItemTypeName) ? $"AND ([Target].[System.WorkItemType] = '{targetWorkItemTypeName}')" : string.Empty)} ORDER BY [System.Id] MODE (MustContain)"
                            },
                            projectId,
                            timePrecision,
                            count,
                            userState,
                            token))
                .SelectMany(workItemQueryResult => workItemTrackingHttpClient.GetWorkItems(workItemQueryResult.WorkItems));
        }

        /// <summary>
        /// Gets the work item deltas between revisions.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="workItemId">The work item identifier.</param>
        /// <param name="count">The amount of updates to retrieve at most.</param>
        /// <param name="skip">How many updates to skip.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemUpdate> GetWorkItemUpdates(this WorkItemTrackingHttpClient workItemTrackingHttpClient, int workItemId, int? count = null, int? skip = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.GetUpdatesAsync(workItemId, count, skip, userState, token))
                .SelectMany(workItemUpdates => workItemUpdates);
        }

        /// <summary>
        /// Gets a specific work item revision delta for the given <paramref name="updateNumber" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="workItemId">The work item identifier.</param>
        /// <param name="updateNumber">The update number to retrieve.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemUpdate> GetWorkItemUpdate(this WorkItemTrackingHttpClient workItemTrackingHttpClient, int workItemId, int updateNumber, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return Observable.FromAsync(token => workItemTrackingHttpClient.GetUpdateAsync(workItemId, updateNumber, userState, token));
        }

        /// <summary>
        /// Gets the work item deltas between revisions.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="workItemId">The work item identifier.</param>
        /// <param name="count">The amount of updates to retrieve at most.</param>
        /// <param name="skip">How many updates to skip.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <param name="expand">The <see cref="WorkItemExpand"/> to apply to the underlying client.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItem> GetWorkItemRevisions(this WorkItemTrackingHttpClient workItemTrackingHttpClient, int workItemId, int? count = null, int? skip = null, object userState = null, WorkItemExpand? expand = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return PagedListHelper.ObservableFromPagedListProducer(
                (continuationCount, continuationSkip, cancellationToken) =>
                    workItemTrackingHttpClient.GetRevisionsAsync(workItemId, continuationCount, continuationSkip, expand, userState, cancellationToken),
                count, skip);
        }

        /// <summary>
        /// Returns information for all work item fields for a specific <paramref name="projectId"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="projectId">The projectId identifier.</param>
        /// <param name="expand">Use ExtensionFields to include extension fields, otherwise exclude them. Unless the feature flag for this parameter is enabled, extension fields are always included.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemField> GetWorkItemFields(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid projectId, GetFieldsExpand? expand = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return workItemTrackingHttpClient.GetWorkItemFields(projectId.ToString(), expand, userState);
        }

        /// <summary>
        /// Returns information for all work item fields, optionally for a specific project only (via its <paramref name="projectNameOrId"/>).
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="projectNameOrId">The projectId name or identifier.</param>
        /// <param name="expand">Use ExtensionFields to include extension fields, otherwise exclude them. Unless the feature flag for this parameter is enabled, extension fields are always included.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient</exception>
        public static IObservable<WorkItemField> GetWorkItemFields(this WorkItemTrackingHttpClient workItemTrackingHttpClient, string projectNameOrId = "", GetFieldsExpand? expand = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));

            return (!string.IsNullOrWhiteSpace(projectNameOrId)
                    ? Observable.FromAsync(token => workItemTrackingHttpClient.GetFieldsAsync(projectNameOrId, expand, userState, token))
                    : Observable.FromAsync(token => workItemTrackingHttpClient.GetFieldsAsync(expand, userState, token)))
                .SelectMany(workItemUpdates => workItemUpdates);
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="wiql"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="wiql">The wiql / query string to run.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="top">The maximum amount of results to return.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// workItemTrackingHttpClient
        /// or
        /// wiql
        /// </exception>
        public static IObservable<WorkItemQueryResult> GetWiqlQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Wiql wiql, bool? timePrecision = null, int? top = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (wiql == null) throw new ArgumentNullException(nameof(wiql));
            return Observable.FromAsync(token => workItemTrackingHttpClient.QueryByWiqlAsync(wiql, timePrecision, top, userState, token));
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="wiql" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="wiql">The wiql / query string to run.</param>
        /// <param name="projectId">The projectId (identifier) to filter the results to.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="top">The maximum amount of results to return.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient
        /// or
        /// wiql</exception>
        public static IObservable<WorkItemQueryResult> GetWiqlQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Wiql wiql, Guid projectId, bool? timePrecision = null, int? top = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (wiql == null) throw new ArgumentNullException(nameof(wiql));

            return Observable.FromAsync(token => workItemTrackingHttpClient.QueryByWiqlAsync(wiql, projectId, timePrecision, top, userState, token));
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="wiql" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="wiql">The wiql / query string to run.</param>
        /// <param name="teamContext">The team context to filter the results for.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="top">The maximum amount of results to return.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient
        /// or
        /// wiql</exception>
        public static IObservable<WorkItemQueryResult> GetWiqlQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Wiql wiql, TeamContext teamContext, bool? timePrecision = null, int? top = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (wiql == null) throw new ArgumentNullException(nameof(wiql));

            return Observable.FromAsync(token => workItemTrackingHttpClient.QueryByWiqlAsync(wiql, teamContext, timePrecision, top, userState, token));
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="wiql"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="wiql">The wiql / query string to run.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="top">The maximum amount of results to return.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// workItemTrackingHttpClient
        /// or
        /// wiql
        /// </exception>
        public static IObservable<WorkItemQueryResult> GetWiqlQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, string wiql, bool? timePrecision = null, int? top = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (wiql == null) throw new ArgumentNullException(nameof(wiql));
            if (string.IsNullOrWhiteSpace(wiql)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(wiql));

            return workItemTrackingHttpClient.GetWiqlQueryResult(new Wiql { Query = wiql }, timePrecision, top, userState);
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="wiql" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="wiql">The wiql / query string to run.</param>
        /// <param name="projectId">The projectId (identifier) to filter the results to.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="top">The maximum amount of results to return.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient
        /// or
        /// wiql</exception>
        public static IObservable<WorkItemQueryResult> GetWiqlQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, string wiql, Guid projectId, bool? timePrecision = null, int? top = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (wiql == null) throw new ArgumentNullException(nameof(wiql));
            if (string.IsNullOrWhiteSpace(wiql)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(wiql));

            return workItemTrackingHttpClient.GetWiqlQueryResult(new Wiql { Query = wiql }, projectId, timePrecision, top, userState);
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="wiql" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="wiql">The wiql / query string to run.</param>
        /// <param name="teamContext">The team context to filter the results for.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="top">The maximum amount of results to return.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient
        /// or
        /// wiql</exception>
        public static IObservable<WorkItemQueryResult> GetWiqlQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, string wiql, TeamContext teamContext, bool? timePrecision = null, int? top = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (wiql == null) throw new ArgumentNullException(nameof(wiql));
            if (string.IsNullOrWhiteSpace(wiql)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(wiql));

            return workItemTrackingHttpClient.GetWiqlQueryResult(new Wiql { Query = wiql }, teamContext, timePrecision, top, userState);
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="queryId"/>.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="queryId">The stored query (identifier) to run.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// workItemTrackingHttpClient
        /// or
        /// wiql
        /// </exception>
        public static IObservable<WorkItemQueryResult> GetStoredQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid queryId, bool? timePrecision = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (queryId == null) throw new ArgumentNullException(nameof(queryId));

            return Observable.FromAsync(token => workItemTrackingHttpClient.QueryByIdAsync(queryId, timePrecision, userState, token));
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="queryId" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="queryId">The stored query (identifier) to run.</param>
        /// <param name="projectId">The projectId (identifier) to filter the results to.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient
        /// or
        /// wiql</exception>
        public static IObservable<WorkItemQueryResult> GetStoredQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid queryId, Guid projectId, bool? timePrecision = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (queryId == null) throw new ArgumentNullException(nameof(queryId));

            return Observable.FromAsync(token => workItemTrackingHttpClient.QueryByIdAsync(projectId, queryId, timePrecision, userState, token));
        }

        /// <summary>
        /// Gets the query result for the provided <paramref name="queryId" />.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="queryId">The stored query (identifier) to run.</param>
        /// <param name="teamContext">The team context to filter the results for.</param>
        /// <param name="timePrecision">[true] if time precision is allowed in the date time comparisons.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemTrackingHttpClient
        /// or
        /// wiql</exception>
        public static IObservable<WorkItemQueryResult> GetStoredQueryResult(this WorkItemTrackingHttpClient workItemTrackingHttpClient, Guid queryId, TeamContext teamContext, bool? timePrecision = null, object userState = null)
        {
            if (workItemTrackingHttpClient == null) throw new ArgumentNullException(nameof(workItemTrackingHttpClient));
            if (queryId == null) throw new ArgumentNullException(nameof(queryId));

            return Observable.FromAsync(token => workItemTrackingHttpClient.QueryByIdAsync(teamContext, queryId, timePrecision, userState, token));
        }
    }
}
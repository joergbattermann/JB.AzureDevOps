using System;
using System.Collections.Generic;
using System.Linq;
using JB.TeamFoundationServer.WorkItemTracking.WellKnownWorkItemFieldReferenceNames;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.TeamFoundationServer.WorkItemTracking
{
    /// <summary>
    /// Extension Methods for <see cref="WorkItem"/> instances.
    /// </summary>
    public static class WorkItemExtensions
    {
        /// <summary>
        /// Gets the related work item ids for the given <paramref name="workItem"/> and <paramref name="linkTypeReferenceName"/>.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="linkTypeReferenceName">The link type reference name (including direction '-forward' or '-reverse', if applicable).</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">linkTypeReferenceName</exception>
        public static IEnumerable<int> GetRelatedWorkItemIds(
            this WorkItem workItem,
            string linkTypeReferenceName)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem.GetRelatedWorkItemIdsAndRelationPositions(linkTypeReferenceName)
                .Select(relatedWorkItemIdsAndRelationPosition => relatedWorkItemIdsAndRelationPosition.RelatedWorkItemId)
                .Distinct();
        }

        /// <summary>
        /// Gets the related work item ids for the given <paramref name="workItems"/> and <paramref name="linkTypeReferenceName"/>.
        /// </summary>
        /// <param name="workItems">The work items.</param>
        /// <param name="linkTypeReferenceName">The link type reference name (including direction '-forward' or '-reverse', if applicable).</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">linkTypeReferenceName</exception>
        public static IEnumerable<(WorkItem WorkItem, int RelatedWorkItemId)> GetRelatedWorkItemIds(
            this IEnumerable<WorkItem> workItems,
            string linkTypeReferenceName)
        {
            return (workItems ?? Enumerable.Empty<WorkItem>())
                .GetRelatedWorkItemIdsAndRelationPositions(linkTypeReferenceName)
                .Select(relatedWorkItemIdsAndRelationPosition => (WorkItem: relatedWorkItemIdsAndRelationPosition.WorkItem, RelatedWorkItemId: relatedWorkItemIdsAndRelationPosition.RelatedWorkItemId));
        }

        /// <summary>
        /// Determines whether the provided <paramref name="workItem"/> is a work item of the given <paramref name="workItemTypeName"/>.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="workItemTypeName">Name of the work item type.</param>
        /// <returns>
        ///   <c>true</c> if [is work item of type] [the specified work item type name]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">workItemTypeName - workItem</exception>
        public static bool IsWorkItemOfType(this WorkItem workItem, string workItemTypeName)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (string.IsNullOrWhiteSpace(workItemTypeName)) throw new ArgumentException($"Parameter value '{nameof(workItemTypeName)}' may not be null, empty or whitespaces only.", nameof(workItem));

            return string.Equals(workItem.GetWorkItemType(), workItemTypeName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the provided <paramref name="workItem" /> has the given <paramref name="workItemState" /> and optionally the <paramref name="workItemReason"/>.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="workItemState">State of the work item.</param>
        /// <param name="workItemReason">The work item reason.</param>
        /// <returns>
        ///   <c>true</c> if [is work item of type] [the specified work item type name]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">workItemState - workItem</exception>
        public static bool HasWorkItemState(this WorkItem workItem, string workItemState, string workItemReason = "")
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (string.IsNullOrWhiteSpace(workItemState)) throw new ArgumentException($"Parameter value '{nameof(workItemState)}' may not be null, empty or whitespaces only.", nameof(workItem));

            return string.Equals(workItem.GetState(), workItemState, StringComparison.OrdinalIgnoreCase)
                && (string.IsNullOrWhiteSpace(workItemReason)
                    || string.Equals(workItem.GetReason(), workItemReason, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether the <paramref name="workItem"/> has a relation for the <paramref name="linkTypeReferenceName"/> from or to the given <paramref name="sourceOrTargetWorkItemId"/>.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="sourceOrTargetWorkItemId">The source or target work item identifier.</param>
        /// <param name="linkTypeReferenceName">The link type reference name (including direction '-forward' or '-reverse', if applicable).</param>
        /// <returns>
        ///   <c>true</c> if there is such a relation; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        public static bool HasRelationFromOrToWorkItemId(this WorkItem workItem, int sourceOrTargetWorkItemId,
            string linkTypeReferenceName)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem.GetRelatedWorkItemIds(linkTypeReferenceName)
                .Any(linkedWorkItemId => linkedWorkItemId == sourceOrTargetWorkItemId);
        }

        /// <summary>
        /// Determines whether the <paramref name="workItem"/> has a relation for the <paramref name="linkTypeReferenceName"/> from or to the given <paramref name="sourceOrTargetWorkItem"/>.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="sourceOrTargetWorkItem">The source or target work item.</param>
        /// <param name="linkTypeReferenceName">The link type reference name (including direction '-forward' or '-reverse', if applicable).</param>
        /// <returns>
        ///   <c>true</c> if there is such a relation; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        public static bool HasRelationFromOrToWorkItem(this WorkItem workItem,
            WorkItem sourceOrTargetWorkItem,
            string linkTypeReferenceName)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (sourceOrTargetWorkItem == null) throw new ArgumentNullException(nameof(sourceOrTargetWorkItem));

            return workItem.GetRelatedWorkItemIds(linkTypeReferenceName)
                .Any(linkedWorkItemId => linkedWorkItemId == sourceOrTargetWorkItem.Id);
        }

        /// <summary>
        /// Gets the related work item ids and their <see cref="WorkItem.Relations"/> positions which is required for potential link/relation modification.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="linkTypeReferenceName">The link type reference name (including direction '-forward' or '-reverse', if applicable).</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">linkTypeReferenceName</exception>
        public static IEnumerable<(int RelatedWorkItemId, int RelationIndexPosition)> GetRelatedWorkItemIdsAndRelationPositions(
            this WorkItem workItem,
            string linkTypeReferenceName)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            if (string.IsNullOrWhiteSpace(linkTypeReferenceName))
                throw new ArgumentException($"Value for '{nameof(linkTypeReferenceName)}' cannot be null or whitespace.", nameof(linkTypeReferenceName));

            if (workItem.Relations == null || workItem.Relations.Count == 0)
                return Enumerable.Empty<(int, int)>();

            return workItem
                .Relations
                .Where(relation => string.Equals(relation.Rel, linkTypeReferenceName,
                                       StringComparison.OrdinalIgnoreCase)
                                   && !string.IsNullOrWhiteSpace(relation.Url) &&
                                   Uri.IsWellFormedUriString(relation.Url, UriKind.Absolute))
                .Select(relation => new
                {
                    RelatedWorkItemId = TryParseWorkItemIdFromUri(new Uri(relation.Url, UriKind.Absolute), out var workItemId)
                        ? workItemId
                        : (int?) null,
                    RelationIndexPosition = workItem.Relations.IndexOf(relation)
                })
                .Where(potentialWorkItemdId => potentialWorkItemdId.RelatedWorkItemId.HasValue)
                .Select(potentialWorkItemdId => (RelatedWorkItemId: potentialWorkItemdId.RelatedWorkItemId.Value, RelationIndexPosition: potentialWorkItemdId.RelationIndexPosition));
        }

        /// <summary>
        /// Gets the related work item ids and their <see cref="WorkItem.Relations"/> positions correlated to the provided <paramref name="workItems"/>.
        /// </summary>
        /// <param name="workItems">The work items.</param>
        /// <param name="linkTypeReferenceName">The link type reference name (including direction '-forward' or '-reverse', if applicable).</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">linkTypeReferenceName</exception>
        public static IEnumerable<(WorkItem WorkItem, int RelatedWorkItemId, int RelationIndexPosition)> GetRelatedWorkItemIdsAndRelationPositions(
            this IEnumerable<WorkItem> workItems,
            string linkTypeReferenceName)
        {
            if (workItems == null)
            {
                workItems = Enumerable.Empty<WorkItem>();
            }

            foreach (var workItemAndRelatedWorkItems in workItems.Select(workItem => new { WorkItem = workItem, RelatedWorkItemIdsAndRelationPositions = workItem.GetRelatedWorkItemIdsAndRelationPositions(linkTypeReferenceName) }))
            {
                foreach (var workItemRelatedWorkItemIdsAndRelationPositions in workItemAndRelatedWorkItems.RelatedWorkItemIdsAndRelationPositions ?? Enumerable.Empty<(int RelationIndexPosition, int RelatedWorkItemId)>())
                {
                    yield return (WorkItem: workItemAndRelatedWorkItems.WorkItem,
                        RelatedWorkItemId: workItemRelatedWorkItemIdsAndRelationPositions.RelatedWorkItemId,
                        RelationIndexPosition: workItemRelatedWorkItemIdsAndRelationPositions.RelationIndexPosition);
                }
            }
        }

        /// <summary>
        /// Tries to parse the <paramref name="workItemUri"/> for the reference work item identifier.
        /// </summary>
        /// <param name="workItemUri">The work item URI.</param>
        /// <param name="workItemId">The work item identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItemUri</exception>
        private static bool TryParseWorkItemIdFromUri(Uri workItemUri, out int? workItemId)
        {
            if (workItemUri == null) throw new ArgumentNullException(nameof(workItemUri));

            if (string.IsNullOrWhiteSpace(workItemUri.LocalPath)
                || !(workItemUri
                    .LocalPath
                    .Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Where(segment => !string.IsNullOrWhiteSpace(segment))
                    .Select(segment => segment.Trim().ToLower())
                    .Where(segment => segment != "/")
                    .ToList() is List<string> uriPathSegments)
                || uriPathSegments.Count < 3)
            {
                workItemId = null;
                return false;
            }

            // else
            var lastPathSegmentValue = uriPathSegments.LastOrDefault();
            var indexOfLastPathSegmentValue = uriPathSegments.IndexOf(lastPathSegmentValue);
            if(indexOfLastPathSegmentValue < 2)
            {
                workItemId = null;
                return false;
            }

            if (!string.Equals(uriPathSegments[indexOfLastPathSegmentValue - 1], "workitems",
                StringComparison.OrdinalIgnoreCase))
            {
                workItemId = null;
                return false;
            }

            if (!string.Equals(uriPathSegments[indexOfLastPathSegmentValue - 2], "wit",
                StringComparison.OrdinalIgnoreCase))
            {
                workItemId = null;
                return false;
            }

            if (!int.TryParse(lastPathSegmentValue, out var potentialWorkItemId))
            {
                workItemId = null;
                return false;
            }

            // else
            workItemId = potentialWorkItemId;
            return true;
        }

        /// <summary>
        /// Tries to get a specific link <see cref="Uri"/> of the <see cref="WorkItem.Links"/> collection
        /// for the given <paramref name="linkName"/>.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="linkName">Name of the link.</param>
        /// <param name="linkUri">The link URI.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace. - linkName</exception>
        public static bool TryGetLinkValue(this WorkItem workItem, string linkName, out Uri linkUri)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (string.IsNullOrWhiteSpace(linkName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkName));

            if (workItem.Links?.Links == null
                || workItem.Links.Links.TryGetValue(linkName, out var dictionaryValue) == false
                || !(dictionaryValue is ReferenceLink referenceLink)
                || string.IsNullOrWhiteSpace(referenceLink.Href)
                || Uri.TryCreate(referenceLink.Href, UriKind.Absolute, out var referenceLinkUri) == false)
            {
                linkUri = null;
                return false;
            }

            // else
            linkUri = referenceLinkUri;
            return true;
        }

        /// <summary>
        /// Tries to get the field value for the provided <paramref name="workItem"/> and <paramref name="fieldReferenceName"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="workItem">The work item.</param>
        /// <param name="fieldReferenceName">Name of the field reference.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static bool TryGetFieldValue<TValue>(this WorkItem workItem, string fieldReferenceName, TValue defaultValue, out TValue value)
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

        /// <summary>
        /// Tries to get the field value for the provided <paramref name="workItem" /> and <paramref name="fieldReferenceName" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="workItem">The work item.</param>
        /// <param name="fieldReferenceName">Name of the field reference.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">
        /// Value cannot be null or whitespace. - fieldReferenceName
        /// or
        /// workItem
        /// </exception>
        public static bool TryGetFieldValue<TValue>(this WorkItem workItem, string fieldReferenceName, out TValue value)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fieldReferenceName));

            if (workItem.Fields == null)
                throw new ArgumentException($"The {nameof(workItem)} must have been retrieved including its {nameof(workItem.Fields)} prior to retrieving a value.");

            if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValue) && workItemValue is TValue workItemValueAsRequestedType)
            {
                value = workItemValueAsRequestedType;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Gets the area path for the provided <paramref name="workItem"/>
        /// assuming the later was originally fetched including the <see cref="System.AreaPath"/> field.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <returns></returns>
        public static string GetAreaPath(this WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            
            return workItem
                .TryGetFieldValue<string>(
                    WellKnownWorkItemFieldReferenceNames.System.AreaPath,
                    string.Empty,
                    out var areaPath)
                ? areaPath
                : string.Empty;
        }

        /// <summary>
        /// Gets the iteration path for the provided <paramref name="workItem"/>
        /// assuming the later was originally fetched including the <see cref="System.IterationPath"/> field.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <returns></returns>
        public static string GetIterationPath(this WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem
                .TryGetFieldValue<string>(
                    WellKnownWorkItemFieldReferenceNames.System.IterationPath,
                    string.Empty,
                    out var iterationPath)
                ? iterationPath
                : string.Empty;
        }

        /// <summary>
        /// Gets the state for the provided <paramref name="workItem"/>
        /// assuming the later was originally fetched including the <see cref="System.State"/> field.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <returns></returns>
        public static string GetState(this WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem
                .TryGetFieldValue<string>(
                    WellKnownWorkItemFieldReferenceNames.System.State,
                    string.Empty,
                    out var state)
                ? state
                : string.Empty;
        }

        /// <summary>
        /// Gets the reason for the provided <paramref name="workItem"/>
        /// assuming the later was originally fetched including the <see cref="System.Reason"/> field.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <returns></returns>
        public static string GetReason(this WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem
                .TryGetFieldValue<string>(
                    WellKnownWorkItemFieldReferenceNames.System.Reason,
                    string.Empty,
                    out var reason)
                ? reason
                : string.Empty;
        }

        /// <summary>
        /// Gets the assigned to value for the provided <paramref name="workItem"/>
        /// assuming the later was originally fetched including the <see cref="System.AssignedTo"/> field.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <returns></returns>
        public static string GetAssignedTo(this WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem
                .TryGetFieldValue<string>(
                    WellKnownWorkItemFieldReferenceNames.System.AssignedTo,
                    string.Empty,
                    out var assignedTo)
                ? assignedTo
                : string.Empty;
        }

        /// <summary>
        /// Gets the work item type for the provided <paramref name="workItem"/>
        /// assuming the later was originally fetched including the <see cref="System.WorkItemType"/> field.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <returns></returns>
        public static string GetWorkItemType(this WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem
                .TryGetFieldValue<string>(
                    WellKnownWorkItemFieldReferenceNames.System.WorkItemType,
                    string.Empty,
                    out var workItemType)
                ? workItemType
                : string.Empty;
        }

        /// <summary>
        /// Gets the team project for the provided <paramref name="workItem"/>
        /// assuming the later was originally fetched including the <see cref="System.TeamProject"/> field.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <returns></returns>
        public static string GetTeamProject(this WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem
                .TryGetFieldValue<string>(
                    WellKnownWorkItemFieldReferenceNames.System.TeamProject,
                    string.Empty,
                    out var teamProject)
                ? teamProject
                : string.Empty;
        }

        /// <summary>
        /// Gets the title for the provided <paramref name="workItem"/>
        /// assuming the later was originally fetched including the <see cref="System.Title"/> field.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <returns></returns>
        public static string GetTitle(this WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem
                .TryGetFieldValue<string>(
                    WellKnownWorkItemFieldReferenceNames.System.Title,
                    string.Empty,
                    out var title)
                ? title
                : string.Empty;
        }

        /// <summary>
        /// Tries to get the field value for the provided <paramref name="workItem" /> and <paramref name="fieldReferenceName" /> using a <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="workItem">The work item.</param>
        /// <param name="fieldReferenceName">Name of the field reference.</param>
        /// <param name="converter">The converter to take the <see cref="WorkItem"/> value and convert it to a <typeparamref name="TValue"/> instance.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace. - fieldReferenceName
        /// or
        /// workItem</exception>
        public static bool TryGetFieldValue<TValue>(this WorkItem workItem, string fieldReferenceName, Func<object, TValue> converter, out TValue value)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fieldReferenceName));

            if (workItem.Fields == null)
                throw new ArgumentException($"The {nameof(workItem)} must have been retrieved including its {nameof(workItem.Fields)} prior to retrieving a value.");

            if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValueAsTValue) && workItemValueAsTValue is TValue workItemValueAsRequestedType)
            {
                value = workItemValueAsRequestedType;
                return true;
            }
            else if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValue))
            {
                value = converter(workItemValue);
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Tries to get the field value for the provided <paramref name="workItem" /> and <paramref name="fieldReferenceName" /> using a <paramref name="converter" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="workItem">The work item.</param>
        /// <param name="fieldReferenceName">Name of the field reference.</param>
        /// <param name="converter">The converter to take the <see cref="WorkItem" /> value and convert it to a <typeparamref name="TValue" /> instance.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace. - fieldReferenceName
        /// or
        /// workItem</exception>
        public static bool TryGetFieldValue<TValue>(this WorkItem workItem, string fieldReferenceName, Func<object, TValue> converter, TValue defaultValue, out TValue value)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fieldReferenceName));

            if (workItem.Fields == null)
                throw new ArgumentException($"The {nameof(workItem)} must have been retrieved including its {nameof(workItem.Fields)} prior to retrieving a value.");

            if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValueAsTValue) && workItemValueAsTValue is TValue workItemValueAsRequestedType)
            {
                value = workItemValueAsRequestedType;
                return true;
            }
            else if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValue))
            {
                value = converter(workItemValue);
                return true;
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }

        /// <summary>
        /// Tries to get the field value for the provided <paramref name="workItem" /> and <paramref name="fieldReferenceName" /> using a <paramref name="converter" />.
        /// </summary>
        /// <typeparam name="TWorkItemValue">The type of the work item value.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="workItem">The work item.</param>
        /// <param name="fieldReferenceName">Name of the field reference.</param>
        /// <param name="converter">The converter to take the <see cref="WorkItem" /> value and convert it to a <typeparamref name="TResult" /> instance.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace. - fieldReferenceName
        /// or
        /// workItem</exception>
        public static bool TryGetFieldValue<TWorkItemValue, TResult>(this WorkItem workItem, string fieldReferenceName, Func<TWorkItemValue, TResult> converter, out TResult value)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fieldReferenceName));

            if (workItem.Fields == null)
                throw new ArgumentException($"The {nameof(workItem)} must have been retrieved including its {nameof(workItem.Fields)} prior to retrieving a value.");

            if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValueAsTValue) && workItemValueAsTValue is TResult workItemValueAsRequestedType)
            {
                value = workItemValueAsRequestedType;
                return true;
            }
            else if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValue) && workItemValue is TWorkItemValue workItemValueAsWorkItemValue)
            {
                value = converter(workItemValueAsWorkItemValue);
                return true;
            }
            else
            {
                value = default(TResult);
                return false;
            }
        }

        /// <summary>
        /// Tries to get the field value for the provided <paramref name="workItem" /> and <paramref name="fieldReferenceName" /> using a <paramref name="converter" />.
        /// </summary>
        /// <typeparam name="TWorkItemValue">The type of the work item value.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="workItem">The work item.</param>
        /// <param name="fieldReferenceName">Name of the field reference.</param>
        /// <param name="converter">The converter to take the <see cref="WorkItem" /> value and convert it to a <typeparamref name="TResult" /> instance.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace. - fieldReferenceName
        /// or
        /// workItem</exception>
        public static bool TryGetFieldValue<TWorkItemValue, TResult>(this WorkItem workItem, string fieldReferenceName, Func<TWorkItemValue, TResult> converter, TResult defaultValue, out TResult value)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            if (string.IsNullOrWhiteSpace(fieldReferenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fieldReferenceName));

            if (workItem.Fields == null)
                throw new ArgumentException($"The {nameof(workItem)} must have been retrieved including its {nameof(workItem.Fields)} prior to retrieving a value.");

            if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValueAsTValue) && workItemValueAsTValue is TResult workItemValueAsRequestedType)
            {
                value = workItemValueAsRequestedType;
                return true;
            }
            else if (workItem.Fields.TryGetValue(fieldReferenceName, out var workItemValue) && workItemValue is TWorkItemValue workItemValueAsWorkItemValue)
            {
                value = converter(workItemValueAsWorkItemValue);
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
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Gets the related work item ids for the given <paramref name="workItem"/> and <paramref name="linkTypeReferenceNameIncludingDirection"/>.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="linkTypeReferenceNameIncludingDirection">The link type reference name including direction ('-forward' or '-reverse').</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">
        /// linkTypeReferenceNameIncludingDirection - linkTypeReferenceNameIncludingDirection
        /// or
        /// linkTypeReferenceNameIncludingDirection - linkTypeReferenceNameIncludingDirection
        /// </exception>
        public static IEnumerable<int> GetRelatedWorkItemIds(
            this WorkItem workItem,
            string linkTypeReferenceNameIncludingDirection)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            
                return workItem.GetRelatedWorkItemIdsAndRelationPositions(linkTypeReferenceNameIncludingDirection)
                    .Select(relatedWorkItemIdsAndRelationPosition => relatedWorkItemIdsAndRelationPosition.RelatedWorkItemId)
                    .Distinct();
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

            return workItem.TryGetFieldValue(WellKnownWorkItemFieldReferenceNames.System.WorkItemType,
                       out string actualWorkItemTypeName)
                   && string.Equals(actualWorkItemTypeName, workItemTypeName, StringComparison.OrdinalIgnoreCase);
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

            return workItem.TryGetFieldValue(WellKnownWorkItemFieldReferenceNames.System.State, out string actualWorkItemState)
                   && string.Equals(actualWorkItemState, workItemState, StringComparison.OrdinalIgnoreCase)
                && (string.IsNullOrWhiteSpace(workItemReason)
                    || workItem.TryGetFieldValue(WellKnownWorkItemFieldReferenceNames.System.Reason, out string actualWorkItemReason)
                    && string.Equals(actualWorkItemReason, workItemReason, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether the <paramref name="workItem"/> has a relation for the <paramref name="linkTypeReferenceNameIncludingDirection"/> from or to the given <paramref name="sourceOrTargetWorkItemId"/>.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="sourceOrTargetWorkItemId">The source or target work item identifier.</param>
        /// <param name="linkTypeReferenceNameIncludingDirection">The link type reference name including direction ('-forward' or '-reverse').</param>
        /// <returns>
        ///   <c>true</c> if there is such a relation; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        public static bool HasRelationFromOrToWorkItem(this WorkItem workItem, int sourceOrTargetWorkItemId,
            string linkTypeReferenceNameIncludingDirection)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            return workItem.GetRelatedWorkItemIds(linkTypeReferenceNameIncludingDirection)
                .Any(linkedWorkItemId => linkedWorkItemId == sourceOrTargetWorkItemId);
        }

        /// <summary>
        /// Gets the related work item ids and their <see cref="WorkItem.Relations"/> positions which is required for potential link/relation modification.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="linkTypeReferenceNameIncludingDirection">The link type reference name including direction ('-forward' or '-reverse').</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workItem</exception>
        /// <exception cref="ArgumentException">
        /// linkTypeReferenceNameIncludingDirection - linkTypeReferenceNameIncludingDirection
        /// or
        /// linkTypeReferenceNameIncludingDirection - linkTypeReferenceNameIncludingDirection
        /// </exception>
        public static IEnumerable<(int RelationIndexPosition, int RelatedWorkItemId)> GetRelatedWorkItemIdsAndRelationPositions(
            this WorkItem workItem,
            string linkTypeReferenceNameIncludingDirection)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            if (string.IsNullOrWhiteSpace(linkTypeReferenceNameIncludingDirection))
                throw new ArgumentException($"Value for '{nameof(linkTypeReferenceNameIncludingDirection)}' cannot be null or whitespace.", nameof(linkTypeReferenceNameIncludingDirection));

            if (linkTypeReferenceNameIncludingDirection.IndexOf(Constants.WorkItems.RelationsForwardSuffix,
                    StringComparison.OrdinalIgnoreCase) < 0
                && linkTypeReferenceNameIncludingDirection.IndexOf(Constants.WorkItems.RelationsReverseSuffix,
                    StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new ArgumentException($"Value for '{nameof(linkTypeReferenceNameIncludingDirection)}' must contain either a '{Constants.WorkItems.RelationsForwardSuffix}' or '{Constants.WorkItems.RelationsReverseSuffix}' suffix.", nameof(linkTypeReferenceNameIncludingDirection));
            }

            if (workItem.Relations == null || workItem.Relations.Count == 0)
                return Enumerable.Empty<(int, int)>();

            return workItem
                .Relations
                .Where(relation => string.Equals(relation.Rel, linkTypeReferenceNameIncludingDirection,
                                       StringComparison.OrdinalIgnoreCase)
                                   && !string.IsNullOrWhiteSpace(relation.Url) &&
                                   Uri.IsWellFormedUriString(relation.Url, UriKind.Absolute))
                .Select(relation => new
                {
                    RelationIndexPosition = workItem.Relations.IndexOf(relation),
                    RelatedWorkItemId = TryParseWorkItemIdFromUri(new Uri(relation.Url, UriKind.Absolute), out var workItemId)
                        ? workItemId
                        : (int?) null
                })
                .Where(potentialWorkItemdId => potentialWorkItemdId.RelatedWorkItemId.HasValue)
                .Select(potentialWorkItemdId => (potentialWorkItemdId.RelationIndexPosition, potentialWorkItemdId.RelatedWorkItemId.Value));
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
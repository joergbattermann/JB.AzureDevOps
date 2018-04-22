using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace JB.TeamFoundationServer.WorkItemTracking
{
    /// <summary>
    /// Extension Methods for <see cref="WorkItem"/> instances.
    /// </summary>
    public static class WorkItemExtensions
    {
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
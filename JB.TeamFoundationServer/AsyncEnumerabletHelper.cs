using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.TeamFoundationServer
{
    /// <summary>
    /// Helper class that works with paged lists
    /// </summary>
    public static class AsyncEnumerabletHelper
    {
        /// <summary>
        /// Gets all items of the provided <paramref name="pagedListProducer"/> as an observable stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedListProducer">The <see cref="IPagedList{T}"/> producer action.
        /// The very first, initial call will pass in a [null] value as continuation token, subsequent calls will provide the pages' <see cref="IPagedList{T}.ContinuationToken"/>.</param>
        /// <param name="count">The amount of items to retrieve at most.</param>
        /// <param name="initialSkip">How many items to skip initially.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// pagedList
        /// or
        /// continuationProducer
        /// </exception>
        public static async IAsyncEnumerable<T> AsyncEnumerableFromPagedListProducer<T>(
            Func<(int? Count, int? Skip, string ContinuationToken, CancellationToken CancellationToken), Task<IPagedList<T>>> pagedListProducer,
            int? count = null,
            int? initialSkip = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pagedListProducer == null) throw new ArgumentNullException(nameof(pagedListProducer));

            await foreach (var item in new AsyncEnumerableResult<T>(pagedListProducer, count, initialSkip).ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return item;
            }
        }

        /// <summary>
        /// Gets all items of the provided <paramref name="listProducer" /> as an observable stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listProducer">The <see cref="IPagedList{T}" /> producer action.
        /// The very first, initial call will pass in a [null] value as continuation token, subsequent calls will provide the pages' <see cref="IPagedList{T}.ContinuationToken" />.</param>
        /// <param name="count">The amount of items to retrieve at most.</param>
        /// <param name="initialSkip">How many items to skip initially.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">pagedList
        /// or
        /// continuationProducer</exception>
        public static async IAsyncEnumerable<T> AsyncEnumerableFromEnumerableProducer<T>(
            Func<(int? Count, int? Skip, CancellationToken CancellationToken), Task<List<T>>> listProducer,
            int? count = null,
            int? initialSkip = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (listProducer == null) throw new ArgumentNullException(nameof(listProducer));

            await foreach (var item in new AsyncEnumerableResult<T>(listProducer, count, initialSkip).ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return item;
            }
        }

        /// <summary>
        /// Gets all items of the provided <paramref name="enumerableProducer" /> as an observable stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerableProducer">The <see cref="IPagedList{T}" /> producer action.
        /// The very first, initial call will pass in a [null] value as continuation token, subsequent calls will provide the pages' <see cref="IPagedList{T}.ContinuationToken" />.</param>
        /// <param name="count">The amount of items to retrieve at most.</param>
        /// <param name="initialSkip">How many items to skip initially.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">pagedList
        /// or
        /// continuationProducer</exception>
        public static async IAsyncEnumerable<T> AsyncEnumerableFromEnumerableProducer<T>(
            Func<(int? Count, int? Skip, CancellationToken CancellationToken), Task<IEnumerable<T>>> enumerableProducer,
            int? count = null,
            int? initialSkip = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (enumerableProducer == null) throw new ArgumentNullException(nameof(enumerableProducer));

            await foreach (var item in new AsyncEnumerableResult<T>(enumerableProducer, count, initialSkip).ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return item;
            }
        }
    }
}
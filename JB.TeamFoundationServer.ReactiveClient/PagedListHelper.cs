using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.TeamFoundationServer
{
    /// <summary>
    /// Helper class that works with paged lists
    /// </summary>
    public static partial class PagedListHelper
    {
        /// <summary>
        /// Gets all items of the provided <paramref name="pagedListProducer"/> as an observable stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedListProducer">The <see cref="IPagedList{T}"/> producer action.
        /// The very first, initial call will pass in a [null] value as continuation token, subsequent calls will provide the pages' <see cref="IPagedList{T}.ContinuationToken"/>.</param>
        /// <param name="count">The amount of items to retrieve at most.</param>
        /// <param name="initialSkip">How many items to skip initially.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// pagedList
        /// or
        /// continuationProducer
        /// </exception>
        public static IObservable<T> ObservableFromPagedListProducer<T>(Func<int?, int?, string, CancellationToken, Task<IPagedList<T>>> pagedListProducer, int? count = null, int? initialSkip = null)
        {
            if (pagedListProducer == null) throw new ArgumentNullException(nameof(pagedListProducer));

            return Observable.Create<T>(async (observer, cancellationToken) =>
            {
                try
                {
                    var receivedItems = 0;
                    var currentSkip = initialSkip ?? 0;

                    var currentPagedList = await pagedListProducer(count, currentSkip, null, cancellationToken).ConfigureAwait(false);

                    while (currentPagedList?.Count > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var exceededCount = false;

                        foreach (var element in currentPagedList)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            observer.OnNext(element);

                            receivedItems++;

                            // check in each iteration whether we've exceeded the (optional) count the caller may have provided
                            if (count != null && receivedItems > count)
                            {
                                exceededCount = true;
                                break;
                            }
                        }

                        // once we've exhausted the current items, check whether we identified inside the loop whether we're done
                        if (exceededCount)
                            break;

                        // otherwise continue
                        currentSkip += currentPagedList.Count;

                        currentPagedList = !string.IsNullOrWhiteSpace(currentPagedList.ContinuationToken)
                            ? await pagedListProducer(count, currentSkip, currentPagedList.ContinuationToken, cancellationToken).ConfigureAwait(false)
                            : new PagedList<T>(new List<T>(), string.Empty);
                    }

                    observer.OnCompleted();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    observer?.OnError(exception);
                }
            });
        }

        /// <summary>
        /// Gets all items of the provided <paramref name="pagedListProducer" /> as an observable stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedListProducer">The <see cref="IPagedList{T}" /> producer action.
        /// The very first, initial call will pass in a [null] value as continuation token, subsequent calls will provide the pages' <see cref="IPagedList{T}.ContinuationToken" />.</param>
        /// <param name="count">The amount of items to retrieve at most.</param>
        /// <param name="initialSkip">How many items to skip initially.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">pagedList
        /// or
        /// continuationProducer</exception>
        public static IObservable<T> ObservableFromPagedListProducer<T>(Func<int?, int?, CancellationToken, Task<List<T>>> pagedListProducer, int? count = null, int? initialSkip = null)
        {
            if (pagedListProducer == null) throw new ArgumentNullException(nameof(pagedListProducer));

            return Observable.Create<T>(async (observer, cancellationToken) =>
            {
                try
                {
                    var receivedItems = 0;
                    var currentSkip = initialSkip ?? 0;

                    var currentPagedList = await pagedListProducer(count, currentSkip, cancellationToken).ConfigureAwait(false);

                    while (currentPagedList?.Count > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var exceededCount = false;

                        foreach (var element in currentPagedList)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            observer.OnNext(element);

                            receivedItems++;

                            // check in each iteration whether we've exceeded the (optional) count the caller may have provided
                            if (count != null && receivedItems > count)
                            {
                                exceededCount = true;
                                break;
                            }
                        }

                        // once we've exhausted the current items, check whether we identified inside the loop whether we're done
                        if (exceededCount)
                            break;

                        // otherwise continue
                        currentSkip += currentPagedList.Count;

                        currentPagedList = await pagedListProducer(count, currentSkip, cancellationToken).ConfigureAwait(false);
                    }

                    observer.OnCompleted();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    observer?.OnError(exception);
                }
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.TeamFoundationServer
{
    public static class PagedListExtensions
    {
        /// <summary>
        /// Gets all items of the provided <paramref name="pagedList"/> and all following values utilizing the <paramref name="continuationProducer"/> as an observable stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedList">The initial paged list instance.</param>
        /// <param name="continuationProducer">The continuation producer.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// pagedList
        /// or
        /// continuationProducer
        /// </exception>
        public static IObservable<T> ToObservable<T>(this IPagedList<T> pagedList,
            Func<CancellationToken, string, Task<IPagedList<T>>> continuationProducer)
        {
            if (pagedList == null) throw new ArgumentNullException(nameof(pagedList));
            if (continuationProducer == null) throw new ArgumentNullException(nameof(continuationProducer));

            return Observable.Create<T>(async (observer, cancellationToken) =>
            {
                try
                {
                    var currentPagedList = pagedList;
                    while (currentPagedList?.Count > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        foreach (var element in currentPagedList)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            
                            observer.OnNext(element);
                        }

                        currentPagedList = !string.IsNullOrWhiteSpace(currentPagedList.ContinuationToken)
                            ? await continuationProducer(cancellationToken, currentPagedList.ContinuationToken)
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
    }
}
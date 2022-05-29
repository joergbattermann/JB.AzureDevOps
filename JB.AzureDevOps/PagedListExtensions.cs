using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.AzureDevOps
{
    public static class PagedListExtensions
    {
        /// <summary>
        /// Gets all items of the provided <paramref name="pagedList"/> by return its values and all following values utilizing the <paramref name="continuationProducer"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedList">The initial paged list instance.</param>
        /// <param name="continuationProducer">The continuation producer.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// pagedList
        /// or
        /// continuationProducer
        /// </exception>
        public static async Task<IReadOnlyList<T>> GetAllItems<T>(
            this IPagedList<T> pagedList,
            Func<CancellationToken, string, Task<IPagedList<T>>> continuationProducer,
            CancellationToken cancellationToken = default)
        {
            if (pagedList == null) throw new ArgumentNullException(nameof(pagedList));
            if (continuationProducer == null) throw new ArgumentNullException(nameof(continuationProducer));
            
            var result = new List<T>();
            var currentPagedList = pagedList;
            while (currentPagedList?.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                result.AddRange(currentPagedList);

                currentPagedList = !string.IsNullOrWhiteSpace(currentPagedList.ContinuationToken)
                    ? await continuationProducer(cancellationToken, currentPagedList.ContinuationToken)
                    : new PagedList<T>(new List<T>(), string.Empty);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }
    }
}
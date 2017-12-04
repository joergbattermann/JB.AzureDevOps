using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.TeamFoundationServer
{
    public static class PagedListExtensions
    {
        public static async Task<IReadOnlyList<T>> GetAllItems<T>(this IPagedList<T> pagedList,
            Func<CancellationToken, string, Task<IPagedList<T>>> continuationProducer,
            CancellationToken cancellationToken = default(CancellationToken))
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
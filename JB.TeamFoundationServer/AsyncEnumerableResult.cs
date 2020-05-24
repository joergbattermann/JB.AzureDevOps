using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.TeamFoundationServer
{
    internal class AsyncEnumerableResult<T> : IAsyncEnumerable<T>
    {
        public Func<(int? Count, int? Skip, string ContinuationToken, CancellationToken CancellationToken), Task<IPagedList<T>>> PagedListProducer { get; }
        public Func<(int? Count, int? Skip, CancellationToken CancellationToken), Task<List<T>>> ListProducer { get; }
        public Func<(int? Count, int? Skip, CancellationToken CancellationToken), Task<IEnumerable<T>>> EnumerableProducer { get; }

        public int? Count { get; }
        public int? InitialSkip { get; }

        public AsyncEnumerableResult(
            Func<(int? Count, int? Skip, string ContinuationToken, CancellationToken CancellationToken), Task<IPagedList<T>>> pagedListProducer,
            int? count = null,
            int? initialSkip = null)
        {
            PagedListProducer = pagedListProducer ?? throw new ArgumentNullException(nameof(pagedListProducer));

            Count = count;
            InitialSkip = initialSkip;
        }

        public AsyncEnumerableResult(
            Func<(int? Count, int? Skip, CancellationToken CancellationToken), Task<List<T>>> listProducer,
            int? count = null,
            int? initialSkip = null)
        {
            ListProducer = listProducer ?? throw new ArgumentNullException(nameof(listProducer));

            Count = count;
            InitialSkip = initialSkip;
        }

        public AsyncEnumerableResult(
            Func<(int? Count, int? Skip, CancellationToken CancellationToken), Task<IEnumerable<T>>> enumerableProducer,
            int? count = null,
            int? initialSkip = null)
        {
            EnumerableProducer = enumerableProducer ?? throw new ArgumentNullException(nameof(enumerableProducer));

            Count = count;
            InitialSkip = initialSkip;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (PagedListProducer != null)
            {
                return new AsyncResultPagedListEnumerator<T>(PagedListProducer, Count, InitialSkip, cancellationToken);
            }
            else if (ListProducer != null)
            {
                return new AsyncResultListEnumerator<T>(ListProducer, Count, InitialSkip, cancellationToken);
            }
            else if (EnumerableProducer != null)
            {
                return new AsyncResultEnumerableEnumerator<T>(EnumerableProducer, Count, InitialSkip, cancellationToken);
            }

            throw new NotImplementedException("The list type is not implemented, yet");
        }
    }
}
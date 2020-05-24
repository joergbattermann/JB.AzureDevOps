using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JB.TeamFoundationServer
{
    internal class AsyncResultEnumerableEnumerator<T> : IAsyncEnumerator<T>
    {
        public int? Count { get; }
        public int ReceivedItems { get; private set; }
        public int ReturnedItems { get; private set; }

        public int? InitialSkip { get; }
        public int NextSkip { get; private set; }

        public CancellationToken CancellationToken { get; }

        public T Current { get; private set; }

        public List<T> CurrentList { get; private set; }
        public int CurrentListIndex { get; private set; }

        public Func<(int? Count, int? Skip, CancellationToken CancellationToken), Task<IEnumerable<T>>> EnumerableProducer { get; }

        public AsyncResultEnumerableEnumerator(
            Func<(int? Count, int? Skip, CancellationToken CancellationToken), Task<IEnumerable<T>>> enumerableProducer,
            int? count = null,
            int? initialSkip = null,
            CancellationToken cancellationToken = default)
        {
            EnumerableProducer = enumerableProducer ?? throw new ArgumentNullException(nameof(enumerableProducer));

            Count = count;
            ReceivedItems = 0;
            ReturnedItems = 0;

            InitialSkip = initialSkip;
            NextSkip = initialSkip ?? 0;

            CancellationToken = cancellationToken;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (Count != null && ReceivedItems >= Count)
            {
                await DisposeAsync().ConfigureAwait(false);
                return false;
            }

            if (CurrentList == null || CurrentListIndex == (CurrentList.Count - 1))
            {
                // (re)fetch
                // ToDo: this is lazy.. there must be a non .ToList() way.. well there is for sure.. but not now.
                CurrentList = (await EnumerableProducer((Count, NextSkip, CancellationToken)).ConfigureAwait(false)).ToList();
                CurrentListIndex = -1;

                ReceivedItems += CurrentList.Count;
                NextSkip += CurrentList.Count;
            }

            // fetch next item in Current List
            if (CurrentList == null
                || CurrentList.Count == 0
                || CurrentListIndex == (CurrentList.Count - 1))
            {
                await DisposeAsync().ConfigureAwait(false);
                return false;
            }

            // else
            var currentListIndex = CurrentListIndex + 1;
            Current = CurrentList[currentListIndex];
            CurrentListIndex = currentListIndex;
            ReturnedItems += 1;

            CancellationToken.ThrowIfCancellationRequested();
            return true;
        }

        public async ValueTask DisposeAsync()
            => await Task.CompletedTask;
    }
}
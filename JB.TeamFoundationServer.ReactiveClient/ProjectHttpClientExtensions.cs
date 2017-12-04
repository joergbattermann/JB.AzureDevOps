using System;
using System.Reactive.Linq;
using Microsoft.TeamFoundation.Core.WebApi;

namespace JB.TeamFoundationServer
{
    /// <summary>
    /// Extension methods for <see cref="ProjectHttpClient"/> instances
    /// </summary>
    public static class ProjectHttpClientExtensions
    {
        /// <summary>
        /// Gets the team project references for the given <paramref name="projectCollectionHttpClient" />.
        /// </summary>
        /// <param name="projectHttpClient">The project HTTP client.</param>
        /// <param name="count">The amount of project collection references to retrieve at most.</param>
        /// <param name="skip">How many project collection references to skip.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">projectCollectionHttpClient</exception>
        public static IObservable<TeamProjectReference> GetTeamProjectReferences(
            this ProjectHttpClient projectHttpClient, int? count = null, int? skip = null, object userState = null)
        {
            if (projectHttpClient == null)
                throw new ArgumentNullException(nameof(projectHttpClient));

            return Observable.FromAsync(token => projectCollectionHttpClient.GetProjectCollections(count, skip, userState))
                .SelectMany(projectCollectionReferences => projectCollectionReferences);
        }
    }
}
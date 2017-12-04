using System;
using System.Reactive.Linq;
using Microsoft.TeamFoundation.Core.WebApi;

namespace JB.TeamFoundationServer
{
    /// <summary>
    /// Extension methods for <see cref="ProjectCollectionHttpClient"/> instances
    /// </summary>
    public static class ProjectCollectionHttpClientExtensions
    {
        /// <summary>
        /// Gets the team project collection references for the given <paramref name="projectCollectionHttpClient" />.
        /// </summary>
        /// <param name="projectCollectionHttpClient">The project collection HTTP client.</param>
        /// <param name="count">The amount of project collection references to retrieve at most.</param>
        /// <param name="skip">How many project collection references to skip.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">projectCollectionHttpClient</exception>
        public static IObservable<TeamProjectCollectionReference> GetTeamProjectCollectionReferences(
            this ProjectCollectionHttpClient projectCollectionHttpClient, int? count = null, int? skip = null, object userState = null)
        {
            if (projectCollectionHttpClient == null)
                throw new ArgumentNullException(nameof(projectCollectionHttpClient));

            return Observable.FromAsync(token => projectCollectionHttpClient.GetProjectCollections(count, skip, userState))
                .SelectMany(projectCollectionReferences => projectCollectionReferences);
        }

        /// <summary>
        /// Gets the team project collections for the given <paramref name="projectCollectionHttpClient" />.
        /// </summary>
        /// <param name="projectCollectionHttpClient">The project collection HTTP client.</param>
        /// <param name="count">The amount of project collections to retrieve at most.</param>
        /// <param name="skip">How many project collections to skip.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">projectCollectionHttpClient</exception>
        public static IObservable<TeamProjectCollection> GetTeamProjectCollections(
            this ProjectCollectionHttpClient projectCollectionHttpClient, int? count = null, int? skip = null, object userState = null)
        {
            if (projectCollectionHttpClient == null)
                throw new ArgumentNullException(nameof(projectCollectionHttpClient));

            return projectCollectionHttpClient.GetTeamProjectCollectionReferences(count, skip, userState)
                .SelectMany(teamProjectCollectionReference => projectCollectionHttpClient.GetProjectCollection(teamProjectCollectionReference.Id.ToString(), userState));
        }
    }
}
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
        /// Gets the team project references for the given <paramref name="projectHttpClient" />.
        /// </summary>
        /// <param name="projectHttpClient">The project HTTP client.</param>
        /// <param name="stateFilter">Filter on team projects in a specific team project state.</param>
        /// <param name="count">The amount of project references to retrieve at most.</param>
        /// <param name="skip">How many project references to skip.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">projectCollectionHttpClient</exception>
        public static IObservable<TeamProjectReference> GetTeamProjectReferences(
            this ProjectHttpClient projectHttpClient, ProjectState? stateFilter = null, int? count = null, int? skip = null, object userState = null)
        {
            if (projectHttpClient == null)
                throw new ArgumentNullException(nameof(projectHttpClient));

            return PagedListHelper.ObservableFromPagedListProducer(
                (continuationCount, continuationSkip, continuationToken, cancellationToken) =>
                    projectHttpClient.GetProjects(stateFilter, continuationCount, continuationSkip, userState, continuationToken),
                count, skip);
        }
        
        /// <summary>
        /// Gets the team projects for the given <paramref name="projectHttpClient" />.
        /// </summary>
        /// <param name="projectHttpClient">The project HTTP client.</param>
        /// <param name="stateFilter">Filter on team projects in a specific team project state.</param>
        /// <param name="count">The amount of projects to retrieve at most.</param>
        /// <param name="skip">How many projects to skip.</param>
        /// <param name="includeCapabilities">Include capabilities (such as source control) in the team project result.</param>
        /// <param name="includeHistory">Search within renamed projects (that had such name in the past).</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">projectHttpClient</exception>
        public static IObservable<TeamProject> GetTeamProjects(
            this ProjectHttpClient projectHttpClient, ProjectState? stateFilter = null, int? count = null, int? skip = null, bool? includeCapabilities = null, bool includeHistory = false, object userState = null)
        {
            if (projectHttpClient == null)
                throw new ArgumentNullException(nameof(projectHttpClient));
            
            return projectHttpClient.GetTeamProjectReferences(stateFilter, count, skip, userState)
                .SelectMany(teamProjectReference => 
                    projectHttpClient.GetTeamProject(teamProjectReference.Id, includeCapabilities, includeHistory, userState));
        }

        /// <summary>
        /// Gets a specific team project for the given <paramref name="projectNameOrId" />.
        /// </summary>
        /// <param name="projectHttpClient">The project HTTP client.</param>
        /// <param name="projectNameOrId">The project name or identifier.</param>
        /// <param name="includeCapabilities">Include capabilities (such as source control) in the team project result.</param>
        /// <param name="includeHistory">Search within renamed projects (that had such name in the past).</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">projectHttpClient</exception>
        /// <exception cref="System.ArgumentNullException">projectHttpClient</exception>
        public static IObservable<TeamProject> GetTeamProject(
            this ProjectHttpClient projectHttpClient, string projectNameOrId, bool? includeCapabilities = null, bool includeHistory = false, object userState = null)
        {
            if (projectHttpClient == null)
                throw new ArgumentNullException(nameof(projectHttpClient));

            return Observable.FromAsync(token => projectHttpClient.GetProject(projectNameOrId, includeCapabilities, includeHistory, userState));
        }

        /// <summary>
        /// Gets a specific team project for the given <paramref name="projectId" />.
        /// </summary>
        /// <param name="projectHttpClient">The project HTTP client.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="includeCapabilities">Include capabilities (such as source control) in the team project result.</param>
        /// <param name="includeHistory">Search within renamed projects (that had such name in the past).</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">projectHttpClient</exception>
        /// <exception cref="System.ArgumentNullException">projectHttpClient</exception>
        public static IObservable<TeamProject> GetTeamProject(
            this ProjectHttpClient projectHttpClient, Guid projectId, bool? includeCapabilities = null, bool includeHistory = false, object userState = null)
        {
            if (projectHttpClient == null)
                throw new ArgumentNullException(nameof(projectHttpClient));

            return projectHttpClient.GetTeamProject(projectId.ToString(), includeCapabilities, includeHistory, userState);
        }
    }
}
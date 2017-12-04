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

            // start with the initial paged list and go deeper / continue from there using the helper method
            return Observable.FromAsync(token => projectHttpClient.GetProjects(stateFilter, count, skip, userState))
                .SelectMany(initalPagedList => 
                    initalPagedList.ToObservable((token, continuationToken) 
                        => projectHttpClient.GetProjects(stateFilter, count, skip, userState, continuationToken)));
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
                    projectHttpClient.GetProject(teamProjectReference.Id.ToString(), includeCapabilities, includeHistory, userState));
        }
    }
}
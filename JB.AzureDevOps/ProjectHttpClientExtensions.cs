using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Core.WebApi;

namespace JB.AzureDevOps
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
        public static IAsyncEnumerable<TeamProjectReference> GetTeamProjectReferences(
            this ProjectHttpClient projectHttpClient, ProjectState? stateFilter = null, int? count = null, int? skip = null, object userState = null)
        {
            if (projectHttpClient == null)
                throw new ArgumentNullException(nameof(projectHttpClient));

            return AsyncEnumerabletHelper.GetAsyncEnumerableForPagedListProducer(
                iterationInput => projectHttpClient.GetProjects(stateFilter, iterationInput.Count, iterationInput.Skip, userState), count, skip);
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
        public static IAsyncEnumerable<TeamProject> GetTeamProjects(
            this ProjectHttpClient projectHttpClient, ProjectState? stateFilter = null, int? count = null, int? skip = null, bool? includeCapabilities = null, bool includeHistory = false, object userState = null)
        {
            if (projectHttpClient == null)
                throw new ArgumentNullException(nameof(projectHttpClient));
            
            return projectHttpClient.GetTeamProjectReferences(stateFilter, count, skip, userState)
                .SelectAwait(teamProjectReference =>
                    new ValueTask<TeamProject>(projectHttpClient.GetProject(teamProjectReference.Id.ToString(), includeCapabilities, includeHistory, userState)));
        }
    }
}
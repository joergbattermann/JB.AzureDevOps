using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Core.WebApi;

namespace JB.TeamFoundationServer
{
    /// <summary>
    /// Extension methods for <see cref="TeamProjectCollectionReference"/> instances
    /// </summary>
    public static class TeamProjectCollectionReferenceExtensions
    {
        /// <summary>
        /// Gets the <see cref="TeamProjectCollection"/> for the provided <paramref name="teamProjectCollectionReference"/> using the corresponding <paramref name="projectCollectionHttpClient"/>.
        /// </summary>
        /// <param name="teamProjectCollectionReference">The team project collection reference.</param>
        /// <param name="projectCollectionHttpClient">The project collection HTTP client.</param>
        /// <param name="userState">The user state object passed along to the underlying <paramref name="projectCollectionHttpClient"/> method.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// teamProjectCollectionReference
        /// or
        /// projectCollectionHttpClient
        /// </exception>
        public static Task<TeamProjectCollection> ToTeamProjectCollection(this TeamProjectCollectionReference teamProjectCollectionReference, ProjectCollectionHttpClient projectCollectionHttpClient, object userState = null)
        {
            if (teamProjectCollectionReference == null)
                throw new ArgumentNullException(nameof(teamProjectCollectionReference));

            if (projectCollectionHttpClient == null)
                throw new ArgumentNullException(nameof(projectCollectionHttpClient));

            return projectCollectionHttpClient.GetProjectCollection(teamProjectCollectionReference.Id.ToString(), userState);
        }
    }
}
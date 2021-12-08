using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.AzureDevOps
{
    /// <summary>
    /// Extension methods for <see cref="TeamHttpClient"/> instances
    /// </summary>
    public static class TeamHttpClientExtensions
    {
        /// <summary>
        /// Gets all teams for the given <paramref name="teamHttpClient" />, optionally for a specific project corresponding to the <paramref name="projectNameOrId" />.
        /// </summary>
        /// <param name="teamHttpClient">The team HTTP client.</param>
        /// <param name="projectNameOrId">The project name or identifier.</param>
        /// <param name="mine">If true return all the teams requesting user is member, otherwise return all the teams user has read access</param>
        /// <param name="count">The amount of teams to retrieve at most.</param>
        /// <param name="skip">How many teams to skip.</param>
        /// <param name="expandIdentity">A value indicating whether or not to expand <see cref="T:Microsoft.VisualStudio.Services.Identity.Identity" /> information in the result <see cref="T:Microsoft.TeamFoundation.Core.WebApi.WebApiTeam" /> object.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">teamHttpClient</exception>
        public static IAsyncEnumerable<WebApiTeam> GetTeams(
            this TeamHttpClient teamHttpClient, string projectNameOrId = "", bool? mine = null, int? count = null, int? skip = null, bool? expandIdentity = null, object userState = null)
        {
            if (teamHttpClient == null)
                throw new ArgumentNullException(nameof(teamHttpClient));

            return !string.IsNullOrWhiteSpace(projectNameOrId)
                ? AsyncEnumerabletHelper.GetAsyncEnumerableForListProducer(
                    iterationInput => teamHttpClient.GetTeamsAsync(projectNameOrId, mine, iterationInput.Count, iterationInput.Skip, expandIdentity, userState, iterationInput.CancellationToken), count, skip)
                : AsyncEnumerabletHelper.GetAsyncEnumerableForListProducer(
                    iterationInput => teamHttpClient.GetAllTeamsAsync(mine, iterationInput.Count, iterationInput.Skip, expandIdentity, userState, iterationInput.CancellationToken), count, skip);
        }

        /// <summary>
        /// Gets the team members for the <paramref name="teamNameOrId" /> in the project corresponding to the <paramref name="projectNameOrId" />.
        /// </summary>
        /// <param name="teamHttpClient">The team HTTP client.</param>
        /// <param name="projectNameOrId">The project name or identifier.</param>
        /// <param name="teamNameOrId">The team name or identifier.</param>
        /// <param name="count">The amount of team members to retrieve at most.</param>
        /// <param name="skip">How many team members to skip.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns>
        /// The updated team.
        /// </returns>
        /// <exception cref="ArgumentNullException">teamHttpClient</exception>
        /// <exception cref="ArgumentOutOfRangeException">projectNameOrId - projectNameOrId
        /// or
        /// teamNameOrId - teamNameOrId</exception>
        public static IAsyncEnumerable<TeamMember> GetTeamMembers(
            this TeamHttpClient teamHttpClient, string projectNameOrId, string teamNameOrId, int? count = null, int? skip = null, object userState = null)
        {
            if (teamHttpClient == null)
                throw new ArgumentNullException(nameof(teamHttpClient));

            if (string.IsNullOrWhiteSpace(projectNameOrId)) throw new ArgumentOutOfRangeException(nameof(projectNameOrId), $"'{nameof(projectNameOrId)}' may not be null or empty or whitespaces only");
            if (string.IsNullOrWhiteSpace(teamNameOrId)) throw new ArgumentOutOfRangeException(nameof(teamNameOrId), $"'{nameof(teamNameOrId)}' may not be null or empty or whitespaces only");

            return AsyncEnumerabletHelper.GetAsyncEnumerableForListProducer(
                iterationInput => teamHttpClient.GetTeamMembersWithExtendedPropertiesAsync(projectNameOrId, teamNameOrId, iterationInput.Count, iterationInput.Skip, userState, iterationInput.CancellationToken),
                count,
                skip);
        }
    }
}
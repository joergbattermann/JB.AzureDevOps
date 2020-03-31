using System;
using System.Reactive.Linq;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.TeamFoundationServer
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
        public static IObservable<WebApiTeam> GetTeams(
            this TeamHttpClient teamHttpClient, string projectNameOrId = "", bool? mine = null, int? count = null, int? skip = null, bool? expandIdentity = null, object userState = null)
        {
            if (teamHttpClient == null)
                throw new ArgumentNullException(nameof(teamHttpClient));

            return !string.IsNullOrWhiteSpace(projectNameOrId)
                ? Observable.FromAsync(cancellationToken => teamHttpClient.GetTeamsAsync(projectNameOrId, mine, count, skip, expandIdentity, userState, cancellationToken))
                    .SelectMany(webApiTeams => webApiTeams)
                : Observable.FromAsync(cancellationToken => teamHttpClient.GetAllTeamsAsync(mine, count, skip, expandIdentity, userState, cancellationToken))
                    .SelectMany(webApiTeams => webApiTeams);
        }

        /// <summary>
        /// Gets a specific team using for the <paramref name="teamNameOrId" /> in the project corresponding to the <paramref name="projectNameOrId" />.
        /// </summary>
        /// <param name="teamHttpClient">The team HTTP client.</param>
        /// <param name="projectNameOrId">The project name or identifier.</param>
        /// <param name="teamNameOrId">The team name or identifier.</param>
        /// <param name="expandIdentity">A value indicating whether or not to expand <see cref="T:Microsoft.VisualStudio.Services.Identity.Identity" /> information in the result <see cref="T:Microsoft.TeamFoundation.Core.WebApi.WebApiTeam" /> object.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">teamHttpClient</exception>
        public static IObservable<WebApiTeam> GetTeam(
            this TeamHttpClient teamHttpClient, string projectNameOrId, string teamNameOrId, bool? expandIdentity = null, object userState = null)
        {
            if (teamHttpClient == null) throw new ArgumentNullException(nameof(teamHttpClient));

            if(string.IsNullOrWhiteSpace(projectNameOrId)) throw new ArgumentOutOfRangeException(nameof(projectNameOrId), $"'{nameof(projectNameOrId)}' may not be null or empty or whitespaces only");
            if(string.IsNullOrWhiteSpace(teamNameOrId)) throw new ArgumentOutOfRangeException(nameof(teamNameOrId), $"'{nameof(teamNameOrId)}' may not be null or empty or whitespaces only");

            return Observable.FromAsync(cancellationToken => teamHttpClient.GetTeamAsync(projectNameOrId, teamNameOrId, expandIdentity, userState, cancellationToken));
        }

        /// <summary>
        /// Creates a new team using the <paramref name="teamToCreate" /> as template in the project corresponding to the <paramref name="projectNameOrId" />.
        /// </summary>
        /// <param name="teamHttpClient">The team HTTP client.</param>
        /// <param name="teamToCreate">The team to create.</param>
        /// <param name="projectNameOrId">The project name or identifier.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns>
        /// The created team.
        /// </returns>
        /// <exception cref="ArgumentNullException">teamHttpClient</exception>
        public static IObservable<WebApiTeam> CreateTeam(
            this TeamHttpClient teamHttpClient, WebApiTeam teamToCreate, string projectNameOrId, object userState = null)
        {
            if (teamHttpClient == null) throw new ArgumentNullException(nameof(teamHttpClient));
            if (teamToCreate == null) throw new ArgumentNullException(nameof(teamToCreate));

            if (string.IsNullOrWhiteSpace(projectNameOrId)) throw new ArgumentOutOfRangeException(nameof(projectNameOrId), $"'{nameof(projectNameOrId)}' may not be null or empty or whitespaces only");

            return Observable.FromAsync(cancellationToken => teamHttpClient.CreateTeamAsync(teamToCreate, projectNameOrId, userState, cancellationToken));
        }

        /// <summary>
        /// Updates an existing team for the <paramref name="teamNameOrId" /> using the <paramref name="updatedTeam" /> as template in the project corresponding to the <paramref name="projectNameOrId" />.
        /// </summary>
        /// <param name="teamHttpClient">The team HTTP client.</param>
        /// <param name="updatedTeam">The updated team.</param>
        /// <param name="projectNameOrId">The project name or identifier.</param>
        /// <param name="teamNameOrId">The team name or identifier.</param>
        /// <param name="userState">The user state object to pass along to the underlying method.</param>
        /// <returns>
        /// The updated team.
        /// </returns>
        /// <exception cref="ArgumentNullException">teamHttpClient</exception>
        public static IObservable<WebApiTeam> UpdateTeam(
            this TeamHttpClient teamHttpClient, WebApiTeam updatedTeam, string projectNameOrId, string teamNameOrId, object userState = null)
        {
            if (teamHttpClient == null) throw new ArgumentNullException(nameof(teamHttpClient));
            if (updatedTeam == null) throw new ArgumentNullException(nameof(updatedTeam));

            if (string.IsNullOrWhiteSpace(projectNameOrId)) throw new ArgumentOutOfRangeException(nameof(projectNameOrId), $"'{nameof(projectNameOrId)}' may not be null or empty or whitespaces only");
            if (string.IsNullOrWhiteSpace(teamNameOrId)) throw new ArgumentOutOfRangeException(nameof(teamNameOrId), $"'{nameof(teamNameOrId)}' may not be null or empty or whitespaces only");

            return Observable.FromAsync(cancellationToken => teamHttpClient.UpdateTeamAsync(updatedTeam, projectNameOrId, teamNameOrId, userState, cancellationToken));
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
        public static IObservable<TeamMember> GetTeamMembers(
            this TeamHttpClient teamHttpClient, string projectNameOrId, string teamNameOrId, int? count = null, int? skip = null, object userState = null)
        {
            if (teamHttpClient == null)
                throw new ArgumentNullException(nameof(teamHttpClient));

            if (string.IsNullOrWhiteSpace(projectNameOrId)) throw new ArgumentOutOfRangeException(nameof(projectNameOrId), $"'{nameof(projectNameOrId)}' may not be null or empty or whitespaces only");
            if (string.IsNullOrWhiteSpace(teamNameOrId)) throw new ArgumentOutOfRangeException(nameof(teamNameOrId), $"'{nameof(teamNameOrId)}' may not be null or empty or whitespaces only");

            return Observable.FromAsync(cancellationToken => teamHttpClient.GetTeamMembersWithExtendedPropertiesAsync(projectNameOrId, teamNameOrId, count, skip, userState, cancellationToken))
                .SelectMany(teamMembers => teamMembers);
        }
    }
}
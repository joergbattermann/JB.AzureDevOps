using System;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;

namespace JB.TeamFoundationServer.WorkHttpClient
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.TeamFoundation.Work.WebApi.WorkHttpClient "/> instances.
    /// </summary>
    public static class WorkHttpClientExtensions
    {
        /// <summary>
        /// Gets the default team area path for the given <paramref name="projectNameOrId"/> and optinally <paramref name="teamNameOrId"/>.
        /// </summary>
        /// <param name="workHttpClient">The work HTTP client.</param>
        /// <param name="projectNameOrId">The project name or identifier.</param>
        /// <param name="teamNameOrId">The team name or identifier.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workHttpClient</exception>
        /// <exception cref="ArgumentOutOfRangeException">projectNameOrId - projectNameOrId</exception>
        public static IObservable<string> GetDefaultTeamAreaPath(
            this Microsoft.TeamFoundation.Work.WebApi.WorkHttpClient workHttpClient, string projectNameOrId, string teamNameOrId = "", object userState = null)
        {
            if (workHttpClient == null)
                throw new ArgumentNullException(nameof(workHttpClient));

            if (string.IsNullOrWhiteSpace(projectNameOrId)) throw new ArgumentOutOfRangeException(nameof(projectNameOrId), $"'{nameof(projectNameOrId)}' may not be null or empty or whitespaces only");

            var teamContext = Guid.TryParse(projectNameOrId, out var projectId)
                ? new TeamContext(projectId)
                : new TeamContext(projectNameOrId);

            if (!string.IsNullOrWhiteSpace(teamNameOrId))
            {
                if (Guid.TryParse(teamNameOrId, out var teamId))
                {
                    teamContext.TeamId = teamId;
                }
                else
                {
                    teamContext.Team = teamNameOrId;
                }
            }

            return workHttpClient.GetTeamFieldValues(projectNameOrId, teamNameOrId, userState)
                .Select(teamFieldValues =>
                {
                    if (!string.Equals(WorkItemTracking.WellKnownWorkItemFieldReferenceNames.System.AreaPath, teamFieldValues?.Field?.ReferenceName, StringComparison.OrdinalIgnoreCase))
                        return string.Empty;

                    return string.IsNullOrWhiteSpace(teamFieldValues?.DefaultValue)
                        ? string.Empty
                        : teamFieldValues.DefaultValue;
                });
        }

        /// <summary>
        /// Gets the team field values for the given <paramref name="projectNameOrId"/> and optinally <paramref name="teamNameOrId"/>.
        /// </summary>
        /// <param name="workHttpClient">The work HTTP client.</param>
        /// <param name="projectNameOrId">The project name or identifier.</param>
        /// <param name="teamNameOrId">The team name or identifier.</param>
        /// <param name="userState">State of the user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">workHttpClient</exception>
        /// <exception cref="ArgumentOutOfRangeException">projectNameOrId - projectNameOrId</exception>
        public static IObservable<TeamFieldValues> GetTeamFieldValues(
            this Microsoft.TeamFoundation.Work.WebApi.WorkHttpClient workHttpClient, string projectNameOrId, string teamNameOrId = "", object userState = null)
        {
            if (workHttpClient == null)
                throw new ArgumentNullException(nameof(workHttpClient));

            if (string.IsNullOrWhiteSpace(projectNameOrId)) throw new ArgumentOutOfRangeException(nameof(projectNameOrId), $"'{nameof(projectNameOrId)}' may not be null or empty or whitespaces only");

            var teamContext = Guid.TryParse(projectNameOrId, out var projectId)
                ? new TeamContext(projectId)
                : new TeamContext(projectNameOrId);

            if (!string.IsNullOrWhiteSpace(teamNameOrId))
            {
                if (Guid.TryParse(teamNameOrId, out var teamId))
                {
                    teamContext.TeamId = teamId;
                }
                else
                {
                    teamContext.Team = teamNameOrId;
                }
            }

            return Observable.FromAsync(cancellationToken => workHttpClient.GetTeamFieldValuesAsync(teamContext, userState, cancellationToken));
        }
    }
}
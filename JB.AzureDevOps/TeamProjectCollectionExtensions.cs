using System;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace JB.AzureDevOps
{
    /// <summary>
    /// Extension methods for <see cref="TeamProjectCollection"/> instances.
    /// </summary>
    public static class TeamProjectCollectionExtensions
    {
        /// <summary>
        /// Gets the <see cref="ProjectHttpClient"/> for the provided <paramref name="teamProjectCollection"/>.
        /// </summary>
        /// <param name="teamProjectCollection">The team project collection.</param>
        /// <param name="vssCredentials">The VSS credentials to use.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// teamProjectCollection
        /// or
        /// vssCredentials
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// teamProjectCollection - teamProjectCollection
        /// or
        /// teamProjectCollection - teamProjectCollection
        /// </exception>
        public static ProjectHttpClient GetProjectHttpClient(this TeamProjectCollection teamProjectCollection,
            VssCredentials vssCredentials)
        {
            if (teamProjectCollection == null) throw new ArgumentNullException(nameof(teamProjectCollection));
            if (vssCredentials == null) throw new ArgumentNullException(nameof(vssCredentials));

            if(teamProjectCollection.Links?.Links == null)
                throw new ArgumentException($"The {nameof(teamProjectCollection)}.{teamProjectCollection.Links} may not be null.", nameof(teamProjectCollection));

            const string projectReferenceLinkKey = "web";

            if (teamProjectCollection.Links.Links.TryGetValue(projectReferenceLinkKey, out var linkObject) == true
                && linkObject is ReferenceLink referenceLink
                && !string.IsNullOrWhiteSpace(referenceLink.Href))
            {
                return new VssConnection(new Uri(referenceLink.Href), vssCredentials).GetClient<ProjectHttpClient>();
            }
            else
            {
                throw new ArgumentException($"The {nameof(teamProjectCollection)} contains no {nameof(ReferenceLink)} with the '{projectReferenceLinkKey}' key that would / should contain the corresponding URL for the {nameof(ProjectHttpClient)}", nameof(teamProjectCollection));
            }
        }
    }
}
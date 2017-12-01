using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace JB.TeamFoundationServer.ReactiveClient.WorkItemTracking
{
    /// <summary>
    /// Extension Methods for <see cref="WorkItem"/> instances.
    /// </summary>
    public static class WorkItemExtensions
    {
        public static IObservable<WorkItem> AddForwardRelation(this WorkItem sourceWorkItem, WorkItem targetWorkItem, string workItemLinkTypeReferenceName, string comment = "", bool? validateOnly = null, bool? bypassRules = null, bool? suppressNotifications = null, object userState = null)
        {
            // see https://github.com/Microsoft/vsts-dotnet-samples/blob/master/ClientLibrary/Snippets/Microsoft.TeamServices.Samples.Client/WorkItemTracking/WorkItemsSample.cs#L466

            throw new NotImplementedException();
        }

        public static IObservable<WorkItem> AddReverseRelation(this WorkItem sourceWorkItem, WorkItem targetWorkItem, string workItemLinkTypeReferenceName, string comment = "", bool? validateOnly = null, bool? bypassRules = null, bool? suppressNotifications = null, object userState = null)
        {
            // see https://github.com/Microsoft/vsts-dotnet-samples/blob/master/ClientLibrary/Snippets/Microsoft.TeamServices.Samples.Client/WorkItemTracking/WorkItemsSample.cs#L242

            throw new NotImplementedException();
        }
    }
}
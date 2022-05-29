namespace JB.AzureDevOps.Constants
{
    public static class WorkItems
    {
        public const string FieldsBasePath = "/fields";

        public const string RelationsBasePath = "/relations";
        public const string RelationsReverseSuffix = "-reverse";
        public const string RelationsForwardSuffix = "-forward";

        public const string RelationReferenceNameForAttachedFiles = "AttachedFile";
        public const string RelationReferenceNameForArtifactLinks = "ArtifactLink";
        public const string RelationReferenceNameForHyperlinks = "Hyperlink";

        internal const string LinksLinkNameSelf = "self";
        internal const string LinksLinkNameUpdates = "workItemUpdates";
        internal const string LinksLinkNameRevisions = "revisions";
        internal const string LinksLinkNameHistory = "history";
        internal const string LinksLinkNameHtml = "html";
        internal const string LinksLinkNameWorkItemType = "workItemType";
        internal const string LinksLinkNameFields = "fields";

        public const int WorkItemBatchSize = 200; // see https://github.com/MicrosoftDocs/vsts-docs/issues/1638
    }
}
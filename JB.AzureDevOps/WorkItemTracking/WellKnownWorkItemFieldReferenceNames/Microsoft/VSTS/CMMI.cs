namespace JB.AzureDevOps.WorkItemTracking.WellKnownWorkItemFieldReferenceNames.Microsoft.VSTS
{
    public static class CMMI
    {
        public const string RequiresReview = "Microsoft.VSTS.CMMI.RequiresReview";
        public const string RequiresTest = "Microsoft.VSTS.CMMI.RequiresTest";
        public const string TaskType = "Microsoft.VSTS.CMMI.TaskType";
        public const string Purpose = "Microsoft.VSTS.CMMI.Purpose";
        public const string Comments = "Microsoft.VSTS.CMMI.Comments";
        public const string Minutes = "Microsoft.VSTS.CMMI.Minutes";
        public const string MeetingType = "Microsoft.VSTS.CMMI.MeetingType";
        public const string CalledDate = "Microsoft.VSTS.CMMI.CalledDate";
        public const string CalledBy = "Microsoft.VSTS.CMMI.CalledBy";
        public const string Symptom = "Microsoft.VSTS.CMMI.Symptom";
        public const string ProposedFix = "Microsoft.VSTS.CMMI.ProposedFix";
        public const string FoundInEnvironment = "Microsoft.VSTS.CMMI.FoundInEnvironment";
        public const string RootCause = "Microsoft.VSTS.CMMI.RootCause";
        public const string HowFound = "Microsoft.VSTS.CMMI.HowFound";
        public const string Analysis = "Microsoft.VSTS.CMMI.Analysis";
        public const string CorrectiveActionPlan = "Microsoft.VSTS.CMMI.CorrectiveActionPlan";
        public const string TargetResolveDate = "Microsoft.VSTS.CMMI.TargetResolveDate";
        public const string ContingencyPlan = "Microsoft.VSTS.CMMI.ContingencyPlan";
        public const string MitigationPlan = "Microsoft.VSTS.CMMI.MitigationPlan";
        public const string Probability = "Microsoft.VSTS.CMMI.Probability";
        public const string RequirementType = "Microsoft.VSTS.CMMI.RequirementType";
        public const string UserAcceptanceTest = "Microsoft.VSTS.CMMI.UserAcceptanceTest";

        private const string SubjectMatterExpertPrefix = "Microsoft.VSTS.CMMI.SubjectMatterExpert";
        /// <summary>
        /// Gets the reference name for required attendee #<paramref name="expertNumber"/> (1-3 as of writing).
        /// </summary>
        /// <param name="expertNumber">The subject matter expert.</param>
        /// <returns>The corresponding reference name.</returns>
        public static string GetSubjectMatterExpert(int expertNumber) => $"{SubjectMatterExpertPrefix}{expertNumber}";

        private const string RequiredAttendeePrefix = "Microsoft.VSTS.CMMI.RequiredAttendee";
        /// <summary>
        /// Gets the reference name for required attendee #<paramref name="attendeeNumber"/> (1-8 as of writing).
        /// </summary>
        /// <param name="attendeeNumber">The attendee number.</param>
        /// <returns>The corresponding reference name.</returns>
        public static string GetRequiredAttendee(int attendeeNumber) => $"{RequiredAttendeePrefix}{attendeeNumber}";

        private const string OptionalAttendeePrefix = "Microsoft.VSTS.CMMI.OptionalAttendee";
        /// <summary>
        /// Gets the reference name for optional attendee #<paramref name="attendeeNumber"/> (1-8 as of writing).
        /// </summary>
        /// <param name="attendeeNumber">The attendee number.</param>
        /// <returns>The corresponding reference name.</returns>
        public static string GetOptionalAttendee(int attendeeNumber) => $"{OptionalAttendeePrefix}{attendeeNumber}";

        private const string ActualAttendeePrefix = "Microsoft.VSTS.CMMI.ActualAttendee";
        /// <summary>
        /// Gets the reference name for actual attendee #<paramref name="attendeeNumber"/> (1-8 as of writing).
        /// </summary>
        /// <param name="attendeeNumber">The attendee number.</param>
        /// <returns>The corresponding reference name.</returns>
        public static string GetActualAttendee(int attendeeNumber) => $"{ActualAttendeePrefix}{attendeeNumber}";
    }
}
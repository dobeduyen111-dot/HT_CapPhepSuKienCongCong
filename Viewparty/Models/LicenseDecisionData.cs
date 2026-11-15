using System;
using System.Collections.Generic;

namespace Viewparty.Models
{
    public sealed class LicenseDecisionCase
    {
        public long CaseId { get; set; }
        public string EventName { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public DateTimeOffset? StartsAt { get; set; }
        public DateTimeOffset? EndsAt { get; set; }
        public int? Attendance { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public int? VenueCapacity { get; set; }
        public bool CanApprove { get; set; }
        public int AssessedRuleCount { get; set; }
        public int MandatoryFailureCount { get; set; }
        public int ApprovedExceptionCount { get; set; }
        public int RespondedOpinionCount { get; set; }
        public string LicenseNumber { get; set; }
        public string RejectionReason { get; set; }
    }

    public sealed class LicenseIssueResult
    {
        public long CaseId { get; set; }
        public string LicenseNumber { get; set; }
    }

    public sealed class IssuedLicenseItem
    {
        public long LicenseId { get; set; }
        public long CaseId { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public string EventName { get; set; }
        public string VenueName { get; set; }
        public string Conditions { get; set; }
    }

    public sealed class ApproverReviewData
    {
        public IList<ApproverOpinionItem> Opinions { get; set; }
        public IList<ApproverDocumentItem> Documents { get; set; }
        public IList<ApproverHistoryItem> History { get; set; }
    }

    public sealed class ApproverOpinionItem
    {
        public string UnitName { get; set; }
        public string Status { get; set; }
        public string Conclusion { get; set; }
        public string Response { get; set; }
        public string Recommendation { get; set; }
        public DateTimeOffset? RespondedAt { get; set; }
    }

    public sealed class ApproverDocumentItem
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public sealed class ApproverHistoryItem
    {
        public string Action { get; set; }
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public string Result { get; set; }
        public string Note { get; set; }
        public DateTimeOffset At { get; set; }
    }
}

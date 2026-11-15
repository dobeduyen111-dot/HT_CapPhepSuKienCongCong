using System;

namespace Viewparty.Models
{
    public sealed class CaseOfficerCaseItem
    {
        public long CaseId { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public string OrganizerName { get; set; }
        public string Status { get; set; }
        public string VenueName { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public int? Attendance { get; set; }
    }

    public sealed class CaseOfficerDocumentItem
    {
        public long DocumentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public sealed class CaseOfficerRuleItem
    {
        public long RuleId { get; set; }
        public string RuleCode { get; set; }
        public int Version { get; set; }
        public string Group { get; set; }
        public string Severity { get; set; }
        public string Condition { get; set; }
        public string Result { get; set; }
        public string ActualValue { get; set; }
        public string Note { get; set; }
    }
}

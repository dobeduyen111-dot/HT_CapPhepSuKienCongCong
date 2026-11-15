using System;

namespace Viewparty.Models
{
    public sealed class InteragencyRequestItem
    {
        public long RequestId { get; set; }
        public long CaseId { get; set; }
        public string EventName { get; set; }
        public string UnitName { get; set; }
        public string Field { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public string Status { get; set; }
        public string Conclusion { get; set; }
        public string Response { get; set; }
        public string Recommendation { get; set; }
    }

    public sealed class InteragencyCaseItem
    {
        public long CaseId { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public string Status { get; set; }
        public int RequiredUnits { get; set; }
        public int RespondedRequiredUnits { get; set; }
    }

    public sealed class InteragencySummaryResult
    {
        public long CaseId { get; set; }
        public int RuleCount { get; set; }
        public int MandatoryFailures { get; set; }
        public int NonExemptMandatoryFailures { get; set; }
        public int Warnings { get; set; }
        public string ProposedConclusion { get; set; }
        public string NextStatus { get; set; }
    }
}

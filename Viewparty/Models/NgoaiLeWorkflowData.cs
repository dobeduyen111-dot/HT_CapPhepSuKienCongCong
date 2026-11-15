using System;

namespace Viewparty.Models
{
    public sealed class NgoaiLeEligibleResult
    {
        public long ResultId { get; set; }
        public string RuleCode { get; set; }
        public int Version { get; set; }
        public string Group { get; set; }
        public short ApprovalLevel { get; set; }
        public string Note { get; set; }
    }

    public sealed class NgoaiLeDecisionItem
    {
        public long ExceptionId { get; set; }
        public long CaseId { get; set; }
        public long ResultId { get; set; }
        public long ProposerId { get; set; }
        public string ProposerName { get; set; }
        public string EventName { get; set; }
        public string RuleCode { get; set; }
        public int Version { get; set; }
        public string Group { get; set; }
        public string AssessmentNote { get; set; }
        public string Reason { get; set; }
        public string Evidence { get; set; }
        public string Mitigation { get; set; }
        public short RequiredApprovalLevel { get; set; }
        public DateTimeOffset ProposedAt { get; set; }
    }
}

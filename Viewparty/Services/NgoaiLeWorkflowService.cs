using System;
using System.Collections.Generic;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class NgoaiLeWorkflowService
    {
        private readonly NgoaiLeWorkflowRepository repository;

        public NgoaiLeWorkflowService() : this(new NgoaiLeWorkflowRepository()) { }

        internal NgoaiLeWorkflowService(NgoaiLeWorkflowRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public IList<NgoaiLeEligibleResult> GetEligibleResults(AuthenticatedUser caseOfficer, long caseId)
        {
            ValidateCaseOfficer(caseOfficer);
            ValidateId(caseId, "Mã hồ sơ");
            return repository.GetEligibleResults(caseId, caseOfficer.AccountId);
        }

        public long Propose(AuthenticatedUser caseOfficer, long caseId, long resultId,
            string reason, string evidence, string mitigation)
        {
            ValidateCaseOfficer(caseOfficer);
            ValidateId(caseId, "Mã hồ sơ");
            ValidateId(resultId, "Mã kết quả thẩm định");
            reason = RequiredText(reason, "Lý do đề xuất");
            evidence = RequiredText(evidence, "Minh chứng");
            mitigation = RequiredText(mitigation, "Biện pháp bù đắp");
            return repository.Propose(caseId, resultId, caseOfficer.AccountId,
                reason, evidence, mitigation);
        }

        public IList<NgoaiLeDecisionItem> GetPendingDecisions(AuthenticatedUser approver)
        {
            ValidateApprover(approver);
            return repository.GetPendingDecisions(approver.AccountId);
        }

        public void Decide(AuthenticatedUser approver, long exceptionId, bool approve, string note)
        {
            ValidateApprover(approver);
            ValidateId(exceptionId, "Mã đề xuất ngoại lệ");
            note = RequiredText(note, "Lý do/căn cứ quyết định");
            repository.Decide(exceptionId, approver.AccountId, approve, note);
        }

        private static string RequiredText(string value, string label)
        {
            value = (value ?? string.Empty).Trim();
            if (value.Length == 0)
                throw new NgoaiLeValidationException(label + " không được để trống.");
            if (value.Length > 4000)
                throw new NgoaiLeValidationException(label + " không được vượt quá 4.000 ký tự.");
            return value;
        }

        private static void ValidateId(long id, string label)
        {
            if (id <= 0) throw new NgoaiLeValidationException(label + " không hợp lệ.");
        }

        private static void ValidateCaseOfficer(AuthenticatedUser user)
        {
            if (user == null || user.AccountId <= 0 || user.RoleCode != "NGUOI_XU_LY")
                throw new NgoaiLeValidationException("Chức năng chỉ dành cho tài khoản Người xử lý.");
        }

        private static void ValidateApprover(AuthenticatedUser user)
        {
            if (user == null || user.AccountId <= 0 || user.RoleCode != "NGUOI_PHE_DUYET" ||
                !user.ApprovalLevel.HasValue || user.ApprovalLevel.Value <= 0)
                throw new NgoaiLeValidationException(
                    "Chức năng chỉ dành cho Người phê duyệt đang có cấp thẩm quyền hợp lệ.");
        }
    }

    public sealed class NgoaiLeValidationException : Exception
    {
        public NgoaiLeValidationException(string message) : base(message) { }
    }
}

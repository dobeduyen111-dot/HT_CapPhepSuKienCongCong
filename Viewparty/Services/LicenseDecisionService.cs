using System;
using System.Collections.Generic;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class LicenseDecisionService
    {
        private readonly LicenseDecisionRepository repository;

        public LicenseDecisionService() : this(new LicenseDecisionRepository()) { }

        internal LicenseDecisionService(LicenseDecisionRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public IList<LicenseDecisionCase> GetDecisionCases(AuthenticatedUser approver)
        {
            ValidateApprover(approver);
            return repository.GetDecisionCases(approver.AccountId);
        }

        public IList<IssuedLicenseItem> GetIssuedLicenses(AuthenticatedUser approver)
        {
            ValidateApprover(approver);
            return repository.GetIssuedLicenses(approver.AccountId);
        }

        public ApproverReviewData GetReviewData(AuthenticatedUser approver, long caseId)
        {
            ValidateApprover(approver);
            ValidateCaseId(caseId);
            return repository.GetReviewData(approver.AccountId, caseId);
        }

        public LicenseIssueResult Approve(
            AuthenticatedUser approver, long caseId, string conditions)
        {
            ValidateApprover(approver);
            ValidateCaseId(caseId);
            if (conditions != null && conditions.Trim().Length > 4000)
                throw new LicenseDecisionValidationException(
                    "Điều kiện kèm theo không được vượt quá 4.000 ký tự.");
            return repository.Approve(caseId, approver.AccountId,
                string.IsNullOrWhiteSpace(conditions) ? null : conditions.Trim());
        }

        public void Reject(AuthenticatedUser approver, long caseId, string reason)
        {
            ValidateApprover(approver);
            ValidateCaseId(caseId);
            reason = (reason ?? string.Empty).Trim();
            if (reason.Length == 0)
                throw new LicenseDecisionValidationException("Phải nhập lý do từ chối.");
            if (reason.Length > 4000)
                throw new LicenseDecisionValidationException(
                    "Lý do từ chối không được vượt quá 4.000 ký tự.");
            repository.Reject(caseId, approver.AccountId, reason);
        }

        private static void ValidateCaseId(long caseId)
        {
            if (caseId <= 0)
                throw new LicenseDecisionValidationException("Mã hồ sơ không hợp lệ.");
        }

        private static void ValidateApprover(AuthenticatedUser user)
        {
            if (user == null || user.AccountId <= 0 || user.RoleCode != "NGUOI_PHE_DUYET" ||
                !user.ApprovalLevel.HasValue || user.ApprovalLevel.Value <= 0)
                throw new LicenseDecisionValidationException(
                    "Chức năng chỉ dành cho Người phê duyệt đang có cấp thẩm quyền hợp lệ.");
        }
    }

    public sealed class LicenseDecisionValidationException : Exception
    {
        public LicenseDecisionValidationException(string message) : base(message) { }
    }
}

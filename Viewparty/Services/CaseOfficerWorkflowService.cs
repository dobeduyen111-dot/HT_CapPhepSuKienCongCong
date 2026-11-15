using System;
using System.Collections.Generic;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class CaseOfficerWorkflowService
    {
        private readonly CaseOfficerWorkflowRepository repository;
        public CaseOfficerWorkflowService() : this(new CaseOfficerWorkflowRepository()) { }
        internal CaseOfficerWorkflowService(CaseOfficerWorkflowRepository repository) { this.repository = repository; }

        public IList<CaseOfficerCaseItem> GetInbox(AuthenticatedUser user) { Validate(user); return repository.GetInbox(user.AccountId); }
        public IList<CaseOfficerCaseItem> GetAssigned(AuthenticatedUser user, string status) { Validate(user); return repository.GetAssigned(user.AccountId, status); }
        public CaseOfficerCaseItem GetCase(AuthenticatedUser user, long caseId) { Validate(user); return repository.GetCase(caseId, user.AccountId); }
        public IList<CaseOfficerDocumentItem> GetDocuments(AuthenticatedUser user, long caseId) { Validate(user); return repository.GetDocuments(caseId, user.AccountId); }
        public IList<CaseOfficerRuleItem> GetRules(AuthenticatedUser user, long caseId) { Validate(user); return repository.GetRules(caseId, user.AccountId); }
        public void Accept(AuthenticatedUser user, long caseId) { Validate(user); repository.Accept(caseId, user.AccountId); }
        public void CheckValidity(AuthenticatedUser user, long caseId, bool valid, string reason)
        {
            Validate(user);
            if (!valid && string.IsNullOrWhiteSpace(reason)) throw new CaseOfficerValidationException("Phải nhập lý do khi yêu cầu bổ sung.");
            repository.CheckValidity(caseId, user.AccountId, valid, reason);
        }
        public void SaveRuleResult(AuthenticatedUser user, long caseId, long ruleId, string result, string actualValue, string note)
        {
            Validate(user);
            if (result != "DAT" && result != "KHONG_DAT" && result != "CANH_BAO") throw new CaseOfficerValidationException("Hãy chọn kết quả thẩm định hợp lệ.");
            repository.SaveRuleResult(caseId, user.AccountId, ruleId, result, actualValue, note);
        }
        public void FinishAppraisal(AuthenticatedUser user, long caseId) { Validate(user); repository.FinishAppraisal(caseId, user.AccountId); }

        private static void Validate(AuthenticatedUser user)
        {
            if (user == null || user.AccountId <= 0 || user.RoleCode != "NGUOI_XU_LY") throw new CaseOfficerValidationException("Chức năng chỉ dành cho tài khoản Người xử lý.");
        }
    }

    public sealed class CaseOfficerValidationException : Exception
    {
        public CaseOfficerValidationException(string message) : base(message) { }
    }
}

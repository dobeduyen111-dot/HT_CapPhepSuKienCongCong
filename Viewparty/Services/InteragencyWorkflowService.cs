using System;
using System.Collections.Generic;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class InteragencyWorkflowService
    {
        private readonly InteragencyWorkflowRepository repository;

        public InteragencyWorkflowService() : this(new InteragencyWorkflowRepository()) { }

        internal InteragencyWorkflowService(InteragencyWorkflowRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public IList<InteragencyCaseItem> GetAssignedCases(AuthenticatedUser caseOfficer)
        {
            ValidateCaseOfficer(caseOfficer);
            return repository.GetAssignedCases(caseOfficer.AccountId);
        }

        public IList<InteragencyRequestItem> GetUnitRequests(AuthenticatedUser interagencyUser)
        {
            ValidateInteragencyUser(interagencyUser);
            return repository.GetUnitRequests(interagencyUser.AccountId);
        }

        public int CreateRequiredRequests(AuthenticatedUser caseOfficer, long caseId)
        {
            ValidateCaseOfficer(caseOfficer);
            if (caseId <= 0) throw new InteragencyValidationException("Mã hồ sơ không hợp lệ.");
            return repository.CreateRequiredRequests(caseId, caseOfficer.AccountId);
        }

        public void SubmitOpinion(
            AuthenticatedUser interagencyUser,
            long requestId,
            string conclusion,
            string response,
            string recommendation)
        {
            ValidateInteragencyUser(interagencyUser);
            if (requestId <= 0) throw new InteragencyValidationException("Mã yêu cầu không hợp lệ.");
            if (conclusion != "PHU_HOP" &&
                conclusion != "PHU_HOP_CO_DIEU_KIEN" &&
                conclusion != "KHONG_PHU_HOP")
                throw new InteragencyValidationException("Hãy chọn một kết luận phản hồi hợp lệ.");

            response = (response ?? string.Empty).Trim();
            recommendation = string.IsNullOrWhiteSpace(recommendation)
                ? null
                : recommendation.Trim();
            if (response.Length == 0)
                throw new InteragencyValidationException("Nội dung phản hồi không được để trống.");
            if (response.Length > 4000)
                throw new InteragencyValidationException("Nội dung phản hồi không được vượt quá 4.000 ký tự.");
            if (recommendation != null && recommendation.Length > 4000)
                throw new InteragencyValidationException("Kiến nghị không được vượt quá 4.000 ký tự.");

            repository.SubmitOpinion(requestId, interagencyUser.AccountId,
                conclusion, response, recommendation);
        }

        public InteragencySummaryResult Summarize(AuthenticatedUser caseOfficer, long caseId)
        {
            ValidateCaseOfficer(caseOfficer);
            if (caseId <= 0) throw new InteragencyValidationException("Mã hồ sơ không hợp lệ.");
            return repository.Summarize(caseId, caseOfficer.AccountId);
        }

        private static void ValidateCaseOfficer(AuthenticatedUser user)
        {
            if (user == null || user.AccountId <= 0 || user.RoleCode != "NGUOI_XU_LY")
                throw new InteragencyValidationException("Chức năng chỉ dành cho tài khoản Người xử lý.");
        }

        private static void ValidateInteragencyUser(AuthenticatedUser user)
        {
            if (user == null || user.AccountId <= 0 || user.RoleCode != "LIEN_NGANH" ||
                !user.UnitId.HasValue || user.UnitId.Value <= 0)
                throw new InteragencyValidationException(
                    "Tài khoản Đơn vị liên ngành phải đang đăng nhập và được gán đơn vị.");
        }
    }

    public sealed class InteragencyValidationException : Exception
    {
        public InteragencyValidationException(string message) : base(message) { }
    }
}

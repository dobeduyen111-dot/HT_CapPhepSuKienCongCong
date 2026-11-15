using System;
using System.Collections.Generic;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class DiaDiemService
    {
        private readonly DiaDiemRepository diaDiems;

        public DiaDiemService() : this(new DiaDiemRepository()) { }

        internal DiaDiemService(DiaDiemRepository diaDiems)
        {
            this.diaDiems = diaDiems ?? throw new ArgumentNullException("diaDiems");
        }

        public IList<DiaDiem> GetAllActive()
        {
            return diaDiems.GetAllActive();
        }

        public IList<HoSoCapPhep> GetOrganizerDrafts(AuthenticatedUser organizer)
        {
            if (organizer == null || organizer.AccountId <= 0 ||
                !string.Equals(organizer.RoleCode, "NGUOI_TO_CHUC", StringComparison.Ordinal))
                throw new DiaDiemValidationException("Tài khoản Người tổ chức không hợp lệ.");

            return diaDiems.GetOrganizerDrafts(organizer.AccountId);
        }

        public IList<DiaDiem> Search(string keyword, DateTimeOffset start, DateTimeOffset end, int audienceSize)
        {
            if (start >= end)
                throw new DiaDiemValidationException("Ngày kết thúc phải sau ngày bắt đầu.");
            if (audienceSize < 1)
                throw new DiaDiemValidationException("Quy mô khách dự kiến phải lớn hơn 0.");
            if (audienceSize > 10000)
                throw new DiaDiemValidationException("Quy mô khách dự kiến không được vượt quá 10.000 người.");
            if (!string.IsNullOrWhiteSpace(keyword) && keyword.Trim().Length > 100)
                throw new DiaDiemValidationException("Từ khóa không được vượt quá 100 ký tự.");

            var results = diaDiems.Search(keyword, start, end, audienceSize);
            var activeAndLargeEnough = new List<DiaDiem>();
            foreach (var venue in results)
            {
                if (venue.trang_thai && venue.suc_chua >= audienceSize)
                    activeAndLargeEnough.Add(venue);
            }
            return activeAndLargeEnough;
        }

        public void SelectForDraft(AuthenticatedUser organizer, long draftId, long venueId)
        {
            if (organizer == null || organizer.AccountId <= 0 ||
                !string.Equals(organizer.RoleCode, "NGUOI_TO_CHUC", StringComparison.Ordinal))
                throw new DiaDiemValidationException("Tài khoản Người tổ chức không hợp lệ.");
            if (draftId <= 0)
                throw new DiaDiemValidationException("Chưa có mã hồ sơ nháp để gán địa điểm.");
            if (venueId <= 0)
                throw new DiaDiemValidationException("Địa điểm được chọn không hợp lệ.");

            diaDiems.AssignToDraft(draftId, organizer.AccountId, venueId);
        }
    }

    public sealed class DiaDiemValidationException : Exception
    {
        public DiaDiemValidationException(string message) : base(message) { }
    }
}

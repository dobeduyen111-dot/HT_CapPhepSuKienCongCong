using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class HoSoService
    {
        private readonly HoSoRepository repository;

        public HoSoService() : this(new HoSoRepository()) { }

        internal HoSoService(HoSoRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public IList<LoaiSuKien> GetActiveEventTypes()
        {
            return repository.GetActiveEventTypes();
        }

        public IList<HoSoCapPhep> GetDrafts(AuthenticatedUser organizer)
        {
            ValidateOrganizer(organizer);
            return repository.GetDrafts(organizer.AccountId);
        }

        public IList<HoSoCapPhep> GetAllByOrganizer(AuthenticatedUser organizer)
        {
            ValidateOrganizer(organizer);
            return repository.GetAllByOrganizer(organizer.AccountId);
        }

        public HoSoTheoDoiDetail GetTrackingDetail(AuthenticatedUser organizer, long caseId)
        {
            ValidateOrganizer(organizer);
            if (caseId <= 0) throw new HoSoValidationException("Mã hồ sơ không hợp lệ.");
            return repository.GetTrackingDetail(organizer.AccountId, caseId);
        }

        public string SupplementApplication(
            AuthenticatedUser organizer,
            long caseId,
            string noiDung,
            string nhanSuYTe,
            string nhanSuAnNinh,
            string phuongAnThoatHiem,
            string phuongAnXuLySuCo,
            string ghiChuGiaiTrinh,
            IList<HoSoFileUpload> newUploads)
        {
            ValidateOrganizer(organizer);
            if (caseId <= 0) throw new HoSoValidationException("Mã hồ sơ cần bổ sung không hợp lệ.");
            noiDung = Normalize(noiDung);
            nhanSuYTe = Normalize(nhanSuYTe);
            nhanSuAnNinh = Normalize(nhanSuAnNinh);
            phuongAnThoatHiem = Normalize(phuongAnThoatHiem);
            phuongAnXuLySuCo = Normalize(phuongAnXuLySuCo);
            ghiChuGiaiTrinh = Normalize(ghiChuGiaiTrinh);
            RequiredLength(noiDung, 4000, "Nội dung chương trình sau chỉnh sửa");
            OptionalLength(nhanSuYTe, 4000, "Phương án y tế");
            OptionalLength(nhanSuAnNinh, 4000, "Phương án an ninh");
            OptionalLength(phuongAnThoatHiem, 4000, "Phương án thoát hiểm/PCCC");
            OptionalLength(phuongAnXuLySuCo, 4000, "Phương án xử lý sự cố");
            OptionalLength(ghiChuGiaiTrinh, 4000, "Nội dung giải trình bổ sung");

            var copiedFiles = new List<string>();
            var cleanupFailures = new List<string>();
            bool persisted = false;
            try
            {
                var newDocuments = new List<TaiLieu>();
                if (newUploads != null)
                {
                    foreach (var upload in newUploads)
                    {
                        if (upload == null) continue;
                        if (upload.LoaiTaiLieu != "PHUONG_AN_AN_TOAN" &&
                            upload.LoaiTaiLieu != "GIAY_TO_LIEN_QUAN" &&
                            upload.LoaiTaiLieu != "TAI_LIEU_KHAC")
                            throw new HoSoValidationException("Loại tài liệu đính kèm không hợp lệ.");
                        string copied = CopyIntoManagedStorage(organizer.AccountId, caseId, upload.SourcePath);
                        copiedFiles.Add(copied);
                        string fileName = Path.GetFileName(upload.SourcePath);
                        if (fileName.Length > 250)
                            throw new HoSoValidationException("Tên tệp không được vượt quá 250 ký tự.");
                        newDocuments.Add(new TaiLieu
                        {
                            ma_ho_so = caseId,
                            loai_tai_lieu = upload.LoaiTaiLieu,
                            ten_tai_lieu = fileName,
                            duong_dan = copied
                        });
                    }
                }

                string nextStatus = repository.SupplementApplication(
                    organizer.AccountId,
                    caseId,
                    noiDung,
                    nhanSuYTe,
                    nhanSuAnNinh,
                    phuongAnThoatHiem,
                    phuongAnXuLySuCo,
                    ghiChuGiaiTrinh,
                    newDocuments);
                persisted = true;
                return nextStatus;
            }
            catch
            {
                if (!persisted)
                {
                    foreach (string copiedFile in copiedFiles)
                        TryDeleteManagedFile(copiedFile, cleanupFailures);
                }
                throw;
            }
        }

        public IList<ThongBao> GetNotifications(AuthenticatedUser user)
        {
            if (user == null || user.AccountId <= 0)
                throw new HoSoValidationException("Tài khoản không hợp lệ.");
            return repository.GetNotifications(user.AccountId);
        }

        public HoSoDraftData GetDraft(AuthenticatedUser organizer, long draftId)
        {
            ValidateOrganizer(organizer);
            if (draftId <= 0) throw new HoSoValidationException("Mã hồ sơ Nháp không hợp lệ.");
            return repository.GetDraft(organizer.AccountId, draftId);
        }

        public Task<HoSoNopResult> NopHoSoAsync(
            AuthenticatedUser organizer, long draftId, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateOrganizer(organizer);
            if (draftId <= 0) throw new HoSoValidationException("Mã hồ sơ cần nộp không hợp lệ.");
            return repository.SubmitAsync(organizer.AccountId, draftId, cancellationToken);
        }

        public HoSoDraftSaveResult SaveDraft(
            AuthenticatedUser organizer,
            HoSoDraftData draft,
            IList<HoSoFileUpload> newDocuments,
            string newProjectionAttachmentPath)
        {
            ValidateOrganizer(organizer);
            if (draft == null) throw new ArgumentNullException("draft");
            ValidateDraft(draft);
            if (!string.IsNullOrWhiteSpace(newProjectionAttachmentPath) && draft.quyen_trinh_chieu == null)
                throw new HoSoValidationException(
                    "Hãy nhập thông tin quyền trình chiếu trước khi đính kèm minh chứng.");

            var copiedFiles = new List<string>();
            string oldProjectionPath = draft.existing_projection_attachment_path ??
                (draft.quyen_trinh_chieu == null ? null : draft.quyen_trinh_chieu.tep_dinh_kem);
            string oldProjectionPathToRetain = draft.quyen_trinh_chieu == null
                ? null
                : draft.quyen_trinh_chieu.tep_dinh_kem;
            var oldDocuments = (draft.existing_documents_snapshot ?? draft.tai_lieu ??
                new List<TaiLieu>()).ToList();
            bool persisted = false;
            var cleanupFailures = new List<string>();

            try
            {
                if (!string.IsNullOrWhiteSpace(newProjectionAttachmentPath))
                {
                    string copied = CopyIntoManagedStorage(
                        organizer.AccountId, draft.ma_ho_so, newProjectionAttachmentPath);
                    copiedFiles.Add(copied);
                    if (draft.quyen_trinh_chieu == null)
                        draft.quyen_trinh_chieu = new QuyenTrinhChieu();
                    draft.quyen_trinh_chieu.tep_dinh_kem = copied;
                    oldProjectionPathToRetain = copied;
                }

                var documents = new List<TaiLieu>(draft.tai_lieu ?? new List<TaiLieu>());
                if (newDocuments != null)
                {
                    foreach (var upload in newDocuments)
                    {
                        if (upload == null) continue;
                        if (upload.LoaiTaiLieu != "PHUONG_AN_AN_TOAN" &&
                            upload.LoaiTaiLieu != "GIAY_TO_LIEN_QUAN" &&
                            upload.LoaiTaiLieu != "TAI_LIEU_KHAC")
                            throw new HoSoValidationException("Loại tài liệu đính kèm không hợp lệ.");
                        string copied = CopyIntoManagedStorage(
                            organizer.AccountId, draft.ma_ho_so, upload.SourcePath);
                        copiedFiles.Add(copied);
                        string fileName = Path.GetFileName(upload.SourcePath);
                        if (fileName.Length > 250)
                            throw new HoSoValidationException("Tên tệp không được vượt quá 250 ký tự.");
                        documents.Add(new TaiLieu
                        {
                            loai_tai_lieu = upload.LoaiTaiLieu,
                            ten_tai_lieu = fileName,
                            duong_dan = copied
                        });
                    }
                }

                long draftId = repository.SaveDraft(organizer.AccountId, draft, documents);
                persisted = true;
                draft.ma_ho_so = draftId;
                draft.tai_lieu = documents;
                draft.existing_documents_snapshot = documents.Select(item => new TaiLieu
                {
                    ma_tai_lieu = item.ma_tai_lieu,
                    ma_ho_so = draftId,
                    loai_tai_lieu = item.loai_tai_lieu,
                    ten_tai_lieu = item.ten_tai_lieu,
                    duong_dan = item.duong_dan,
                    ngay_tai_len = item.ngay_tai_len
                }).ToList();
                draft.existing_projection_attachment_path = oldProjectionPathToRetain;

                var retained = new HashSet<string>(
                    documents.Select(item => item.duong_dan), StringComparer.OrdinalIgnoreCase);
                foreach (var oldDocument in oldDocuments)
                    if (!retained.Contains(oldDocument.duong_dan))
                        TryDeleteManagedFile(oldDocument.duong_dan, cleanupFailures);

                if (!string.Equals(oldProjectionPath, oldProjectionPathToRetain, StringComparison.OrdinalIgnoreCase))
                    TryDeleteManagedFile(oldProjectionPath, cleanupFailures);

                return new HoSoDraftSaveResult(draftId, cleanupFailures);
            }
            catch
            {
                if (!persisted)
                    foreach (string copiedFile in copiedFiles)
                        TryDeleteManagedFile(copiedFile, cleanupFailures);
                throw;
            }
        }

        private static void ValidateDraft(HoSoDraftData draft)
        {
            draft.ten_don_vi_to_chuc = Normalize(draft.ten_don_vi_to_chuc);
            draft.ten_su_kien = Normalize(draft.ten_su_kien);
            if (draft.ma_loai_su_kien <= 0)
                throw new HoSoValidationException("Hãy chọn loại sự kiện.");
            RequiredLength(draft.ten_don_vi_to_chuc, 200, "Đơn vị tổ chức");
            RequiredLength(draft.ten_su_kien, 200, "Tên sự kiện");
            OptionalLength(draft.noi_dung, 4000, "Nội dung chương trình");
            if (draft.bat_dau.HasValue != draft.ket_thuc.HasValue)
                throw new HoSoValidationException("Hãy nhập đủ thời gian bắt đầu và kết thúc.");
            if (draft.bat_dau.HasValue && draft.bat_dau.Value >= draft.ket_thuc.Value)
                throw new HoSoValidationException("Thời gian kết thúc phải sau thời gian bắt đầu.");
            if (draft.quy_mo.HasValue && (draft.quy_mo.Value < 1 || draft.quy_mo.Value > 10000))
                throw new HoSoValidationException("Quy mô khách phải từ 1 đến 10.000 người.");

            if (!string.IsNullOrEmpty(draft.hinh_thuc_thuong_mai) &&
                draft.hinh_thuc_thuong_mai != "PHI_THUONG_MAI" &&
                draft.hinh_thuc_thuong_mai != "THUONG_MAI")
                throw new HoSoValidationException("Hình thức thương mại không hợp lệ.");
            if (!string.IsNullOrEmpty(draft.loai_ve) &&
                draft.loai_ve != "MIEN_PHI" && draft.loai_ve != "THU_MOI" && draft.loai_ve != "BAN_VE")
                throw new HoSoValidationException("Loại vé không hợp lệ.");
            if (draft.gia_ve.HasValue && draft.gia_ve.Value < 0)
                throw new HoSoValidationException("Giá vé không được âm.");
            if ((draft.so_luong_ve.HasValue && draft.so_luong_ve.Value < 0) ||
                (draft.quy_mo.HasValue && draft.so_luong_ve.HasValue &&
                 draft.so_luong_ve.Value > draft.quy_mo.Value))
                throw new HoSoValidationException("Số vé phải không âm và không vượt quá quy mô khách.");
            if (draft.loai_ve == "BAN_VE" && (!draft.gia_ve.HasValue || draft.gia_ve.Value <= 0))
                throw new HoSoValidationException("Vé bán cần có giá vé lớn hơn 0.");
            OptionalLength(draft.thong_tin_tai_tro, 2000, "Thông tin tài trợ");
            OptionalLength(draft.kenh_phat_hanh_ve, 2000, "Kênh phát hành vé");
            OptionalLength(draft.cach_kiem_soat_ve, 2000, "Cách kiểm soát vé");
            OptionalLength(draft.nhan_su_y_te, 4000, "Phương án y tế");
            OptionalLength(draft.nhan_su_an_ninh, 4000, "Phương án an ninh");
            OptionalLength(draft.phuong_an_thoat_hiem, 4000, "Phương án thoát hiểm/PCCC");
            OptionalLength(draft.phuong_an_xu_ly_su_co, 4000, "Phương án xử lý sự cố");

            var right = draft.quyen_trinh_chieu;
            if (right == null) return;
            right.don_vi_cap = Normalize(right.don_vi_cap);
            right.so_van_ban = Normalize(right.so_van_ban);
            right.loai_quyen = Normalize(right.loai_quyen);
            right.nguon_tin_hieu = Normalize(right.nguon_tin_hieu);
            right.noi_dung_duoc_cap = Normalize(right.noi_dung_duoc_cap);
            right.pham_vi_dia_diem = Normalize(right.pham_vi_dia_diem);
            bool hasRights = !string.IsNullOrWhiteSpace(right.don_vi_cap) ||
                             !string.IsNullOrWhiteSpace(right.so_van_ban) ||
                             !string.IsNullOrWhiteSpace(right.loai_quyen) ||
                             !string.IsNullOrWhiteSpace(right.nguon_tin_hieu) ||
                             !string.IsNullOrWhiteSpace(right.noi_dung_duoc_cap) ||
                             !string.IsNullOrWhiteSpace(right.pham_vi_dia_diem) ||
                             !string.IsNullOrWhiteSpace(right.tep_dinh_kem);
            if (!hasRights)
            {
                draft.quyen_trinh_chieu = null;
                return;
            }
            RequiredLength(right.don_vi_cap, 200, "Đơn vị cấp quyền trình chiếu");
            RequiredLength(right.loai_quyen, 100, "Loại quyền trình chiếu");
            RequiredLength(right.nguon_tin_hieu, 200, "Nguồn tín hiệu");
            RequiredLength(right.noi_dung_duoc_cap, 4000, "Nội dung được cấp quyền");
            RequiredLength(right.pham_vi_dia_diem, 2000, "Phạm vi địa điểm");
            if (right.hieu_luc_tu > right.hieu_luc_den)
                throw new HoSoValidationException("Ngày bắt đầu hiệu lực quyền phải trước hoặc bằng ngày kết thúc.");
        }

        private static string CopyIntoManagedStorage(long organizerId, long? draftId, string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                throw new HoSoValidationException("Không tìm thấy tệp đính kèm: " + (sourcePath ?? string.Empty));

            string extension = Path.GetExtension(sourcePath);
            if (extension.Length > 12)
                throw new HoSoValidationException("Phần mở rộng của tệp đính kèm không hợp lệ.");

            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ViewParty", "HoSoFiles", organizerId.ToString(CultureInfo.InvariantCulture),
                draftId.HasValue ? draftId.Value.ToString(CultureInfo.InvariantCulture) : "new");
            Directory.CreateDirectory(folder);
            string destination = Path.Combine(folder, Guid.NewGuid().ToString("N") + extension);
            if (destination.Length > 500)
                throw new HoSoValidationException("Đường dẫn lưu tài liệu quá dài.");
            File.Copy(sourcePath, destination, false);
            return destination;
        }

        private static void TryDeleteManagedFile(string path, IList<string> failures)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            try
            {
                string root = Path.GetFullPath(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ViewParty", "HoSoFiles")) + Path.DirectorySeparatorChar;
                string fullPath = Path.GetFullPath(path);
                if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase)) return;
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
                if (failures != null) failures.Add(Path.GetFileName(path));
            }
        }

        private static void ValidateOrganizer(AuthenticatedUser organizer)
        {
            if (organizer == null || organizer.AccountId <= 0 ||
                !string.Equals(organizer.RoleCode, "NGUOI_TO_CHUC", StringComparison.Ordinal))
                throw new HoSoValidationException("Tài khoản Người tổ chức không hợp lệ.");
        }

        private static string Normalize(string value) { return string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
        private static void RequiredLength(string value, int maxLength, string label)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new HoSoValidationException("Vui lòng nhập " + label.ToLowerInvariant() + ".");
            if (value.Length > maxLength)
                throw new HoSoValidationException(label + " không được vượt quá " + maxLength + " ký tự.");
        }
        private static void OptionalLength(string value, int maxLength, string label)
        {
            if (value != null && value.Length > maxLength)
                throw new HoSoValidationException(label + " không được vượt quá " + maxLength + " ký tự.");
        }
    }

    public sealed class HoSoValidationException : Exception
    {
        public HoSoValidationException(string message) : base(message) { }
    }

    public sealed class HoSoDraftSaveResult
    {
        public long DraftId { get; private set; }
        public IList<string> CleanupFailures { get; private set; }

        internal HoSoDraftSaveResult(long draftId, IList<string> cleanupFailures)
        {
            DraftId = draftId;
            CleanupFailures = cleanupFailures ?? new List<string>();
        }
    }
}

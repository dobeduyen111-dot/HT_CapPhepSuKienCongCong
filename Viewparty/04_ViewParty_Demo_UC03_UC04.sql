/*
VIEW PARTY | DU LIEU DEMO UC03-UC04 VA KIEM TRA TRUNG LICH
SQL Server 2019+; chi chay tren database demo ViewPartyDB_UC28.

Thu tu:
  1. Chay 01_ViewParty_CauTruc_16Bang (1).sql
  2. Chay 03_ViewParty_NghiepVu_SP_Function_Trigger.sql
  3. Dang ky it nhat mot tai khoan NGUOI_TO_CHUC bang ung dung.
  4. Chay file nay.

Khong tao tai khoan NGUOI_TO_CHUC. Tai khoan nguoi to chuc da dang ky
duoc dung lam chu so huu ho so demo chiem lich va ho so NHAP san sang nop.
Tai khoan nhan vien demo dung chung mat khau: Demo@2026!
Khong dung du lieu nay tren production.

Script them du lieu theo khoa ten/code de co the chay lai. Khong xoa hay
ghi de ho so/danh muc co san. Ho so demo chiem lich duoc tao o trang thai
DA_CAP_PHEP; ho so san sang nop duoc tao qua sp_TaoHoSoNhap.
*/
USE [ViewPartyDB_UC28];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() <> N'ViewPartyDB_UC28'
    THROW 52900, N'Script nay chi duoc phep chay tren ViewPartyDB_UC28.', 1;

IF OBJECT_ID(N'dbo.sp_TaoHoSoNhap', N'P') IS NULL
   OR OBJECT_ID(N'dbo.sp_LuuNoiDungHoSo', N'P') IS NULL
   OR OBJECT_ID(N'dbo.sp_ChonDiaDiemNhap', N'P') IS NULL
    THROW 52901, N'Chua co cac stored procedure UC03/UC28. Hay chay file 03 truoc.', 1;

IF NOT EXISTS (
    SELECT 1 FROM dbo.TAI_KHOAN
    WHERE vai_tro = 'NGUOI_TO_CHUC' AND trang_thai = 'HOAT_DONG'
)
    THROW 52902, N'Hay dang ky tai khoan NGUOI_TO_CHUC tren ung dung truoc khi chay script demo.', 1;

DECLARE @OrganizerId BIGINT;
SELECT TOP (1) @OrganizerId = ma_tai_khoan
FROM dbo.TAI_KHOAN
WHERE vai_tro = 'NGUOI_TO_CHUC' AND trang_thai = 'HOAT_DONG'
ORDER BY ma_tai_khoan;

DECLARE @OffsetDays INT;
DECLARE @CandidateStart DATETIMEOFFSET(0);
DECLARE @CandidateEnd DATETIMEOFFSET(0);

BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO dbo.DON_VI_LIEN_NGANH(ten_don_vi, linh_vuc)
    SELECT v.ten_don_vi, v.linh_vuc
    FROM (VALUES
        (N'Đơn vị An ninh Mẫu', N'An ninh trật tự'),
        (N'Đơn vị PCCC Mẫu', N'PCCC và an toàn')
    ) v(ten_don_vi, linh_vuc)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.DON_VI_LIEN_NGANH d
        WHERE d.ten_don_vi = v.ten_don_vi
    );

    INSERT INTO dbo.LOAI_SU_KIEN(ten_loai, mo_ta)
    SELECT v.ten_loai, v.mo_ta
    FROM (VALUES
        (N'View Party Esports', N'Xem chung giải đấu thể thao điện tử.'),
        (N'View Party thể thao', N'Xem chung sự kiện thể thao truyền thống.')
    ) v(ten_loai, mo_ta)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.LOAI_SU_KIEN l WHERE l.ten_loai = v.ten_loai
    );

    INSERT INTO dbo.DIA_DIEM(ten_dia_diem, dia_chi, suc_chua, trang_thai)
    SELECT v.ten_dia_diem, v.dia_chi, v.suc_chua, 1
    FROM (VALUES
        (N'VP Demo - Phòng hội nghị nhỏ', N'Địa chỉ giả lập 01', 1500),
        (N'VP Demo - Trung tâm Esports', N'Địa chỉ giả lập 02', 3500),
        (N'VP Demo - Nhà thi đấu', N'Địa chỉ giả lập 03', 6000),
        (N'VP Demo - Sân vận động', N'Địa chỉ giả lập 04', 10000),
        (N'VP Demo - Trung tâm đang giữ lịch', N'Địa chỉ giả lập 05', 5000)
    ) v(ten_dia_diem, dia_chi, suc_chua)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.DIA_DIEM d WHERE d.ten_dia_diem = v.ten_dia_diem
    );

    DECLARE @SecurityUnitId BIGINT, @FireUnitId BIGINT;
    SELECT @SecurityUnitId = ma_don_vi
    FROM dbo.DON_VI_LIEN_NGANH
    WHERE ten_don_vi = N'Đơn vị An ninh Mẫu' AND trang_thai = 1;
    SELECT @FireUnitId = ma_don_vi
    FROM dbo.DON_VI_LIEN_NGANH
    WHERE ten_don_vi = N'Đơn vị PCCC Mẫu' AND trang_thai = 1;

    IF @SecurityUnitId IS NULL OR @FireUnitId IS NULL
        THROW 52903, N'Don vi An ninh/PCCC mau khong ton tai hoac dang ngung hoat dong.', 1;

    DECLARE @EsportsId BIGINT, @SportsId BIGINT;
    SELECT @EsportsId = ma_loai_su_kien FROM dbo.LOAI_SU_KIEN
    WHERE ten_loai = N'View Party Esports' AND trang_thai = 1;
    SELECT @SportsId = ma_loai_su_kien FROM dbo.LOAI_SU_KIEN
    WHERE ten_loai = N'View Party thể thao' AND trang_thai = 1;

    IF @EsportsId IS NULL OR @SportsId IS NULL
        THROW 52904, N'Hai loai su kien demo phai ton tai va dang hoat dong.', 1;

    DECLARE @Hash VARCHAR(255) =
        '$2b$10$Ubd4FkvM6IKsug/9sUUmR.LhsIOiRqzykD8up2iw5NkOAFT2HToLy';
    DECLARE @DemoAccounts TABLE
    (
        username VARCHAR(80) NOT NULL,
        full_name NVARCHAR(150) NOT NULL,
        email VARCHAR(150) NOT NULL,
        role_code VARCHAR(30) NOT NULL,
        unit_id BIGINT NULL,
        approval_level SMALLINT NULL
    );

    INSERT INTO @DemoAccounts(username, full_name, email, role_code, unit_id, approval_level)
    VALUES
        ('vp_xuly_demo', N'Cán bộ thẩm định Demo', 'vp_xuly_demo@example.test', 'NGUOI_XU_LY', NULL, NULL),
        ('vp_liennganh_an_ninh', N'Cán bộ An ninh Demo', 'vp_liennganh_an_ninh@example.test', 'LIEN_NGANH', @SecurityUnitId, NULL),
        ('vp_liennganh_pccc', N'Cán bộ PCCC Demo', 'vp_liennganh_pccc@example.test', 'LIEN_NGANH', @FireUnitId, NULL),
        ('vp_pheduyet_demo', N'Người phê duyệt Demo', 'vp_pheduyet_demo@example.test', 'NGUOI_PHE_DUYET', NULL, 2),
        ('vp_quantri_demo', N'Quản trị viên Demo', 'vp_quantri_demo@example.test', 'QUAN_TRI_VIEN', NULL, NULL);

    INSERT INTO dbo.TAI_KHOAN
        (ma_don_vi, ten_dang_nhap, mat_khau_bam, ho_ten, email,
         so_dien_thoai, vai_tro, cap_tham_quyen, trang_thai)
    SELECT a.unit_id, a.username, @Hash, a.full_name, a.email,
           NULL, a.role_code, a.approval_level, 'HOAT_DONG'
    FROM @DemoAccounts a
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.TAI_KHOAN t WHERE t.ten_dang_nhap = a.username
    );

    IF EXISTS (
        SELECT 1
        FROM @DemoAccounts a
        LEFT JOIN dbo.TAI_KHOAN t ON t.ten_dang_nhap = a.username
        WHERE t.ma_tai_khoan IS NULL
           OR t.vai_tro <> a.role_code
           OR t.trang_thai <> 'HOAT_DONG'
           OR ISNULL(t.ma_don_vi, -1) <> ISNULL(a.unit_id, -1)
           OR ISNULL(t.cap_tham_quyen, -1) <> ISNULL(a.approval_level, -1)
    )
        THROW 52905, N'Tai khoan demo da ton tai nhung vai tro/don vi/trang thai khong khop; khong tu dong sua tai khoan.', 1;

    DECLARE @OfficerId BIGINT, @ApproverId BIGINT, @AdminId BIGINT;
    SELECT @OfficerId = ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap = 'vp_xuly_demo';
    SELECT @ApproverId = ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap = 'vp_pheduyet_demo';
    SELECT @AdminId = ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap = 'vp_quantri_demo';

    DECLARE @RuleSeeds TABLE
    (
        rule_code VARCHAR(60) NOT NULL,
        event_type_id BIGINT NOT NULL,
        rule_group NVARCHAR(80) NOT NULL,
        severity VARCHAR(20) NOT NULL,
        applies NVARCHAR(MAX) NOT NULL,
        passes NVARCHAR(MAX) NOT NULL
    );
    INSERT INTO @RuleSeeds(rule_code, event_type_id, rule_group, severity, applies, passes)
    VALUES
        ('DEMO_ESPORTS_VENUE', @EsportsId, N'Địa điểm', 'BAT_BUOC',
         N'{"field":"ma_dia_diem"}', N'{"check":"suc_chua_du_va_khong_trung_lich"}'),
        ('DEMO_ESPORTS_SAFETY', @EsportsId, N'An toàn', 'BAT_BUOC',
         N'{"field":"phuong_an_an_toan"}', N'{"check":"co_phuong_an_y_te_an_ninh_pccc"}'),
        ('DEMO_ESPORTS_INFO', @EsportsId, N'Thông tin công chúng', 'KHUYEN_NGHI',
         N'{"field":"noi_dung"}', N'{"check":"thong_tin_chuong_trinh_ro_rang"}'),
        ('DEMO_SPORTS_VENUE', @SportsId, N'Địa điểm', 'BAT_BUOC',
         N'{"field":"ma_dia_diem"}', N'{"check":"suc_chua_du_va_khong_trung_lich"}'),
        ('DEMO_SPORTS_SAFETY', @SportsId, N'An toàn', 'BAT_BUOC',
         N'{"field":"phuong_an_an_toan"}', N'{"check":"co_phuong_an_y_te_an_ninh_pccc"}'),
        ('DEMO_SPORTS_ACCESS', @SportsId, N'Tiếp cận', 'KHUYEN_NGHI',
         N'{"field":"noi_dung"}', N'{"check":"huong_dan_di_lai_de_tiep_can"}');

    INSERT INTO dbo.QUY_TAC
        (ma_quy_tac, phien_ban, ma_loai_su_kien, nhom, muc_do,
         dieu_kien_ap_dung, dieu_kien_dat, cho_phep_ngoai_le,
         cap_tham_quyen, hieu_luc_tu, trang_thai)
    SELECT r.rule_code, 1, r.event_type_id, r.rule_group, r.severity,
           r.applies, r.passes, 0, NULL,
           CONVERT(DATE, DATEADD(DAY, -1, GETDATE())), 1
    FROM @RuleSeeds r
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.QUY_TAC q
        WHERE q.ma_quy_tac = r.rule_code AND q.phien_ban = 1
    );

    IF EXISTS (
        SELECT 1 FROM @RuleSeeds r
        JOIN dbo.QUY_TAC q ON q.ma_quy_tac = r.rule_code AND q.phien_ban = 1
        WHERE q.ma_loai_su_kien <> r.event_type_id
           OR q.muc_do <> r.severity
    )
        THROW 52906, N'Ma quy tac demo da ton tai voi loai su kien/muc do khac.', 1;

    INSERT INTO dbo.CAU_HINH_LIEN_NGANH
        (ma_loai_su_kien, ma_don_vi, quy_tac_id, bat_buoc,
         so_ngay_phan_hoi, hieu_luc_tu, trang_thai)
    SELECT e.event_type_id, u.unit_id, NULL, 1, 7,
           CONVERT(DATE, DATEADD(DAY, -1, GETDATE())), 1
    FROM (VALUES (@EsportsId), (@SportsId)) e(event_type_id)
    CROSS JOIN (VALUES (@SecurityUnitId), (@FireUnitId)) u(unit_id)
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.CAU_HINH_LIEN_NGANH c
        WHERE c.ma_loai_su_kien = e.event_type_id
          AND c.ma_don_vi = u.unit_id
          AND c.bat_buoc = 1
          AND c.trang_thai = 1
          AND c.hieu_luc_tu <= CONVERT(DATE, GETDATE())
          AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den >= CONVERT(DATE, GETDATE()))
    );

    DECLARE @SmallVenueId BIGINT, @BusyVenueId BIGINT;
    SELECT @SmallVenueId = ma_dia_diem FROM dbo.DIA_DIEM
    WHERE ten_dia_diem = N'VP Demo - Phòng hội nghị nhỏ' AND trang_thai = 1;
    SELECT @BusyVenueId = ma_dia_diem FROM dbo.DIA_DIEM
    WHERE ten_dia_diem = N'VP Demo - Trung tâm đang giữ lịch' AND trang_thai = 1;

    IF @SmallVenueId IS NULL OR @BusyVenueId IS NULL
        THROW 52907, N'Thieu dia diem demo nho hoac dia diem giu lich.', 1;

    DECLARE @BusyEventId BIGINT, @BusyStart DATETIMEOFFSET(0), @BusyEnd DATETIMEOFFSET(0);
    SELECT @BusyEventId = ma_ho_so, @BusyStart = bat_dau, @BusyEnd = ket_thuc
    FROM dbo.HO_SO_CAP_PHEP
    WHERE ten_su_kien = N'VP-DEMO-CHIEM-LICH'
      AND ma_dia_diem = @BusyVenueId;

    IF @BusyEventId IS NULL
    BEGIN
        SET @OffsetDays = 45;
        WHILE @OffsetDays <= 365 AND @BusyStart IS NULL
        BEGIN
            SET @CandidateStart = DATEADD(HOUR, 18,
                DATEADD(DAY, @OffsetDays, CONVERT(DATETIMEOFFSET(0), CONVERT(DATE, SYSDATETIMEOFFSET()))));
            SET @CandidateEnd = DATEADD(HOUR, 4, @CandidateStart);

            IF NOT EXISTS (
                SELECT 1 FROM dbo.HO_SO_CAP_PHEP h
                WHERE h.ma_dia_diem = @BusyVenueId
                  AND h.trang_thai IN ('DA_NOP','DANG_KIEM_TRA','YEU_CAU_BO_SUNG',
                      'DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH','TONG_HOP_LIEN_NGANH',
                      'XU_LY_NGOAI_LE','CHO_PHE_DUYET','DA_CAP_PHEP')
                  AND h.bat_dau < @CandidateEnd AND h.ket_thuc > @CandidateStart
            )
            BEGIN
                SET @BusyStart = @CandidateStart;
                SET @BusyEnd = @CandidateEnd;
            END

            SET @OffsetDays = @OffsetDays + 7;
        END

        IF @BusyStart IS NULL
            THROW 52908, N'Khong tim duoc lich trong cho de tao ho so demo chiem lich trong 365 ngay toi.', 1;

        INSERT INTO dbo.HO_SO_CAP_PHEP
            (nguoi_to_chuc_id, ma_loai_su_kien, ma_dia_diem, trang_thai, ngay_nop,
             ten_su_kien, bat_dau, ket_thuc, quy_mo, noi_dung,
             hinh_thuc_thuong_mai, co_tai_tro, loai_ve, gia_ve, so_luong_ve,
             kenh_phat_hanh_ve, cach_kiem_soat_ve, nhan_su_y_te, nhan_su_an_ninh,
             phuong_an_thoat_hiem, phuong_an_xu_ly_su_co)
        VALUES
            (@OrganizerId, @EsportsId, @BusyVenueId, 'DA_CAP_PHEP',
             DATEADD(DAY, -5, @BusyStart), N'VP-DEMO-CHIEM-LICH',
             @BusyStart, @BusyEnd, 3000,
             N'Hồ sơ giả lập đã được cấp phép, dùng để kiểm tra lịch địa điểm.',
             'PHI_THUONG_MAI', 0, 'MIEN_PHI', 0, 3000,
             N'Đăng ký vé QR giả lập', N'Quét QR tại cổng',
             N'Tổ y tế trực sự kiện', N'Đội an ninh trực sự kiện',
             N'Lối thoát hiểm và sơ đồ sơ tán', N'Quy trình xử lý sự cố');

        SET @BusyEventId = CONVERT(BIGINT, SCOPE_IDENTITY());

        INSERT INTO dbo.LICH_SU_XU_LY
            (ma_ho_so, ma_tai_khoan, hanh_dong, tu_trang_thai, den_trang_thai, ket_qua)
        VALUES
            (@BusyEventId, @OrganizerId, N'Tạo hồ sơ demo chiếm lịch', NULL, 'DA_CAP_PHEP', N'Dữ liệu demo');
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.QUYEN_TRINH_CHIEU WHERE ma_ho_so = @BusyEventId)
        INSERT INTO dbo.QUYEN_TRINH_CHIEU
            (ma_ho_so, don_vi_cap, so_van_ban, loai_quyen, nguon_tin_hieu,
             noi_dung_duoc_cap, pham_vi_dia_diem, hieu_luc_tu, hieu_luc_den, tep_dinh_kem)
        VALUES
            (@BusyEventId, N'Đơn vị sở hữu nội dung Demo', N'VP-DEMO-BUSY',
             N'Quyền trình chiếu demo', N'Tín hiệu giả lập', N'View Party Esports',
             N'TOAN_QUOC', CONVERT(DATE, DATEADD(DAY, -1, @BusyStart)),
             CONVERT(DATE, DATEADD(DAY, 1, @BusyEnd)), N'demo/quyen-chiem-lich.pdf');

    IF NOT EXISTS (
        SELECT 1 FROM dbo.TAI_LIEU
        WHERE ma_ho_so = @BusyEventId AND loai_tai_lieu = 'PHUONG_AN_AN_TOAN'
    )
        INSERT INTO dbo.TAI_LIEU(ma_ho_so, loai_tai_lieu, ten_tai_lieu, duong_dan)
        VALUES (@BusyEventId, 'PHUONG_AN_AN_TOAN',
                N'Phương án an toàn Demo', N'demo/chiem-lich-an-toan.pdf');

    DECLARE @DraftTitle NVARCHAR(200) =
        CONCAT(N'VP-DEMO-UC03-TOCHUC-', CONVERT(NVARCHAR(30), @OrganizerId));
    DECLARE @DraftId BIGINT;
    SELECT @DraftId = ma_ho_so
    FROM dbo.HO_SO_CAP_PHEP
    WHERE nguoi_to_chuc_id = @OrganizerId AND ten_su_kien = @DraftTitle;

    IF @DraftId IS NULL
    BEGIN
        DECLARE @CandidateVenues TABLE
        (
            row_no INT IDENTITY(1,1) PRIMARY KEY,
            venue_id BIGINT NOT NULL
        );
        INSERT INTO @CandidateVenues(venue_id)
        SELECT ma_dia_diem
        FROM dbo.DIA_DIEM
        WHERE trang_thai = 1
          AND suc_chua >= 2500
          AND ten_dia_diem IN
              (N'VP Demo - Trung tâm Esports', N'VP Demo - Nhà thi đấu', N'VP Demo - Sân vận động')
        ORDER BY suc_chua, ten_dia_diem;

        DECLARE @VenueRow INT = 1, @VenueCount INT, @DraftVenueId BIGINT;
        DECLARE @DraftStart DATETIMEOFFSET(0) = NULL, @DraftEnd DATETIMEOFFSET(0);
        SELECT @VenueCount = COUNT(*) FROM @CandidateVenues;

        WHILE @VenueRow <= @VenueCount AND @DraftStart IS NULL
        BEGIN
            SELECT @DraftVenueId = venue_id
            FROM @CandidateVenues WHERE row_no = @VenueRow;

            SET @OffsetDays = 60;
            WHILE @OffsetDays <= 365 AND @DraftStart IS NULL
            BEGIN
                SET @CandidateStart = DATEADD(HOUR, 12,
                    DATEADD(DAY, @OffsetDays, CONVERT(DATETIMEOFFSET(0), CONVERT(DATE, SYSDATETIMEOFFSET()))));
                SET @CandidateEnd = DATEADD(HOUR, 4, @CandidateStart);

                IF NOT EXISTS (
                    SELECT 1 FROM dbo.HO_SO_CAP_PHEP h
                    WHERE h.ma_dia_diem = @DraftVenueId
                      AND h.trang_thai IN ('DA_NOP','DANG_KIEM_TRA','YEU_CAU_BO_SUNG',
                          'DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH','TONG_HOP_LIEN_NGANH',
                          'XU_LY_NGOAI_LE','CHO_PHE_DUYET','DA_CAP_PHEP')
                      AND h.bat_dau < @CandidateEnd AND h.ket_thuc > @CandidateStart
                )
                BEGIN
                    SET @DraftStart = @CandidateStart;
                    SET @DraftEnd = @CandidateEnd;
                END

                SET @OffsetDays = @OffsetDays + 1;
            END

            SET @VenueRow = @VenueRow + 1;
        END

        IF @DraftStart IS NULL
            THROW 52909, N'Khong tim duoc lich trong cho de tao ho so UC03 san sang nop.', 1;

        DECLARE @NewDraft TABLE(ma_ho_so BIGINT, trang_thai VARCHAR(30));
        INSERT INTO @NewDraft(ma_ho_so, trang_thai)
        EXEC dbo.sp_TaoHoSoNhap
            @NguoiToChucId = @OrganizerId,
            @MaLoaiSuKien = @EsportsId,
            @TenSuKien = @DraftTitle;
        SELECT @DraftId = ma_ho_so FROM @NewDraft;

        EXEC dbo.sp_LuuNoiDungHoSo
            @MaHoSo = @DraftId,
            @NguoiToChucId = @OrganizerId,
            @TenSuKien = @DraftTitle,
            @BatDau = @DraftStart,
            @KetThuc = @DraftEnd,
            @QuyMo = 2500,
            @NoiDung = N'Hồ sơ demo đầy đủ để thử luồng UC03 đến UC04.',
            @HinhThucThuongMai = 'PHI_THUONG_MAI',
            @LoaiVe = 'MIEN_PHI',
            @GiaVe = 0,
            @SoLuongVe = 2500,
            @KenhPhatHanhVe = N'Đăng ký vé QR giả lập',
            @CachKiemSoatVe = N'Quét mã QR tại cổng',
            @NhanSuYTe = N'Tổ y tế trực sự kiện',
            @NhanSuAnNinh = N'Đội an ninh trực sự kiện',
            @PhuongAnThoatHiem = N'Lối thoát hiểm và sơ đồ sơ tán',
            @PhuongAnXuLySuCo = N'Quy trình xử lý sự cố';

        EXEC dbo.sp_ChonDiaDiemNhap
            @MaHoSo = @DraftId,
            @NguoiToChucId = @OrganizerId,
            @MaDiaDiem = @DraftVenueId;

        INSERT INTO dbo.QUYEN_TRINH_CHIEU
            (ma_ho_so, don_vi_cap, so_van_ban, loai_quyen, nguon_tin_hieu,
             noi_dung_duoc_cap, pham_vi_dia_diem, hieu_luc_tu, hieu_luc_den, tep_dinh_kem)
        VALUES
            (@DraftId, N'Đơn vị sở hữu nội dung Demo', N'VP-DEMO-READY',
             N'Quyền trình chiếu demo', N'Tín hiệu giả lập', N'View Party Esports',
             N'TOAN_QUOC', CONVERT(DATE, DATEADD(DAY, -1, @DraftStart)),
             CONVERT(DATE, DATEADD(DAY, 1, @DraftEnd)), N'demo/quyen-uc03.pdf');

        INSERT INTO dbo.TAI_LIEU(ma_ho_so, loai_tai_lieu, ten_tai_lieu, duong_dan)
        VALUES (@DraftId, 'PHUONG_AN_AN_TOAN',
                N'Phương án an toàn Demo UC03', N'demo/uc03-an-toan.pdf');
    END

    DECLARE @CollisionTitle NVARCHAR(200) =
        CONCAT(N'VP-DEMO-TRUNG-LICH-TOCHUC-', CONVERT(NVARCHAR(30), @OrganizerId));
    DECLARE @CollisionDraftId BIGINT;
    SELECT @CollisionDraftId = ma_ho_so
    FROM dbo.HO_SO_CAP_PHEP
    WHERE nguoi_to_chuc_id = @OrganizerId AND ten_su_kien = @CollisionTitle;

    IF @CollisionDraftId IS NULL
    BEGIN
        DECLARE @CollisionDraft TABLE(ma_ho_so BIGINT, trang_thai VARCHAR(30));
        INSERT INTO @CollisionDraft(ma_ho_so, trang_thai)
        EXEC dbo.sp_TaoHoSoNhap
            @NguoiToChucId = @OrganizerId,
            @MaLoaiSuKien = @SportsId,
            @TenSuKien = @CollisionTitle;
        SELECT @CollisionDraftId = ma_ho_so FROM @CollisionDraft;

        EXEC dbo.sp_LuuNoiDungHoSo
            @MaHoSo = @CollisionDraftId,
            @NguoiToChucId = @OrganizerId,
            @TenSuKien = @CollisionTitle,
            @BatDau = @BusyStart,
            @KetThuc = @BusyEnd,
            @QuyMo = 2500,
            @NoiDung = N'Hồ sơ giả lập để xác nhận UC04 từ chối lịch đã bị chiếm.',
            @HinhThucThuongMai = 'PHI_THUONG_MAI',
            @LoaiVe = 'MIEN_PHI',
            @GiaVe = 0,
            @SoLuongVe = 2500,
            @KenhPhatHanhVe = N'Đăng ký vé QR giả lập',
            @CachKiemSoatVe = N'Quét mã QR tại cổng',
            @NhanSuYTe = N'Tổ y tế trực sự kiện',
            @NhanSuAnNinh = N'Đội an ninh trực sự kiện',
            @PhuongAnThoatHiem = N'Lối thoát hiểm và sơ đồ sơ tán',
            @PhuongAnXuLySuCo = N'Quy trình xử lý sự cố';

        EXEC dbo.sp_ChonDiaDiemNhap
            @MaHoSo = @CollisionDraftId,
            @NguoiToChucId = @OrganizerId,
            @MaDiaDiem = @BusyVenueId;

        INSERT INTO dbo.QUYEN_TRINH_CHIEU
            (ma_ho_so, don_vi_cap, so_van_ban, loai_quyen, nguon_tin_hieu,
             noi_dung_duoc_cap, pham_vi_dia_diem, hieu_luc_tu, hieu_luc_den, tep_dinh_kem)
        VALUES
            (@CollisionDraftId, N'Đơn vị sở hữu nội dung Demo', N'VP-DEMO-COLLISION',
             N'Quyền trình chiếu demo', N'Tín hiệu giả lập', N'View Party thể thao',
             N'TOAN_QUOC', CONVERT(DATE, DATEADD(DAY, -1, @BusyStart)),
             CONVERT(DATE, DATEADD(DAY, 1, @BusyEnd)), N'demo/trung-lich.pdf');

        INSERT INTO dbo.TAI_LIEU(ma_ho_so, loai_tai_lieu, ten_tai_lieu, duong_dan)
        VALUES (@CollisionDraftId, 'PHUONG_AN_AN_TOAN',
                N'Phương án an toàn Demo trùng lịch', N'demo/trung-lich-an-toan.pdf');
    END

    COMMIT TRANSACTION;

    SELECT N'Tai khoan nhan vien demo (mat khau chung: Demo@2026!)' AS nhom,
           ten_dang_nhap, vai_tro, trang_thai
    FROM dbo.TAI_KHOAN
    WHERE ten_dang_nhap IN
        ('vp_xuly_demo','vp_liennganh_an_ninh','vp_liennganh_pccc','vp_pheduyet_demo','vp_quantri_demo')
    ORDER BY vai_tro, ten_dang_nhap;

    SELECT l.ten_loai, q.ma_quy_tac, q.nhom, q.muc_do
    FROM dbo.QUY_TAC q
    JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien = q.ma_loai_su_kien
    WHERE q.ma_quy_tac LIKE 'DEMO[_]%'
    ORDER BY l.ten_loai, q.muc_do, q.ma_quy_tac;

    SELECT d.ten_dia_diem, d.suc_chua,
           CASE WHEN d.ma_dia_diem = @BusyVenueId THEN N'Có hồ sơ demo đang giữ lịch' ELSE N'Không gán lịch demo' END AS ghi_chu
    FROM dbo.DIA_DIEM d
    WHERE d.ten_dia_diem LIKE N'VP Demo - %'
    ORDER BY d.suc_chua;

    SELECT h.ma_ho_so, h.ten_su_kien, h.trang_thai,
           d.ten_dia_diem, h.quy_mo, h.bat_dau, h.ket_thuc,
           h.nguoi_to_chuc_id
    FROM dbo.HO_SO_CAP_PHEP h
    LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem = h.ma_dia_diem
    WHERE h.ma_ho_so IN (@BusyEventId, @DraftId, @CollisionDraftId)
    ORDER BY h.ma_ho_so;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

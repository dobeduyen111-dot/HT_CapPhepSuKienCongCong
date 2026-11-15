-- ====================================================================================
-- TỆP SQL: KHÔI PHỤC VÀ TẠO MỚI TÀI KHOẢN QUẢN TRỊ VIÊN (QUAN_TRI_VIEN)
-- Hệ thống: ViewParty Portal
-- Mật khẩu mặc định cho các tài khoản: Demo@2026!
-- ====================================================================================

-- 1. CHỌN CƠ SỞ DỮ LIỆU
IF DB_ID('ViewPartyDB_UC28') IS NOT NULL
    USE [ViewPartyDB_UC28];
ELSE IF DB_ID('ViewPartyDB') IS NOT NULL
    USE [ViewPartyDB];
ELSE
BEGIN
    PRINT N'Không tìm thấy CSDL ViewPartyDB_UC28 hoặc ViewPartyDB. Vui lòng kiểm tra lại tên database.';
    RETURN;
END
GO

SET NOCOUNT ON;
PRINT N'Đang thao tác trên database: ' + DB_NAME();

-- Mật khẩu băm BCrypt chuẩn cho mật khẩu: Demo@2026!
DECLARE @BcryptHash VARCHAR(255) = '$2b$10$Ubd4FkvM6IKsug/9sUUmR.LhsIOiRqzykD8up2iw5NkOAFT2HToLy';

-- ====================================================================================
-- PHẦN 1: KHÔI PHỤC LẠI QUYỀN QUẢN TRỊ VIÊN CHO TÀI KHOẢN MẪU 'vp_quantri'
-- (Phòng trường hợp bạn lỡ đổi vai trò của vp_quantri sang vai trò khác)
-- ====================================================================================
IF EXISTS (SELECT 1 FROM dbo.TAI_KHOAN WHERE ten_dang_nhap = 'vp_quantri')
BEGIN
    UPDATE dbo.TAI_KHOAN
    SET vai_tro = 'QUAN_TRI_VIEN',
        trang_thai = 'HOAT_DONG',
        ma_don_vi = NULL,
        cap_tham_quyen = NULL,
        mat_khau_bam = @BcryptHash
    WHERE ten_dang_nhap = 'vp_quantri';

    PRINT N'-> [1] ĐÃ KHÔI PHỤC tài khoản "vp_quantri" về vai trò QUAN_TRI_VIEN (Mật khẩu: Demo@2026!).';
END
ELSE
BEGIN
    INSERT INTO dbo.TAI_KHOAN (
        ma_don_vi, ten_dang_nhap, mat_khau_bam, ho_ten, email, so_dien_thoai, vai_tro, cap_tham_quyen, trang_thai
    )
    VALUES (
        NULL, 'vp_quantri', @BcryptHash, N'Quản trị viên Demo', 'vp_quantri@example.test', '0901111111', 'QUAN_TRI_VIEN', NULL, 'HOAT_DONG'
    );
    PRINT N'-> [1] ĐÃ TẠO MỚI tài khoản "vp_quantri" với vai trò QUAN_TRI_VIEN (Mật khẩu: Demo@2026!).';
END;

-- ====================================================================================
-- PHẦN 2: TẠO TÀI KHOẢN QUẢN TRỊ VIÊN MỚI 'vp_admin'
-- ====================================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.TAI_KHOAN WHERE ten_dang_nhap = 'vp_admin')
BEGIN
    INSERT INTO dbo.TAI_KHOAN (
        ma_don_vi, ten_dang_nhap, mat_khau_bam, ho_ten, email, so_dien_thoai, vai_tro, cap_tham_quyen, trang_thai
    )
    VALUES (
        NULL,
        'vp_admin',
        @BcryptHash,
        N'Quản trị viên Hệ thống',
        'vp_admin@viewparty.vn',
        '0902222222',
        'QUAN_TRI_VIEN',
        NULL,
        'HOAT_DONG'
    );
    PRINT N'-> [2] ĐÃ TẠO THÀNH CÔNG tài khoản quản trị mới: "vp_admin" (Mật khẩu: Demo@2026!).';
END
ELSE
BEGIN
    UPDATE dbo.TAI_KHOAN
    SET vai_tro = 'QUAN_TRI_VIEN',
        trang_thai = 'HOAT_DONG',
        mat_khau_bam = @BcryptHash,
        ma_don_vi = NULL,
        cap_tham_quyen = NULL
    WHERE ten_dang_nhap = 'vp_admin';
    PRINT N'-> [2] Tài khoản "vp_admin" đã tồn tại, ĐÃ CẬP NHẬT lại vai trò QUAN_TRI_VIEN và mật khẩu: Demo@2026!.';
END;

-- ====================================================================================
-- PHẦN 3: TẠO TÀI KHOẢN QUẢN TRỊ DỰ PHÒNG 'admin_backup'
-- ====================================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.TAI_KHOAN WHERE ten_dang_nhap = 'admin_backup')
BEGIN
    INSERT INTO dbo.TAI_KHOAN (
        ma_don_vi, ten_dang_nhap, mat_khau_bam, ho_ten, email, so_dien_thoai, vai_tro, cap_tham_quyen, trang_thai
    )
    VALUES (
        NULL,
        'admin_backup',
        @BcryptHash,
        N'Quản trị viên Dự phòng',
        'admin_backup@viewparty.vn',
        '0903333333',
        'QUAN_TRI_VIEN',
        NULL,
        'HOAT_DONG'
    );
    PRINT N'-> [3] ĐÃ TẠO THÀNH CÔNG tài khoản dự phòng: "admin_backup" (Mật khẩu: Demo@2026!).';
END
ELSE
BEGIN
    UPDATE dbo.TAI_KHOAN
    SET vai_tro = 'QUAN_TRI_VIEN',
        trang_thai = 'HOAT_DONG',
        mat_khau_bam = @BcryptHash,
        ma_don_vi = NULL,
        cap_tham_quyen = NULL
    WHERE ten_dang_nhap = 'admin_backup';
    PRINT N'-> [3] Tài khoản "admin_backup" đã được cập nhật vai trò QUAN_TRI_VIEN (Mật khẩu: Demo@2026!).';
END;

-- ====================================================================================
-- KIỂM TRA LẠI TẤT CẢ TÀI KHOẢN QUẢN TRỊ VIÊN HIỆN CÓ
-- ====================================================================================
PRINT N'------------------------------------------------------------------------------------';
PRINT N'DANH SÁCH TÀI KHOẢN CÓ QUYỀN QUẢN TRỊ VIÊN (QUAN_TRI_VIEN):';
SELECT 
    ma_tai_khoan AS [Mã TK],
    ten_dang_nhap AS [Tên đăng nhập],
    ho_ten AS [Họ và tên],
    email AS [Email],
    vai_tro AS [Vai trò],
    trang_thai AS [Trạng thái]
FROM dbo.TAI_KHOAN
WHERE vai_tro = 'QUAN_TRI_VIEN';
GO


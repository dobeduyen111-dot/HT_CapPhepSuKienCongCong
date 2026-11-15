/*
VIEW PARTY | SQL Server | FILE 01/03 - 16 BẢNG THƯỜNG (chính/phụ), CHỈ MỤC, MẪU BẢNG TẠM
Thiết kế đồng bộ sơ đồ Chen 16 thực thể (Hình 4.1).
- KHÔNG DROP bảng hoặc xóa dữ liệu sẵn có.
- Tạo riêng DB ViewPartyDB_UC28 để không làm thay đổi DB hiện tại.
- Chạy lần lượt File 01 -> File 02 -> File 03 trong SSMS.
- IF OBJECT_ID... chỉ tránh tạo bảng trùng, không phải công cụ migration.
- Hồ sơ NHAP được phép thiếu các trường chưa hoàn thiện và MaDiaDiem = NULL.
- UC28 chỉ chọn địa điểm; UC04 mới nộp và khóa lịch qua sp_NopHoSo.
- Quy mô 1000–10000 và hạn >=30 ngày là quy tắc mô phỏng của đồ án.
- Bảng tạm #tmp_tong_hop_tham_dinh chỉ tồn tại trong UC15, không nằm trong ERD.
- Backend chỉ nên được cấp quyền EXECUTE Stored Procedure; tránh UPDATE thẳng bảng hồ sơ.
*/

IF DB_ID(N'ViewPartyDB_UC28') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [ViewPartyDB_UC28]');
END;
GO
USE [ViewPartyDB_UC28];
GO

-- 01. Đơn vị liên ngành
IF OBJECT_ID(N'dbo.DON_VI_LIEN_NGANH', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DON_VI_LIEN_NGANH (
        ma_don_vi BIGINT IDENTITY(1,1) NOT NULL,
        ten_don_vi NVARCHAR(200) NOT NULL,
        linh_vuc NVARCHAR(150) NOT NULL,
        trang_thai BIT NOT NULL CONSTRAINT DF_DVLN_trang_thai DEFAULT (1),
        CONSTRAINT PK_DON_VI_LIEN_NGANH PRIMARY KEY (ma_don_vi)
    );
END;
GO

-- 02. Loại sự kiện
IF OBJECT_ID(N'dbo.LOAI_SU_KIEN', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LOAI_SU_KIEN (
        ma_loai_su_kien BIGINT IDENTITY(1,1) NOT NULL,
        ten_loai NVARCHAR(150) NOT NULL,
        mo_ta NVARCHAR(MAX) NULL,
        trang_thai BIT NOT NULL CONSTRAINT DF_LSK_trang_thai DEFAULT (1),
        CONSTRAINT PK_LOAI_SU_KIEN PRIMARY KEY (ma_loai_su_kien),
        CONSTRAINT UQ_LOAI_SU_KIEN_ten_loai UNIQUE (ten_loai)
    );
END;
GO

-- 03. Địa điểm tổ chức
IF OBJECT_ID(N'dbo.DIA_DIEM', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DIA_DIEM (
        ma_dia_diem BIGINT IDENTITY(1,1) NOT NULL,
        ten_dia_diem NVARCHAR(200) NOT NULL,
        dia_chi NVARCHAR(300) NOT NULL,
        suc_chua INT NOT NULL,
        trang_thai BIT NOT NULL CONSTRAINT DF_DD_trang_thai DEFAULT (1),
        CONSTRAINT PK_DIA_DIEM PRIMARY KEY (ma_dia_diem),
        CONSTRAINT CK_DIA_DIEM_suc_chua CHECK (suc_chua > 0)
    );
END;
GO

-- 04. Tài khoản, 5 vai trò, cấp thẩm quyền người duyệt
IF OBJECT_ID(N'dbo.TAI_KHOAN', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TAI_KHOAN (
        ma_tai_khoan BIGINT IDENTITY(1,1) NOT NULL,
        ma_don_vi BIGINT NULL,
        ten_dang_nhap VARCHAR(80) NOT NULL,
        mat_khau_bam VARCHAR(255) NOT NULL,
        ho_ten NVARCHAR(150) NOT NULL,
        email VARCHAR(150) NOT NULL,
        so_dien_thoai VARCHAR(20) NULL,
        vai_tro VARCHAR(30) NOT NULL,
        cap_tham_quyen SMALLINT NULL,
        trang_thai VARCHAR(25) NOT NULL CONSTRAINT DF_TK_trang_thai DEFAULT ('HOAT_DONG'),
        ngay_tao DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_TK_ngay_tao DEFAULT (SYSDATETIMEOFFSET()),
        CONSTRAINT PK_TAI_KHOAN PRIMARY KEY (ma_tai_khoan),
        CONSTRAINT UQ_TAI_KHOAN_ten_dang_nhap UNIQUE (ten_dang_nhap),
        CONSTRAINT UQ_TAI_KHOAN_email UNIQUE (email),
        CONSTRAINT FK_TK_DVLN FOREIGN KEY (ma_don_vi) REFERENCES dbo.DON_VI_LIEN_NGANH(ma_don_vi),
        CONSTRAINT CK_TK_vai_tro CHECK (vai_tro IN (
            'NGUOI_TO_CHUC', 'NGUOI_XU_LY', 'LIEN_NGANH',
            'NGUOI_PHE_DUYET', 'QUAN_TRI_VIEN'
        )),
        CONSTRAINT CK_TK_cap CHECK (cap_tham_quyen IS NULL OR cap_tham_quyen > 0),
        CONSTRAINT CK_TK_trang_thai CHECK (trang_thai IN (
            'CHO_XAC_MINH', 'HOAT_DONG', 'TAM_KHOA', 'NGUNG_HOAT_DONG'
        )),
        CONSTRAINT CK_TK_lien_nganh CHECK (vai_tro <> 'LIEN_NGANH' OR ma_don_vi IS NOT NULL),
        CONSTRAINT CK_TK_phe_duyet CHECK (vai_tro <> 'NGUOI_PHE_DUYET' OR cap_tham_quyen IS NOT NULL)
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.TAI_KHOAN')
      AND name = N'UX_TAI_KHOAN_so_dien_thoai'
)
BEGIN
    CREATE UNIQUE INDEX UX_TAI_KHOAN_so_dien_thoai
        ON dbo.TAI_KHOAN(so_dien_thoai)
        WHERE so_dien_thoai IS NOT NULL;
END;
GO

-- 05. Hồ sơ cấp phép (gộp sự kiện, phương án vé và an toàn)
IF OBJECT_ID(N'dbo.HO_SO_CAP_PHEP', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HO_SO_CAP_PHEP (
        ma_ho_so BIGINT IDENTITY(1,1) NOT NULL,
        nguoi_to_chuc_id BIGINT NOT NULL,
        nguoi_xu_ly_hien_tai_id BIGINT NULL,
        ma_loai_su_kien BIGINT NOT NULL,
        ma_dia_diem BIGINT NULL,
        trang_thai VARCHAR(30) NOT NULL CONSTRAINT DF_HS_trang_thai DEFAULT ('NHAP'),
        ngay_tao DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_HS_ngay_tao DEFAULT (SYSDATETIMEOFFSET()),
        ngay_nop DATETIMEOFFSET(0) NULL,
        ten_don_vi_to_chuc NVARCHAR(200) NULL,
        ten_su_kien NVARCHAR(200) NULL,
        bat_dau DATETIMEOFFSET(0) NULL,
        ket_thuc DATETIMEOFFSET(0) NULL,
        quy_mo INT NULL,
        noi_dung NVARCHAR(MAX) NULL,
        hinh_thuc_thuong_mai VARCHAR(20) NULL,
        co_tai_tro BIT NOT NULL CONSTRAINT DF_HS_co_tai_tro DEFAULT (0),
        thong_tin_tai_tro NVARCHAR(MAX) NULL,
        loai_ve VARCHAR(20) NULL,
        gia_ve DECIMAL(14,2) NULL,
        so_luong_ve INT NULL,
        kenh_phat_hanh_ve NVARCHAR(MAX) NULL,
        cach_kiem_soat_ve NVARCHAR(MAX) NULL,
        nhan_su_y_te NVARCHAR(MAX) NULL,
        nhan_su_an_ninh NVARCHAR(MAX) NULL,
        phuong_an_thoat_hiem NVARCHAR(MAX) NULL,
        phuong_an_xu_ly_su_co NVARCHAR(MAX) NULL,
        ly_do_tu_choi NVARCHAR(MAX) NULL,
        CONSTRAINT PK_HO_SO_CAP_PHEP PRIMARY KEY (ma_ho_so),
        CONSTRAINT FK_HS_nguoi_to_chuc FOREIGN KEY (nguoi_to_chuc_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT FK_HS_nguoi_xu_ly FOREIGN KEY (nguoi_xu_ly_hien_tai_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT FK_HS_loai_su_kien FOREIGN KEY (ma_loai_su_kien) REFERENCES dbo.LOAI_SU_KIEN(ma_loai_su_kien),
        CONSTRAINT FK_HS_dia_diem FOREIGN KEY (ma_dia_diem) REFERENCES dbo.DIA_DIEM(ma_dia_diem),
        CONSTRAINT CK_HS_thoi_gian CHECK (bat_dau IS NULL OR ket_thuc IS NULL OR bat_dau < ket_thuc),
        CONSTRAINT CK_HS_quy_mo CHECK (trang_thai = 'NHAP' OR quy_mo IS NULL OR quy_mo BETWEEN 1000 AND 10000),
        CONSTRAINT CK_HS_thuong_mai CHECK (hinh_thuc_thuong_mai IN ('PHI_THUONG_MAI', 'THUONG_MAI')),
        CONSTRAINT CK_HS_loai_ve CHECK (loai_ve IN ('MIEN_PHI', 'THU_MOI', 'BAN_VE')),
        CONSTRAINT CK_HS_gia_ve CHECK (gia_ve IS NULL OR gia_ve >= 0),
        CONSTRAINT CK_HS_ban_ve CHECK (loai_ve IS NULL OR loai_ve <> 'BAN_VE' OR (gia_ve IS NOT NULL AND gia_ve > 0)),
        CONSTRAINT CK_HS_so_ve CHECK (so_luong_ve IS NULL OR (so_luong_ve >= 0 AND (quy_mo IS NULL OR so_luong_ve <= quy_mo))),
        CONSTRAINT CK_HS_trang_thai CHECK (trang_thai IN (
            'NHAP', 'DA_NOP', 'DANG_KIEM_TRA', 'YEU_CAU_BO_SUNG',
            'DANG_THAM_DINH', 'CHO_Y_KIEN_LIEN_NGANH', 'TONG_HOP_LIEN_NGANH',
            'XU_LY_NGOAI_LE', 'CHO_PHE_DUYET', 'DA_CAP_PHEP', 'TU_CHOI'
        )),
        CONSTRAINT CK_HS_dia_diem_khi_nop CHECK (trang_thai = 'NHAP' OR ma_dia_diem IS NOT NULL),
        CONSTRAINT CK_HS_ngay_nop CHECK (trang_thai = 'NHAP' OR ngay_nop IS NOT NULL),
        CONSTRAINT CK_HS_du_du_lieu_khi_nop CHECK (
             trang_thai = 'NHAP' OR (
               ten_su_kien IS NOT NULL AND bat_dau IS NOT NULL AND ket_thuc IS NOT NULL
               AND quy_mo IS NOT NULL AND noi_dung IS NOT NULL
               AND hinh_thuc_thuong_mai IS NOT NULL AND loai_ve IS NOT NULL
               AND so_luong_ve IS NOT NULL AND kenh_phat_hanh_ve IS NOT NULL
               AND cach_kiem_soat_ve IS NOT NULL AND nhan_su_y_te IS NOT NULL
               AND nhan_su_an_ninh IS NOT NULL AND phuong_an_thoat_hiem IS NOT NULL
               AND phuong_an_xu_ly_su_co IS NOT NULL
             )
        )
    );
END;
GO

IF COL_LENGTH(N'dbo.HO_SO_CAP_PHEP', N'ten_don_vi_to_chuc') IS NULL
    ALTER TABLE dbo.HO_SO_CAP_PHEP ADD ten_don_vi_to_chuc NVARCHAR(200) NULL;
GO

-- 06. Quyền trình chiếu (một hồ sơ có nhiều minh chứng)
IF OBJECT_ID(N'dbo.QUYEN_TRINH_CHIEU', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QUYEN_TRINH_CHIEU (
        ma_quyen BIGINT IDENTITY(1,1) NOT NULL,
        ma_ho_so BIGINT NOT NULL,
        don_vi_cap NVARCHAR(200) NOT NULL,
        so_van_ban NVARCHAR(100) NULL,
        loai_quyen NVARCHAR(100) NOT NULL,
        nguon_tin_hieu NVARCHAR(200) NOT NULL,
        noi_dung_duoc_cap NVARCHAR(MAX) NOT NULL,
        pham_vi_dia_diem NVARCHAR(MAX) NOT NULL,
        hieu_luc_tu DATE NOT NULL,
        hieu_luc_den DATE NOT NULL,
        tep_dinh_kem NVARCHAR(500) NULL,
        trang_thai BIT NOT NULL CONSTRAINT DF_QTC_trang_thai DEFAULT (1),
        CONSTRAINT PK_QUYEN_TRINH_CHIEU PRIMARY KEY (ma_quyen),
        CONSTRAINT FK_QTC_HS FOREIGN KEY (ma_ho_so) REFERENCES dbo.HO_SO_CAP_PHEP(ma_ho_so),
        CONSTRAINT CK_QTC_hieu_luc CHECK (hieu_luc_tu <= hieu_luc_den)
    );
END;
GO

-- 07. Tài liệu đính kèm
IF OBJECT_ID(N'dbo.TAI_LIEU', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TAI_LIEU (
        ma_tai_lieu BIGINT IDENTITY(1,1) NOT NULL,
        ma_ho_so BIGINT NOT NULL,
        loai_tai_lieu VARCHAR(50) NOT NULL,
        ten_tai_lieu NVARCHAR(250) NOT NULL,
        duong_dan NVARCHAR(500) NOT NULL,
        ngay_tai_len DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_TL_ngay_tai_len DEFAULT (SYSDATETIMEOFFSET()),
        CONSTRAINT PK_TAI_LIEU PRIMARY KEY (ma_tai_lieu),
        CONSTRAINT FK_TL_HS FOREIGN KEY (ma_ho_so) REFERENCES dbo.HO_SO_CAP_PHEP(ma_ho_so)
    );
END;
GO

-- 08. Quy tắc thẩm định, lưu theo phiên bản, điều kiện JSON
IF OBJECT_ID(N'dbo.QUY_TAC', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QUY_TAC (
        quy_tac_id BIGINT IDENTITY(1,1) NOT NULL,
        ma_quy_tac VARCHAR(60) NOT NULL,
        phien_ban INT NOT NULL,
        ma_loai_su_kien BIGINT NOT NULL,
        nhom NVARCHAR(80) NOT NULL,
        muc_do VARCHAR(20) NOT NULL,
        dieu_kien_ap_dung NVARCHAR(MAX) NOT NULL,
        dieu_kien_dat NVARCHAR(MAX) NOT NULL,
        cho_phep_ngoai_le BIT NOT NULL CONSTRAINT DF_QT_cho_ngoai_le DEFAULT (0),
        cap_tham_quyen SMALLINT NULL,
        hieu_luc_tu DATE NOT NULL,
        hieu_luc_den DATE NULL,
        trang_thai BIT NOT NULL CONSTRAINT DF_QT_trang_thai DEFAULT (1),
        CONSTRAINT PK_QUY_TAC PRIMARY KEY (quy_tac_id),
        CONSTRAINT UQ_QT_ma_phien_ban UNIQUE (ma_quy_tac, phien_ban),
        CONSTRAINT FK_QT_loai FOREIGN KEY (ma_loai_su_kien) REFERENCES dbo.LOAI_SU_KIEN(ma_loai_su_kien),
        CONSTRAINT CK_QT_phien_ban CHECK (phien_ban > 0),
        CONSTRAINT CK_QT_muc_do CHECK (muc_do IN ('BAT_BUOC', 'KHUYEN_NGHI')),
        CONSTRAINT CK_QT_json_ap_dung CHECK (ISJSON(dieu_kien_ap_dung) = 1),
        CONSTRAINT CK_QT_json_dat CHECK (ISJSON(dieu_kien_dat) = 1),
        CONSTRAINT CK_QT_cap CHECK (cap_tham_quyen IS NULL OR cap_tham_quyen > 0),
        CONSTRAINT CK_QT_ngoai_le_cap CHECK (cho_phep_ngoai_le = 0 OR cap_tham_quyen IS NOT NULL),
        CONSTRAINT CK_QT_hieu_luc CHECK (hieu_luc_den IS NULL OR hieu_luc_tu <= hieu_luc_den)
    );
END;
GO

-- 09. Cấu hình đơn vị lấy ý kiến theo loại sự kiện
IF OBJECT_ID(N'dbo.CAU_HINH_LIEN_NGANH', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CAU_HINH_LIEN_NGANH (
        ma_cau_hinh BIGINT IDENTITY(1,1) NOT NULL,
        ma_loai_su_kien BIGINT NOT NULL,
        ma_don_vi BIGINT NOT NULL,
        quy_tac_id BIGINT NULL,
        bat_buoc BIT NOT NULL CONSTRAINT DF_CHLN_bat_buoc DEFAULT (1),
        so_ngay_phan_hoi INT NOT NULL,
        hieu_luc_tu DATE NOT NULL,
        hieu_luc_den DATE NULL,
        trang_thai BIT NOT NULL CONSTRAINT DF_CHLN_trang_thai DEFAULT (1),
        CONSTRAINT PK_CAU_HINH_LIEN_NGANH PRIMARY KEY (ma_cau_hinh),
        CONSTRAINT FK_CHLN_loai FOREIGN KEY (ma_loai_su_kien) REFERENCES dbo.LOAI_SU_KIEN(ma_loai_su_kien),
        CONSTRAINT FK_CHLN_don_vi FOREIGN KEY (ma_don_vi) REFERENCES dbo.DON_VI_LIEN_NGANH(ma_don_vi),
        CONSTRAINT FK_CHLN_quy_tac FOREIGN KEY (quy_tac_id) REFERENCES dbo.QUY_TAC(quy_tac_id),
        CONSTRAINT CK_CHLN_ngay CHECK (so_ngay_phan_hoi > 0),
        CONSTRAINT CK_CHLN_hieu_luc CHECK (hieu_luc_den IS NULL OR hieu_luc_tu <= hieu_luc_den)
    );
END;
GO

-- 10. Kết quả kiểm tra (giữ đúng ID phiên bản quy tắc)
IF OBJECT_ID(N'dbo.KET_QUA_KIEM_TRA', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.KET_QUA_KIEM_TRA (
        ma_ket_qua BIGINT IDENTITY(1,1) NOT NULL,
        ma_ho_so BIGINT NOT NULL,
        quy_tac_id BIGINT NOT NULL,
        nguoi_kiem_tra_id BIGINT NOT NULL,
        ket_qua VARCHAR(20) NOT NULL,
        gia_tri_thuc_te NVARCHAR(MAX) NULL,
        ghi_chu NVARCHAR(MAX) NULL,
        ngay_danh_gia DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_KQ_ngay DEFAULT (SYSDATETIMEOFFSET()),
        CONSTRAINT PK_KET_QUA_KIEM_TRA PRIMARY KEY (ma_ket_qua),
        CONSTRAINT UQ_KQ_HS_quy_tac UNIQUE (ma_ho_so, quy_tac_id),
        CONSTRAINT FK_KQ_HS FOREIGN KEY (ma_ho_so) REFERENCES dbo.HO_SO_CAP_PHEP(ma_ho_so),
        CONSTRAINT FK_KQ_quy_tac FOREIGN KEY (quy_tac_id) REFERENCES dbo.QUY_TAC(quy_tac_id),
        CONSTRAINT FK_KQ_nguoi FOREIGN KEY (nguoi_kiem_tra_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT CK_KQ_ket_qua CHECK (ket_qua IN ('DAT', 'KHONG_DAT', 'CANH_BAO')),
        CONSTRAINT CK_KQ_json CHECK (gia_tri_thuc_te IS NULL OR ISJSON(gia_tri_thuc_te) = 1)
    );
END;
GO

-- 11. Yêu cầu và phản hồi ý kiến liên ngành
IF OBJECT_ID(N'dbo.YEU_CAU_Y_KIEN', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YEU_CAU_Y_KIEN (
        ma_yeu_cau BIGINT IDENTITY(1,1) NOT NULL,
        ma_ho_so BIGINT NOT NULL,
        ma_don_vi BIGINT NOT NULL,
        ma_cau_hinh BIGINT NOT NULL,
        nguoi_gui_id BIGINT NOT NULL,
        ngay_gui DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_YCYK_ngay_gui DEFAULT (SYSDATETIMEOFFSET()),
        han_phan_hoi DATETIMEOFFSET(0) NOT NULL,
        trang_thai VARCHAR(20) NOT NULL CONSTRAINT DF_YCYK_trang_thai DEFAULT ('CHO_PHAN_HOI'),
        nguoi_phan_hoi_id BIGINT NULL,
        ket_luan VARCHAR(30) NULL,
        noi_dung_phan_hoi NVARCHAR(MAX) NULL,
        kien_nghi NVARCHAR(MAX) NULL,
        ngay_phan_hoi DATETIMEOFFSET(0) NULL,
        tep_dinh_kem NVARCHAR(500) NULL,
        CONSTRAINT PK_YEU_CAU_Y_KIEN PRIMARY KEY (ma_yeu_cau),
        CONSTRAINT UQ_YCYK_ho_so_don_vi UNIQUE (ma_ho_so, ma_don_vi),
        CONSTRAINT FK_YCYK_HS FOREIGN KEY (ma_ho_so) REFERENCES dbo.HO_SO_CAP_PHEP(ma_ho_so),
        CONSTRAINT FK_YCYK_don_vi FOREIGN KEY (ma_don_vi) REFERENCES dbo.DON_VI_LIEN_NGANH(ma_don_vi),
        CONSTRAINT FK_YCYK_cau_hinh FOREIGN KEY (ma_cau_hinh) REFERENCES dbo.CAU_HINH_LIEN_NGANH(ma_cau_hinh),
        CONSTRAINT FK_YCYK_nguoi_gui FOREIGN KEY (nguoi_gui_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT FK_YCYK_nguoi_phan_hoi FOREIGN KEY (nguoi_phan_hoi_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT CK_YCYK_han CHECK (han_phan_hoi >= ngay_gui),
        CONSTRAINT CK_YCYK_trang_thai CHECK (trang_thai IN ('CHO_PHAN_HOI', 'QUA_HAN', 'DA_PHAN_HOI')),
        CONSTRAINT CK_YCYK_ket_luan CHECK (ket_luan IS NULL OR ket_luan IN (
            'PHU_HOP', 'PHU_HOP_CO_DIEU_KIEN', 'KHONG_PHU_HOP'
        )),
        CONSTRAINT CK_YCYK_da_phan_hoi CHECK (trang_thai <> 'DA_PHAN_HOI' OR
            (nguoi_phan_hoi_id IS NOT NULL AND ket_luan IS NOT NULL AND
             noi_dung_phan_hoi IS NOT NULL AND ngay_phan_hoi IS NOT NULL))
    );
END;
GO

-- 12. Đề xuất và quyết định ngoại lệ
IF OBJECT_ID(N'dbo.NGOAI_LE', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NGOAI_LE (
        ma_ngoai_le BIGINT IDENTITY(1,1) NOT NULL,
        ma_ho_so BIGINT NOT NULL,
        ma_ket_qua BIGINT NOT NULL,
        nguoi_de_xuat_id BIGINT NOT NULL,
        nguoi_phe_duyet_id BIGINT NULL,
        ly_do NVARCHAR(MAX) NOT NULL,
        minh_chung NVARCHAR(MAX) NOT NULL,
        bien_phap_bu_dap NVARCHAR(MAX) NOT NULL,
        cap_tham_quyen_yeu_cau SMALLINT NOT NULL,
        trang_thai VARCHAR(20) NOT NULL CONSTRAINT DF_NL_trang_thai DEFAULT ('CHO_DUYET'),
        ngay_de_xuat DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_NL_ngay DEFAULT (SYSDATETIMEOFFSET()),
        ngay_quyet_dinh DATETIMEOFFSET(0) NULL,
        ghi_chu_quyet_dinh NVARCHAR(MAX) NULL,
        CONSTRAINT PK_NGOAI_LE PRIMARY KEY (ma_ngoai_le),
        CONSTRAINT UQ_NL_ma_ket_qua UNIQUE (ma_ket_qua),
        CONSTRAINT FK_NL_HS FOREIGN KEY (ma_ho_so) REFERENCES dbo.HO_SO_CAP_PHEP(ma_ho_so),
        CONSTRAINT FK_NL_KQ FOREIGN KEY (ma_ket_qua) REFERENCES dbo.KET_QUA_KIEM_TRA(ma_ket_qua),
        CONSTRAINT FK_NL_nguoi_de_xuat FOREIGN KEY (nguoi_de_xuat_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT FK_NL_nguoi_phe_duyet FOREIGN KEY (nguoi_phe_duyet_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT CK_NL_cap CHECK (cap_tham_quyen_yeu_cau > 0),
        CONSTRAINT CK_NL_trang_thai CHECK (trang_thai IN ('CHO_DUYET', 'DA_CHAP_THUAN', 'BI_TU_CHOI')),
        CONSTRAINT CK_NL_tach_nguoi CHECK (nguoi_phe_duyet_id IS NULL OR nguoi_phe_duyet_id <> nguoi_de_xuat_id),
        CONSTRAINT CK_NL_quyet_dinh CHECK (trang_thai = 'CHO_DUYET' OR
            (nguoi_phe_duyet_id IS NOT NULL AND ngay_quyet_dinh IS NOT NULL))
    );
END;
GO

-- 13. Giấy phép (mỗi hồ sơ tối đa một giấy phép)
IF OBJECT_ID(N'dbo.GIAY_PHEP', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.GIAY_PHEP (
        ma_giay_phep BIGINT IDENTITY(1,1) NOT NULL,
        ma_ho_so BIGINT NOT NULL,
        nguoi_phe_duyet_id BIGINT NOT NULL,
        so_giay_phep VARCHAR(80) NOT NULL,
        ngay_cap DATE NOT NULL,
        hieu_luc_tu DATE NOT NULL,
        hieu_luc_den DATE NOT NULL,
        dieu_kien_kem_theo NVARCHAR(MAX) NULL,
        CONSTRAINT PK_GIAY_PHEP PRIMARY KEY (ma_giay_phep),
        CONSTRAINT UQ_GP_ma_ho_so UNIQUE (ma_ho_so),
        CONSTRAINT UQ_GP_so_giay_phep UNIQUE (so_giay_phep),
        CONSTRAINT FK_GP_HS FOREIGN KEY (ma_ho_so) REFERENCES dbo.HO_SO_CAP_PHEP(ma_ho_so),
        CONSTRAINT FK_GP_nguoi FOREIGN KEY (nguoi_phe_duyet_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT CK_GP_hieu_luc CHECK (hieu_luc_tu <= hieu_luc_den)
    );
END;
GO

-- 14. Lịch sử xử lý hồ sơ
IF OBJECT_ID(N'dbo.LICH_SU_XU_LY', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LICH_SU_XU_LY (
        ma_lich_su BIGINT IDENTITY(1,1) NOT NULL,
        ma_ho_so BIGINT NOT NULL,
        ma_tai_khoan BIGINT NULL,
        hanh_dong NVARCHAR(100) NOT NULL,
        tu_trang_thai VARCHAR(30) NULL,
        den_trang_thai VARCHAR(30) NULL,
        ket_qua NVARCHAR(100) NULL,
        ghi_chu NVARCHAR(MAX) NULL,
        thoi_diem DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_LSXL_thoi_diem DEFAULT (SYSDATETIMEOFFSET()),
        CONSTRAINT PK_LICH_SU_XU_LY PRIMARY KEY (ma_lich_su),
        CONSTRAINT FK_LSXL_HS FOREIGN KEY (ma_ho_so) REFERENCES dbo.HO_SO_CAP_PHEP(ma_ho_so),
        CONSTRAINT FK_LSXL_TK FOREIGN KEY (ma_tai_khoan) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan)
    );
END;
GO

-- 15. Nhật ký cấu hình (dữ liệu trước/sau dưới dạng JSON)
IF OBJECT_ID(N'dbo.NHAT_KY_CAU_HINH', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NHAT_KY_CAU_HINH (
        ma_nhat_ky BIGINT IDENTITY(1,1) NOT NULL,
        ma_tai_khoan BIGINT NOT NULL,
        doi_tuong NVARCHAR(80) NOT NULL,
        khoa_doi_tuong NVARCHAR(120) NULL,
        hanh_dong NVARCHAR(50) NOT NULL,
        gia_tri_truoc NVARCHAR(MAX) NULL,
        gia_tri_sau NVARCHAR(MAX) NULL,
        thoi_diem DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_NKCH_thoi_diem DEFAULT (SYSDATETIMEOFFSET()),
        CONSTRAINT PK_NHAT_KY_CAU_HINH PRIMARY KEY (ma_nhat_ky),
        CONSTRAINT FK_NKCH_TK FOREIGN KEY (ma_tai_khoan) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT CK_NKCH_json_truoc CHECK (gia_tri_truoc IS NULL OR ISJSON(gia_tri_truoc) = 1),
        CONSTRAINT CK_NKCH_json_sau CHECK (gia_tri_sau IS NULL OR ISJSON(gia_tri_sau) = 1)
    );
END;
GO

-- 16. Thông báo trạng thái hệ thống
IF OBJECT_ID(N'dbo.THONG_BAO', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.THONG_BAO (
        ma_thong_bao BIGINT IDENTITY(1,1) NOT NULL,
        nguoi_nhan_id BIGINT NOT NULL,
        ma_ho_so BIGINT NULL,
        loai_thong_bao VARCHAR(40) NOT NULL,
        trang_thai VARCHAR(20) NOT NULL CONSTRAINT DF_TB_trang_thai DEFAULT ('CHO_GUI'),
        noi_dung NVARCHAR(MAX) NOT NULL,
        thoi_diem_tao DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_TB_thoi_diem_tao DEFAULT (SYSDATETIMEOFFSET()),
        thoi_diem_gui DATETIMEOFFSET(0) NULL,
        so_lan_thu INT NOT NULL CONSTRAINT DF_TB_so_lan_thu DEFAULT (0),
        CONSTRAINT PK_THONG_BAO PRIMARY KEY (ma_thong_bao),
        CONSTRAINT FK_TB_nguoi_nhan FOREIGN KEY (nguoi_nhan_id) REFERENCES dbo.TAI_KHOAN(ma_tai_khoan),
        CONSTRAINT FK_TB_HS FOREIGN KEY (ma_ho_so) REFERENCES dbo.HO_SO_CAP_PHEP(ma_ho_so),
        CONSTRAINT CK_TB_trang_thai CHECK (trang_thai IN ('CHO_GUI', 'DA_GUI', 'GUI_THAT_BAI')),
        CONSTRAINT CK_TB_so_lan_thu CHECK (so_lan_thu >= 0)
    );
END;
GO

-- Chỉ mục hỗ trợ tra cứu, thẩm định và kiểm tra xung đột lịch.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HS_dia_diem_lich' AND object_id = OBJECT_ID(N'dbo.HO_SO_CAP_PHEP'))
    CREATE INDEX IX_HS_dia_diem_lich ON dbo.HO_SO_CAP_PHEP(ma_dia_diem, bat_dau, ket_thuc) INCLUDE (trang_thai);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HS_nguoi_to_chuc' AND object_id = OBJECT_ID(N'dbo.HO_SO_CAP_PHEP'))
    CREATE INDEX IX_HS_nguoi_to_chuc ON dbo.HO_SO_CAP_PHEP(nguoi_to_chuc_id, ngay_tao DESC);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HS_nguoi_xu_ly' AND object_id = OBJECT_ID(N'dbo.HO_SO_CAP_PHEP'))
    CREATE INDEX IX_HS_nguoi_xu_ly ON dbo.HO_SO_CAP_PHEP(nguoi_xu_ly_hien_tai_id, trang_thai);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_QTC_ho_so' AND object_id = OBJECT_ID(N'dbo.QUYEN_TRINH_CHIEU'))
    CREATE INDEX IX_QTC_ho_so ON dbo.QUYEN_TRINH_CHIEU(ma_ho_so);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TL_ho_so' AND object_id = OBJECT_ID(N'dbo.TAI_LIEU'))
    CREATE INDEX IX_TL_ho_so ON dbo.TAI_LIEU(ma_ho_so);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CHLN_loai' AND object_id = OBJECT_ID(N'dbo.CAU_HINH_LIEN_NGANH'))
    CREATE INDEX IX_CHLN_loai ON dbo.CAU_HINH_LIEN_NGANH(ma_loai_su_kien, trang_thai, ma_don_vi);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_YCYK_don_vi' AND object_id = OBJECT_ID(N'dbo.YEU_CAU_Y_KIEN'))
    CREATE INDEX IX_YCYK_don_vi ON dbo.YEU_CAU_Y_KIEN(ma_don_vi, trang_thai, han_phan_hoi);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_NL_ho_so' AND object_id = OBJECT_ID(N'dbo.NGOAI_LE'))
    CREATE INDEX IX_NL_ho_so ON dbo.NGOAI_LE(ma_ho_so, trang_thai);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LSXL_ho_so' AND object_id = OBJECT_ID(N'dbo.LICH_SU_XU_LY'))
    CREATE INDEX IX_LSXL_ho_so ON dbo.LICH_SU_XU_LY(ma_ho_so, thoi_diem DESC);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TB_nguoi_nhan' AND object_id = OBJECT_ID(N'dbo.THONG_BAO'))
    CREATE INDEX IX_TB_nguoi_nhan ON dbo.THONG_BAO(nguoi_nhan_id, trang_thai, thoi_diem_tao DESC);
GO



-- ============================================================
-- BẢNG TẠM (LOCAL TEMP TABLE, KHÔNG PHẢI BẢNG SỐ 17)
-- Thí dụ độc lập để kiểm tra cách tạo bảng tạm trong SQL Server.
-- Bảng #... tự mất khi đóng phiên; thủ tục sp_TongHopThamDinh
-- trong File 03 cũng tạo bảng này và tự DROP sau khi dùng.
-- ============================================================
CREATE TABLE #tmp_tong_hop_tham_dinh (
    ma_ho_so BIGINT NOT NULL PRIMARY KEY,
    so_quy_tac INT NOT NULL,
    so_khong_dat_bat_buoc INT NOT NULL,
    so_canh_bao INT NOT NULL
);
DROP TABLE #tmp_tong_hop_tham_dinh;
GO

SELECT COUNT(*) AS SoBangThuong
FROM sys.tables WHERE schema_id = SCHEMA_ID('dbo'); -- Dự kiến 16
GO

/*
FILE 03/03 - STORED PROCEDURE + FUNCTION + TRIGGER + VIEW
SQL Server 2019+ (CREATE OR ALTER). CHẠY SAU FILE 01 VÀ 02.
Các chức năng DB thực hiện xác nhận/quy trình, web cần bổ sung auth/session,
quản lý tệp đính kèm, gửi email/Telegram và đánh giá quy tắc JSON trong Service.
Tất cả giới hạn 1000-10000 người và 30 ngày là GIẢ ĐỊNH ĐỒ ÁN.
Backend dùng tài khoản SQL với quyền EXECUTE, tránh cho quyền UPDATE bảng trực tiếp.
*/
USE [ViewPartyDB_UC28];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ================================================================
-- A. FUNCTIONS: tra cứu và điều kiện nghiệp vụ
-- ================================================================
CREATE OR ALTER FUNCTION dbo.fn_DuSucChua(@MaDiaDiem BIGINT,@QuyMo INT)
RETURNS BIT
AS
BEGIN
 DECLARE @KQ BIT=0;
 IF EXISTS (SELECT 1 FROM dbo.DIA_DIEM WHERE ma_dia_diem=@MaDiaDiem
            AND trang_thai=1 AND @QuyMo>0 AND suc_chua>=@QuyMo)
   SET @KQ=1;
 RETURN @KQ;
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_TrungLichDiaDiem
 (@MaDiaDiem BIGINT,@BatDau DATETIMEOFFSET(0),@KetThuc DATETIMEOFFSET(0),@BoQuaMaHoSo BIGINT)
RETURNS BIT
AS
BEGIN
 DECLARE @KQ BIT=0;
 IF @BatDau IS NULL OR @KetThuc IS NULL OR @BatDau>=@KetThuc RETURN 1;
 IF EXISTS (
  SELECT 1 FROM dbo.HO_SO_CAP_PHEP h
  WHERE h.ma_dia_diem=@MaDiaDiem
    AND (@BoQuaMaHoSo IS NULL OR h.ma_ho_so<>@BoQuaMaHoSo)
    AND h.trang_thai IN ('DA_NOP','DANG_KIEM_TRA','YEU_CAU_BO_SUNG',
      'DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH','TONG_HOP_LIEN_NGANH',
      'XU_LY_NGOAI_LE','CHO_PHE_DUYET','DA_CAP_PHEP')
    AND h.bat_dau<@KetThuc AND h.ket_thuc>@BatDau
 ) SET @KQ=1;
 RETURN @KQ;
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_DuYKienBatBuoc(@MaHoSo BIGINT)
RETURNS BIT
AS
BEGIN
 DECLARE @KQ BIT=0,@Loai BIGINT,@Ngay DATE;
 SELECT @Loai=ma_loai_su_kien,@Ngay=CONVERT(DATE,ngay_nop)
 FROM dbo.HO_SO_CAP_PHEP WHERE ma_ho_so=@MaHoSo;
 IF @Ngay IS NULL RETURN 0;
 IF NOT EXISTS (SELECT 1 FROM dbo.CAU_HINH_LIEN_NGANH c
    WHERE c.ma_loai_su_kien=@Loai AND c.bat_buoc=1 AND c.trang_thai=1
      AND c.hieu_luc_tu<=@Ngay AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den>=@Ngay))
  RETURN 0;
 IF NOT EXISTS (
    SELECT 1 FROM dbo.CAU_HINH_LIEN_NGANH c
    WHERE c.ma_loai_su_kien=@Loai AND c.bat_buoc=1 AND c.trang_thai=1
      AND c.hieu_luc_tu<=@Ngay AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den>=@Ngay)
      AND NOT EXISTS (
        SELECT 1 FROM dbo.CAU_HINH_LIEN_NGANH newer
        WHERE newer.ma_loai_su_kien=c.ma_loai_su_kien
          AND newer.ma_don_vi=c.ma_don_vi AND newer.bat_buoc=1 AND newer.trang_thai=1
          AND newer.hieu_luc_tu<=@Ngay
          AND (newer.hieu_luc_den IS NULL OR newer.hieu_luc_den>=@Ngay)
          AND (newer.hieu_luc_tu>c.hieu_luc_tu OR
               (newer.hieu_luc_tu=c.hieu_luc_tu AND newer.ma_cau_hinh>c.ma_cau_hinh))
      )
      AND NOT EXISTS (
        SELECT 1 FROM dbo.YEU_CAU_Y_KIEN y
        WHERE y.ma_ho_so=@MaHoSo AND y.ma_cau_hinh=c.ma_cau_hinh
          AND y.ma_don_vi=c.ma_don_vi AND y.trang_thai='DA_PHAN_HOI'
          AND y.ket_luan IS NOT NULL
      )
 ) SET @KQ=1;
 RETURN @KQ;
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_DuDieuKienPheDuyet(@MaHoSo BIGINT)
RETURNS BIT
AS
BEGIN
 DECLARE @KQ BIT=0;
 IF NOT EXISTS (SELECT 1 FROM dbo.HO_SO_CAP_PHEP
                 WHERE ma_ho_so=@MaHoSo AND trang_thai='CHO_PHE_DUYET') RETURN 0;
 IF dbo.fn_DuYKienBatBuoc(@MaHoSo)=0 RETURN 0;
 IF NOT EXISTS(SELECT 1 FROM dbo.KET_QUA_KIEM_TRA WHERE ma_ho_so=@MaHoSo) RETURN 0;
 -- Không phê duyệt khi đơn vị liên ngành kết luận KHÔNG PHÙ HỢP.
 IF EXISTS(SELECT 1 FROM dbo.YEU_CAU_Y_KIEN
   WHERE ma_ho_so=@MaHoSo AND ket_luan='KHONG_PHU_HOP') RETURN 0;
 -- Không được thiếu bản đánh giá của bất kỳ quy tắc bắt buộc đang áp dụng.
 IF EXISTS(
   SELECT 1 FROM dbo.QUY_TAC r JOIN dbo.HO_SO_CAP_PHEP h
     ON h.ma_loai_su_kien=r.ma_loai_su_kien
   WHERE h.ma_ho_so=@MaHoSo AND r.trang_thai=1 AND r.muc_do='BAT_BUOC'
     AND r.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop)
     AND (r.hieu_luc_den IS NULL OR r.hieu_luc_den>=CONVERT(DATE,h.ngay_nop))
     AND NOT EXISTS(SELECT 1 FROM dbo.KET_QUA_KIEM_TRA k
                    WHERE k.ma_ho_so=@MaHoSo AND k.quy_tac_id=r.quy_tac_id)
 ) RETURN 0;
 IF EXISTS (
   SELECT 1 FROM dbo.KET_QUA_KIEM_TRA k
   JOIN dbo.QUY_TAC q ON q.quy_tac_id=k.quy_tac_id
   WHERE k.ma_ho_so=@MaHoSo AND q.muc_do='BAT_BUOC' AND k.ket_qua<>'DAT'
     AND NOT EXISTS(SELECT 1 FROM dbo.NGOAI_LE n
                    WHERE n.ma_ket_qua=k.ma_ket_qua AND n.trang_thai='DA_CHAP_THUAN')
 ) RETURN 0;
 SET @KQ=1;
 RETURN @KQ;
END;
GO

-- ================================================================
-- B. VIEWS: dữ liệu tổng hợp cho web/WinForms (không phải bảng)
-- ================================================================
CREATE OR ALTER VIEW dbo.v_ho_so_tong_quan AS
 SELECT h.ma_ho_so,h.ten_su_kien,h.trang_thai,h.ngay_tao,h.ngay_nop,
        h.bat_dau,h.ket_thuc,h.quy_mo,h.nguoi_to_chuc_id,
        t.ho_ten AS nguoi_to_chuc,l.ten_loai,d.ten_dia_diem,d.suc_chua
 FROM dbo.HO_SO_CAP_PHEP h
 JOIN dbo.TAI_KHOAN t ON t.ma_tai_khoan=h.nguoi_to_chuc_id
 JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien=h.ma_loai_su_kien
 LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem=h.ma_dia_diem;
GO
CREATE OR ALTER VIEW dbo.v_ho_so_cho_phe_duyet AS
 SELECT h.ma_ho_so,h.ten_su_kien,h.trang_thai,h.bat_dau,h.quy_mo,
        t.ho_ten AS nguoi_to_chuc,d.ten_dia_diem
 FROM dbo.HO_SO_CAP_PHEP h
 JOIN dbo.TAI_KHOAN t ON t.ma_tai_khoan=h.nguoi_to_chuc_id
 LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem=h.ma_dia_diem
 WHERE h.trang_thai='CHO_PHE_DUYET';
GO
CREATE OR ALTER VIEW dbo.v_theo_doi_lien_nganh AS
 SELECT h.ma_ho_so,h.ten_su_kien,y.ma_yeu_cau,d.ten_don_vi,
        c.bat_buoc,y.ngay_gui,y.han_phan_hoi,y.trang_thai,y.ket_luan,y.ngay_phan_hoi
 FROM dbo.YEU_CAU_Y_KIEN y
 JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=y.ma_ho_so
 JOIN dbo.DON_VI_LIEN_NGANH d ON d.ma_don_vi=y.ma_don_vi
 JOIN dbo.CAU_HINH_LIEN_NGANH c ON c.ma_cau_hinh=y.ma_cau_hinh;
GO

-- ================================================================
-- C. QUẢN LÝ TÀI KHOẢN (UC01, UC02, UC20)
-- ================================================================
CREATE OR ALTER PROCEDURE dbo.sp_DangKyNguoiToChuc
 @TenDangNhap VARCHAR(80), @MatKhauBam VARCHAR(255),
 @HoTen NVARCHAR(150), @Email VARCHAR(150), @SoDienThoai VARCHAR(20)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @TenDangNhap=LTRIM(RTRIM(@TenDangNhap));
 SET @HoTen=LTRIM(RTRIM(@HoTen));
 SET @Email=LOWER(LTRIM(RTRIM(@Email)));
 SET @SoDienThoai=NULLIF(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
   LTRIM(RTRIM(@SoDienThoai)),' ',''),'-',''),'(',''),')',''),'.',''),'');
 IF NULLIF(LTRIM(RTRIM(@TenDangNhap)),'') IS NULL
    OR NULLIF(LTRIM(RTRIM(@HoTen)),N'') IS NULL
    OR NULLIF(LTRIM(RTRIM(@Email)),'') IS NULL
    OR LEN(@MatKhauBam)<50
   THROW 52001,N'Thông tin đăng ký hoặc chuỗi băm mật khẩu không hợp lệ.',1;
 IF EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ten_dang_nhap=@TenDangNhap)
   THROW 52002,N'Tên đăng nhập đã được sử dụng.',1;
 IF EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE LOWER(email)=@Email)
   THROW 52006,N'Email đã được sử dụng.',1;
 IF @SoDienThoai IS NOT NULL AND EXISTS(
   SELECT 1 FROM dbo.TAI_KHOAN WHERE so_dien_thoai IS NOT NULL
     AND REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
       LTRIM(RTRIM(so_dien_thoai)),' ',''),'-',''),'(',''),')',''),'.','')=@SoDienThoai)
   THROW 52007,N'Số điện thoại đã được sử dụng.',1;
 INSERT dbo.TAI_KHOAN(ten_dang_nhap,mat_khau_bam,ho_ten,email,so_dien_thoai,vai_tro,trang_thai)
 VALUES(@TenDangNhap,@MatKhauBam,@HoTen,@Email,@SoDienThoai,'NGUOI_TO_CHUC','HOAT_DONG');
 SELECT CONVERT(BIGINT,SCOPE_IDENTITY()) AS ma_tai_khoan, 'HOAT_DONG' AS trang_thai;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LayTaiKhoanDangNhap @TenDangNhap VARCHAR(80)
AS
BEGIN
 SET NOCOUNT ON;
 SELECT ma_tai_khoan,ten_dang_nhap,mat_khau_bam,ho_ten,vai_tro,cap_tham_quyen,
        trang_thai,ma_don_vi
 FROM dbo.TAI_KHOAN WHERE ten_dang_nhap=@TenDangNhap;
 -- Mật khẩu được xác minh trong BACKEND (BCrypt.Verify), tuyệt đối không trả hash ra FE.
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_QuanTriTaiKhoan
 @AdminId BIGINT,@MaTaiKhoan BIGINT,@VaiTro VARCHAR(30)=NULL,@TrangThai VARCHAR(25)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
    WHERE ma_tai_khoan=@AdminId AND vai_tro='QUAN_TRI_VIEN' AND trang_thai='HOAT_DONG')
    THROW 52003,N'Không có quyền quản trị tài khoản.',1;
 IF @AdminId=@MaTaiKhoan AND @TrangThai IN ('TAM_KHOA','NGUNG_HOAT_DONG')
    THROW 52004,N'Không tự khóa tài khoản quản trị của mình.',1;
 DECLARE @Truoc NVARCHAR(MAX), @Sau NVARCHAR(MAX);
 BEGIN TRY
 BEGIN TRANSACTION;
 SELECT @Truoc=(SELECT vai_tro,trang_thai,cap_tham_quyen
                 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@MaTaiKhoan
                 FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
 IF @Truoc IS NULL THROW 52005,N'Tài khoản không tồn tại.',1;
 UPDATE dbo.TAI_KHOAN SET vai_tro=COALESCE(@VaiTro,vai_tro),
    trang_thai=COALESCE(@TrangThai,trang_thai)
 WHERE ma_tai_khoan=@MaTaiKhoan;
 SELECT @Sau=(SELECT vai_tro,trang_thai,cap_tham_quyen
               FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@MaTaiKhoan
               FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
 INSERT dbo.NHAT_KY_CAU_HINH(ma_tai_khoan,doi_tuong,khoa_doi_tuong,hanh_dong,gia_tri_truoc,gia_tri_sau)
 VALUES(@AdminId,N'TAI_KHOAN',CONVERT(NVARCHAR(30),@MaTaiKhoan),N'CAP_NHAT',@Truoc,@Sau);
 COMMIT TRANSACTION;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

-- ================================================================
-- D. HỒ SƠ: tạo, cập nhật Nháp, UC28, UC04, theo dõi (UC03-06,28)
-- ================================================================
CREATE OR ALTER PROCEDURE dbo.sp_TaoHoSoNhap
 @NguoiToChucId BIGINT,@MaLoaiSuKien BIGINT,@TenSuKien NVARCHAR(200)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@NguoiToChucId
  AND vai_tro='NGUOI_TO_CHUC' AND trang_thai='HOAT_DONG')
  THROW 52011,N'Tài khoản không được phép tạo hồ sơ.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.LOAI_SU_KIEN WHERE ma_loai_su_kien=@MaLoaiSuKien AND trang_thai=1)
  THROW 52012,N'Loại sự kiện không hoạt động.',1;
 BEGIN TRANSACTION;
 INSERT dbo.HO_SO_CAP_PHEP(nguoi_to_chuc_id,ma_loai_su_kien,ten_su_kien)
 VALUES(@NguoiToChucId,@MaLoaiSuKien,@TenSuKien);
 DECLARE @MaHoSo BIGINT=CONVERT(BIGINT,SCOPE_IDENTITY());
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,den_trang_thai)
 VALUES(@MaHoSo,@NguoiToChucId,N'Tạo hồ sơ','NHAP');
 COMMIT TRANSACTION;
 SELECT @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LuuNoiDungHoSo
 @MaHoSo BIGINT,@NguoiToChucId BIGINT,
 @TenSuKien NVARCHAR(200)=NULL,@BatDau DATETIMEOFFSET(0)=NULL,@KetThuc DATETIMEOFFSET(0)=NULL,
 @QuyMo INT=NULL,@NoiDung NVARCHAR(MAX)=NULL,@HinhThucThuongMai VARCHAR(20)=NULL,
 @LoaiVe VARCHAR(20)=NULL,@GiaVe DECIMAL(14,2)=NULL,@SoLuongVe INT=NULL,
 @KenhPhatHanhVe NVARCHAR(MAX)=NULL,@CachKiemSoatVe NVARCHAR(MAX)=NULL,
 @NhanSuYTe NVARCHAR(MAX)=NULL,@NhanSuAnNinh NVARCHAR(MAX)=NULL,
 @PhuongAnThoatHiem NVARCHAR(MAX)=NULL,@PhuongAnXuLySuCo NVARCHAR(MAX)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 -- Để tránh làm lịch đang giữ bị lệch, chỉ sửa các trường này khi NHAP.
 UPDATE dbo.HO_SO_CAP_PHEP
 SET ten_su_kien=COALESCE(@TenSuKien,ten_su_kien),
 bat_dau=COALESCE(@BatDau,bat_dau),ket_thuc=COALESCE(@KetThuc,ket_thuc),
 quy_mo=COALESCE(@QuyMo,quy_mo),noi_dung=COALESCE(@NoiDung,noi_dung),
 hinh_thuc_thuong_mai=COALESCE(@HinhThucThuongMai,hinh_thuc_thuong_mai),
 loai_ve=COALESCE(@LoaiVe,loai_ve),gia_ve=COALESCE(@GiaVe,gia_ve),
 so_luong_ve=COALESCE(@SoLuongVe,so_luong_ve),
 kenh_phat_hanh_ve=COALESCE(@KenhPhatHanhVe,kenh_phat_hanh_ve),
 cach_kiem_soat_ve=COALESCE(@CachKiemSoatVe,cach_kiem_soat_ve),
 nhan_su_y_te=COALESCE(@NhanSuYTe,nhan_su_y_te),
 nhan_su_an_ninh=COALESCE(@NhanSuAnNinh,nhan_su_an_ninh),
 phuong_an_thoat_hiem=COALESCE(@PhuongAnThoatHiem,phuong_an_thoat_hiem),
 phuong_an_xu_ly_su_co=COALESCE(@PhuongAnXuLySuCo,phuong_an_xu_ly_su_co)
 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId AND trang_thai='NHAP';
 IF @@ROWCOUNT<>1 THROW 52013,N'Chỉ được sửa nội dung hồ sơ Nháp của mình.',1;
 SELECT ma_ho_so,trang_thai FROM dbo.HO_SO_CAP_PHEP WHERE ma_ho_so=@MaHoSo;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LuuHoSoNhapDayDu
 @MaHoSo BIGINT,@NguoiToChucId BIGINT,@MaLoaiSuKien BIGINT,
 @TenDonViToChuc NVARCHAR(200),@TenSuKien NVARCHAR(200),
 @BatDau DATETIMEOFFSET(0)=NULL,@KetThuc DATETIMEOFFSET(0)=NULL,
 @QuyMo INT=NULL,@NoiDung NVARCHAR(MAX)=NULL,
 @HinhThucThuongMai VARCHAR(20)=NULL,@CoTaiTro BIT=0,@ThongTinTaiTro NVARCHAR(MAX)=NULL,
 @LoaiVe VARCHAR(20)=NULL,@GiaVe DECIMAL(14,2)=NULL,@SoLuongVe INT=NULL,
 @KenhPhatHanhVe NVARCHAR(MAX)=NULL,@CachKiemSoatVe NVARCHAR(MAX)=NULL,
 @NhanSuYTe NVARCHAR(MAX)=NULL,@NhanSuAnNinh NVARCHAR(MAX)=NULL,
 @PhuongAnThoatHiem NVARCHAR(MAX)=NULL,@PhuongAnXuLySuCo NVARCHAR(MAX)=NULL,
 @DonViCap NVARCHAR(200)=NULL,@SoVanBan NVARCHAR(100)=NULL,
 @LoaiQuyen NVARCHAR(100)=NULL,@NguonTinHieu NVARCHAR(200)=NULL,
 @NoiDungDuocCap NVARCHAR(MAX)=NULL,@PhamViDiaDiem NVARCHAR(MAX)=NULL,
 @HieuLucTu DATE=NULL,@HieuLucDen DATE=NULL,@TepQuyen NVARCHAR(500)=NULL,
 @TaiLieuXml XML
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DECLARE @DaTao BIT=0,@MaQuyen BIGINT;
 BEGIN TRY
  BEGIN TRANSACTION;
  IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
                WHERE ma_tai_khoan=@NguoiToChucId
                  AND vai_tro='NGUOI_TO_CHUC' AND trang_thai='HOAT_DONG')
   THROW 52011,N'Tài khoản không được phép tạo hoặc sửa hồ sơ.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.LOAI_SU_KIEN
                WHERE ma_loai_su_kien=@MaLoaiSuKien AND trang_thai=1)
   THROW 52012,N'Loại sự kiện không hoạt động.',1;
  IF @BatDau IS NOT NULL AND @KetThuc IS NOT NULL AND @BatDau>=@KetThuc
   THROW 52016,N'Thời gian kết thúc phải sau thời gian bắt đầu.',1;

  IF @MaHoSo IS NULL
  BEGIN
   INSERT dbo.HO_SO_CAP_PHEP
    (nguoi_to_chuc_id,ma_loai_su_kien,trang_thai,ten_don_vi_to_chuc,ten_su_kien,
     bat_dau,ket_thuc,quy_mo,noi_dung,hinh_thuc_thuong_mai,co_tai_tro,thong_tin_tai_tro,
     loai_ve,gia_ve,so_luong_ve,kenh_phat_hanh_ve,cach_kiem_soat_ve,
     nhan_su_y_te,nhan_su_an_ninh,phuong_an_thoat_hiem,phuong_an_xu_ly_su_co)
   VALUES
    (@NguoiToChucId,@MaLoaiSuKien,'NHAP',@TenDonViToChuc,@TenSuKien,
     @BatDau,@KetThuc,@QuyMo,@NoiDung,@HinhThucThuongMai,@CoTaiTro,@ThongTinTaiTro,
     @LoaiVe,@GiaVe,@SoLuongVe,@KenhPhatHanhVe,@CachKiemSoatVe,
     @NhanSuYTe,@NhanSuAnNinh,@PhuongAnThoatHiem,@PhuongAnXuLySuCo);
   SET @MaHoSo=CONVERT(BIGINT,SCOPE_IDENTITY());
   SET @DaTao=1;
   INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,den_trang_thai)
   VALUES(@MaHoSo,@NguoiToChucId,N'Tạo và lưu hồ sơ Nháp','NHAP');
  END
  ELSE
  BEGIN
   IF NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
                 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId
                   AND trang_thai='NHAP')
    THROW 52013,N'Chỉ được sửa hồ sơ Nháp thuộc tài khoản hiện tại.',1;
   UPDATE dbo.HO_SO_CAP_PHEP
   SET ma_loai_su_kien=@MaLoaiSuKien,ten_don_vi_to_chuc=@TenDonViToChuc,
       ten_su_kien=@TenSuKien,bat_dau=@BatDau,ket_thuc=@KetThuc,quy_mo=@QuyMo,
       noi_dung=@NoiDung,hinh_thuc_thuong_mai=@HinhThucThuongMai,
       co_tai_tro=@CoTaiTro,thong_tin_tai_tro=@ThongTinTaiTro,
       loai_ve=@LoaiVe,gia_ve=@GiaVe,so_luong_ve=@SoLuongVe,
       kenh_phat_hanh_ve=@KenhPhatHanhVe,cach_kiem_soat_ve=@CachKiemSoatVe,
       nhan_su_y_te=@NhanSuYTe,nhan_su_an_ninh=@NhanSuAnNinh,
       phuong_an_thoat_hiem=@PhuongAnThoatHiem,
       phuong_an_xu_ly_su_co=@PhuongAnXuLySuCo
   WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId AND trang_thai='NHAP';
   IF @@ROWCOUNT<>1 THROW 52013,N'Chỉ được sửa hồ sơ Nháp thuộc tài khoản hiện tại.',1;
  END;

  SELECT TOP(1) @MaQuyen=ma_quyen
  FROM dbo.QUYEN_TRINH_CHIEU
  WHERE ma_ho_so=@MaHoSo AND trang_thai=1
  ORDER BY ma_quyen;
  IF @DonViCap IS NOT NULL
  BEGIN
   IF @LoaiQuyen IS NULL OR @NguonTinHieu IS NULL OR @NoiDungDuocCap IS NULL
      OR @PhamViDiaDiem IS NULL OR @HieuLucTu IS NULL OR @HieuLucDen IS NULL
      OR @HieuLucTu>@HieuLucDen
    THROW 52017,N'Thông tin quyền trình chiếu chưa đầy đủ hoặc thời hạn không hợp lệ.',1;
  END;
  IF @DonViCap IS NOT NULL
  BEGIN
   IF @MaQuyen IS NULL
    INSERT dbo.QUYEN_TRINH_CHIEU
     (ma_ho_so,don_vi_cap,so_van_ban,loai_quyen,nguon_tin_hieu,
      noi_dung_duoc_cap,pham_vi_dia_diem,hieu_luc_tu,hieu_luc_den,tep_dinh_kem)
    VALUES
     (@MaHoSo,@DonViCap,@SoVanBan,@LoaiQuyen,@NguonTinHieu,
      @NoiDungDuocCap,@PhamViDiaDiem,@HieuLucTu,@HieuLucDen,@TepQuyen);
   ELSE
    UPDATE dbo.QUYEN_TRINH_CHIEU
    SET don_vi_cap=@DonViCap,so_van_ban=@SoVanBan,loai_quyen=@LoaiQuyen,
        nguon_tin_hieu=@NguonTinHieu,noi_dung_duoc_cap=@NoiDungDuocCap,
        pham_vi_dia_diem=@PhamViDiaDiem,hieu_luc_tu=@HieuLucTu,
        hieu_luc_den=@HieuLucDen,tep_dinh_kem=@TepQuyen
    WHERE ma_quyen=@MaQuyen AND ma_ho_so=@MaHoSo;
  END
  ELSE IF @MaQuyen IS NOT NULL
   UPDATE dbo.QUYEN_TRINH_CHIEU SET trang_thai=0
   WHERE ma_quyen=@MaQuyen AND ma_ho_so=@MaHoSo;

  DELETE dbo.TAI_LIEU WHERE ma_ho_so=@MaHoSo;
  INSERT dbo.TAI_LIEU(ma_ho_so,loai_tai_lieu,ten_tai_lieu,duong_dan)
  SELECT @MaHoSo,
         item.value('@Loai','varchar(50)'),
         item.value('@Ten','nvarchar(250)'),
         item.value('@DuongDan','nvarchar(500)')
  FROM @TaiLieuXml.nodes('/TaiLieu/Item') AS documents(item);

  COMMIT TRANSACTION;
  SELECT @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai,@DaTao AS da_tao;
 END TRY
 BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LayHoSoNhapToChuc
 @NguoiToChucId BIGINT,@MaHoSo BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
               WHERE ma_tai_khoan=@NguoiToChucId
                 AND vai_tro='NGUOI_TO_CHUC' AND trang_thai='HOAT_DONG')
  THROW 52011,N'Tài khoản Người tổ chức không hoạt động.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP
               WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId
                 AND trang_thai='NHAP')
  THROW 52013,N'Chỉ được mở hồ sơ Nháp thuộc tài khoản hiện tại.',1;
 SELECT ma_ho_so,ma_loai_su_kien,ma_dia_diem,ten_don_vi_to_chuc,ten_su_kien,
        bat_dau,ket_thuc,quy_mo,noi_dung,hinh_thuc_thuong_mai,co_tai_tro,
        thong_tin_tai_tro,loai_ve,gia_ve,so_luong_ve,kenh_phat_hanh_ve,
        cach_kiem_soat_ve,nhan_su_y_te,nhan_su_an_ninh,phuong_an_thoat_hiem,
        phuong_an_xu_ly_su_co
 FROM dbo.HO_SO_CAP_PHEP
 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId AND trang_thai='NHAP';
 SELECT TOP(1) don_vi_cap,so_van_ban,loai_quyen,nguon_tin_hieu,noi_dung_duoc_cap,
        pham_vi_dia_diem,hieu_luc_tu,hieu_luc_den,tep_dinh_kem
 FROM dbo.QUYEN_TRINH_CHIEU WHERE ma_ho_so=@MaHoSo AND trang_thai=1
 ORDER BY ma_quyen;
 SELECT ma_tai_lieu,loai_tai_lieu,ten_tai_lieu,duong_dan
 FROM dbo.TAI_LIEU WHERE ma_ho_so=@MaHoSo ORDER BY ma_tai_lieu;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LietKeHoSoNhapToChuc
 @NguoiToChucId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
               WHERE ma_tai_khoan=@NguoiToChucId
                 AND vai_tro='NGUOI_TO_CHUC' AND trang_thai='HOAT_DONG')
  THROW 52011,N'Tài khoản Người tổ chức không hoạt động.',1;
 SELECT ma_ho_so,ten_su_kien,ten_don_vi_to_chuc,bat_dau,ket_thuc,quy_mo
 FROM dbo.HO_SO_CAP_PHEP
 WHERE nguoi_to_chuc_id=@NguoiToChucId AND trang_thai='NHAP'
 ORDER BY ngay_tao DESC,ma_ho_so DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_TraCuuDiaDiem
 @TuKhoa NVARCHAR(100)=NULL,@QuyMo INT=NULL,
 @BatDau DATETIMEOFFSET(0)=NULL,@KetThuc DATETIMEOFFSET(0)=NULL
AS
BEGIN
 SET NOCOUNT ON;
 SELECT d.ma_dia_diem,d.ten_dia_diem,d.dia_chi,d.suc_chua,d.trang_thai,
        CASE WHEN @BatDau IS NULL OR @KetThuc IS NULL THEN N'CHUA_KIEM_TRA'
             WHEN dbo.fn_TrungLichDiaDiem(d.ma_dia_diem,@BatDau,@KetThuc,NULL)=1 THEN N'CANH_BAO_TRUNG_LICH'
             ELSE N'CHUA_TRUNG_LICH' END AS canh_bao_lich
 FROM dbo.DIA_DIEM d
 WHERE d.trang_thai=1 AND (@QuyMo IS NULL OR d.suc_chua>=@QuyMo)
   AND (@TuKhoa IS NULL OR d.ten_dia_diem LIKE N'%' + @TuKhoa + N'%'
        OR d.dia_chi LIKE N'%' + @TuKhoa + N'%')
 ORDER BY d.suc_chua,d.ten_dia_diem;
 -- Kết quả chỉ tham khảo, không giữ chỗ.
 -- Backend tự lọc quyền truy cập theo người dùng đăng nhập.
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ChonDiaDiemNhap
 @MaHoSo BIGINT,@NguoiToChucId BIGINT,@MaDiaDiem BIGINT
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DECLARE @QuyMo INT,@TrangThaiHoSo VARCHAR(30),
         @SucChua INT,@TrangThaiDiaDiem BIT;
 BEGIN TRY
 BEGIN TRANSACTION;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
               WHERE ma_tai_khoan=@NguoiToChucId
                 AND vai_tro='NGUOI_TO_CHUC' AND trang_thai='HOAT_DONG')
  THROW 52028,N'Tài khoản Người tổ chức không hoạt động.',1;
 SELECT @QuyMo=quy_mo,@TrangThaiHoSo=trang_thai
 FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId;
 IF @TrangThaiHoSo IS NULL OR @TrangThaiHoSo<>'NHAP'
  THROW 52015,N'Chỉ được chọn địa điểm cho hồ sơ Nháp của tài khoản hiện tại.',1;
 IF @QuyMo IS NULL OR @QuyMo<=0
  THROW 52027,N'Cần lưu quy mô khách dự kiến trước khi chọn địa điểm.',1;
 SELECT @SucChua=suc_chua,@TrangThaiDiaDiem=trang_thai
 FROM dbo.DIA_DIEM WITH(UPDLOCK,HOLDLOCK)
 WHERE ma_dia_diem=@MaDiaDiem;
 IF @TrangThaiDiaDiem IS NULL OR @TrangThaiDiaDiem<>1
  THROW 52014,N'Địa điểm không tồn tại hoặc ngừng hoạt động.',1;
 IF @SucChua<@QuyMo
  THROW 52025,N'Địa điểm không đủ sức chứa theo quy mô khách dự kiến trong hồ sơ.',1;
 UPDATE dbo.HO_SO_CAP_PHEP SET ma_dia_diem=@MaDiaDiem
 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId
   AND trang_thai='NHAP' AND quy_mo=@QuyMo;
 IF @@ROWCOUNT<>1 THROW 52015,N'Không thể chọn địa điểm cho hồ sơ.',1;
 COMMIT TRANSACTION;
 SELECT @MaHoSo AS ma_ho_so,@MaDiaDiem AS ma_dia_diem,N'Chưa giữ chỗ' AS ghi_chu;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_NopHoSo
 @MaHoSo BIGINT,@NguoiToChucId BIGINT
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DECLARE @Dia BIGINT,@DiaBanDau BIGINT,@BD DATETIMEOFFSET(0),@KT DATETIMEOFFSET(0),
         @QuyMo INT,@TrangThai VARCHAR(30),@SucChua INT,@DiaHoatDong BIT,
         @TenDiaDiem NVARCHAR(200),@DiaChi NVARCHAR(300),
         @TenDonVi NVARCHAR(200),@TenSuKien NVARCHAR(200),@NoiDung NVARCHAR(MAX),
         @HinhThuc VARCHAR(20),@LoaiVe VARCHAR(20),@GiaVe DECIMAL(14,2),
         @SoLuongVe INT,@KenhVe NVARCHAR(MAX),@KiemSoatVe NVARCHAR(MAX),
         @LockResult INT,@LockName NVARCHAR(255),@Now DATETIMEOFFSET(0)=SYSDATETIMEOFFSET();
 BEGIN TRY
 BEGIN TRANSACTION;

 -- Get the venue key before locking the draft, so concurrent submissions for the
 -- same venue serialize without holding one draft row while waiting on another.
 SELECT @DiaBanDau=ma_dia_diem
 FROM dbo.HO_SO_CAP_PHEP
 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId;
 IF @@ROWCOUNT=0
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53001 AS ma_loi,
         N'Hồ sơ không tồn tại hoặc không thuộc tài khoản Người tổ chức hiện tại.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF @DiaBanDau IS NOT NULL
 BEGIN
  SET @LockName=CONCAT(N'VIEWPARTY_DIA_DIEM_',CONVERT(NVARCHAR(30),@DiaBanDau));
  EXEC @LockResult=sys.sp_getapplock @Resource=@LockName,@LockMode='Exclusive',
                        @LockOwner='Transaction',@LockTimeout=10000;
  IF @LockResult<0
  BEGIN
   ROLLBACK TRANSACTION;
   SELECT CAST(0 AS BIT) AS thanh_cong,53018 AS ma_loi,
          N'Không thể khóa lịch địa điểm. Hồ sơ vẫn ở trạng thái Nháp; hãy thử lại.' AS ly_do,
          @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
   RETURN;
  END;
 END;

 SELECT @Dia=ma_dia_diem,@BD=bat_dau,@KT=ket_thuc,@QuyMo=quy_mo,
        @TrangThai=trang_thai,@TenDonVi=ten_don_vi_to_chuc,
        @TenSuKien=ten_su_kien,@NoiDung=noi_dung,
        @HinhThuc=hinh_thuc_thuong_mai,@LoaiVe=loai_ve,@GiaVe=gia_ve,
        @SoLuongVe=so_luong_ve,@KenhVe=kenh_phat_hanh_ve,
        @KiemSoatVe=cach_kiem_soat_ve
 FROM dbo.HO_SO_CAP_PHEP WITH (UPDLOCK,HOLDLOCK)
 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId;

 IF @TrangThai IS NULL OR @TrangThai<>'NHAP'
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53002 AS ma_loi,
         N'Chỉ hồ sơ ở trạng thái Nháp mới được nộp. Hồ sơ không bị thay đổi.' AS ly_do,
         @MaHoSo AS ma_ho_so,@TrangThai AS trang_thai;
  RETURN;
 END;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WITH(UPDLOCK,HOLDLOCK)
               WHERE ma_tai_khoan=@NguoiToChucId
                 AND vai_tro='NGUOI_TO_CHUC' AND trang_thai='HOAT_DONG')
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53003 AS ma_loi,
         N'Tài khoản Người tổ chức không hoạt động; không thể nộp hồ sơ.' AS ly_do,
         @MaHoSo AS ma_ho_so,@TrangThai AS trang_thai;
  RETURN;
 END;
 IF @Dia<>@DiaBanDau
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53018 AS ma_loi,
         N'Địa điểm của hồ sơ vừa thay đổi. Hãy tải lại hồ sơ và thử nộp lại.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF @QuyMo IS NULL OR @QuyMo NOT BETWEEN 1000 AND 10000
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53004 AS ma_loi,
         N'Quy mô khách phải từ 1.000 đến 10.000 người.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF @BD IS NULL OR @KT IS NULL OR @BD>=@KT
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53005 AS ma_loi,
         N'Cần nhập thời gian bắt đầu và kết thúc hợp lệ.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF @BD<DATEADD(DAY,30,@Now)
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53006 AS ma_loi,
         N'Thời điểm bắt đầu sự kiện phải cách thời điểm nộp ít nhất 30 ngày.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF @Dia IS NULL
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53007 AS ma_loi,
         N'Hãy chọn địa điểm trước khi nộp hồ sơ.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 SELECT @SucChua=suc_chua,@DiaHoatDong=trang_thai,
        @TenDiaDiem=ten_dia_diem,@DiaChi=dia_chi
 FROM dbo.DIA_DIEM WITH(UPDLOCK,HOLDLOCK)
 WHERE ma_dia_diem=@Dia;
 IF @DiaHoatDong IS NULL OR @DiaHoatDong<>1
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53008 AS ma_loi,
         N'Địa điểm đã ngừng hoạt động hoặc không còn tồn tại.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF @SucChua<@QuyMo
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53009 AS ma_loi,
         N'Địa điểm không đủ sức chứa cho quy mô đã khai báo.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
   WHERE ma_dia_diem=@Dia AND ma_ho_so<>@MaHoSo
    AND trang_thai IN ('DA_NOP','DANG_KIEM_TRA','YEU_CAU_BO_SUNG',
      'DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH','TONG_HOP_LIEN_NGANH',
      'XU_LY_NGOAI_LE','CHO_PHE_DUYET','DA_CAP_PHEP')
    AND bat_dau<@KT AND ket_thuc>@BD)
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53010 AS ma_loi,
         N'Địa điểm trùng lịch với một hồ sơ đã nộp hoặc đang xử lý.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF NULLIF(LTRIM(RTRIM(@TenDonVi)),N'') IS NULL
    OR NULLIF(LTRIM(RTRIM(@TenSuKien)),N'') IS NULL
    OR NULLIF(LTRIM(RTRIM(@NoiDung)),N'') IS NULL
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53011 AS ma_loi,
         N'Cần bổ sung đơn vị tổ chức, tên sự kiện và nội dung chương trình.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF @HinhThuc IS NULL OR @HinhThuc NOT IN ('PHI_THUONG_MAI','THUONG_MAI')
    OR @LoaiVe IS NULL OR @LoaiVe NOT IN ('MIEN_PHI','THU_MOI','BAN_VE')
    OR @SoLuongVe IS NULL OR @SoLuongVe<1 OR @SoLuongVe>@QuyMo
    OR NULLIF(LTRIM(RTRIM(@KenhVe)),N'') IS NULL
    OR NULLIF(LTRIM(RTRIM(@KiemSoatVe)),N'') IS NULL
    OR (@LoaiVe='BAN_VE' AND (@GiaVe IS NULL OR @GiaVe<=0))
    OR (@LoaiVe<>'BAN_VE' AND ISNULL(@GiaVe,0)<>0)
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53012 AS ma_loi,
         N'Phương án vé/QR không hợp lệ: cần loại vé, số lượng từ 1 đến quy mô, kênh phát hành và cách kiểm soát; vé bán phải có giá dương.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF NOT EXISTS(SELECT 1 FROM dbo.QUYEN_TRINH_CHIEU q WITH(UPDLOCK,HOLDLOCK)
  WHERE q.ma_ho_so=@MaHoSo AND q.trang_thai=1
    AND q.hieu_luc_tu<=CONVERT(DATE,@BD) AND q.hieu_luc_den>=CONVERT(DATE,@KT)
    AND NULLIF(LTRIM(RTRIM(q.don_vi_cap)),N'') IS NOT NULL
    AND NULLIF(LTRIM(RTRIM(q.loai_quyen)),N'') IS NOT NULL
    AND NULLIF(LTRIM(RTRIM(q.nguon_tin_hieu)),N'') IS NOT NULL
    AND NULLIF(LTRIM(RTRIM(q.noi_dung_duoc_cap)),N'') IS NOT NULL
    AND NULLIF(LTRIM(RTRIM(q.pham_vi_dia_diem)),N'') IS NOT NULL
    AND NULLIF(LTRIM(RTRIM(q.tep_dinh_kem)),N'') IS NOT NULL)
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53013 AS ma_loi,
         N'Thiếu bằng chứng quyền trình chiếu còn hiệu lực cho toàn bộ thời gian sự kiện.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 IF NOT EXISTS(SELECT 1 FROM dbo.QUYEN_TRINH_CHIEU q
  WHERE q.ma_ho_so=@MaHoSo AND q.trang_thai=1
    AND q.hieu_luc_tu<=CONVERT(DATE,@BD) AND q.hieu_luc_den>=CONVERT(DATE,@KT)
    AND (CHARINDEX(N'TOAN_QUOC',UPPER(q.pham_vi_dia_diem))>0
      OR CHARINDEX(LOWER(@TenDiaDiem),LOWER(q.pham_vi_dia_diem))>0
      OR CHARINDEX(LOWER(@DiaChi),LOWER(q.pham_vi_dia_diem))>0))
 BEGIN
  ROLLBACK TRANSACTION;
  SELECT CAST(0 AS BIT) AS thanh_cong,53014 AS ma_loi,
         N'Phạm vi quyền trình chiếu không khớp địa điểm đã chọn. Ghi rõ tên/địa chỉ địa điểm hoặc TOAN_QUOC trong bằng chứng.' AS ly_do,
         @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
  RETURN;
 END;
 UPDATE dbo.HO_SO_CAP_PHEP SET trang_thai='DA_NOP',ngay_nop=@Now
 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId AND trang_thai='NHAP';
 IF @@ROWCOUNT<>1
  THROW 53019,N'Trạng thái hồ sơ vừa thay đổi trong lúc nộp.',1;
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai)
 VALUES(@MaHoSo,@NguoiToChucId,N'Nộp hồ sơ',@TrangThai,'DA_NOP');
 COMMIT TRANSACTION;
 SELECT CAST(1 AS BIT) AS thanh_cong,CAST(NULL AS INT) AS ma_loi,
        N'Hồ sơ đã được nộp thành công.' AS ly_do,
        @MaHoSo AS ma_ho_so,'DA_NOP' AS trang_thai;
 END TRY BEGIN CATCH
  IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
  IF ERROR_NUMBER() IN (52605,53019)
  BEGIN
   SELECT CAST(0 AS BIT) AS thanh_cong,
          CASE WHEN ERROR_NUMBER()=52605 THEN 53010 ELSE 53018 END AS ma_loi,
          CASE WHEN ERROR_NUMBER()=52605
               THEN N'Địa điểm vừa phát sinh hồ sơ trùng lịch. Hồ sơ vẫn ở trạng thái Nháp.'
               ELSE N'Trạng thái hồ sơ vừa thay đổi; hãy tải lại hồ sơ trước khi thử nộp lại.'
          END AS ly_do,
          @MaHoSo AS ma_ho_so,'NHAP' AS trang_thai;
   RETURN;
  END;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_TheoDoiHoSo @MaHoSo BIGINT,@NguoiToChucId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP
   WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId)
   THROW 52027,N'Không có quyền xem hồ sơ này.',1;
 SELECT * FROM dbo.v_ho_so_tong_quan WHERE ma_ho_so=@MaHoSo;
 SELECT ma_lich_su,hanh_dong,tu_trang_thai,den_trang_thai,ghi_chu,thoi_diem
 FROM dbo.LICH_SU_XU_LY WHERE ma_ho_so=@MaHoSo ORDER BY thoi_diem;
END;
GO

-- ================================================================
-- E. TIẾP NHẬN, BỔ SUNG, THẨM ĐỊNH (UC06-12)
-- ================================================================
CREATE OR ALTER PROCEDURE dbo.sp_TiepNhanHoSo
 @MaHoSo BIGINT,@NguoiXuLyId BIGINT
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@NguoiXuLyId
 AND vai_tro='NGUOI_XU_LY' AND trang_thai='HOAT_DONG')
  THROW 52101,N'Không có quyền tiếp nhận hồ sơ.',1;
 BEGIN TRY
 BEGIN TRANSACTION;
 UPDATE dbo.HO_SO_CAP_PHEP SET nguoi_xu_ly_hien_tai_id=@NguoiXuLyId,
  trang_thai='DANG_KIEM_TRA'
 WHERE ma_ho_so=@MaHoSo AND trang_thai='DA_NOP';
 IF @@ROWCOUNT<>1 THROW 52102,N'Hồ sơ chưa được nộp hoặc đã có người tiếp nhận.',1;
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai)
 VALUES(@MaHoSo,@NguoiXuLyId,N'Tiếp nhận hồ sơ','DA_NOP','DANG_KIEM_TRA');
 COMMIT TRANSACTION;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_KiemTraHopLeHoSo
 @MaHoSo BIGINT,@NguoiXuLyId BIGINT,@HopLe BIT,@LyDo NVARCHAR(MAX)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF @HopLe IS NULL THROW 52112,N'Phải chọn kết luận hợp lệ hay yêu cầu bổ sung.',1;
 IF @HopLe=0 AND NULLIF(LTRIM(RTRIM(@LyDo)),N'') IS NULL
  THROW 52103,N'Phải nhập lý do yêu cầu bổ sung.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN t JOIN dbo.HO_SO_CAP_PHEP h
  ON h.nguoi_xu_ly_hien_tai_id=t.ma_tai_khoan
  WHERE h.ma_ho_so=@MaHoSo AND t.ma_tai_khoan=@NguoiXuLyId
  AND t.vai_tro='NGUOI_XU_LY' AND t.trang_thai='HOAT_DONG')
  THROW 52104,N'Không có quyền kiểm tra hồ sơ này.',1;
 DECLARE @TrangThai VARCHAR(30)=CASE WHEN @HopLe=1 THEN 'DANG_THAM_DINH' ELSE 'YEU_CAU_BO_SUNG' END;
 BEGIN TRY
 BEGIN TRANSACTION;
 UPDATE dbo.HO_SO_CAP_PHEP SET trang_thai=@TrangThai
 WHERE ma_ho_so=@MaHoSo AND trang_thai='DANG_KIEM_TRA';
 IF @@ROWCOUNT<>1 THROW 52105,N'Trạng thái hồ sơ không phù hợp.',1;
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai,ghi_chu)
 VALUES(@MaHoSo,@NguoiXuLyId,N'Kiểm tra hợp lệ','DANG_KIEM_TRA',@TrangThai,@LyDo);
 IF @HopLe=0
  INSERT dbo.THONG_BAO(nguoi_nhan_id,ma_ho_so,loai_thong_bao,noi_dung)
  SELECT nguoi_to_chuc_id,ma_ho_so,'YEU_CAU_BO_SUNG',@LyDo
  FROM dbo.HO_SO_CAP_PHEP WHERE ma_ho_so=@MaHoSo;
 COMMIT TRANSACTION;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

-- Bổ sung nội dung khi YEU_CAU_BO_SUNG, không thay thời gian/địa điểm đã giữ.
-- Nếu thực sự cần đổi ngày/địa điểm, cán bộ phải trả hồ sơ về Nháp,
-- giải phóng lịch rồi cho người tổ chức chọn/nộp lại (quy trình mở rộng).
CREATE OR ALTER PROCEDURE dbo.sp_BoSungNoiDung
 @MaHoSo BIGINT,@NguoiToChucId BIGINT,@NoiDung NVARCHAR(MAX),
 @NhanSuYTe NVARCHAR(MAX),@NhanSuAnNinh NVARCHAR(MAX),
 @PhuongAnThoatHiem NVARCHAR(MAX),@PhuongAnXuLySuCo NVARCHAR(MAX)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 UPDATE dbo.HO_SO_CAP_PHEP
 SET noi_dung=@NoiDung,nhan_su_y_te=@NhanSuYTe,nhan_su_an_ninh=@NhanSuAnNinh,
     phuong_an_thoat_hiem=@PhuongAnThoatHiem,phuong_an_xu_ly_su_co=@PhuongAnXuLySuCo
 WHERE ma_ho_so=@MaHoSo AND nguoi_to_chuc_id=@NguoiToChucId
 AND trang_thai='YEU_CAU_BO_SUNG';
 IF @@ROWCOUNT<>1 THROW 52106,N'Không thể bổ sung nội dung cho hồ sơ này.',1;
 -- Sau đó gọi sp_NopHoSo để nộp lại (UC06).
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GhiKetQuaThamDinh
 @MaHoSo BIGINT,@NguoiXuLyId BIGINT,@QuyTacId BIGINT,
 @KetQua VARCHAR(20),@GiaTriThucTe NVARCHAR(MAX)=NULL,@GhiChu NVARCHAR(MAX)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF @KetQua NOT IN ('DAT','KHONG_DAT','CANH_BAO')
  THROW 52107,N'Kết quả thẩm định không hợp lệ.',1;
 IF @GiaTriThucTe IS NOT NULL AND ISJSON(@GiaTriThucTe)<>1
  THROW 52108,N'Giá trị thực tế phải là JSON hợp lệ.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP h
  JOIN dbo.TAI_KHOAN t ON t.ma_tai_khoan=h.nguoi_xu_ly_hien_tai_id
  JOIN dbo.QUY_TAC r ON r.quy_tac_id=@QuyTacId
  WHERE h.ma_ho_so=@MaHoSo AND h.trang_thai='DANG_THAM_DINH'
    AND t.ma_tai_khoan=@NguoiXuLyId AND t.vai_tro='NGUOI_XU_LY' AND t.trang_thai='HOAT_DONG'
    AND r.ma_loai_su_kien=h.ma_loai_su_kien AND r.trang_thai=1
    AND r.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop)
    AND (r.hieu_luc_den IS NULL OR r.hieu_luc_den>=CONVERT(DATE,h.ngay_nop)))
  THROW 52109,N'Hồ sơ, người kiểm tra hoặc phiên bản quy tắc không hợp lệ.',1;
 BEGIN TRY
 BEGIN TRANSACTION;
 UPDATE dbo.KET_QUA_KIEM_TRA
 SET ket_qua=@KetQua,gia_tri_thuc_te=@GiaTriThucTe,ghi_chu=@GhiChu,
     ngay_danh_gia=SYSDATETIMEOFFSET(),nguoi_kiem_tra_id=@NguoiXuLyId
 WHERE ma_ho_so=@MaHoSo AND quy_tac_id=@QuyTacId;
 IF @@ROWCOUNT=0
  INSERT dbo.KET_QUA_KIEM_TRA(ma_ho_so,quy_tac_id,nguoi_kiem_tra_id,ket_qua,gia_tri_thuc_te,ghi_chu)
  VALUES(@MaHoSo,@QuyTacId,@NguoiXuLyId,@KetQua,@GiaTriThucTe,@GhiChu);
 COMMIT TRANSACTION;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_KetThucThamDinh
 @MaHoSo BIGINT,@NguoiXuLyId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP
     WHERE ma_ho_so=@MaHoSo AND nguoi_xu_ly_hien_tai_id=@NguoiXuLyId
       AND trang_thai='DANG_THAM_DINH')
  THROW 52110,N'Không phải người thẩm định được phân công.',1;
 IF EXISTS(SELECT 1 FROM dbo.QUY_TAC r JOIN dbo.HO_SO_CAP_PHEP h
     ON h.ma_loai_su_kien=r.ma_loai_su_kien
     WHERE h.ma_ho_so=@MaHoSo AND r.trang_thai=1 AND r.muc_do='BAT_BUOC'
       AND r.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop)
       AND (r.hieu_luc_den IS NULL OR r.hieu_luc_den>=CONVERT(DATE,h.ngay_nop))
       AND NOT EXISTS(SELECT 1 FROM dbo.KET_QUA_KIEM_TRA k
                      WHERE k.ma_ho_so=@MaHoSo AND k.quy_tac_id=r.quy_tac_id))
  THROW 52111,N'Chưa thẩm định hết quy tắc bắt buộc.',1;
 SELECT k.ma_ket_qua,r.ma_quy_tac,r.phien_ban,k.ket_qua,k.ghi_chu
 FROM dbo.KET_QUA_KIEM_TRA k JOIN dbo.QUY_TAC r ON r.quy_tac_id=k.quy_tac_id
 WHERE k.ma_ho_so=@MaHoSo;
END;
GO

-- ================================================================
-- F. LIÊN NGÀNH VÀ TỔNG HỢP (UC13,14,15)
-- ================================================================
CREATE OR ALTER PROCEDURE dbo.sp_GuiYeuCauLienNganh
 @MaHoSo BIGINT,@NguoiXuLyId BIGINT
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DECLARE @Loai BIGINT,@Ngay DATE,@TrangThai VARCHAR(30);
 BEGIN TRY
 BEGIN TRANSACTION;
 SELECT @Loai=ma_loai_su_kien,@Ngay=CONVERT(DATE,ngay_nop),@TrangThai=trang_thai
 FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
 WHERE ma_ho_so=@MaHoSo AND nguoi_xu_ly_hien_tai_id=@NguoiXuLyId
   AND trang_thai IN ('DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH');
 IF @Loai IS NULL THROW 52201,N'Hồ sơ không ở giai đoạn gửi ý kiến hoặc không được phân công.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WITH(UPDLOCK,HOLDLOCK)
               WHERE ma_tai_khoan=@NguoiXuLyId
                 AND vai_tro='NGUOI_XU_LY' AND trang_thai='HOAT_DONG')
   THROW 52202,N'Người gửi không hợp lệ.',1;
 IF EXISTS(SELECT 1 FROM dbo.QUY_TAC r WITH(HOLDLOCK)
   WHERE r.ma_loai_su_kien=@Loai AND r.trang_thai=1 AND r.muc_do='BAT_BUOC'
     AND r.hieu_luc_tu<=@Ngay AND (r.hieu_luc_den IS NULL OR r.hieu_luc_den>=@Ngay)
     AND NOT EXISTS(SELECT 1 FROM dbo.KET_QUA_KIEM_TRA k WITH(HOLDLOCK)
                    WHERE k.ma_ho_so=@MaHoSo AND k.quy_tac_id=r.quy_tac_id))
   THROW 52211,N'Chưa kiểm tra đủ quy tắc bắt buộc, không thể xin ý kiến.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.CAU_HINH_LIEN_NGANH WITH(HOLDLOCK)
     WHERE ma_loai_su_kien=@Loai AND trang_thai=1 AND bat_buoc=1
       AND hieu_luc_tu<=@Ngay AND (hieu_luc_den IS NULL OR hieu_luc_den>=@Ngay))
   THROW 52203,N'Không có cấu hình liên ngành bắt buộc còn hiệu lực.',1;
 IF EXISTS(
   SELECT 1 FROM dbo.CAU_HINH_LIEN_NGANH c WITH(HOLDLOCK)
   LEFT JOIN dbo.DON_VI_LIEN_NGANH d ON d.ma_don_vi=c.ma_don_vi AND d.trang_thai=1
   WHERE c.ma_loai_su_kien=@Loai AND c.trang_thai=1 AND c.bat_buoc=1
     AND c.hieu_luc_tu<=@Ngay AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den>=@Ngay)
     AND (d.ma_don_vi IS NULL OR NOT EXISTS(
       SELECT 1 FROM dbo.TAI_KHOAN t WITH(HOLDLOCK)
       WHERE t.ma_don_vi=c.ma_don_vi AND t.vai_tro='LIEN_NGANH'
         AND t.trang_thai='HOAT_DONG'))
 )
   THROW 52212,N'Cấu hình có đơn vị bắt buộc nhưng đơn vị chưa hoạt động hoặc chưa có tài khoản liên ngành.',1;

 ;WITH cau_hinh AS (
   SELECT c.*,ROW_NUMBER() OVER(PARTITION BY c.ma_don_vi ORDER BY c.hieu_luc_tu DESC,c.ma_cau_hinh DESC) AS rn
   FROM dbo.CAU_HINH_LIEN_NGANH c WITH(HOLDLOCK)
   WHERE c.ma_loai_su_kien=@Loai AND c.trang_thai=1 AND c.bat_buoc=1
     AND c.hieu_luc_tu<=@Ngay AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den>=@Ngay)
 )
 INSERT dbo.YEU_CAU_Y_KIEN(ma_ho_so,ma_don_vi,ma_cau_hinh,nguoi_gui_id,han_phan_hoi)
 SELECT @MaHoSo,c.ma_don_vi,c.ma_cau_hinh,@NguoiXuLyId,
        DATEADD(DAY,c.so_ngay_phan_hoi,SYSDATETIMEOFFSET())
 FROM cau_hinh c WHERE c.rn=1
 AND NOT EXISTS(SELECT 1 FROM dbo.YEU_CAU_Y_KIEN y
                WHERE y.ma_ho_so=@MaHoSo AND y.ma_don_vi=c.ma_don_vi);
 DECLARE @Moi INT=@@ROWCOUNT;
 UPDATE dbo.HO_SO_CAP_PHEP SET trang_thai='CHO_Y_KIEN_LIEN_NGANH'
 WHERE ma_ho_so=@MaHoSo AND trang_thai='DANG_THAM_DINH';
 IF @@ROWCOUNT=1
  INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai)
  VALUES(@MaHoSo,@NguoiXuLyId,N'Xin ý kiến liên ngành','DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH');
 INSERT dbo.THONG_BAO(nguoi_nhan_id,ma_ho_so,loai_thong_bao,noi_dung)
 SELECT t.ma_tai_khoan,@MaHoSo,'XIN_Y_KIEN',N'Có yêu cầu cho ý kiến hồ sơ View Party mới.'
 FROM dbo.TAI_KHOAN t JOIN dbo.YEU_CAU_Y_KIEN y ON y.ma_don_vi=t.ma_don_vi
 WHERE y.ma_ho_so=@MaHoSo AND t.vai_tro='LIEN_NGANH' AND t.trang_thai='HOAT_DONG'
 AND NOT EXISTS(SELECT 1 FROM dbo.THONG_BAO n WHERE n.nguoi_nhan_id=t.ma_tai_khoan
   AND n.ma_ho_so=@MaHoSo AND n.loai_thong_bao='XIN_Y_KIEN');
 COMMIT TRANSACTION;
 SELECT @MaHoSo AS ma_ho_so,@Moi AS so_yeu_cau_moi;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DanhSachHoSoXinYKien
 @NguoiXuLyId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
               WHERE ma_tai_khoan=@NguoiXuLyId
                 AND vai_tro='NGUOI_XU_LY' AND trang_thai='HOAT_DONG')
   THROW 52202,N'Người xử lý không hoạt động hoặc không hợp lệ.',1;

 SELECT h.ma_ho_so,h.ten_su_kien,l.ten_loai,h.trang_thai,
        COUNT(DISTINCT c.ma_don_vi) AS so_don_vi_bat_buoc,
        COUNT(DISTINCT CASE WHEN y.trang_thai='DA_PHAN_HOI'
                            THEN c.ma_don_vi END) AS so_don_vi_da_phan_hoi
 FROM dbo.HO_SO_CAP_PHEP h
 JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien=h.ma_loai_su_kien
 LEFT JOIN dbo.CAU_HINH_LIEN_NGANH c
   ON c.ma_loai_su_kien=h.ma_loai_su_kien AND c.bat_buoc=1 AND c.trang_thai=1
  AND c.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop)
  AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den>=CONVERT(DATE,h.ngay_nop))
 LEFT JOIN dbo.YEU_CAU_Y_KIEN y
   ON y.ma_ho_so=h.ma_ho_so AND y.ma_don_vi=c.ma_don_vi
  AND EXISTS(SELECT 1 FROM dbo.CAU_HINH_LIEN_NGANH cy
             WHERE cy.ma_cau_hinh=y.ma_cau_hinh AND cy.bat_buoc=1 AND cy.trang_thai=1
               AND cy.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop)
               AND (cy.hieu_luc_den IS NULL OR cy.hieu_luc_den>=CONVERT(DATE,h.ngay_nop)))
 WHERE h.nguoi_xu_ly_hien_tai_id=@NguoiXuLyId
   AND h.trang_thai IN ('DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH',
                        'TONG_HOP_LIEN_NGANH','XU_LY_NGOAI_LE')
 GROUP BY h.ma_ho_so,h.ten_su_kien,l.ten_loai,h.trang_thai
 ORDER BY h.ma_ho_so DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DanhSachYeuCauLienNganh
 @NguoiPhanHoiId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
               WHERE ma_tai_khoan=@NguoiPhanHoiId
                 AND vai_tro='LIEN_NGANH' AND trang_thai='HOAT_DONG'
                 AND ma_don_vi IS NOT NULL)
   THROW 52205,N'Tài khoản liên ngành không hoạt động hoặc chưa được gán đơn vị.',1;

 SELECT y.ma_yeu_cau,y.ma_ho_so,h.ten_su_kien,d.ten_don_vi,d.linh_vuc,
        y.han_phan_hoi,y.trang_thai,y.ket_luan,y.noi_dung_phan_hoi,
        y.kien_nghi,y.ngay_phan_hoi
 FROM dbo.YEU_CAU_Y_KIEN y
 JOIN dbo.TAI_KHOAN t ON t.ma_don_vi=y.ma_don_vi
 JOIN dbo.DON_VI_LIEN_NGANH d ON d.ma_don_vi=y.ma_don_vi
 JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=y.ma_ho_so
 WHERE t.ma_tai_khoan=@NguoiPhanHoiId AND t.vai_tro='LIEN_NGANH'
 ORDER BY CASE WHEN y.trang_thai='DA_PHAN_HOI' THEN 1 ELSE 0 END,
          y.han_phan_hoi,y.ma_yeu_cau DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PhanHoiLienNganh
 @MaYeuCau BIGINT,@NguoiPhanHoiId BIGINT,@KetLuan VARCHAR(30),
 @NoiDung NVARCHAR(MAX),@KienNghi NVARCHAR(MAX)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF @KetLuan IS NULL OR @KetLuan NOT IN ('PHU_HOP','PHU_HOP_CO_DIEU_KIEN','KHONG_PHU_HOP')
    OR NULLIF(LTRIM(RTRIM(@NoiDung)),N'') IS NULL
  THROW 52204,N'Kết luận hoặc nội dung phản hồi không hợp lệ.',1;
 DECLARE @HoSo BIGINT;
 SELECT @HoSo=ma_ho_so FROM dbo.YEU_CAU_Y_KIEN WHERE ma_yeu_cau=@MaYeuCau;
 IF @HoSo IS NULL THROW 52206,N'Yêu cầu không tồn tại hoặc không còn hiệu lực.',1;
 BEGIN TRY
 BEGIN TRANSACTION;
 IF NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
               WHERE ma_ho_so=@HoSo)
  THROW 52206,N'Hồ sơ của yêu cầu không còn tồn tại.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.YEU_CAU_Y_KIEN y WITH(UPDLOCK,HOLDLOCK)
               JOIN dbo.TAI_KHOAN t WITH(UPDLOCK,HOLDLOCK)
                 ON t.ma_don_vi=y.ma_don_vi
               WHERE y.ma_yeu_cau=@MaYeuCau AND t.ma_tai_khoan=@NguoiPhanHoiId
                 AND t.vai_tro='LIEN_NGANH' AND t.trang_thai='HOAT_DONG')
  THROW 52205,N'Không có quyền phản hồi yêu cầu này hoặc tài khoản không hoạt động.',1;
 UPDATE dbo.YEU_CAU_Y_KIEN
 SET nguoi_phan_hoi_id=@NguoiPhanHoiId,ket_luan=@KetLuan,
 noi_dung_phan_hoi=@NoiDung,kien_nghi=@KienNghi,
 ngay_phan_hoi=SYSDATETIMEOFFSET(),trang_thai='DA_PHAN_HOI'
 WHERE ma_yeu_cau=@MaYeuCau AND trang_thai IN ('CHO_PHAN_HOI','QUA_HAN');
 IF @@ROWCOUNT<>1 THROW 52206,N'Yêu cầu đã được phản hồi hoặc không còn hiệu lực.',1;
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,ghi_chu)
 VALUES(@HoSo,@NguoiPhanHoiId,N'Gửi ý kiến liên ngành',@NoiDung);
 COMMIT TRANSACTION;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_TongHopThamDinh
 @MaHoSo BIGINT,@NguoiXuLyId BIGINT
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 -- Dữ liệu trung gian chỉ sống trong phạm vi Stored Procedure.
 CREATE TABLE #tmp_tong_hop_tham_dinh (
  ma_ho_so BIGINT PRIMARY KEY,
  so_quy_tac INT NOT NULL,
  so_khong_dat_bat_buoc INT NOT NULL,
  so_khong_duoc_ngoai_le INT NOT NULL,
  so_canh_bao INT NOT NULL
 );
 BEGIN TRY
 BEGIN TRANSACTION;
 IF NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
    WHERE ma_ho_so=@MaHoSo AND nguoi_xu_ly_hien_tai_id=@NguoiXuLyId
    AND trang_thai IN ('CHO_Y_KIEN_LIEN_NGANH','TONG_HOP_LIEN_NGANH'))
   THROW 52207,N'Hồ sơ chưa được phân công hoặc sai trạng thái tổng hợp.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WITH(UPDLOCK,HOLDLOCK)
               WHERE ma_tai_khoan=@NguoiXuLyId AND vai_tro='NGUOI_XU_LY'
                 AND trang_thai='HOAT_DONG')
   THROW 52202,N'Người xử lý không hoạt động hoặc không hợp lệ.',1;
 -- Hold the effective mandatory configuration and its request rows until
 -- aggregation commits so a concurrent reply/config change cannot bypass the gate.
 DECLARE @LockSink BIGINT;
 SELECT @LockSink=MAX(c.ma_cau_hinh)
 FROM dbo.CAU_HINH_LIEN_NGANH c WITH(UPDLOCK,HOLDLOCK)
 JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=@MaHoSo
 WHERE c.ma_loai_su_kien=h.ma_loai_su_kien AND c.bat_buoc=1 AND c.trang_thai=1
   AND c.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop)
   AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den>=CONVERT(DATE,h.ngay_nop));
 SELECT @LockSink=MAX(y.ma_yeu_cau)
 FROM dbo.YEU_CAU_Y_KIEN y WITH(UPDLOCK,HOLDLOCK)
 WHERE y.ma_ho_so=@MaHoSo;
 IF dbo.fn_DuYKienBatBuoc(@MaHoSo)=0
   THROW 52208,N'Chưa nhận đủ ý kiến liên ngành bắt buộc.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.KET_QUA_KIEM_TRA WITH(HOLDLOCK) WHERE ma_ho_so=@MaHoSo)
   THROW 52209,N'Chưa có kết quả kiểm tra.',1;
 INSERT #tmp_tong_hop_tham_dinh
 SELECT @MaHoSo,COUNT(*),
        COALESCE(SUM(CASE WHEN q.muc_do='BAT_BUOC' AND k.ket_qua='KHONG_DAT' THEN 1 ELSE 0 END),0),
        COALESCE(SUM(CASE WHEN q.muc_do='BAT_BUOC' AND k.ket_qua='KHONG_DAT' AND q.cho_phep_ngoai_le=0 THEN 1 ELSE 0 END),0),
        COALESCE(SUM(CASE WHEN k.ket_qua='CANH_BAO' THEN 1 ELSE 0 END),0)
 FROM dbo.KET_QUA_KIEM_TRA k JOIN dbo.QUY_TAC q ON q.quy_tac_id=k.quy_tac_id
 WHERE k.ma_ho_so=@MaHoSo;
 DECLARE @TrangThaiCu VARCHAR(30),@TrangThaiMoi VARCHAR(30),@KetLuan VARCHAR(40);
 SELECT @TrangThaiMoi=CASE
     WHEN so_quy_tac=0 OR so_khong_duoc_ngoai_le>0 OR EXISTS (
       SELECT 1 FROM dbo.YEU_CAU_Y_KIEN
       WHERE ma_ho_so=@MaHoSo AND ket_luan='KHONG_PHU_HOP'
     ) THEN 'TONG_HOP_LIEN_NGANH'
     WHEN so_khong_dat_bat_buoc>0 THEN 'XU_LY_NGOAI_LE'
     ELSE 'CHO_PHE_DUYET' END,
        @KetLuan=CASE
     WHEN so_quy_tac=0 THEN 'CHUA_THAM_DINH'
     WHEN so_khong_duoc_ngoai_le>0 OR EXISTS (
       SELECT 1 FROM dbo.YEU_CAU_Y_KIEN
       WHERE ma_ho_so=@MaHoSo AND ket_luan='KHONG_PHU_HOP'
     ) THEN 'KHONG_DU_DIEU_KIEN'
     WHEN so_khong_dat_bat_buoc>0 THEN 'CAN_XET_NGOAI_LE'
     ELSE 'DU_DIEU_KIEN' END
 FROM #tmp_tong_hop_tham_dinh;
 SELECT @TrangThaiCu=trang_thai FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
 WHERE ma_ho_so=@MaHoSo;
 UPDATE dbo.HO_SO_CAP_PHEP SET trang_thai=@TrangThaiMoi
 WHERE ma_ho_so=@MaHoSo AND nguoi_xu_ly_hien_tai_id=@NguoiXuLyId
   AND trang_thai IN ('CHO_Y_KIEN_LIEN_NGANH','TONG_HOP_LIEN_NGANH');
 IF @@ROWCOUNT<>1 THROW 52210,N'Hồ sơ đã thay đổi trạng thái trong khi tổng hợp.',1;
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai,ghi_chu)
 VALUES(@MaHoSo,@NguoiXuLyId,N'Tổng hợp thẩm định',@TrangThaiCu,@TrangThaiMoi,@KetLuan);
 COMMIT TRANSACTION;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
 SELECT *,@KetLuan AS ket_luan_de_xuat,@TrangThaiMoi AS trang_thai_tiep_theo
 FROM #tmp_tong_hop_tham_dinh;
 DROP TABLE #tmp_tong_hop_tham_dinh;
END;
GO

-- ================================================================
-- G. NGOẠI LỆ / PHÊ DUYỆT / GIẤY PHÉP (UC16-19,27)
-- ================================================================
CREATE OR ALTER PROCEDURE dbo.sp_DanhSachHoSoChoQuyetDinh
 @NguoiPheDuyetId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
   WHERE ma_tai_khoan=@NguoiPheDuyetId AND vai_tro='NGUOI_PHE_DUYET'
     AND trang_thai='HOAT_DONG' AND cap_tham_quyen>0)
   THROW 52306,N'Không có thẩm quyền xem danh sách quyết định cấp phép.',1;

 SELECT h.ma_ho_so,h.ten_su_kien,h.trang_thai,h.ngay_nop,h.bat_dau,h.ket_thuc,
        h.quy_mo,h.ly_do_tu_choi,d.ten_dia_diem,d.dia_chi,d.suc_chua,
        CASE WHEN h.trang_thai='CHO_PHE_DUYET'
               AND dbo.fn_DuDieuKienPheDuyet(h.ma_ho_so)=1 THEN CAST(1 AS BIT)
             ELSE CAST(0 AS BIT) END AS du_dieu_kien_phe_duyet,
        (SELECT COUNT(*) FROM dbo.KET_QUA_KIEM_TRA k
         WHERE k.ma_ho_so=h.ma_ho_so) AS so_quy_tac_da_danh_gia,
        (SELECT COUNT(*) FROM dbo.KET_QUA_KIEM_TRA k
         JOIN dbo.QUY_TAC r ON r.quy_tac_id=k.quy_tac_id
         WHERE k.ma_ho_so=h.ma_ho_so AND r.muc_do='BAT_BUOC'
           AND k.ket_qua='KHONG_DAT') AS so_khong_dat_bat_buoc,
        (SELECT COUNT(*) FROM dbo.NGOAI_LE n
         WHERE n.ma_ho_so=h.ma_ho_so AND n.trang_thai='DA_CHAP_THUAN')
           AS so_ngoai_le_duoc_chap_thuan,
        (SELECT COUNT(*) FROM dbo.YEU_CAU_Y_KIEN y
         WHERE y.ma_ho_so=h.ma_ho_so AND y.trang_thai='DA_PHAN_HOI')
           AS so_y_kien_da_phan_hoi,
        gp.so_giay_phep
 FROM dbo.HO_SO_CAP_PHEP h
 LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem=h.ma_dia_diem
 LEFT JOIN dbo.GIAY_PHEP gp ON gp.ma_ho_so=h.ma_ho_so
 WHERE h.trang_thai IN ('CHO_PHE_DUYET','TONG_HOP_LIEN_NGANH')
 ORDER BY CASE WHEN h.trang_thai='CHO_PHE_DUYET' THEN 0 ELSE 1 END,
          h.ngay_nop,h.ma_ho_so;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DanhSachGiayPhep
 @NguoiPheDuyetId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
   WHERE ma_tai_khoan=@NguoiPheDuyetId AND vai_tro='NGUOI_PHE_DUYET'
     AND trang_thai='HOAT_DONG' AND cap_tham_quyen>0)
   THROW 52306,N'Không có thẩm quyền xem danh sách giấy phép.',1;

 SELECT TOP (500) gp.ma_giay_phep,gp.ma_ho_so,gp.so_giay_phep,
        gp.ngay_cap,gp.hieu_luc_tu,gp.hieu_luc_den,gp.dieu_kien_kem_theo,
        h.ten_su_kien,d.ten_dia_diem
 FROM dbo.GIAY_PHEP gp
 JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=gp.ma_ho_so
 LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem=h.ma_dia_diem
 ORDER BY gp.ngay_cap DESC,gp.ma_giay_phep DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DanhSachKetQuaDuocDeXuatNgoaiLe
 @MaHoSo BIGINT,@NguoiXuLyId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP h
   JOIN dbo.TAI_KHOAN t ON t.ma_tai_khoan=@NguoiXuLyId
   WHERE h.ma_ho_so=@MaHoSo AND h.nguoi_xu_ly_hien_tai_id=@NguoiXuLyId
     AND h.trang_thai='XU_LY_NGOAI_LE'
     AND t.vai_tro='NGUOI_XU_LY' AND t.trang_thai='HOAT_DONG')
   THROW 52303,N'Hồ sơ không ở bước xử lý ngoại lệ hoặc không được phân công cho bạn.',1;
 IF dbo.fn_DuYKienBatBuoc(@MaHoSo)=0
   THROW 52302,N'Chưa nhận đủ ý kiến liên ngành bắt buộc.',1;

 SELECT k.ma_ket_qua,r.ma_quy_tac,r.phien_ban,r.nhom,r.cap_tham_quyen,
        k.ket_qua,k.ghi_chu
 FROM dbo.KET_QUA_KIEM_TRA k
 JOIN dbo.QUY_TAC r ON r.quy_tac_id=k.quy_tac_id
 WHERE k.ma_ho_so=@MaHoSo AND k.ket_qua='KHONG_DAT'
   AND r.muc_do='BAT_BUOC' AND r.cho_phep_ngoai_le=1
   AND r.cap_tham_quyen IS NOT NULL
   AND NOT EXISTS(SELECT 1 FROM dbo.NGOAI_LE n WHERE n.ma_ket_qua=k.ma_ket_qua)
 ORDER BY r.cap_tham_quyen,r.ma_quy_tac,r.phien_ban;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DanhSachNgoaiLeChoPheDuyet
 @NguoiPheDuyetId BIGINT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN
   WHERE ma_tai_khoan=@NguoiPheDuyetId AND vai_tro='NGUOI_PHE_DUYET'
     AND trang_thai='HOAT_DONG' AND cap_tham_quyen>0)
   THROW 52305,N'Tài khoản không đủ quyền xem đề xuất ngoại lệ.',1;

 SELECT n.ma_ngoai_le,n.ma_ho_so,n.ma_ket_qua,n.nguoi_de_xuat_id,
        proposer.ho_ten AS ten_nguoi_de_xuat,n.ly_do,n.minh_chung,
        n.bien_phap_bu_dap,n.cap_tham_quyen_yeu_cau,n.ngay_de_xuat,
        r.ma_quy_tac,r.phien_ban,r.nhom,k.ghi_chu AS ghi_chu_tham_dinh,
        h.ten_su_kien
 FROM dbo.NGOAI_LE n
 JOIN dbo.TAI_KHOAN proposer ON proposer.ma_tai_khoan=n.nguoi_de_xuat_id
 JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=n.ma_ho_so
 JOIN dbo.KET_QUA_KIEM_TRA k ON k.ma_ket_qua=n.ma_ket_qua
 JOIN dbo.QUY_TAC r ON r.quy_tac_id=k.quy_tac_id
 JOIN dbo.TAI_KHOAN approver ON approver.ma_tai_khoan=@NguoiPheDuyetId
 WHERE n.trang_thai='CHO_DUYET'
   AND h.trang_thai='XU_LY_NGOAI_LE'
   AND k.ket_qua='KHONG_DAT' AND r.muc_do='BAT_BUOC'
   AND r.cho_phep_ngoai_le=1
   AND r.cap_tham_quyen=n.cap_tham_quyen_yeu_cau
   AND n.nguoi_de_xuat_id<>@NguoiPheDuyetId
   AND approver.cap_tham_quyen>=n.cap_tham_quyen_yeu_cau
 ORDER BY n.cap_tham_quyen_yeu_cau,n.ngay_de_xuat,n.ma_ngoai_le;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DeXuatNgoaiLe
 @MaHoSo BIGINT,@MaKetQua BIGINT,@NguoiXuLyId BIGINT,
 @LyDo NVARCHAR(MAX),@MinhChung NVARCHAR(MAX),@BienPhapBuDap NVARCHAR(MAX)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NULLIF(LTRIM(RTRIM(@LyDo)),N'') IS NULL
 OR NULLIF(LTRIM(RTRIM(@MinhChung)),N'') IS NULL
 OR NULLIF(LTRIM(RTRIM(@BienPhapBuDap)),N'') IS NULL
  THROW 52301,N'Cần đủ lý do, minh chứng và biện pháp bù đắp.',1;
 DECLARE @Cap SMALLINT,@TrangThai VARCHAR(30),@LockSink BIGINT;
 BEGIN TRY
 BEGIN TRANSACTION;
 SELECT @TrangThai=h.trang_thai
 FROM dbo.HO_SO_CAP_PHEP h WITH(UPDLOCK,HOLDLOCK)
 WHERE h.ma_ho_so=@MaHoSo AND h.nguoi_xu_ly_hien_tai_id=@NguoiXuLyId;
 IF @TrangThai<>'XU_LY_NGOAI_LE' OR @TrangThai IS NULL
  THROW 52303,N'Hồ sơ không ở bước xử lý ngoại lệ hoặc không được phân công cho bạn.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WITH(UPDLOCK,HOLDLOCK)
   WHERE ma_tai_khoan=@NguoiXuLyId AND vai_tro='NGUOI_XU_LY'
     AND trang_thai='HOAT_DONG')
  THROW 52303,N'Tài khoản Người xử lý không hoạt động hoặc không hợp lệ.',1;
 SELECT @LockSink=MAX(c.ma_cau_hinh)
 FROM dbo.CAU_HINH_LIEN_NGANH c WITH(UPDLOCK,HOLDLOCK)
 JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=@MaHoSo
 WHERE c.ma_loai_su_kien=h.ma_loai_su_kien AND c.bat_buoc=1 AND c.trang_thai=1
   AND c.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop)
   AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den>=CONVERT(DATE,h.ngay_nop));
 SELECT @LockSink=MAX(y.ma_yeu_cau)
 FROM dbo.YEU_CAU_Y_KIEN y WITH(UPDLOCK,HOLDLOCK)
 WHERE y.ma_ho_so=@MaHoSo;
 IF dbo.fn_DuYKienBatBuoc(@MaHoSo)=0
  THROW 52302,N'Chưa nhận đủ ý kiến liên ngành bắt buộc.',1;
 SELECT @Cap=r.cap_tham_quyen
 FROM dbo.KET_QUA_KIEM_TRA k WITH(UPDLOCK,HOLDLOCK)
 JOIN dbo.QUY_TAC r WITH(HOLDLOCK) ON r.quy_tac_id=k.quy_tac_id
 WHERE k.ma_ket_qua=@MaKetQua AND k.ma_ho_so=@MaHoSo
   AND k.ket_qua='KHONG_DAT' AND r.muc_do='BAT_BUOC'
   AND r.cho_phep_ngoai_le=1 AND r.cap_tham_quyen IS NOT NULL;
 IF @Cap IS NULL THROW 52303,N'Chỉ được đề xuất ngoại lệ cho quy tắc BẮT BUỘC không đạt có cho phép ngoại lệ.',1;
 INSERT dbo.NGOAI_LE(ma_ho_so,ma_ket_qua,nguoi_de_xuat_id,ly_do,minh_chung,bien_phap_bu_dap,cap_tham_quyen_yeu_cau)
 VALUES(@MaHoSo,@MaKetQua,@NguoiXuLyId,LTRIM(RTRIM(@LyDo)),
        LTRIM(RTRIM(@MinhChung)),LTRIM(RTRIM(@BienPhapBuDap)),@Cap);
 DECLARE @MaNgoaiLe BIGINT=CONVERT(BIGINT,SCOPE_IDENTITY());
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,
                          tu_trang_thai,den_trang_thai,ghi_chu)
 VALUES(@MaHoSo,@NguoiXuLyId,N'Đề xuất ngoại lệ',
        @TrangThai,@TrangThai,@LyDo);
 INSERT dbo.THONG_BAO(nguoi_nhan_id,ma_ho_so,loai_thong_bao,noi_dung)
 SELECT ma_tai_khoan,@MaHoSo,'CHO_DUYET_NGOAI_LE',N'Có đề xuất ngoại lệ cần xử lý.'
 FROM dbo.TAI_KHOAN WHERE vai_tro='NGUOI_PHE_DUYET' AND trang_thai='HOAT_DONG'
 AND cap_tham_quyen>=@Cap;
 COMMIT TRANSACTION;
 SELECT @MaNgoaiLe AS ma_ngoai_le;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_QuyetDinhNgoaiLe
 @MaNgoaiLe BIGINT,@NguoiPheDuyetId BIGINT,@ChapThuan BIT,@GhiChu NVARCHAR(MAX)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF @ChapThuan IS NULL THROW 52315,N'Phải chọn chấp thuận hoặc từ chối ngoại lệ.',1;
 IF NULLIF(LTRIM(RTRIM(@GhiChu)),N'') IS NULL
  THROW 52315,N'Phải ghi lý do/căn cứ cho quyết định ngoại lệ.',1;
 DECLARE @MaHoSo BIGINT,@HoSoBanDau BIGINT,@MaKetQua BIGINT,
         @CapYeuCau SMALLINT,@NguoiDeXuat BIGINT,
         @TrangThaiNgoaiLe VARCHAR(20),@TrangThaiHoSo VARCHAR(30),
         @TrangThaiMoi VARCHAR(30),@LockSink BIGINT;
 SELECT @HoSoBanDau=ma_ho_so FROM dbo.NGOAI_LE WHERE ma_ngoai_le=@MaNgoaiLe;
 SET @MaHoSo=@HoSoBanDau;
 IF @MaHoSo IS NULL THROW 52304,N'Đề xuất ngoại lệ không tồn tại.',1;
 BEGIN TRY
 BEGIN TRANSACTION;
 SELECT @TrangThaiHoSo=trang_thai
 FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
 WHERE ma_ho_so=@MaHoSo;
 IF @TrangThaiHoSo<>'XU_LY_NGOAI_LE' OR @TrangThaiHoSo IS NULL
  THROW 52316,N'Hồ sơ không còn ở trạng thái chờ xử lý ngoại lệ.',1;
 SELECT @MaHoSo=ma_ho_so,@MaKetQua=ma_ket_qua,@CapYeuCau=cap_tham_quyen_yeu_cau,
        @NguoiDeXuat=nguoi_de_xuat_id,@TrangThaiNgoaiLe=trang_thai
 FROM dbo.NGOAI_LE WITH(UPDLOCK,HOLDLOCK) WHERE ma_ngoai_le=@MaNgoaiLe;
 IF @TrangThaiNgoaiLe IS NULL OR @TrangThaiNgoaiLe<>'CHO_DUYET'
    OR @MaHoSo<>@HoSoBanDau
  THROW 52304,N'Ngoại lệ không còn ở trạng thái chờ duyệt.',1;
 IF @NguoiPheDuyetId=@NguoiDeXuat OR NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WITH(UPDLOCK,HOLDLOCK)
   WHERE ma_tai_khoan=@NguoiPheDuyetId AND vai_tro='NGUOI_PHE_DUYET'
   AND trang_thai='HOAT_DONG' AND cap_tham_quyen>=@CapYeuCau)
  THROW 52305,N'Không đủ thẩm quyền hoặc trùng người đề xuất.',1;
 SELECT @LockSink=MAX(c.ma_cau_hinh)
 FROM dbo.CAU_HINH_LIEN_NGANH c WITH(UPDLOCK,HOLDLOCK)
 JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=@MaHoSo
 WHERE c.ma_loai_su_kien=h.ma_loai_su_kien AND c.bat_buoc=1 AND c.trang_thai=1
   AND c.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop)
   AND (c.hieu_luc_den IS NULL OR c.hieu_luc_den>=CONVERT(DATE,h.ngay_nop));
 SELECT @LockSink=MAX(y.ma_yeu_cau)
 FROM dbo.YEU_CAU_Y_KIEN y WITH(UPDLOCK,HOLDLOCK)
 WHERE y.ma_ho_so=@MaHoSo;
 IF NOT EXISTS(
   SELECT 1 FROM dbo.KET_QUA_KIEM_TRA k WITH(UPDLOCK,HOLDLOCK)
   JOIN dbo.QUY_TAC r WITH(HOLDLOCK) ON r.quy_tac_id=k.quy_tac_id
   WHERE k.ma_ket_qua=@MaKetQua
     AND k.ma_ho_so=@MaHoSo AND k.ket_qua='KHONG_DAT'
     AND r.muc_do='BAT_BUOC' AND r.cho_phep_ngoai_le=1
     AND r.cap_tham_quyen=@CapYeuCau)
  THROW 52303,N'Kết quả thẩm định gốc không còn đủ điều kiện xét ngoại lệ.',1;
 UPDATE dbo.NGOAI_LE SET trang_thai=CASE WHEN @ChapThuan=1 THEN 'DA_CHAP_THUAN' ELSE 'BI_TU_CHOI' END,
  nguoi_phe_duyet_id=@NguoiPheDuyetId,ngay_quyet_dinh=SYSDATETIMEOFFSET(),
  ghi_chu_quyet_dinh=LTRIM(RTRIM(@GhiChu))
 WHERE ma_ngoai_le=@MaNgoaiLe;
 IF NOT EXISTS(SELECT 1 FROM dbo.NGOAI_LE WITH(UPDLOCK,HOLDLOCK)
               WHERE ma_ho_so=@MaHoSo AND trang_thai='CHO_DUYET')
 BEGIN
  IF EXISTS(
    SELECT 1 FROM dbo.KET_QUA_KIEM_TRA k
    JOIN dbo.QUY_TAC r ON r.quy_tac_id=k.quy_tac_id
    WHERE k.ma_ho_so=@MaHoSo AND r.muc_do='BAT_BUOC' AND k.ket_qua='KHONG_DAT'
      AND NOT EXISTS(SELECT 1 FROM dbo.NGOAI_LE n
                     WHERE n.ma_ket_qua=k.ma_ket_qua AND n.trang_thai='DA_CHAP_THUAN'))
   SET @TrangThaiMoi='TONG_HOP_LIEN_NGANH';
  ELSE
   SET @TrangThaiMoi='CHO_PHE_DUYET';
  UPDATE dbo.HO_SO_CAP_PHEP SET trang_thai=@TrangThaiMoi
  WHERE ma_ho_so=@MaHoSo AND trang_thai='XU_LY_NGOAI_LE';
  IF @@ROWCOUNT<>1 THROW 52316,N'Hồ sơ đã đổi trạng thái trong khi quyết định ngoại lệ.',1;
 END;
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai,ket_qua,ghi_chu)
 VALUES(@MaHoSo,@NguoiPheDuyetId,N'Quyết định ngoại lệ',@TrangThaiHoSo,
        COALESCE(@TrangThaiMoi,@TrangThaiHoSo),
        CASE WHEN @ChapThuan=1 THEN N'DA_CHAP_THUAN' ELSE N'BI_TU_CHOI' END,@GhiChu);
 INSERT dbo.THONG_BAO(nguoi_nhan_id,ma_ho_so,loai_thong_bao,noi_dung)
 SELECT nguoi_to_chuc_id,ma_ho_so,'KET_QUA_NGOAI_LE',
 CASE WHEN @ChapThuan=1 THEN N'Ngoại lệ đã được chấp thuận.' ELSE N'Ngoại lệ bị từ chối.' END
 FROM dbo.HO_SO_CAP_PHEP WHERE ma_ho_so=@MaHoSo;
 COMMIT TRANSACTION;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PheDuyetCapGiayPhep
 @MaHoSo BIGINT,@NguoiPheDuyetId BIGINT,@DieuKienKemTheo NVARCHAR(MAX)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DECLARE @BatDau DATETIMEOFFSET(0),@KetThuc DATETIMEOFFSET(0),
         @MaDiaDiem BIGINT,@QuyMo INT,@SoGiayPhep VARCHAR(80),@LockName NVARCHAR(255),@LockResult INT,
         @TrangThaiHienTai VARCHAR(30);
 BEGIN TRY
 BEGIN TRANSACTION;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@NguoiPheDuyetId
      AND vai_tro='NGUOI_PHE_DUYET' AND trang_thai='HOAT_DONG' AND cap_tham_quyen>0)
  THROW 52306,N'Không có thẩm quyền phê duyệt.',1;
 SELECT @BatDau=bat_dau,@KetThuc=ket_thuc,@MaDiaDiem=ma_dia_diem,
        @QuyMo=quy_mo,@TrangThaiHienTai=trang_thai
 FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK) WHERE ma_ho_so=@MaHoSo;
 IF @TrangThaiHienTai IS NULL OR @TrangThaiHienTai<>'CHO_PHE_DUYET'
  THROW 52316,N'Hồ sơ đã đổi trạng thái khi phê duyệt.',1;
 IF dbo.fn_DuDieuKienPheDuyet(@MaHoSo)=0
  THROW 52307,N'Hồ sơ chưa hoàn tất thẩm định/ngoại lệ hoặc ý kiến liên ngành chưa đủ; không thể cấp phép.',1;
 IF @BatDau<=SYSDATETIMEOFFSET() THROW 52308,N'Sự kiện đã bắt đầu.',1;
 SET @LockName=CONCAT(N'VIEWPARTY_DIA_DIEM_',CONVERT(NVARCHAR(30),@MaDiaDiem));
 EXEC @LockResult=sys.sp_getapplock @Resource=@LockName,@LockMode='Exclusive',
                       @LockOwner='Transaction',@LockTimeout=10000;
 IF @LockResult<0 THROW 52309,N'Không khóa được lịch địa điểm.',1;
 IF dbo.fn_DuSucChua(@MaDiaDiem,@QuyMo)=0
  OR dbo.fn_TrungLichDiaDiem(@MaDiaDiem,@BatDau,@KetThuc,@MaHoSo)=1
  THROW 52310,N'Địa điểm không còn hợp lệ.',1;
 IF EXISTS(SELECT 1 FROM dbo.GIAY_PHEP WHERE ma_ho_so=@MaHoSo)
  THROW 52311,N'Hồ sơ đã được cấp giấy phép.',1;
 SET @SoGiayPhep=CONCAT('VP-',DATEPART(YEAR,SYSDATETIMEOFFSET()),'-',@MaHoSo);
 INSERT dbo.GIAY_PHEP(ma_ho_so,nguoi_phe_duyet_id,so_giay_phep,
 ngay_cap,hieu_luc_tu,hieu_luc_den,dieu_kien_kem_theo)
 VALUES(@MaHoSo,@NguoiPheDuyetId,@SoGiayPhep,
 CONVERT(DATE,SYSDATETIMEOFFSET()),CONVERT(DATE,@BatDau),CONVERT(DATE,@KetThuc),@DieuKienKemTheo);
 UPDATE dbo.HO_SO_CAP_PHEP SET trang_thai='DA_CAP_PHEP'
 WHERE ma_ho_so=@MaHoSo AND trang_thai='CHO_PHE_DUYET';
 IF @@ROWCOUNT<>1 THROW 52316,N'Hồ sơ đã đổi trạng thái khi cấp giấy phép.',1;
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai)
 VALUES(@MaHoSo,@NguoiPheDuyetId,N'Phê duyệt và cấp giấy phép','CHO_PHE_DUYET','DA_CAP_PHEP');
 INSERT dbo.THONG_BAO(nguoi_nhan_id,ma_ho_so,loai_thong_bao,noi_dung)
 SELECT nguoi_to_chuc_id,ma_ho_so,'CAP_PHEP',CONCAT(N'Giấy phép đã cấp: ',@SoGiayPhep)
 FROM dbo.HO_SO_CAP_PHEP WHERE ma_ho_so=@MaHoSo;
 COMMIT TRANSACTION;
 SELECT @MaHoSo AS ma_ho_so,@SoGiayPhep AS so_giay_phep;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_TuChoiCapPhep
 @MaHoSo BIGINT,@NguoiPheDuyetId BIGINT,@LyDo NVARCHAR(MAX)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NULLIF(LTRIM(RTRIM(@LyDo)),N'') IS NULL
  THROW 52312,N'Phải nhập lý do từ chối.',1;
 DECLARE @TrangThaiCu VARCHAR(30);
 BEGIN TRY
 BEGIN TRANSACTION;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WITH(UPDLOCK,HOLDLOCK)
  WHERE ma_tai_khoan=@NguoiPheDuyetId AND vai_tro='NGUOI_PHE_DUYET'
  AND trang_thai='HOAT_DONG' AND cap_tham_quyen>0)
  THROW 52313,N'Không đủ quyền từ chối hồ sơ.',1;
 SELECT @TrangThaiCu=trang_thai FROM dbo.HO_SO_CAP_PHEP WITH(UPDLOCK,HOLDLOCK)
 WHERE ma_ho_so=@MaHoSo;
 IF @TrangThaiCu NOT IN ('CHO_PHE_DUYET','TONG_HOP_LIEN_NGANH') OR @TrangThaiCu IS NULL
  THROW 52314,N'Hồ sơ chưa đến bước quyết định từ chối.',1;
 IF dbo.fn_DuYKienBatBuoc(@MaHoSo)=0
  THROW 52307,N'Chưa nhận đủ ý kiến liên ngành bắt buộc; không thể ra quyết định.',1;
 UPDATE dbo.HO_SO_CAP_PHEP SET trang_thai='TU_CHOI',ly_do_tu_choi=LTRIM(RTRIM(@LyDo))
 WHERE ma_ho_so=@MaHoSo AND trang_thai=@TrangThaiCu;
 IF @@ROWCOUNT<>1 THROW 52316,N'Hồ sơ đã đổi trạng thái khi từ chối.',1;
 INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai,ghi_chu)
 VALUES(@MaHoSo,@NguoiPheDuyetId,N'Từ chối cấp phép',@TrangThaiCu,'TU_CHOI',@LyDo);
 INSERT dbo.THONG_BAO(nguoi_nhan_id,ma_ho_so,loai_thong_bao,noi_dung)
 SELECT nguoi_to_chuc_id,ma_ho_so,'TU_CHOI',@LyDo
 FROM dbo.HO_SO_CAP_PHEP WHERE ma_ho_so=@MaHoSo;
 COMMIT TRANSACTION;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

-- ================================================================
-- H. QUẢN TRỊ LOẠI SỰ KIỆN, QUY TẮC, LIÊN NGÀNH, THẨM QUYỀN
-- (UC21,22,23,24,25)
-- ================================================================
CREATE OR ALTER PROCEDURE dbo.sp_QuanTriLoaiSuKien
 @AdminId BIGINT,@TenLoai NVARCHAR(150),@MoTa NVARCHAR(MAX)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@AdminId
 AND vai_tro='QUAN_TRI_VIEN' AND trang_thai='HOAT_DONG')
  THROW 52401,N'Không có quyền quản trị loại sự kiện.',1;
 BEGIN TRANSACTION;
 IF NOT EXISTS(SELECT 1 FROM dbo.LOAI_SU_KIEN WHERE ten_loai=@TenLoai)
  INSERT dbo.LOAI_SU_KIEN(ten_loai,mo_ta) VALUES(@TenLoai,@MoTa);
 ELSE
  UPDATE dbo.LOAI_SU_KIEN SET mo_ta=COALESCE(@MoTa,mo_ta) WHERE ten_loai=@TenLoai;
 INSERT dbo.NHAT_KY_CAU_HINH(ma_tai_khoan,doi_tuong,khoa_doi_tuong,hanh_dong,gia_tri_sau)
 VALUES(@AdminId,N'LOAI_SU_KIEN',@TenLoai,N'CAP_NHAT',
  (SELECT ten_loai,mo_ta,trang_thai FROM dbo.LOAI_SU_KIEN WHERE ten_loai=@TenLoai
   FOR JSON PATH,WITHOUT_ARRAY_WRAPPER));
 COMMIT TRANSACTION;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_QuanTriQuyTacPhienBan
 @AdminId BIGINT,@MaQuyTac VARCHAR(60),@MaLoaiSuKien BIGINT,
 @Nhom NVARCHAR(80),@MucDo VARCHAR(20),@DieuKienApDung NVARCHAR(MAX),
 @DieuKienDat NVARCHAR(MAX),@ChoPhepNgoaiLe BIT,
 @HieuLucTu DATE,@CapThamQuyen SMALLINT=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@AdminId
    AND vai_tro='QUAN_TRI_VIEN' AND trang_thai='HOAT_DONG')
  THROW 52402,N'Không có quyền cấu hình quy tắc.',1;
 IF ISJSON(@DieuKienApDung)<>1 OR ISJSON(@DieuKienDat)<>1
  THROW 52403,N'Điều kiện quy tắc phải là JSON.',1;
 IF @ChoPhepNgoaiLe=1 AND (@CapThamQuyen IS NULL OR @CapThamQuyen<1)
  THROW 52404,N'Quy tắc ngoại lệ phải có cấp thẩm quyền.',1;
 DECLARE @PhienBan INT,@LockResult INT,@LockName NVARCHAR(255);
 BEGIN TRY
 BEGIN TRANSACTION;
 SET @LockName=CONCAT(N'VIEWPARTY_QUY_TAC_',@MaQuyTac);
 EXEC @LockResult=sys.sp_getapplock
 @Resource=@LockName,@LockMode='Exclusive',
 @LockOwner='Transaction',@LockTimeout=10000;
 IF @LockResult<0 THROW 52405,N'Không khóa được quy tắc để tạo phiên bản.',1;
 SELECT @PhienBan=ISNULL(MAX(phien_ban),0)+1 FROM dbo.QUY_TAC WHERE ma_quy_tac=@MaQuyTac;
 IF EXISTS(SELECT 1 FROM dbo.QUY_TAC
  WHERE ma_quy_tac=@MaQuyTac AND hieu_luc_tu>=@HieuLucTu)
  THROW 52412,N'Phiên bản mới phải có ngày hiệu lực sau phiên bản trước.',1;
 -- Kết thúc kỳ áp dụng cũ; vẫn giữ bản ghi để truy vết.
 UPDATE dbo.QUY_TAC SET hieu_luc_den=DATEADD(DAY,-1,@HieuLucTu)
 WHERE ma_quy_tac=@MaQuyTac AND hieu_luc_den IS NULL;
 INSERT dbo.QUY_TAC(ma_quy_tac,phien_ban,ma_loai_su_kien,nhom,muc_do,
 dieu_kien_ap_dung,dieu_kien_dat,cho_phep_ngoai_le,cap_tham_quyen,hieu_luc_tu)
 VALUES(@MaQuyTac,@PhienBan,@MaLoaiSuKien,@Nhom,@MucDo,
 @DieuKienApDung,@DieuKienDat,@ChoPhepNgoaiLe,@CapThamQuyen,@HieuLucTu);
 DECLARE @RuleId BIGINT=CONVERT(BIGINT,SCOPE_IDENTITY());
 INSERT dbo.NHAT_KY_CAU_HINH(ma_tai_khoan,doi_tuong,khoa_doi_tuong,hanh_dong,gia_tri_sau)
 VALUES(@AdminId,N'QUY_TAC',CONVERT(NVARCHAR(30),@RuleId),N'TAO_PHIEN_BAN',
  (SELECT ma_quy_tac,phien_ban,muc_do,cho_phep_ngoai_le FROM dbo.QUY_TAC
   WHERE quy_tac_id=@RuleId FOR JSON PATH,WITHOUT_ARRAY_WRAPPER));
 COMMIT TRANSACTION;
 SELECT @RuleId AS quy_tac_id,@PhienBan AS phien_ban;
 END TRY BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_QuanTriCauHinhLienNganh
 @AdminId BIGINT,@MaLoaiSuKien BIGINT,@MaDonVi BIGINT,
 @BatBuoc BIT,@SoNgayPhanHoi INT,@HieuLucTu DATE
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@AdminId
  AND vai_tro='QUAN_TRI_VIEN' AND trang_thai='HOAT_DONG')
  THROW 52406,N'Không có quyền cấu hình liên ngành.',1;
 IF @SoNgayPhanHoi<=0 THROW 52407,N'Hạn phản hồi không hợp lệ.',1;
 BEGIN TRANSACTION;
 IF EXISTS(SELECT 1 FROM dbo.CAU_HINH_LIEN_NGANH
   WHERE ma_loai_su_kien=@MaLoaiSuKien AND ma_don_vi=@MaDonVi
     AND hieu_luc_tu>=@HieuLucTu)
 BEGIN
  ROLLBACK TRANSACTION;
  THROW 52413,N'Cấu hình mới phải có ngày hiệu lực sau cấu hình cũ.',1;
 END;
 UPDATE dbo.CAU_HINH_LIEN_NGANH SET hieu_luc_den=DATEADD(DAY,-1,@HieuLucTu)
 WHERE ma_loai_su_kien=@MaLoaiSuKien AND ma_don_vi=@MaDonVi AND hieu_luc_den IS NULL;
 INSERT dbo.CAU_HINH_LIEN_NGANH(ma_loai_su_kien,ma_don_vi,bat_buoc,so_ngay_phan_hoi,hieu_luc_tu)
 VALUES(@MaLoaiSuKien,@MaDonVi,@BatBuoc,@SoNgayPhanHoi,@HieuLucTu);
 DECLARE @Id BIGINT=CONVERT(BIGINT,SCOPE_IDENTITY());
 INSERT dbo.NHAT_KY_CAU_HINH(ma_tai_khoan,doi_tuong,khoa_doi_tuong,hanh_dong,gia_tri_sau)
 VALUES(@AdminId,N'CAU_HINH_LIEN_NGANH',CONVERT(NVARCHAR(30),@Id),N'TAO',
  (SELECT ma_loai_su_kien,ma_don_vi,bat_buoc,so_ngay_phan_hoi
   FROM dbo.CAU_HINH_LIEN_NGANH WHERE ma_cau_hinh=@Id FOR JSON PATH,WITHOUT_ARRAY_WRAPPER));
 COMMIT TRANSACTION;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GanThamQuyenPheDuyet
 @AdminId BIGINT,@MaTaiKhoan BIGINT,@CapThamQuyen SMALLINT
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@AdminId
  AND vai_tro='QUAN_TRI_VIEN' AND trang_thai='HOAT_DONG')
  THROW 52408,N'Không có quyền gán người phê duyệt.',1;
 IF @CapThamQuyen<1 THROW 52409,N'Cấp thẩm quyền không hợp lệ.',1;
 BEGIN TRANSACTION;
 UPDATE dbo.TAI_KHOAN SET vai_tro='NGUOI_PHE_DUYET',cap_tham_quyen=@CapThamQuyen
 WHERE ma_tai_khoan=@MaTaiKhoan;
 IF @@ROWCOUNT<>1
 BEGIN
  ROLLBACK TRANSACTION;
  THROW 52410,N'Tài khoản cần gán không tồn tại.',1;
 END;
 INSERT dbo.NHAT_KY_CAU_HINH(ma_tai_khoan,doi_tuong,khoa_doi_tuong,hanh_dong,gia_tri_sau)
 VALUES(@AdminId,N'TAI_KHOAN',CONVERT(NVARCHAR(30),@MaTaiKhoan),N'GAN_THAM_QUYEN',
  (SELECT vai_tro,cap_tham_quyen FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@MaTaiKhoan
   FOR JSON PATH,WITHOUT_ARRAY_WRAPPER));
 COMMIT TRANSACTION;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_XemLichSuVaNhatKy
 @AdminId BIGINT,@MaHoSo BIGINT=NULL
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@AdminId
  AND vai_tro='QUAN_TRI_VIEN' AND trang_thai='HOAT_DONG')
  THROW 52411,N'Không có quyền xem lịch sử và nhật ký.',1;
 SELECT * FROM dbo.LICH_SU_XU_LY WHERE @MaHoSo IS NULL OR ma_ho_so=@MaHoSo ORDER BY thoi_diem DESC;
 SELECT * FROM dbo.NHAT_KY_CAU_HINH ORDER BY thoi_diem DESC;
END;
GO

-- ================================================================
-- I. THÔNG BÁO: DB CHỈ XẾP HÀNG VÀ GHI KẾT QUẢ, KHÔNG GỬI EMAIL
-- ================================================================
CREATE OR ALTER PROCEDURE dbo.sp_LayThongBaoChoGui @SoLuong INT=50
AS
BEGIN
 SET NOCOUNT ON;
 IF @SoLuong NOT BETWEEN 1 AND 500 THROW 52501,N'Số lượng thông báo không hợp lệ.',1;
 SELECT TOP (@SoLuong) ma_thong_bao,nguoi_nhan_id,ma_ho_so,loai_thong_bao,noi_dung,so_lan_thu
 FROM dbo.THONG_BAO WHERE trang_thai IN ('CHO_GUI','GUI_THAT_BAI') AND so_lan_thu<5
 ORDER BY thoi_diem_tao,ma_thong_bao;
 -- Backend worker phải dùng quy trình nhận việc/claim an toàn khi chạy đa tiến trình.
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_CapNhatKetQuaGuiThongBao
 @MaThongBao BIGINT,@GuiThanhCong BIT
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF @GuiThanhCong IS NULL THROW 52503,N'Kết quả gửi không hợp lệ.',1;
 UPDATE dbo.THONG_BAO SET
  trang_thai=CASE WHEN @GuiThanhCong=1 THEN 'DA_GUI' ELSE 'GUI_THAT_BAI' END,
  thoi_diem_gui=CASE WHEN @GuiThanhCong=1 THEN SYSDATETIMEOFFSET() ELSE NULL END,
  so_lan_thu=so_lan_thu+1
 WHERE ma_thong_bao=@MaThongBao AND trang_thai<>'DA_GUI';
 IF @@ROWCOUNT<>1 THROW 52502,N'Thông báo không tồn tại hoặc đã gửi.',1;
END;
GO

-- ================================================================
-- J. TRIGGER: GIỮ LỊCH SỬ BẤT BIẾN + TÍNH NHẤT QUÁN DỮ LIỆU
-- ================================================================
CREATE OR ALTER TRIGGER dbo.tr_LichSuXuLy_BatBien
ON dbo.LICH_SU_XU_LY AFTER UPDATE,DELETE
AS
BEGIN
 SET NOCOUNT ON;
 THROW 52601,N'Lịch sử xử lý chỉ thêm bản ghi, không sửa hoặc xóa.',1;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_NhatKyCauHinh_BatBien
ON dbo.NHAT_KY_CAU_HINH AFTER UPDATE,DELETE
AS
BEGIN
 SET NOCOUNT ON;
 THROW 52602,N'Nhật ký cấu hình chỉ thêm bản ghi, không sửa hoặc xóa.',1;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_NgoaiLe_KhopKetQua
ON dbo.NGOAI_LE AFTER INSERT,UPDATE
AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i
 LEFT JOIN dbo.KET_QUA_KIEM_TRA k ON k.ma_ket_qua=i.ma_ket_qua
 LEFT JOIN dbo.QUY_TAC r ON r.quy_tac_id=k.quy_tac_id
 WHERE k.ma_ket_qua IS NULL OR k.ma_ho_so<>i.ma_ho_so
    OR k.ket_qua<>'KHONG_DAT' OR r.muc_do<>'BAT_BUOC' OR r.cho_phep_ngoai_le<>1
    OR i.cap_tham_quyen_yeu_cau<>r.cap_tham_quyen)
  THROW 52603,N'Ngoại lệ phải trỏ đến kết quả bắt buộc không đạt, được phép ngoại lệ.',1;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_YeuCauYKien_KhopCauHinh
ON dbo.YEU_CAU_Y_KIEN AFTER INSERT,UPDATE
AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i
 JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=i.ma_ho_so
 JOIN dbo.CAU_HINH_LIEN_NGANH c ON c.ma_cau_hinh=i.ma_cau_hinh
 WHERE c.ma_loai_su_kien<>h.ma_loai_su_kien OR c.ma_don_vi<>i.ma_don_vi)
  THROW 52604,N'Đơn vị/cấu hình liên ngành không khớp hồ sơ.',1;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_HoSo_ChongTrungLich
ON dbo.HO_SO_CAP_PHEP AFTER INSERT,UPDATE
AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS (
  SELECT 1 FROM inserted i JOIN dbo.HO_SO_CAP_PHEP h WITH(UPDLOCK,HOLDLOCK)
   ON h.ma_dia_diem=i.ma_dia_diem AND h.ma_ho_so<>i.ma_ho_so
  WHERE i.trang_thai IN ('DA_NOP','DANG_KIEM_TRA','YEU_CAU_BO_SUNG',
    'DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH','TONG_HOP_LIEN_NGANH',
    'XU_LY_NGOAI_LE','CHO_PHE_DUYET','DA_CAP_PHEP')
    AND h.trang_thai IN ('DA_NOP','DANG_KIEM_TRA','YEU_CAU_BO_SUNG',
    'DANG_THAM_DINH','CHO_Y_KIEN_LIEN_NGANH','TONG_HOP_LIEN_NGANH',
    'XU_LY_NGOAI_LE','CHO_PHE_DUYET','DA_CAP_PHEP')
    AND i.bat_dau<h.ket_thuc AND i.ket_thuc>h.bat_dau
 ) THROW 52605,N'Hồ sơ có xung đột lịch địa điểm.',1;
END;
GO

-- ================================================================
-- K. DANH SÁCH ĐỐI TƯỢNG ĐÃ TẠO
-- ================================================================
SELECT N'FUNCTION' AS Loai,name FROM sys.objects WHERE type IN ('FN','IF','TF') AND schema_id=SCHEMA_ID('dbo')
UNION ALL SELECT N'VIEW',name FROM sys.views WHERE schema_id=SCHEMA_ID('dbo')
UNION ALL SELECT N'PROCEDURE',name FROM sys.procedures WHERE schema_id=SCHEMA_ID('dbo')
UNION ALL SELECT N'TRIGGER',t.name FROM sys.triggers t
 JOIN sys.objects o ON o.object_id=t.object_id
 WHERE t.parent_class=1 AND o.schema_id=SCHEMA_ID('dbo')
ORDER BY Loai,name;
GO

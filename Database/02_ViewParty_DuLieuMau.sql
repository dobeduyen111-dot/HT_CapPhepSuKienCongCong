/*
FILE 02/03 - DỮ LIỆU MẪU VIEW PARTY (SQL Server 2019+)
Chạy SAU 01_ViewParty_CauTruc_16Bang.sql, TRƯỚC file 03.
Dữ liệu chỉ để DEMO, không dùng người thật, không dùng trên production.
Tài khoản demo dùng bcrypt hash của mật khẩu chung Demo@2026! (chỉ ở môi trường lab).
Backend phải BCrypt.Verify(mật khẩu nhập, mat_khau_bam); không so sánh chuỗi thô trong SQL.
Chạy lại hạn chế tạo bản ghi lặp, dựa vào định danh tên/email và tên sự kiện.
*/
USE [ViewPartyDB_UC28];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- 1. BẢNG DANH MỤC
INSERT INTO dbo.DON_VI_LIEN_NGANH(ten_don_vi, linh_vuc)
SELECT v.ten, v.linh_vuc
FROM (VALUES
 (N'Đơn vị An ninh Mẫu', N'An ninh trật tự'),
 (N'Đơn vị PCCC Mẫu', N'PCCC và an toàn')
) v(ten, linh_vuc)
WHERE NOT EXISTS (SELECT 1 FROM dbo.DON_VI_LIEN_NGANH d WHERE d.ten_don_vi=v.ten);

INSERT INTO dbo.LOAI_SU_KIEN(ten_loai, mo_ta)
SELECT v.ten, v.mo_ta
FROM (VALUES
 (N'VIEW_PARTY_ESPORTS', N'Xem chung giải eSports'),
 (N'VIEW_PARTY_THE_THAO', N'Xem chung giải thể thao truyền thống')
) v(ten, mo_ta)
WHERE NOT EXISTS (SELECT 1 FROM dbo.LOAI_SU_KIEN l WHERE l.ten_loai=v.ten);

INSERT INTO dbo.DIA_DIEM(ten_dia_diem, dia_chi, suc_chua)
SELECT v.ten, v.dia_chi, v.suc_chua
FROM (VALUES
 (N'Trung tâm Sự kiện Mẫu A', N'Quận 1, TP.HCM (giả lập)', 4500),
 (N'Nhà thi đấu Mẫu B', N'Thủ Đức, TP.HCM (giả lập)', 8000),
 (N'Sân vận động Mẫu C', N'Bình Thạnh, TP.HCM (giả lập)', 10000)
) v(ten, dia_chi, suc_chua)
WHERE NOT EXISTS (SELECT 1 FROM dbo.DIA_DIEM d WHERE d.ten_dia_diem=v.ten);

DECLARE @Esports BIGINT=(SELECT ma_loai_su_kien FROM dbo.LOAI_SU_KIEN WHERE ten_loai=N'VIEW_PARTY_ESPORTS'),
        @DiaA BIGINT=(SELECT ma_dia_diem FROM dbo.DIA_DIEM WHERE ten_dia_diem=N'Trung tâm Sự kiện Mẫu A'),
        @DiaB BIGINT=(SELECT ma_dia_diem FROM dbo.DIA_DIEM WHERE ten_dia_diem=N'Nhà thi đấu Mẫu B'),
        @DiaC BIGINT=(SELECT ma_dia_diem FROM dbo.DIA_DIEM WHERE ten_dia_diem=N'Sân vận động Mẫu C'),
        @DonViAnNinh BIGINT=(SELECT ma_don_vi FROM dbo.DON_VI_LIEN_NGANH WHERE ten_don_vi=N'Đơn vị An ninh Mẫu'),
        @DonViPCCC BIGINT=(SELECT ma_don_vi FROM dbo.DON_VI_LIEN_NGANH WHERE ten_don_vi=N'Đơn vị PCCC Mẫu');

-- 2. TÀI KHOẢN 5 VAI TRÒ; HASH BCRYPT, KHÔNG PHẢI PASSWORD THÔ
DECLARE @Bcrypt VARCHAR(255)='$2b$10$Ubd4FkvM6IKsug/9sUUmR.LhsIOiRqzykD8up2iw5NkOAFT2HToLy';
INSERT dbo.TAI_KHOAN(ma_don_vi,ten_dang_nhap,mat_khau_bam,ho_ten,email,so_dien_thoai,vai_tro,cap_tham_quyen)
SELECT v.ma_don_vi,v.username,@Bcrypt,v.ho_ten,v.email,NULL,v.vai_tro,v.cap
FROM (VALUES
 (CAST(NULL AS BIGINT),'vp_tochuc1',N'Người tổ chức Demo A','vp_tochuc1@example.test','NGUOI_TO_CHUC',CAST(NULL AS SMALLINT)),
 (NULL,'vp_tochuc2',N'Người tổ chức Demo B','vp_tochuc2@example.test','NGUOI_TO_CHUC',NULL),
 (NULL,'vp_xuly',N'Cán bộ thẩm định Demo','vp_xuly@example.test','NGUOI_XU_LY',NULL),
 (@DonViAnNinh,'vp_liennganh1',N'Cán bộ An ninh Demo','vp_liennganh1@example.test','LIEN_NGANH',NULL),
 (@DonViPCCC,'vp_liennganh2',N'Cán bộ PCCC Demo','vp_liennganh2@example.test','LIEN_NGANH',NULL),
 (NULL,'vp_pheduyet',N'Người phê duyệt Demo','vp_pheduyet@example.test','NGUOI_PHE_DUYET',CAST(2 AS SMALLINT)),
 (NULL,'vp_quantri',N'Quản trị viên Demo','vp_quantri@example.test','QUAN_TRI_VIEN',NULL)
) v(ma_don_vi,username,ho_ten,email,vai_tro,cap)
WHERE NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN t WHERE t.ten_dang_nhap=v.username);

DECLARE @ToChuc BIGINT=(SELECT ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap='vp_tochuc1'),
        @ToChuc2 BIGINT=(SELECT ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap='vp_tochuc2'),
        @XuLy BIGINT=(SELECT ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap='vp_xuly'),
        @Lien1 BIGINT=(SELECT ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap='vp_liennganh1'),
        @Lien2 BIGINT=(SELECT ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap='vp_liennganh2'),
        @PheDuyet BIGINT=(SELECT ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap='vp_pheduyet'),
        @Admin BIGINT=(SELECT ma_tai_khoan FROM dbo.TAI_KHOAN WHERE ten_dang_nhap='vp_quantri');

-- 3. QUY TẮC VÀ CẤU HÌNH LIÊN NGÀNH
INSERT dbo.QUY_TAC(ma_quy_tac,phien_ban,ma_loai_su_kien,nhom,muc_do,dieu_kien_ap_dung,dieu_kien_dat,cho_phep_ngoai_le,cap_tham_quyen,hieu_luc_tu)
SELECT r.ma,1,@Esports,r.nhom,r.muc_do,r.ap_dung,r.dat,r.ngoai_le,r.cap,CONVERT(DATE,'20260101',112)
FROM (VALUES
 ('VP_DIA_DIEM',N'Địa điểm','BAT_BUOC',N'{"truong":"ma_dia_diem"}',N'{"quy_tac":"du_suc_chua_va_khong_trung_lich"}',CAST(0 AS BIT),CAST(NULL AS SMALLINT)),
 ('VP_AN_TOAN',N'An toàn','BAT_BUOC',N'{"truong":"phuong_an_an_toan"}',N'{"quy_tac":"du_phuong_an_an_toan"}',CAST(1 AS BIT),CAST(2 AS SMALLINT)),
 ('VP_QUYEN_CHIEU',N'Quyền trình chiếu','BAT_BUOC',N'{"truong":"quyen_trinh_chieu"}',N'{"quy_tac":"con_hieu_luc"}',CAST(0 AS BIT),CAST(NULL AS SMALLINT)),
 ('VP_VE_QR',N'Vé và QR','BAT_BUOC',N'{"truong":"cach_kiem_soat_ve"}',N'{"quy_tac":"co_kiem_soat_ve"}',CAST(0 AS BIT),CAST(NULL AS SMALLINT))
) r(ma,nhom,muc_do,ap_dung,dat,ngoai_le,cap)
WHERE NOT EXISTS(SELECT 1 FROM dbo.QUY_TAC q WHERE q.ma_quy_tac=r.ma AND q.phien_ban=1);

INSERT dbo.CAU_HINH_LIEN_NGANH(ma_loai_su_kien,ma_don_vi,quy_tac_id,bat_buoc,so_ngay_phan_hoi,hieu_luc_tu)
SELECT @Esports,v.ma_don_vi,NULL,1,7,CONVERT(DATE,'20260101',112)
FROM (VALUES (@DonViAnNinh),(@DonViPCCC)) v(ma_don_vi)
WHERE NOT EXISTS(SELECT 1 FROM dbo.CAU_HINH_LIEN_NGANH c WHERE c.ma_loai_su_kien=@Esports AND c.ma_don_vi=v.ma_don_vi);

-- 4. HỒ SƠ MẪU: NHÁP THIẾU, NHÁP ĐỦ, CHỜ DUYỆT, ĐÃ CẤP, XỬ LÝ NGOẠI LỆ, CHỜ Ý KIẾN
INSERT dbo.HO_SO_CAP_PHEP(nguoi_to_chuc_id,nguoi_xu_ly_hien_tai_id,ma_loai_su_kien,ma_dia_diem,
 trang_thai,ngay_nop,ten_su_kien,bat_dau,ket_thuc,quy_mo,noi_dung,
 hinh_thuc_thuong_mai,co_tai_tro,loai_ve,gia_ve,so_luong_ve,kenh_phat_hanh_ve,cach_kiem_soat_ve,
 nhan_su_y_te,nhan_su_an_ninh,phuong_an_thoat_hiem,phuong_an_xu_ly_su_co)
SELECT CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN @ToChuc2 ELSE @ToChuc END,
       CASE WHEN v.trang_thai='NHAP' THEN NULL ELSE @XuLy END,
       @Esports,v.dia_diem,v.trang_thai,
       CASE WHEN v.trang_thai='NHAP' THEN NULL ELSE DATEADD(DAY,-5,SYSDATETIMEOFFSET()) END,
       v.ten,
       DATEADD(DAY,v.ngay_toi,SYSDATETIMEOFFSET()),
       DATEADD(HOUR,5,DATEADD(DAY,v.ngay_toi,SYSDATETIMEOFFSET())),
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE v.quy_mo END,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE N'Xem trực tiếp giải đấu eSports giả lập' END,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE 'PHI_THUONG_MAI' END,
       0,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE 'MIEN_PHI' END,
       0,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE v.quy_mo END,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE N'Đăng ký nhận vé QR trực tuyến' END,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE N'Quét mã QR ở cổng' END,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE N'2 tổ y tế trực ca' END,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE N'3 đội an ninh' END,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE N'Sơ đồ thoát hiểm mẫu' END,
       CASE WHEN v.ten=N'VP-DEMO-NHAP-THIEU' THEN NULL ELSE N'Phương án xử lý sự cố mẫu' END
FROM (VALUES
 (N'VP-DEMO-NHAP-THIEU','NHAP',CAST(NULL AS BIGINT),42,CAST(NULL AS INT)),
 (N'VP-DEMO-NHAP-DU','NHAP',@DiaA,55,2000),
 (N'VP-DEMO-CHO-DUYET','CHO_PHE_DUYET',@DiaB,65,2500),
 (N'VP-DEMO-DA-CAP','DA_CAP_PHEP',@DiaA,95,2300),
 (N'VP-DEMO-NGOAI-LE','XU_LY_NGOAI_LE',@DiaC,105,3500),
 (N'VP-DEMO-CHO-Y-KIEN','CHO_Y_KIEN_LIEN_NGANH',@DiaB,115,2500)
) v(ten,trang_thai,dia_diem,ngay_toi,quy_mo)
WHERE NOT EXISTS(SELECT 1 FROM dbo.HO_SO_CAP_PHEP h WHERE h.ten_su_kien=v.ten);

DECLARE @NhapDu BIGINT=(SELECT ma_ho_so FROM dbo.HO_SO_CAP_PHEP WHERE ten_su_kien=N'VP-DEMO-NHAP-DU'),
        @ChoDuyet BIGINT=(SELECT ma_ho_so FROM dbo.HO_SO_CAP_PHEP WHERE ten_su_kien=N'VP-DEMO-CHO-DUYET'),
        @DaCap BIGINT=(SELECT ma_ho_so FROM dbo.HO_SO_CAP_PHEP WHERE ten_su_kien=N'VP-DEMO-DA-CAP'),
        @NgoaiLe BIGINT=(SELECT ma_ho_so FROM dbo.HO_SO_CAP_PHEP WHERE ten_su_kien=N'VP-DEMO-NGOAI-LE'),
        @ChoYKien BIGINT=(SELECT ma_ho_so FROM dbo.HO_SO_CAP_PHEP WHERE ten_su_kien=N'VP-DEMO-CHO-Y-KIEN');

-- 5. TÀI LIỆU, QUYỀN TRÌNH CHIẾU
INSERT dbo.QUYEN_TRINH_CHIEU(ma_ho_so,don_vi_cap,so_van_ban,loai_quyen,nguon_tin_hieu,noi_dung_duoc_cap,pham_vi_dia_diem,hieu_luc_tu,hieu_luc_den,tep_dinh_kem)
SELECT h.ma_ho_so,N'Đơn vị sở hữu nội dung Mẫu',N'VP-DEMO-QUYEN',N'Quyền công cộng mẫu',N'Tín hiệu giả lập',N'View Party eSports',N'TOAN_QUOC',
       CONVERT(DATE,DATEADD(DAY,-20,SYSDATETIMEOFFSET())),
       CONVERT(DATE,DATEADD(DAY,365,SYSDATETIMEOFFSET())),N'demo/quyen-trinh-chieu.pdf'
FROM dbo.HO_SO_CAP_PHEP h
WHERE h.ten_su_kien IN (N'VP-DEMO-NHAP-DU',N'VP-DEMO-CHO-DUYET',N'VP-DEMO-DA-CAP',N'VP-DEMO-NGOAI-LE',N'VP-DEMO-CHO-Y-KIEN')
  AND NOT EXISTS(SELECT 1 FROM dbo.QUYEN_TRINH_CHIEU q WHERE q.ma_ho_so=h.ma_ho_so);

INSERT dbo.TAI_LIEU(ma_ho_so,loai_tai_lieu,ten_tai_lieu,duong_dan)
SELECT h.ma_ho_so,'PHUONG_AN_AN_TOAN',N'Phương án an toàn giả lập',N'demo/phuong-an-an-toan.pdf'
FROM dbo.HO_SO_CAP_PHEP h
WHERE h.ten_su_kien IN (N'VP-DEMO-NHAP-DU',N'VP-DEMO-CHO-DUYET',N'VP-DEMO-DA-CAP',N'VP-DEMO-NGOAI-LE',N'VP-DEMO-CHO-Y-KIEN')
  AND NOT EXISTS(SELECT 1 FROM dbo.TAI_LIEU d WHERE d.ma_ho_so=h.ma_ho_so AND d.loai_tai_lieu='PHUONG_AN_AN_TOAN');

-- 6. KẾT QUẢ THẨM ĐỊNH: NGOẠI LỆ MẪU CÓ QUY TẮC AN TOÀN KHÔNG ĐẠT
INSERT dbo.KET_QUA_KIEM_TRA(ma_ho_so,quy_tac_id,nguoi_kiem_tra_id,ket_qua,gia_tri_thuc_te,ghi_chu)
SELECT h.ma_ho_so,q.quy_tac_id,@XuLy,
       CASE WHEN h.ten_su_kien=N'VP-DEMO-NGOAI-LE' AND q.ma_quy_tac='VP_AN_TOAN' THEN 'KHONG_DAT' ELSE 'DAT' END,
       N'{"minh_chung":"du_lieu_mau"}',
       N'Kết quả kiểm tra minh họa'
FROM dbo.HO_SO_CAP_PHEP h
CROSS JOIN dbo.QUY_TAC q
WHERE h.ten_su_kien IN (N'VP-DEMO-CHO-DUYET',N'VP-DEMO-DA-CAP',N'VP-DEMO-NGOAI-LE',N'VP-DEMO-CHO-Y-KIEN')
 AND q.ma_loai_su_kien=@Esports
 AND NOT EXISTS(SELECT 1 FROM dbo.KET_QUA_KIEM_TRA k WHERE k.ma_ho_so=h.ma_ho_so AND k.quy_tac_id=q.quy_tac_id);

-- 7. Ý KIẾN LIÊN NGÀNH: 3 HỒ SƠ ĐÃ ĐỦ, 1 HỒ SƠ ĐANG CHỜ PCCC
INSERT dbo.YEU_CAU_Y_KIEN(ma_ho_so,ma_don_vi,ma_cau_hinh,nguoi_gui_id,han_phan_hoi,
 trang_thai,nguoi_phan_hoi_id,ket_luan,noi_dung_phan_hoi,ngay_phan_hoi)
SELECT h.ma_ho_so,c.ma_don_vi,c.ma_cau_hinh,@XuLy,
       DATEADD(DAY,7,SYSDATETIMEOFFSET()),
       CASE WHEN h.ten_su_kien=N'VP-DEMO-CHO-Y-KIEN' AND c.ma_don_vi=@DonViPCCC THEN 'CHO_PHAN_HOI' ELSE 'DA_PHAN_HOI' END,
       CASE WHEN h.ten_su_kien=N'VP-DEMO-CHO-Y-KIEN' AND c.ma_don_vi=@DonViPCCC THEN NULL
            WHEN c.ma_don_vi=@DonViAnNinh THEN @Lien1 ELSE @Lien2 END,
       CASE WHEN h.ten_su_kien=N'VP-DEMO-CHO-Y-KIEN' AND c.ma_don_vi=@DonViPCCC THEN NULL ELSE 'PHU_HOP' END,
       CASE WHEN h.ten_su_kien=N'VP-DEMO-CHO-Y-KIEN' AND c.ma_don_vi=@DonViPCCC THEN NULL ELSE N'Đã xem xét hồ sơ demo' END,
       CASE WHEN h.ten_su_kien=N'VP-DEMO-CHO-Y-KIEN' AND c.ma_don_vi=@DonViPCCC THEN NULL ELSE SYSDATETIMEOFFSET() END
FROM dbo.HO_SO_CAP_PHEP h
JOIN dbo.CAU_HINH_LIEN_NGANH c ON c.ma_loai_su_kien=h.ma_loai_su_kien
WHERE h.ten_su_kien IN (N'VP-DEMO-CHO-DUYET',N'VP-DEMO-DA-CAP',N'VP-DEMO-NGOAI-LE',N'VP-DEMO-CHO-Y-KIEN')
 AND NOT EXISTS(SELECT 1 FROM dbo.YEU_CAU_Y_KIEN y WHERE y.ma_ho_so=h.ma_ho_so AND y.ma_don_vi=c.ma_don_vi);

-- 8. NGOẠI LỆ CHỜ DUYỆT, GIẤY PHÉP ĐÃ CẤP
INSERT dbo.NGOAI_LE(ma_ho_so,ma_ket_qua,nguoi_de_xuat_id,ly_do,minh_chung,bien_phap_bu_dap,cap_tham_quyen_yeu_cau)
SELECT @NgoaiLe,k.ma_ket_qua,@XuLy,N'Đề nghị xét biện pháp an toàn thay thế',N'Minh chứng demo',N'Bổ sung lực lượng và lối thoát hiểm',2
FROM dbo.KET_QUA_KIEM_TRA k
JOIN dbo.QUY_TAC r ON r.quy_tac_id=k.quy_tac_id
WHERE k.ma_ho_so=@NgoaiLe AND r.ma_quy_tac='VP_AN_TOAN'
  AND NOT EXISTS(SELECT 1 FROM dbo.NGOAI_LE n WHERE n.ma_ket_qua=k.ma_ket_qua);

INSERT dbo.GIAY_PHEP(ma_ho_so,nguoi_phe_duyet_id,so_giay_phep,ngay_cap,hieu_luc_tu,hieu_luc_den,dieu_kien_kem_theo)
SELECT @DaCap,@PheDuyet,CONCAT('VP-DEMO-',@DaCap),CONVERT(DATE,SYSDATETIMEOFFSET()),
       CONVERT(DATE,h.bat_dau),CONVERT(DATE,h.ket_thuc),N'Tuân thủ kế hoạch an ninh/an toàn'
FROM dbo.HO_SO_CAP_PHEP h WHERE h.ma_ho_so=@DaCap
 AND NOT EXISTS(SELECT 1 FROM dbo.GIAY_PHEP g WHERE g.ma_ho_so=@DaCap);

-- 9. LỊCH SỬ, NHẬT KÝ QUẢN TRỊ VÀ THÔNG BÁO
INSERT dbo.LICH_SU_XU_LY(ma_ho_so,ma_tai_khoan,hanh_dong,tu_trang_thai,den_trang_thai,ket_qua)
SELECT h.ma_ho_so,@ToChuc,N'Tạo hồ sơ demo',NULL,h.trang_thai,N'Thành công'
FROM dbo.HO_SO_CAP_PHEP h
WHERE h.ten_su_kien LIKE N'VP-DEMO-%'
 AND NOT EXISTS(SELECT 1 FROM dbo.LICH_SU_XU_LY l WHERE l.ma_ho_so=h.ma_ho_so AND l.hanh_dong=N'Tạo hồ sơ demo');

IF NOT EXISTS(SELECT 1 FROM dbo.NHAT_KY_CAU_HINH WHERE doi_tuong=N'QUY_TAC' AND hanh_dong=N'TAO_DU_LIEU_MAU')
 INSERT dbo.NHAT_KY_CAU_HINH(ma_tai_khoan,doi_tuong,khoa_doi_tuong,hanh_dong,gia_tri_sau)
 VALUES(@Admin,N'QUY_TAC',N'VP_DEMO',N'TAO_DU_LIEU_MAU',N'{"nguon":"file02"}');

INSERT dbo.THONG_BAO(nguoi_nhan_id,ma_ho_so,loai_thong_bao,noi_dung)
SELECT @ToChuc,h.ma_ho_so,'CAP_PHEP',N'Giấy phép mẫu đã được cấp cho hồ sơ'
FROM dbo.HO_SO_CAP_PHEP h WHERE h.ma_ho_so=@DaCap
AND NOT EXISTS(SELECT 1 FROM dbo.THONG_BAO t WHERE t.ma_ho_so=@DaCap AND t.loai_thong_bao='CAP_PHEP');

COMMIT TRANSACTION;
GO

-- KIỂM TRA DỮ LIỆU MẪU
SELECT h.ma_ho_so,h.ten_su_kien,h.trang_thai,d.ten_dia_diem
FROM dbo.HO_SO_CAP_PHEP h
LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem=h.ma_dia_diem
WHERE h.ten_su_kien LIKE N'VP-DEMO-%' ORDER BY h.ma_ho_so;
GO

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viewparty.Models;

namespace Viewparty.Data
{
    internal sealed class AdminWorkflowRepository
    {
        public IList<AdminAccountItem> Accounts(long adminId) { return QueryAccounts(adminId); }
        private IList<AdminAccountItem> QueryAccounts(long adminId)
        {
            var result=new List<AdminAccountItem>(); try { using(var c=Db.OpenConnection()) using(var q=new SqlCommand("SELECT ma_tai_khoan,ten_dang_nhap,ho_ten,email,vai_tro,trang_thai,cap_tham_quyen,ma_don_vi FROM dbo.TAI_KHOAN ORDER BY ten_dang_nhap",c)) using(var r=q.ExecuteReader()){while(r.Read()) result.Add(new AdminAccountItem{Id=r.GetInt64(0),Username=S(r,1),FullName=S(r,2),Email=S(r,3),Role=S(r,4),Status=S(r,5),ApprovalLevel=NI(r,6),UnitId=NI64(r,7)});} return result;} catch(SqlException e){throw new AdminWorkflowException("Không thể tải tài khoản.",e);} }
        public void SaveAccount(long admin,long id,string role,string status){ Exec("dbo.sp_QuanTriTaiKhoan",c=>{P(c,"@AdminId",SqlDbType.BigInt,admin);P(c,"@MaTaiKhoan",SqlDbType.BigInt,id);P(c,"@VaiTro",SqlDbType.VarChar,role);P(c,"@TrangThai",SqlDbType.VarChar,status);}); }
        public void SaveApprovalAuthority(long admin,long id,short level){ Exec("dbo.sp_GanThamQuyenPheDuyet",c=>{P(c,"@AdminId",SqlDbType.BigInt,admin);P(c,"@MaTaiKhoan",SqlDbType.BigInt,id);P(c,"@CapThamQuyen",SqlDbType.SmallInt,level);}); }
        public IList<AdminEventTypeItem> EventTypes(long admin){var x=new List<AdminEventTypeItem>(); Query("SELECT ten_loai,mo_ta,trang_thai FROM dbo.LOAI_SU_KIEN ORDER BY ten_loai",r=>x.Add(new AdminEventTypeItem{Name=S(r,0),Description=S(r,1),Active=r.GetBoolean(2)}));return x;}
        public void SaveEventType(long admin,string name,string desc){Exec("dbo.sp_QuanTriLoaiSuKien",c=>{P(c,"@AdminId",SqlDbType.BigInt,admin);P(c,"@TenLoai",SqlDbType.NVarChar,name);P(c,"@MoTa",SqlDbType.NVarChar,string.IsNullOrWhiteSpace(desc)?(object)DBNull.Value:desc);});}
        public IList<AdminVenueItem> Venues(long admin){var x=new List<AdminVenueItem>();Query("SELECT ma_dia_diem,ten_dia_diem,dia_chi,suc_chua,trang_thai FROM dbo.DIA_DIEM ORDER BY ten_dia_diem",r=>x.Add(new AdminVenueItem{Id=r.GetInt64(0),Name=S(r,1),Address=S(r,2),Capacity=r.GetInt32(3),Active=r.GetBoolean(4)}));return x;}
        public void SaveVenue(long admin,long? id,string name,string address,int capacity,bool active)
        {
            const string sql = @"
IF NOT EXISTS(SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@AdminId AND vai_tro='QUAN_TRI_VIEN' AND trang_thai='HOAT_DONG')
    THROW 52401, N'Không có quyền quản trị địa điểm.', 1;
BEGIN TRANSACTION;
DECLARE @TargetId BIGINT = @MaDiaDiem;
IF @TargetId IS NULL OR @TargetId <= 0 OR NOT EXISTS(SELECT 1 FROM dbo.DIA_DIEM WHERE ma_dia_diem=@TargetId)
BEGIN
    INSERT dbo.DIA_DIEM(ten_dia_diem, dia_chi, suc_chua, trang_thai)
    VALUES(@TenDiaDiem, @DiaChi, @SucChua, @TrangThai);
    SET @TargetId = CONVERT(BIGINT, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE dbo.DIA_DIEM
    SET ten_dia_diem=@TenDiaDiem, dia_chi=@DiaChi, suc_chua=@SucChua, trang_thai=@TrangThai
    WHERE ma_dia_diem=@TargetId;
END
INSERT dbo.NHAT_KY_CAU_HINH(ma_tai_khoan, doi_tuong, khoa_doi_tuong, hanh_dong, gia_tri_sau)
VALUES(@AdminId, N'DIA_DIEM', CONVERT(NVARCHAR(30), @TargetId), N'CAP_NHAT',
    (SELECT ma_dia_diem, ten_dia_diem, dia_chi, suc_chua, trang_thai FROM dbo.DIA_DIEM WHERE ma_dia_diem=@TargetId FOR JSON PATH, WITHOUT_ARRAY_WRAPPER));
COMMIT TRANSACTION;";
            try
            {
                using (var c = Db.OpenConnection())
                using (var q = new SqlCommand(sql, c))
                {
                    P(q, "@AdminId", SqlDbType.BigInt, admin);
                    P(q, "@MaDiaDiem", SqlDbType.BigInt, id.HasValue && id.Value > 0 ? (object)id.Value : DBNull.Value);
                    P(q, "@TenDiaDiem", SqlDbType.NVarChar, name);
                    P(q, "@DiaChi", SqlDbType.NVarChar, address);
                    P(q, "@SucChua", SqlDbType.Int, capacity);
                    P(q, "@TrangThai", SqlDbType.Bit, active);
                    q.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                throw new AdminWorkflowException("Không thể lưu thông tin địa điểm.", e);
            }
        }
        public IList<AdminRuleItem> Rules(long admin){var x=new List<AdminRuleItem>();Query("SELECT q.quy_tac_id,q.ma_quy_tac,q.phien_ban,l.ten_loai,q.nhom,q.muc_do,q.cho_phep_ngoai_le,q.hieu_luc_tu FROM dbo.QUY_TAC q JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien=q.ma_loai_su_kien ORDER BY q.ma_quy_tac,q.phien_ban DESC",r=>x.Add(new AdminRuleItem{Id=r.GetInt64(0),Code=S(r,1),Version=r.GetInt32(2),EventType=S(r,3),Group=S(r,4),Severity=S(r,5),ExceptionAllowed=r.GetBoolean(6),EffectiveFrom=r.GetDateTime(7)}));return x;}
        public void SaveRule(long admin,string code,long eventType,string group,string severity,string apply,string pass,bool allow,DateTime from,short? level){Exec("dbo.sp_QuanTriQuyTacPhienBan",c=>{P(c,"@AdminId",SqlDbType.BigInt,admin);P(c,"@MaQuyTac",SqlDbType.VarChar,code);P(c,"@MaLoaiSuKien",SqlDbType.BigInt,eventType);P(c,"@Nhom",SqlDbType.NVarChar,group);P(c,"@MucDo",SqlDbType.VarChar,severity);P(c,"@DieuKienApDung",SqlDbType.NVarChar,apply);P(c,"@DieuKienDat",SqlDbType.NVarChar,pass);P(c,"@ChoPhepNgoaiLe",SqlDbType.Bit,allow);P(c,"@HieuLucTu",SqlDbType.Date,from.Date);P(c,"@CapThamQuyen",SqlDbType.SmallInt,level.HasValue?(object)level.Value:DBNull.Value);});}
        public IList<AdminInteragencyConfigItem> Configs(long admin){var x=new List<AdminInteragencyConfigItem>();Query("SELECT c.ma_cau_hinh,l.ten_loai,d.ten_don_vi,c.bat_buoc,c.so_ngay_phan_hoi,c.hieu_luc_tu FROM dbo.CAU_HINH_LIEN_NGANH c JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien=c.ma_loai_su_kien JOIN dbo.DON_VI_LIEN_NGANH d ON d.ma_don_vi=c.ma_don_vi ORDER BY c.hieu_luc_tu DESC",r=>x.Add(new AdminInteragencyConfigItem{Id=r.GetInt64(0),EventType=S(r,1),Unit=S(r,2),Required=r.GetBoolean(3),Days=r.GetInt32(4),EffectiveFrom=r.GetDateTime(5)}));return x;}
        public void SaveConfig(long admin,long eventType,long unit,bool required,int days,DateTime from){Exec("dbo.sp_QuanTriCauHinhLienNganh",c=>{P(c,"@AdminId",SqlDbType.BigInt,admin);P(c,"@MaLoaiSuKien",SqlDbType.BigInt,eventType);P(c,"@MaDonVi",SqlDbType.BigInt,unit);P(c,"@BatBuoc",SqlDbType.Bit,required);P(c,"@SoNgayPhanHoi",SqlDbType.Int,days);P(c,"@HieuLucTu",SqlDbType.Date,from.Date);});}
        public AdminAuditData Audit(long admin){var x=new AdminAuditData();try{using(var c=Db.OpenConnection())using(var q=new SqlCommand("dbo.sp_XemLichSuVaNhatKy",c)){q.CommandType=CommandType.StoredProcedure;P(q,"@AdminId",SqlDbType.BigInt,admin);P(q,"@MaHoSo",SqlDbType.BigInt,DBNull.Value);using(var r=q.ExecuteReader()){while(r.Read())x.History.Add(new AdminAuditItem{At=r.GetDateTimeOffset(r.GetOrdinal("thoi_diem")),AccountId=NI64(r,r.GetOrdinal("ma_tai_khoan")),ObjectName="HỒ SƠ",Action=S(r,r.GetOrdinal("hanh_dong")),Result=S(r,r.GetOrdinal("ket_qua")),Key=S(r,r.GetOrdinal("ma_ho_so"))});if(r.NextResult())while(r.Read())x.Configuration.Add(new AdminAuditItem{At=r.GetDateTimeOffset(r.GetOrdinal("thoi_diem")),AccountId=r.GetInt64(r.GetOrdinal("ma_tai_khoan")),ObjectName=S(r,r.GetOrdinal("doi_tuong")),Key=S(r,r.GetOrdinal("khoa_doi_tuong")),Action=S(r,r.GetOrdinal("hanh_dong")),Result="Đã ghi nhật ký"});}}return x;}catch(SqlException e){throw new AdminWorkflowException("Không thể tải nhật ký.",e);}}
        private static void Query(string sql,Action<IDataRecord> read){try{using(var c=Db.OpenConnection())using(var q=new SqlCommand(sql,c))using(var r=q.ExecuteReader()){while(r.Read())read(r);}}catch(SqlException e){throw new AdminWorkflowException("Không thể tải dữ liệu quản trị.",e);}}
        private static void Exec(string name,Action<SqlCommand> add){try{using(var c=Db.OpenConnection())using(var q=new SqlCommand(name,c)){q.CommandType=CommandType.StoredProcedure;add(q);q.ExecuteNonQuery();}}catch(SqlException e){throw new AdminWorkflowException("Không thể cập nhật dữ liệu quản trị.",e);}}
        private static void P(SqlCommand c,string n,SqlDbType t,object v){c.Parameters.Add(n,t).Value=v??DBNull.Value;}
        private static string S(IDataRecord r,int i){return r.IsDBNull(i)?null:r.GetValue(i).ToString();}
        private static int? NI(IDataRecord r,int i){return r.IsDBNull(i)?(int?)null:Convert.ToInt32(r.GetValue(i));}
        private static long? NI64(IDataRecord r,int i){return r.IsDBNull(i)?(long?)null:Convert.ToInt64(r.GetValue(i));}
    }
    internal sealed class AdminWorkflowException:Exception{public AdminWorkflowException(string m,Exception e):base(m,e){}}
}

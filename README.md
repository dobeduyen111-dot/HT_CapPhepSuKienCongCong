# HỆ THỐNG CẤP PHÉP SỰ KIỆN CÔNG CỘNG (VIEW PARTY PORTAL)

> **Báo cáo tiểu luận môn học:** Phân tích và Thiết kế Hướng đối tượng (UML)  
> **Trường:** Đại học Tài nguyên và Môi trường TP. Hồ Chí Minh — Khoa Công nghệ Thông tin  
> **Giảng viên hướng dẫn:** Th.S Phạm Trọng Huynh  
> **Sinh viên thực hiện:** Đỗ Thị Bé Duyên — **MSSV:** 1250080040 — **Lớp:** 12_CNPM1  
> **Kho lưu trữ chính thức (GitHub):** [https://github.com/dobeduyen111-dot/HT_CapPhepSuKienCongCong](https://github.com/dobeduyen111-dot/HT_CapPhepSuKienCongCong)

---

## 1. Giới thiệu đề tài

Hệ thống **View Party Portal** là giải pháp phần mềm toàn diện hỗ trợ số hóa và tự động hóa quy trình nộp, tiếp nhận, thẩm định điều kiện an toàn, lấy ý kiến chuyên ngành và phê duyệt cấp phép cho các sự kiện trình chiếu công cộng (View Party thể thao, âm nhạc ngoài trời, lễ hội văn hóa).

Hệ thống được thiết kế theo phương pháp luận **Phân tích và Thiết kế Hướng đối tượng (OOAD)** sử dụng chuẩn mô hình hóa **UML**, bao phủ đầy đủ **28 Use Case** nghiệp vụ và **16 bảng cơ sở dữ liệu quan hệ chuẩn hóa 3NF**.

---

## 2. Các vai trò người dùng (5 Phân hệ)

1. **Người tổ chức sự kiện (TC):** Đăng ký tài khoản, tra cứu địa điểm tổ chức, tạo và nộp hồ sơ xin cấp phép trực tuyến, theo dõi tiến độ xử lý và tải giấy phép điện tử.
2. **Cán bộ tiếp nhận & thẩm định (XL):** Tiếp nhận hồ sơ, kiểm tra điều kiện an toàn trật tự, PCCC/CNCH, quyền trình chiếu, xin ý kiến phối hợp liên ngành và đề xuất ngoại lệ.
3. **Đơn vị phối hợp liên ngành (LN):** Tiếp nhận văn bản xin ý kiến (Công an, Giao thông, Y tế...) và gửi văn bản phản hồi chuyên môn.
4. **Lãnh đạo phê duyệt (PD):** Xem xét kết quả tổng hợp thẩm định, quyết định phê duyệt cấp phép hoặc từ chối có nêu rõ lý do.
5. **Quản trị viên hệ thống (QT):** Quản lý tài khoản, phân quyền, cấu hình loại sự kiện, cấu hình quy tắc cấp phép và kiểm toán nhật ký xử lý.

---

## 3. Kiến trúc phần mềm & Công nghệ

* **Kiến trúc:** 3 tầng chuẩn mực (Three-Layer Architecture):
  * **Presentation Layer (Tầng Giao diện):** C# Windows Forms (.NET Framework 4.7.2) với giao diện Dashboard theo từng vai trò.
  * **Business Logic Layer (Tầng Nghiệp vụ):** Các lớp Services (`HoSoService`, `DiaDiemService`, `AuthenticationService`...) xử lý quy tắc và kiểm soát nghiệp vụ.
  * **Data Access Layer (Tầng Truy cập dữ liệu):** Sử dụng thư viện `ADO.NET` (`Microsoft.Data.SqlClient`) kết nối cơ sở dữ liệu qua lớp `Db.cs` tập trung, thực thi parameterized queries và Stored Procedures an toàn.
* **Cơ sở dữ liệu:** Microsoft SQL Server 2019/2022 (16 bảng quan hệ, ràng buộc toàn vẹn PK/FK/CHECK/UNIQUE, Stored Procedures, Functions, Triggers bảo đảm tính toàn vẹn ACID).
* **Bảo mật & Tiện ích:**
  * Thuật toán băm mật khẩu: `BCrypt.Net-Next` an toàn chống tấn công brute-force.
  * Thư viện xử lý dữ liệu: `Newtonsoft.Json`.

---

## 4. Cấu trúc thư mục kho lưu trữ (Repository)

```text
├── Database/                                     # Tập lệnh kịch bản CSDL SQL Server
│   ├── 01_ViewParty_CauTruc_16Bang.sql           # Script khởi tạo cấu trúc 16 bảng CSDL chuẩn 3NF
│   ├── 02_ViewParty_DuLieuMau.sql                # Script nạp dữ liệu danh mục & tài khoản khởi tạo
│   ├── 03_ViewParty_NghiepVu_SP_Function_Trigger.sql # Toàn bộ Stored Procedures, Functions & Triggers
│   ├── 04_ViewParty_Demo_UC03_UC04.sql           # Dữ liệu kịch bản demo kiểm thử
│   └── 05_Tao_Va_Khoi_Phuc_Tai_Khoan_Quan_Tri.sql # Script hỗ trợ khôi phục tài khoản Admin
├── Docs/                                         # Tài liệu báo cáo chính thức môn học
│   └── BaoCao_ViewParty_HoanThien_ChuanNop_2026.docx # Báo cáo hoàn chỉnh 7 chương định dạng .docx
├── Viewparty/                                    # Mã nguồn ứng dụng WinForms C#
│   ├── Controls/                                 # Các UserControl giao diện phân hệ chức năng
│   ├── Data/                                     # Data Access Layer & Repositories (ADO.NET)
│   ├── Forms/                                    # Form giao diện người dùng theo từng vai trò
│   ├── Models/                                   # Các thực thể dữ liệu (Entity Models)
│   ├── Services/                                 # Business Logic Services
│   ├── SDBC.drawio                               # Sơ đồ thiết kế và giao diện
│   ├── App.config                                # Cấu hình chuỗi kết nối CSDL SQL Server
│   └── Viewparty.csproj                          # File dự án C#
├── Viewparty.sln                                 # File Solution mở trong Visual Studio
├── .gitignore                                    # Cấu hình bỏ qua tệp tạm build của Visual Studio
└── README.md                                     # Tài liệu tổng quan đề tài và hướng dẫn triển khai
```

---

## 5. Hướng dẫn cài đặt và Chạy thử nghiệm

### Bước 1: Khởi tạo Cơ sở dữ liệu trên Microsoft SQL Server
1. Mở **SQL Server Management Studio (SSMS)** và kết nối tới SQL Server của máy (hoặc LocalDB).
2. Tạo cơ sở dữ liệu có tên: `ViewPartyDB_UC28`.
3. Chạy lần lượt các script trong thư mục `Database/`:
   * Chạy `01_ViewParty_CauTruc_16Bang.sql` (Khởi tạo 16 bảng).
   * Chạy `02_ViewParty_DuLieuMau.sql` (Nạp dữ liệu mẫu ban đầu).
   * Chạy `03_ViewParty_NghiepVu_SP_Function_Trigger.sql` (Nạp Stored Procedures, Triggers).
   * *(Tùy chọn)* Chạy `04_ViewParty_Demo_UC03_UC04.sql` nếu muốn có dữ liệu demo hồ sơ nháp.

### Bước 2: Cấu hình kết nối (App.config)
Mở tệp `Viewparty/App.config`, kiểm tra chuỗi kết nối phù hợp với máy của bạn:
```xml
<connectionStrings>
    <add name="ViewPartyDB" 
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ViewPartyDB_UC28;Integrated Security=True;Connect Timeout=30;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Bước 3: Biên dịch và Khởi chạy ứng dụng
1. Mở tệp `Viewparty.sln` bằng **Microsoft Visual Studio 2019 / 2022**.
2. Chọn cấu hình **Debug** hoặc **Release**, chọn nền tảng **Any CPU** hoặc **x86/x64**.
3. Nhấn **Build Solution** (`Ctrl + Shift + B`) — Dự án biên dịch thành công 100% với 0 lỗi (0 Errors).
4. Nhấn **Start** (`F5` hoặc `Ctrl + F5`) để bắt đầu chạy ứng dụng.

---

## 6. Tài khoản thử nghiệm mặc định

| Vai trò | Tên đăng nhập | Mật khẩu mặc định | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Quản trị viên (QT)** | `admin_system` | `Admin@2026!` | Quản trị tài khoản, phân quyền, cấu hình hệ thống |
| **Cán bộ xử lý (XL)** | `canboxuly01` | `Demo@2026!` | Tiếp nhận, thẩm định hồ sơ |
| **Lãnh đạo phê duyệt (PD)** | `lanhdao01` | `Demo@2026!` | Phê duyệt hoặc từ chối cấp phép |
| **Đơn vị liên ngành (LN)** | `liennganh_pccc` | `Demo@2026!` | Phối hợp cho ý kiến chuyên môn PCCC |
| **Người tổ chức (TC)** | *Đăng ký mới trực tiếp trên Form Đăng ký (UC01)* |

---
© 2026 Đỗ Thị Bé Duyên — Đề tài Tiểu luận môn Phân tích và Thiết kế Hướng đối tượng — HCMUNRE.

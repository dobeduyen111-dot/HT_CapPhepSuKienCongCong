# HỆ THỐNG CẤP PHÉP SỰ KIỆN CÔNG CỘNG (VIEW PARTY PORTAL)

> **Môn học:** Phân tích và Thiết kế Hướng đối tượng (UML)  
> **Trường:** Đại học Tài nguyên và Môi trường TP. Hồ Chí Minh — Khoa Công nghệ Thông tin  
> **Giảng viên hướng dẫn:** Th.S Phạm Trọng Huynh  
> **Sinh viên thực hiện:** Đỗ Thị Bé Duyên — **MSSV:** 1250080040 — **Lớp:** 12_CNPM1  
> **GitHub Repository:** [https://github.com/dobeduyen111-dot/HT_CapPhepSuKienCongCong](https://github.com/dobeduyen111-dot/HT_CapPhepSuKienCongCong)

---

## 1. Giới thiệu dự án

Hệ thống **View Party Portal** là giải pháp phần mềm quản lý và cấp phép số hóa cho các sự kiện trình chiếu công cộng (View Party thể thao, âm nhạc ngoài trời, lễ hội cộng đồng). Hệ thống giải quyết trọn vẹn quy trình nghiệp vụ phức tạp từ khâu nộp hồ sơ, kiểm soát trùng lịch địa điểm, đối soát bản quyền phát sóng, thẩm định an toàn PCCC, lấy ý kiến phối hợp liên ngành đến phê duyệt và ban hành Giấy phép điện tử.

* **Kiến trúc phần mềm:** Mô hình 3 tầng chuẩn mực (Three-Layer Architecture):
  * **Presentation Layer:** C# Windows Forms (.NET Framework 4.7.2) với giao diện làm việc riêng biệt cho 5 vai trò.
  * **Business Logic Layer:** Các lớp Services (`HoSoService`, `DiaDiemService`, `AuthenticationService`...) kiểm soát toàn bộ quy tắc nghiệp vụ.
  * **Data Access Layer:** Lớp `Db.cs` và các Repositories thực thi kết nối an toàn qua `ADO.NET` (`System.Data.SqlClient`), sử dụng Parameterized Queries và Stored Procedures.
* **Cơ sở dữ liệu:** Microsoft SQL Server (16 bảng chuẩn hóa 3NF, bảo đảm ràng buộc toàn vẹn ACID).
* **Bảo mật:** Băm mật khẩu người dùng bằng thuật toán an toàn `BCrypt.Net-Next`.

---

## 2. Yêu cầu môi trường & Công cụ cần thiết

Để biên dịch và chạy dự án, máy tính cần đáp ứng các yêu cầu sau:

1. **Hệ điều hành:** Windows 10 hoặc Windows 11 (64-bit).
2. **Môi trường phát triển:** Microsoft Visual Studio 2019 hoặc Visual Studio 2022 (phiên bản Community / Professional / Enterprise).
   * *Yêu cầu khi cài đặt Visual Studio:* Phải tích chọn Workload **".NET desktop development"**.
3. **Nền tảng .NET:** .NET Framework 4.7.2 Developer Pack / Runtime.
4. **Hệ quản trị CSDL:** Microsoft SQL Server 2019 / 2022 (hoặc SQL Server Express / SQL Server LocalDB có sẵn đi kèm Visual Studio).
5. **Công cụ quản lý CSDL:** SQL Server Management Studio (SSMS).

---

## 3. Hướng dẫn cài đặt Cơ sở dữ liệu (Database Setup)

Trước khi khởi chạy ứng dụng, bạn cần khởi tạo cơ sở dữ liệu trên SQL Server theo các bước sau:

### Bước 3.1: Mở SQL Server Management Studio (SSMS)
* Kết nối vào SQL Server trên máy của bạn (ví dụ: `.` hoặc `localhost` hoặc `(localdb)\MSSQLLocalDB`).

### Bước 3.2: Tạo Cơ sở dữ liệu mới
* Mở cửa sổ **New Query** và thực thi lệnh sau:
  ```sql
  CREATE DATABASE ViewPartyDB_UC28;
  GO
  USE ViewPartyDB_UC28;
  GO
  ```

### Bước 3.3: Chạy lần lượt các tập lệnh trong thư mục `Database/`
Bạn mở và thực thi lần lượt **đúng thứ tự** các file SQL sau (các file đều nằm trong thư mục `Database/` của dự án):

1. **`01_ViewParty_CauTruc_16Bang.sql`**: Khởi tạo cấu trúc 16 bảng dữ liệu chuẩn 3NF và các ràng buộc toàn vẹn (PK, FK, UNIQUE, CHECK).
2. **`02_ViewParty_DuLieuMau.sql`**: Nạp dữ liệu danh mục ban đầu (địa điểm, loại sự kiện, danh mục đơn vị liên ngành, tài khoản mẫu).
3. **`03_ViewParty_NghiepVu_SP_Function_Trigger.sql`**: Nạp toàn bộ 28 Stored Procedures nghiệp vụ, Functions và Triggers bảo đảm an toàn dữ liệu.
4. **`04_ViewParty_Demo_UC03_UC04.sql`**: Nạp dữ liệu các hồ sơ mẫu để kiểm thử quy trình nộp hồ sơ và kiểm tra tự động cảnh báo trùng lịch địa điểm.
5. *(Tùy chọn)* **`05_Tao_Va_Khoi_Phuc_Tai_Khoan_Quan_Tri.sql`**: Dùng khi cần khôi phục hoặc tạo mới tài khoản Quản trị viên hệ thống.

---

## 4. Hướng dẫn cấu hình kết nối ứng dụng (App.config)

Mở tệp `Viewparty/App.config` trong Visual Studio hoặc bằng Notepad để kiểm tra chuỗi kết nối:

```xml
<connectionStrings>
    <add name="ViewPartyDB" 
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ViewPartyDB_UC28;Integrated Security=True;Connect Timeout=30;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

* **Nếu máy bạn sử dụng SQL Server LocalDB:** Giữ nguyên chuỗi kết nối mặc định như trên.
* **Nếu máy bạn sử dụng SQL Server phiên bản đầy đủ (SQLEXPRESS hoặc Local):** Bạn chỉ cần thay `(localdb)\MSSQLLocalDB` bằng tên Server của bạn (ví dụ: `.\SQLEXPRESS` hoặc `localhost`):
  ```xml
  connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=ViewPartyDB_UC28;Integrated Security=True;Connect Timeout=30;"
  ```

---

## 5. Hướng dẫn biên dịch và Chạy chương trình

1. Nhấp đúp chuột vào tệp **`Viewparty.sln`** để mở dự án trong **Microsoft Visual Studio**.
2. Trên thanh công cụ Visual Studio:
   * Chọn cấu hình build: **Debug** hoặc **Release**.
   * Chọn nền tảng: **Any CPU** (hoặc x86 / x64).
3. Biên dịch dự án: Nhấn tổ hợp phím **`Ctrl + Shift + B`** (hoặc vào menu *Build* $\rightarrow$ *Build Solution*).
   * *Kiểm tra cửa sổ Output:* Dự án biên dịch thành công 100% (**0 Errors, 0 Warnings**).
4. Khởi chạy ứng dụng: Nhấn phím **`F5`** (chạy kèm Debug) hoặc **`Ctrl + F5`** (chạy trực tiếp không Debug).
5. Ứng dụng sẽ tự động kiểm tra kết nối CSDL và mở ra **Trang chủ công khai (HomeForm)**. Từ đây, bạn chọn **"Đăng nhập"** để vào hệ thống.

---

## 6. Danh sách Tài khoản thử nghiệm (Test Accounts)

Hệ thống đã chuẩn bị sẵn tài khoản thử nghiệm đầy đủ cho cả **5 vai trò người dùng**. Tất cả mật khẩu mặc định đều tuân thủ chính sách bảo mật và được băm an toàn bằng BCrypt.

| STT | Phân hệ / Vai trò | Tên đăng nhập | Mật khẩu mặc định | Chức năng nghiệp vụ kiểm thử |
| :---: | :--- | :--- | :--- | :--- |
| **1** | **Quản trị viên hệ thống (QT)** | `admin_system` | `Admin@2026!` | • Quản lý tài khoản và phân quyền (UC20)<br>• Cấu hình loại sự kiện & ma trận liên ngành (UC21, UC23)<br>• Cấu hình bộ quy tắc nghiệp vụ (UC22)<br>• Xem nhật ký kiểm toán hệ thống Audit Log (UC25) |
| **2** | **Cán bộ tiếp nhận & thẩm định (XL)** | `canboxuly01` | `Demo@2026!` | • Tiếp nhận và đối soát hồ sơ hợp lệ (UC07, UC08)<br>• Thẩm định địa điểm & phương án an toàn PCCC (UC09, UC10, UC11)<br>• Gửi yêu cầu xin ý kiến liên ngành (UC13)<br>• Đề xuất ngoại lệ (UC27) và tổng hợp kết quả (UC15) |
| **3** | **Lãnh đạo phê duyệt (PD)** | `lanhdao01` | `Demo@2026!` | • Xem danh sách hồ sơ chờ phê duyệt (UC16)<br>• Xem xét và phê duyệt đề xuất ngoại lệ (UC18)<br>• Phê duyệt cấp Giấy phép sự kiện chính thức (UC19)<br>• Từ chối cấp phép có nêu rõ lý do (UC17) |
| **4** | **Đơn vị phối hợp liên ngành (LN)** | `liennganh_pccc`<br>`liennganh_antt`<br>`liennganh_yte` | `Demo@2026!` | • Tiếp nhận văn bản xin ý kiến phối hợp chuyên ngành (UC13)<br>• Gửi văn bản phản hồi ý kiến chuyên môn (UC14 - Mẫu VB04) |
| **5** | **Người tổ chức sự kiện (TC)** | `tochuc_demo01`<br>*(hoặc tự đăng ký mới)* | `Demo@2026!` | • Đăng ký tài khoản mới trực tuyến (UC01)<br>• Tra cứu và kiểm tra lịch trống địa điểm (UC28)<br>• Tạo mới / chỉnh sửa hồ sơ nháp (UC03)<br>• Nộp hồ sơ xin cấp phép chính thức (UC04)<br>• Theo dõi tiến độ & bổ sung hồ sơ (UC05, UC06) |

> 💡 **Mẹo kiểm thử nhanh:** Đối với vai trò Người tổ chức (TC), bạn có thể bấm trực tiếp nút **"Đăng ký"** trên màn hình Đăng nhập để tạo một tài khoản mới mang thông tin của chính mình và trải nghiệm toàn bộ quy trình từ đầu.

---

## 7. Kịch bản kiểm thử nhanh các tính năng tiêu biểu

* **Kịch bản 1: Kiểm tra tính năng tra cứu và kiểm soát trùng lịch (UC28 & UC04)**  
  Đăng nhập tài khoản `tochuc_demo01` $\rightarrow$ Chọn *Tạo hồ sơ* $\rightarrow$ Bấm *Tra cứu địa điểm*: Hệ thống tự động lọc các địa điểm đủ sức chứa và hiển thị cảnh báo trực quan nếu khung giờ sự kiện bị trùng với sự kiện khác đã được duyệt.
* **Kịch bản 2: Kiểm tra đối soát điều kiện nộp hồ sơ (Quy tắc BR01–BR40)**  
  Khi nộp hồ sơ, hệ thống tự động kiểm tra trong Transaction: quy mô khán giả phải từ 1.000 đến 10.000 người, thời điểm nộp phải cách ngày khai mạc ít nhất 30 ngày, phải có đầy đủ bằng chứng bản quyền phát sóng và phương án an toàn.
* **Kịch bản 3: Quy trình liên ngành và kết xuất văn bản hành chính**  
  Cán bộ xử lý (`canboxuly01`) gửi yêu cầu phối hợp $\rightarrow$ Đơn vị liên ngành (`liennganh_pccc`) phản hồi $\rightarrow$ Lãnh đạo (`lanhdao01`) phê duyệt cấp phép: Hệ thống tự động điền dữ liệu và kết xuất Giấy phép tổ chức sự kiện chuẩn mực (Mẫu VB08).

---

## 8. Cấu trúc thư mục kho lưu trữ

```text
├── Database/                                     # Tập lệnh kịch bản CSDL SQL Server (01 -> 05)
│   ├── 01_ViewParty_CauTruc_16Bang.sql           # Tạo 16 bảng dữ liệu chuẩn hóa 3NF
│   ├── 02_ViewParty_DuLieuMau.sql                # Nạp dữ liệu danh mục & tài khoản mẫu
│   ├── 03_ViewParty_NghiepVu_SP_Function_Trigger.sql # 28 Stored Procedures, Functions, Triggers
│   ├── 04_ViewParty_Demo_UC03_UC04.sql           # Kịch bản hồ sơ demo thử nghiệm
│   └── 05_Tao_Va_Khoi_Phuc_Tai_Khoan_Quan_Tri.sql # Script khôi phục tài khoản Admin
├── Diagrams/                                     # Toàn bộ sơ đồ thiết kế hệ thống
│   └── SDBC_ViewParty_Diagrams.drawio            # Sơ đồ 28 Use Case, Sequence, Class, UI Wireframes
├── Viewparty/                                    # Toàn bộ mã nguồn ứng dụng WinForms C#
│   ├── Controls/                                 # UserControl màn hình làm việc của 5 vai trò
│   ├── Data/                                     # Data Access Layer & Repositories (ADO.NET)
│   ├── Forms/                                    # Giao diện người dùng theo từng phân hệ
│   ├── Models/                                   # Các lớp thực thể dữ liệu (Entity Models)
│   ├── Services/                                 # Business Logic Services
│   ├── Templates/                                # 10 tệp biểu mẫu văn bản hành chính (VB01 - VB10)
│   ├── App.config                                # Cấu hình chuỗi kết nối cơ sở dữ liệu
│   └── Viewparty.csproj                          # File dự án C#
├── Viewparty.sln                                 # File Solution mở trong Visual Studio
├── .gitignore                                    # Cấu hình loại bỏ file rác, file tạm build
└── README.md                                     # Hướng dẫn chi tiết cài đặt và kiểm thử dự án
```

---
© 2026 Đỗ Thị Bé Duyên — Tiểu luận môn Phân tích và Thiết kế Hướng đối tượng — HCMUNRE.

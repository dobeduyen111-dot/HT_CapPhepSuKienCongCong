# ViewParty Forms ↔ SDBC.drawio / Use Case mapping

## Evidence and mapping rule

The source is `Viewparty/SDBC.drawio`. I read the actual text from Trang chủ and Trang-12–Trang-16. Trang-1 and Trang-10 in this file expose only the `«include»` edge label through their cell text; they do not expose readable UC identifiers/names. UC ranges below are therefore recorded only where they are explicitly named in the SQL project comments (`03_ViewParty_NghiepVu_SP_Function_Trigger.sql`). They are workflow-family references, not invented one-to-one UC names. The design screens themselves remain the source for screen contents.

Shared shell: sidebar and header are reusable controls, while each row below is a separate `Form` subclass with its own `.Designer.cs` and `.resx`.

| Trang Draw.io | Tên Form | Nội dung giao diện | Vai trò | UC đối chiếu |
|---|---|---|---|---|
| Trang chủ | HomeForm | Hero, quy trình 4 bước, tra cứu/hướng dẫn, CTA đăng nhập/đăng ký | Công khai | Trang chủ không ghi mã UC |
| Trang chủ | LoginForm | Tên đăng nhập, mật khẩu, ghi nhớ, quên mật khẩu, đăng nhập; giữ cấu hình SQL hiện có | Công khai | UC login chưa được xác định từ nhãn Trang-1/10; `sp_LayTaiKhoanDangNhap` tra theo tên đăng nhập |
| Trang chủ | RegisterForm | Người đại diện, email, điện thoại, tài khoản, mật khẩu, xác nhận mật khẩu, tổ chức, địa chỉ và đồng ý chính sách; giao diện chưa kết nối Service | Người tổ chức | UC01; Stored Procedure đăng ký chỉ nhận thông tin tài khoản/người đại diện, còn tên tổ chức/địa chỉ không có cột trong TAI_KHOAN |
| Trang-12 | OrganizerDashboardForm | Header/sidebar Người tổ chức, 4 thẻ thống kê, hồ sơ gần đây, thao tác nhanh | Người tổ chức | UC03–UC06 family (SQL ghi hồ sơ/theo dõi); chi tiết UC cần đặc tả UC |
| Trang-12 | HoSoListForm | Bộ lọc, danh sách, trạng thái và hành động từng hồ sơ | Người tổ chức | UC03–UC06 family |
| Trang-12 | HoSoCreateForm | Bước 1: tên/loại sự kiện, thời gian, quy mô, nội dung | Người tổ chức | UC03 family |
| Trang-12 | ScreeningRightsForm | Bước 2: vé/QR, kênh phát hành, bản quyền và minh chứng | Người tổ chức | UC03 family |
| Trang-12 | SafetyDocumentsForm | Bước 3: an ninh, y tế, PCCC, sự cố và tệp đính kèm | Người tổ chức | UC03 family |
| Trang-12 | HoSoSubmitForm | Bước 4: xem lại dữ liệu, checklist điều kiện, lưu nháp/nộp | Người tổ chức | UC04 (SQL nêu nộp/khóa lịch) |
| Trang-12 | VenueSearchForm | Bộ lọc khu vực/ngày/quy mô, kết quả địa điểm và chọn địa điểm | Người tổ chức | UC28 (SQL ghi rõ chọn địa điểm) |
| Trang-12 | HoSoDetailForm | Chi tiết hồ sơ và timeline xử lý | Người tổ chức | UC05–UC06 family |
| Trang-12 | SupplementDocumentsForm | Yêu cầu bổ sung, chọn tài liệu mới, giải trình, nộp lại | Người tổ chức | UC06 (SQL ghi nộp lại) |
| Trang-12 | NotificationForm | Thông báo gần đây và xuất văn bản Word theo hồ sơ | Người tổ chức | UC26 chưa thể xác nhận từ nhãn sơ đồ; yêu cầu tra UC specification |
| Trang-12 | WordFormsForm | Thư viện mẫu VB01–VB10, chọn hồ sơ, xem trước/xuất DOCX | Người tổ chức / dùng chung | UC chưa thể xác nhận từ nhãn sơ đồ |
| Trang-13 | CaseOfficerDashboardForm | 4 thẻ, danh sách ưu tiên xử lý và hành động | Người xử lý | UC06–UC12 family |
| Trang-13 | HoSoIntakeForm | Danh sách hồ sơ chờ tiếp nhận và xem/tiếp nhận | Người xử lý | UC06–UC12 family |
| Trang-13 | HoSoValidationForm | Checklist hợp lệ, chọn hợp lệ/chưa hợp lệ, lý do bổ sung | Người xử lý | UC06–UC12 family |
| Trang-13 | HoSoAppraisalForm | Tab địa điểm/an toàn/điều kiện; bảng quy tắc và lưu kết quả | Người xử lý | UC06–UC12 family |
| Trang-13 | InteragencyRequestForm | Hồ sơ, loại sự kiện, đơn vị/lĩnh vực/bắt buộc/hạn, gửi yêu cầu | Người xử lý | UC13–UC15 family (SQL ghi nhóm liên ngành/tổng hợp) |
| Trang-13 | OpinionSummaryForm | Thẻ kết quả, bảng tổng hợp, phương án kết luận | Người xử lý | UC13–UC15 family |
| Trang-13 | ExceptionProposalForm | Quy tắc/phiên bản, cấp thẩm quyền, lý do, biện pháp, minh chứng | Người xử lý | UC16–UC19 family |
| Trang-14 | InteragencyDashboardForm | 4 thẻ và yêu cầu cần phản hồi | Đơn vị liên ngành | UC13–UC15 family |
| Trang-14 | RequestListForm | Bảng yêu cầu, hồ sơ, sự kiện, hạn, hành động | Đơn vị liên ngành | UC13–UC15 family |
| Trang-14 | OpinionResponseForm | Thông tin hồ sơ/tài liệu, kết luận, nhận xét, kiến nghị, VB04 | Đơn vị liên ngành | UC13–UC15 family |
| Trang-15 | ApproverDashboardForm | 4 thẻ và hồ sơ cần quyết định | Người phê duyệt | UC16–UC19 family |
| Trang-15 | PendingApprovalForm | Kết quả thẩm định/liên ngành, điều kiện, quyết định duyệt/từ chối | Người phê duyệt | UC16–UC19 family |
| Trang-15 | ExceptionDecisionForm | Quy tắc, cấp thẩm quyền, minh chứng, lý do, chấp thuận/từ chối | Người phê duyệt | UC16–UC19 family |
| Trang-15 | PermitManagementForm | Danh sách giấy phép/văn bản và xuất VB06/VB08 | Người phê duyệt | UC19 / UC27 family (SQL ghi giấy phép UC27) |
| Trang-16 | AdminDashboardForm | Thẻ tài khoản/địa điểm/quy tắc/lượt thay đổi, nhật ký gần đây | Quản trị viên | UC20–UC25 family |
| Trang-16 | AccountManagementForm | Tìm kiếm/lọc vai trò, danh sách, thêm/sửa/khóa tài khoản | Quản trị viên | UC20 (SQL ghi quản lý tài khoản) |
| Trang-16 | EventTypeManagementForm | Danh mục loại sự kiện, trạng thái, thêm/sửa | Quản trị viên | UC21–UC25 family |
| Trang-16 | VenueManagementForm | Danh sách/form địa điểm, địa chỉ, sức chứa, lưu | Quản trị viên | UC21–UC25 family |
| Trang-16 | RuleManagementForm | Danh sách rule/version và form điều kiện/hiệu lực/ngoại lệ | Quản trị viên | UC21–UC25 family |
| Trang-16 | InteragencyConfigForm | Cấu hình loại sự kiện/đơn vị/bắt buộc/hạn/hiệu lực | Quản trị viên | UC21–UC25 family |
| Trang-16 | AuditLogForm | Nhật ký chỉ đọc: thời gian, người, đối tượng, hành động, phiên bản | Quản trị viên | UC21–UC25 family |

The SQL comments do not define UC26, and the available diagram pages do not contain readable UC labels for it. No screen is assigned to UC26 by guess. UC22–UC25 are referenced only as an admin configuration family because the SQL comment groups them and the Draw.io page shows those configuration screens without individual UC labels. A definitive one-to-one audit needs a readable UC specification; screen inventory above is complete for the visible Draw.io screens.

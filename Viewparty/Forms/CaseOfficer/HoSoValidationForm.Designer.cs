using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.CaseOfficer
{
 partial class HoSoValidationForm
 {
  private System.ComponentModel.IContainer components = null;
  private Panel header;
  private Panel sidebar;
  private Panel content;
  private Label lblBrand;
  private Label lblRole;
  private Label lblTitle;
  private Label lblSubtitle;
  private Button nav0;
  private Button nav1;
  private Button nav2;
  private Button nav3;
  private Button nav4;
  private Button nav5;
  private GroupBox casePanel;
  private GroupBox checkPanel;
  private GroupBox resultPanel;
  private CheckBox check0;
  private Label checkStatus0;
  private CheckBox check1;
  private Label checkStatus1;
  private CheckBox check2;
  private Label checkStatus2;
  private CheckBox check3;
  private Label checkStatus3;
  private CheckBox check4;
  private Label checkStatus4;
  private CheckBox check5;
  private Label checkStatus5;
  private RadioButton rbValid;
  private RadioButton rbInvalid;
  private TextBox txtReason;
  private Button btnRequest;
  private Button btnConfirm;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
            SuspendLayout();
            Text = "Kiểm tra tính hợp lệ"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(1440, 900); MinimumSize = new Size(1180, 760); BackColor = ColorTranslator.FromHtml("#F5F8FC"); Font = new Font("Times New Roman", 9.5F);
            header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White, Name = "header" };
            sidebar = new Panel { Dock = DockStyle.Left, Width = 244, BackColor = ColorTranslator.FromHtml("#142C41"), AutoScroll = true, Name = "sidebar" };
            content = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F5F8FC"), AutoScroll = true, Name = "content" };
            lblBrand = new Label { Text = "VIEWPARTY  |  EVENT PERMIT PORTAL", Location = new Point(22, 18), AutoSize = true, Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole = new Label { Text = "Người xử lý", Location = new Point(18, 20), Size = new Size(208, 28), Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle = new Label { Text = "Kiểm tra tính hợp lệ", Location = new Point(26, 18), AutoSize = true, Font = new Font("Times New Roman", 20, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle = new Label { Text = "Đối chiếu thông tin và tài liệu do người tổ chức cung cấp.", Location = new Point(28, 58), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0 = new Button { Text = "Tổng quan", Tag = "Tổng quan", Location = new Point(10, 76), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav0" }; nav0.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav0);
            nav1 = new Button { Text = "Tiếp nhận hồ sơ", Tag = "Tiếp nhận hồ sơ", Location = new Point(10, 122), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav1" }; nav1.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav1);
            nav2 = new Button { Text = "Kiểm tra hợp lệ", Tag = "Kiểm tra hợp lệ", Location = new Point(10, 168), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav2" }; nav2.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav2);
            nav3 = new Button { Text = "Thẩm định hồ sơ", Tag = "Thẩm định hồ sơ", Location = new Point(10, 214), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav3" }; nav3.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav3);
            nav4 = new Button { Text = "Ý kiến liên ngành", Tag = "Ý kiến liên ngành", Location = new Point(10, 260), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav4" }; nav4.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav4);
            nav5 = new Button { Text = "Tổng hợp / ngoại lệ", Tag = "Tổng hợp / ngoại lệ", Location = new Point(10, 306), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav5" }; nav5.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav5);
            casePanel = new GroupBox { Text = "Hồ sơ được chọn", Location = new Point(26,108), Size = new Size(1080,92), BackColor = Color.White, ForeColor = ColorTranslator.FromHtml("#142C41") }; casePanel.Controls.Add(new Label { Text = "Mã hồ sơ / tên sự kiện / người tổ chức sẽ tải từ SQL", Location = new Point(18,38), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }); content.Controls.Add(casePanel);
            checkPanel = new GroupBox { Text = "Thông tin và tài liệu do người tổ chức cung cấp", Location = new Point(26,210), Size = new Size(1080,300), BackColor = Color.White, ForeColor = ColorTranslator.FromHtml("#142C41") };
            check0 = new CheckBox { Text = "Thông tin sự kiện", Location = new Point(20,44), AutoSize = true, Name = "check0" }; checkStatus0 = new Label { Text = "Chưa đánh giá", Location = new Point(260,46), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; checkPanel.Controls.AddRange(new Control[] { check0,checkStatus0 });
            check1 = new CheckBox { Text = "Địa điểm", Location = new Point(520,44), AutoSize = true, Name = "check1" }; checkStatus1 = new Label { Text = "Chưa đánh giá", Location = new Point(760,46), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; checkPanel.Controls.AddRange(new Control[] { check1,checkStatus1 });
            check2 = new CheckBox { Text = "Vé / QR", Location = new Point(20,120), AutoSize = true, Name = "check2" }; checkStatus2 = new Label { Text = "Chưa đánh giá", Location = new Point(260,122), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; checkPanel.Controls.AddRange(new Control[] { check2,checkStatus2 });
            check3 = new CheckBox { Text = "Quyền trình chiếu", Location = new Point(520,120), AutoSize = true, Name = "check3" }; checkStatus3 = new Label { Text = "Chưa đánh giá", Location = new Point(760,122), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; checkPanel.Controls.AddRange(new Control[] { check3,checkStatus3 });
            check4 = new CheckBox { Text = "Phương án an toàn", Location = new Point(20,196), AutoSize = true, Name = "check4" }; checkStatus4 = new Label { Text = "Chưa đánh giá", Location = new Point(260,198), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; checkPanel.Controls.AddRange(new Control[] { check4,checkStatus4 });
            check5 = new CheckBox { Text = "Tài liệu đính kèm", Location = new Point(520,196), AutoSize = true, Name = "check5" }; checkStatus5 = new Label { Text = "Chưa đánh giá", Location = new Point(760,198), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; checkPanel.Controls.AddRange(new Control[] { check5,checkStatus5 });
            resultPanel = new GroupBox { Text = "Kết luận kiểm tra", Location = new Point(26,520), Size = new Size(1080,190), BackColor = Color.White, ForeColor = ColorTranslator.FromHtml("#142C41") };
            rbValid = new RadioButton { Text = "Hợp lệ → Chuyển thẩm định", Location = new Point(20,40), AutoSize = true }; rbInvalid = new RadioButton { Text = "Chưa hợp lệ → Yêu cầu bổ sung", Location = new Point(300,40), AutoSize = true }; txtReason = new TextBox { Location = new Point(20,78), Size = new Size(720,68), Multiline = true }; btnRequest = new Button { Text = "Yêu cầu bổ sung", Location = new Point(760,78), Size = new Size(140,38), BackColor = Color.White }; btnConfirm = new Button { Text = "Xác nhận hợp lệ", Location = new Point(910,78), Size = new Size(140,38), BackColor = ColorTranslator.FromHtml("#137A78"), ForeColor = Color.White }; resultPanel.Controls.AddRange(new Control[] { rbValid,rbInvalid,txtReason,btnRequest,btnConfirm }); content.Controls.AddRange(new Control[] { checkPanel,resultPanel });
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}


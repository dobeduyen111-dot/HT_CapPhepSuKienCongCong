using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.CaseOfficer
{
 partial class InteragencyRequestForm
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
  private GroupBox caseInfo;
  private Label lblCaseId, lblEventType;
  private TextBox txtCaseId, txtEventType;
  private DataGridView requestsGrid;
  private Button btnSendRequest;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
            SuspendLayout();
            Text = "Xin ý kiến liên ngành"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(1440, 900); MinimumSize = new Size(1180, 760); BackColor = ColorTranslator.FromHtml("#F5F8FC"); Font = new Font("Times New Roman", 9.5F);
            header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White, Name = "header" };
            sidebar = new Panel { Dock = DockStyle.Left, Width = 244, BackColor = ColorTranslator.FromHtml("#142C41"), AutoScroll = true, Name = "sidebar" };
            content = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F5F8FC"), AutoScroll = true, Name = "content" };
            lblBrand = new Label { Text = "VIEWPARTY  |  EVENT PERMIT PORTAL", Location = new Point(22, 18), AutoSize = true, Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole = new Label { Text = "Người xử lý", Location = new Point(18, 20), Size = new Size(208, 28), Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle = new Label { Text = "Xin ý kiến liên ngành", Location = new Point(26, 18), AutoSize = true, Font = new Font("Times New Roman", 20, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle = new Label { Text = "Danh sách đơn vị theo cấu hình liên ngành và phiên bản quy tắc.", Location = new Point(28, 58), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0 = new Button { Text = "Tổng quan", Tag = "Tổng quan", Location = new Point(10, 76), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav0" }; nav0.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav0);
            nav1 = new Button { Text = "Tiếp nhận hồ sơ", Tag = "Tiếp nhận hồ sơ", Location = new Point(10, 122), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav1" }; nav1.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav1);
            nav2 = new Button { Text = "Kiểm tra hợp lệ", Tag = "Kiểm tra hợp lệ", Location = new Point(10, 168), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav2" }; nav2.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav2);
            nav3 = new Button { Text = "Thẩm định hồ sơ", Tag = "Thẩm định hồ sơ", Location = new Point(10, 214), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav3" }; nav3.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav3);
            nav4 = new Button { Text = "Ý kiến liên ngành", Tag = "Ý kiến liên ngành", Location = new Point(10, 260), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav4" }; nav4.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav4);
            nav5 = new Button { Text = "Tổng hợp / ngoại lệ", Tag = "Tổng hợp / ngoại lệ", Location = new Point(10, 306), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav5" }; nav5.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav5);
            caseInfo = new GroupBox { Text = "Thông tin hồ sơ", Location = new Point(26,112), Size = new Size(1080,130), BackColor = Color.White, ForeColor = ColorTranslator.FromHtml("#142C41") };
            caseInfo.Controls.Add(new Label { Text = "Mã hồ sơ, loại sự kiện và người tổ chức sẽ tải từ hồ sơ được chọn.", Location = new Point(18,48), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") });
            lblCaseId = new Label { Text="M\u00e3 h\u1ed3 s\u01a1", Location=new Point(18,38), AutoSize=true }; txtCaseId = new TextBox { Location=new Point(165,34), Size=new Size(300,28), ReadOnly=true };
            lblEventType = new Label { Text="Lo\u1ea1i s\u1ef1 ki\u1ec7n", Location=new Point(520,38), AutoSize=true }; txtEventType = new TextBox { Location=new Point(680,34), Size=new Size(350,28), ReadOnly=true };
            caseInfo.Controls.AddRange(new Control[] { lblCaseId,txtCaseId,lblEventType,txtEventType });
            requestsGrid = new DataGridView { Location = new Point(26,262), Size = new Size(1080,360), ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, EnableHeadersVisualStyles = false };
            requestsGrid.Columns.Add("request0","Đơn vị liên ngành");
            requestsGrid.Columns.Add("request1","Lĩnh vực");
            requestsGrid.Columns.Add("request2","Bắt buộc");
            requestsGrid.Columns.Add("request3","Hạn phản hồi");
            btnSendRequest = new Button { Text = "Gửi yêu cầu liên ngành", Location = new Point(26,650), Size = new Size(200,42), BackColor = ColorTranslator.FromHtml("#137A78"), ForeColor = Color.White }; content.Controls.AddRange(new Control[] { caseInfo,requestsGrid,btnSendRequest });
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

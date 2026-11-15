using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.CaseOfficer
{
 partial class ExceptionProposalForm
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
  private GroupBox exceptionForm;
  private TextBox txtReason;
  private Label lblEvidence;
  private TextBox txtEvidence;
  private TextBox txtMitigation;
  private Button btnSaveProposal;
  private Label lblRule;
  private Label lblLevel;
  private Label lblReason;
  private Label lblMitigation;
  private ComboBox cmbLevel;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
            SuspendLayout();
            Text = "Đề xuất ngoại lệ"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(1440, 900); MinimumSize = new Size(1180, 760); BackColor = ColorTranslator.FromHtml("#F5F8FC"); Font = new Font("Times New Roman", 9.5F);
            header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White, Name = "header" };
            sidebar = new Panel { Dock = DockStyle.Left, Width = 244, BackColor = ColorTranslator.FromHtml("#142C41"), AutoScroll = true, Name = "sidebar" };
            content = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F5F8FC"), AutoScroll = true, Name = "content" };
            lblBrand = new Label { Text = "VIEWPARTY  |  EVENT PERMIT PORTAL", Location = new Point(22, 18), AutoSize = true, Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole = new Label { Text = "Người xử lý", Location = new Point(18, 20), Size = new Size(208, 28), Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle = new Label { Text = "Đề xuất ngoại lệ", Location = new Point(26, 18), AutoSize = true, Font = new Font("Times New Roman", 20, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle = new Label { Text = "Kết quả thẩm định gốc vẫn được lưu riêng, không sửa thành Đạt.", Location = new Point(28, 58), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0 = new Button { Text = "Tổng quan", Tag = "Tổng quan", Location = new Point(10, 76), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav0" }; nav0.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav0);
            nav1 = new Button { Text = "Tiếp nhận hồ sơ", Tag = "Tiếp nhận hồ sơ", Location = new Point(10, 122), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav1" }; nav1.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav1);
            nav2 = new Button { Text = "Kiểm tra hợp lệ", Tag = "Kiểm tra hợp lệ", Location = new Point(10, 168), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav2" }; nav2.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav2);
            nav3 = new Button { Text = "Thẩm định hồ sơ", Tag = "Thẩm định hồ sơ", Location = new Point(10, 214), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav3" }; nav3.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav3);
            nav4 = new Button { Text = "Ý kiến liên ngành", Tag = "Ý kiến liên ngành", Location = new Point(10, 260), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav4" }; nav4.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav4);
            nav5 = new Button { Text = "Tổng hợp / ngoại lệ", Tag = "Tổng hợp / ngoại lệ", Location = new Point(10, 306), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav5" }; nav5.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav5);
            casePanel = new GroupBox { Text="Điều kiện không đạt và hồ sơ", Location=new Point(26,112), Size=new Size(1080,130), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41") }; casePanel.Controls.Add(new Label { Text="Quy tắc / phiên bản và cấp thẩm quyền sẽ lấy từ kết quả thẩm định.", Location=new Point(18,48), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") });
            exceptionForm = new GroupBox { Text="Phiếu đề xuất ngoại lệ", Location=new Point(26,260), Size=new Size(1080,470), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41") };
            lblRule = new Label { Text="Quy tắc / Phiên bản", Location=new Point(18,42), AutoSize=true }; exceptionForm.Controls.Add(lblRule);
            lblLevel = new Label { Text="Cấp thẩm quyền dự kiến", Location=new Point(18,96), AutoSize=true }; exceptionForm.Controls.Add(lblLevel);
            lblReason = new Label { Text="Lý do đề xuất", Location=new Point(18,150), AutoSize=true }; exceptionForm.Controls.Add(lblReason);
            lblMitigation = new Label { Text="Biện pháp bù đắp", Location=new Point(18,280), AutoSize=true }; exceptionForm.Controls.Add(lblMitigation);
            cmbLevel = new ComboBox { Location=new Point(285,92), Size=new Size(700,30), DropDownStyle=ComboBoxStyle.DropDownList }; cmbLevel.Items.AddRange(new object[] { "Cấp 1", "Cấp 2", "Cấp 3" }); exceptionForm.Controls.Add(cmbLevel);
            txtReason = new TextBox { Location=new Point(285,146), Size=new Size(700,110), Multiline=true, ScrollBars=ScrollBars.Vertical }; txtMitigation = new TextBox { Location=new Point(285,276), Size=new Size(700,90), Multiline=true, ScrollBars=ScrollBars.Vertical }; exceptionForm.Controls.AddRange(new Control[] { txtReason,txtMitigation });
            lblEvidence = new Label { Text="Minh ch\u1ee9ng k\u00e8m theo", Location=new Point(18,388), AutoSize=true };
            txtEvidence = new TextBox { Location=new Point(285,384), Size=new Size(700,32), ReadOnly=true, Name="txtEvidence" };
            exceptionForm.Controls.AddRange(new Control[] { lblEvidence,txtEvidence });
            btnSaveProposal = new Button { Text="Lưu đề xuất và gửi duyệt", Name="btnSaveProposal", Location=new Point(26,750), Size=new Size(220,42), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White }; content.Controls.AddRange(new Control[] { casePanel,exceptionForm,btnSaveProposal });
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.CaseOfficer
{
 partial class HoSoAppraisalForm
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
  private TabControl tabs;
  private TabPage tabVenue;
  private TabPage tabSafety;
  private TabPage tabConditions;
  private DataGridView rulesGrid;
  private Button btnSaveEvaluation;
  private Label lblEvaluation, lblActual;
  private ComboBox cmbEvaluation;
  private TextBox txtEvaluationNote;
  private Label venueInfo;
  private Label safetyInfo;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
            SuspendLayout();
            Text = "Thẩm định hồ sơ"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(1440, 900); MinimumSize = new Size(1180, 760); BackColor = ColorTranslator.FromHtml("#F5F8FC"); Font = new Font("Times New Roman", 9.5F);
            header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White, Name = "header" };
            sidebar = new Panel { Dock = DockStyle.Left, Width = 244, BackColor = ColorTranslator.FromHtml("#142C41"), AutoScroll = true, Name = "sidebar" };
            content = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F5F8FC"), AutoScroll = true, Name = "content" };
            lblBrand = new Label { Text = "VIEWPARTY  |  EVENT PERMIT PORTAL", Location = new Point(22, 18), AutoSize = true, Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole = new Label { Text = "Người xử lý", Location = new Point(18, 20), Size = new Size(208, 28), Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle = new Label { Text = "Thẩm định hồ sơ", Location = new Point(26, 18), AutoSize = true, Font = new Font("Times New Roman", 20, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle = new Label { Text = "Đánh giá theo địa điểm, an toàn và các điều kiện.", Location = new Point(28, 58), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0 = new Button { Text = "Tổng quan", Tag = "Tổng quan", Location = new Point(10, 76), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav0" }; nav0.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav0);
            nav1 = new Button { Text = "Tiếp nhận hồ sơ", Tag = "Tiếp nhận hồ sơ", Location = new Point(10, 122), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav1" }; nav1.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav1);
            nav2 = new Button { Text = "Kiểm tra hợp lệ", Tag = "Kiểm tra hợp lệ", Location = new Point(10, 168), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav2" }; nav2.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav2);
            nav3 = new Button { Text = "Thẩm định hồ sơ", Tag = "Thẩm định hồ sơ", Location = new Point(10, 214), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav3" }; nav3.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav3);
            nav4 = new Button { Text = "Ý kiến liên ngành", Tag = "Ý kiến liên ngành", Location = new Point(10, 260), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav4" }; nav4.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav4);
            nav5 = new Button { Text = "Tổng hợp / ngoại lệ", Tag = "Tổng hợp / ngoại lệ", Location = new Point(10, 306), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav5" }; nav5.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav5);
            tabs = new TabControl { Location = new Point(26,112), Size = new Size(1080,570), Name = "tabs" }; tabVenue = new TabPage("Địa điểm"); tabSafety = new TabPage("An toàn"); tabConditions = new TabPage("Điều kiện cấp phép"); tabs.TabPages.AddRange(new TabPage[] { tabVenue,tabSafety,tabConditions });
            venueInfo = new Label { Text = "Thông tin địa điểm và sức chứa tải từ hồ sơ", Location = new Point(20,24), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") };
            safetyInfo = new Label { Text = "Phương án an toàn và tài liệu tải từ hồ sơ", Location = new Point(20,24), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") };
            tabVenue.Controls.Add(venueInfo); tabSafety.Controls.Add(safetyInfo);
            rulesGrid = new DataGridView { Location = new Point(12,12), Size = new Size(1030,430), ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, Name = "rulesGrid" };
            rulesGrid.Columns.Add("rule0","Mã quy tắc");
            rulesGrid.Columns.Add("rule1","Điều kiện");
            rulesGrid.Columns.Add("rule2","Đánh giá");
            tabConditions.Controls.Add(rulesGrid);
            lblEvaluation = new Label { Text = "K\u1ebft qu\u1ea3", Location = new Point(14,454), AutoSize = true };
            cmbEvaluation = new ComboBox { Location = new Point(165,450), Size = new Size(240,30), DropDownStyle = ComboBoxStyle.DropDownList, Name = "cmbEvaluation" };
            cmbEvaluation.Items.AddRange(new object[] { "Ch\u1ecdn k\u1ebft qu\u1ea3", "\u0110\u1ea0T", "KH\u00d4NG \u0110\u1ea0T", "C\u1ea2NH B\u00c1O" }); cmbEvaluation.SelectedIndex = 0;
            lblActual = new Label { Text = "Gi\u00e1 tr\u1ecb th\u1ef1c t\u1ebf / Ghi ch\u00fa", Location = new Point(430,454), AutoSize = true };
            txtEvaluationNote = new TextBox { Location = new Point(635,450), Size = new Size(390,58), Multiline = true, Name = "txtEvaluationNote" };
            tabConditions.Controls.AddRange(new Control[] { lblEvaluation,cmbEvaluation,lblActual,txtEvaluationNote }); btnSaveEvaluation = new Button { Text = "Lưu kết quả", Location = new Point(26,700), Size = new Size(140,40), BackColor = ColorTranslator.FromHtml("#137A78"), ForeColor = Color.White }; content.Controls.AddRange(new Control[] { tabs,btnSaveEvaluation });
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

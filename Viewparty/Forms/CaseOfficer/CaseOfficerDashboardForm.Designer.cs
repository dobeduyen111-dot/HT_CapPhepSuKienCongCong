using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.CaseOfficer
{
 partial class CaseOfficerDashboardForm
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
  private TableLayoutPanel statsPanel;
  private Panel statCard0;
  private Label statLabel0;
  private Label statValue0;
  private Label statNote0;
  private Panel statCard1;
  private Label statLabel1;
  private Label statValue1;
  private Label statNote1;
  private Panel statCard2;
  private Label statLabel2;
  private Label statValue2;
  private Label statNote2;
  private Panel statCard3;
  private Label statLabel3;
  private Label statValue3;
  private Label statNote3;
  private GroupBox section;
  private DataGridView grid;
  private Button btnIntake;
  private Button btnValidate;
  private Button btnAppraise;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
            SuspendLayout();
            Text = "Tổng quan Người xử lý"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(1440, 900); MinimumSize = new Size(1180, 760); BackColor = ColorTranslator.FromHtml("#F5F8FC"); Font = new Font("Times New Roman", 9.5F);
            header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White, Name = "header" };
            sidebar = new Panel { Dock = DockStyle.Left, Width = 244, BackColor = ColorTranslator.FromHtml("#142C41"), AutoScroll = true, Name = "sidebar" };
            content = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F5F8FC"), AutoScroll = true, Name = "content" };
            lblBrand = new Label { Text = "VIEWPARTY  |  EVENT PERMIT PORTAL", Location = new Point(22, 18), AutoSize = true, Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole = new Label { Text = "Người xử lý", Location = new Point(18, 20), Size = new Size(208, 28), Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle = new Label { Text = "Tổng quan Người xử lý", Location = new Point(26, 18), AutoSize = true, Font = new Font("Times New Roman", 20, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle = new Label { Text = "Danh sách cần xử lý ưu tiên theo hạn và trạng thái.", Location = new Point(28, 58), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0 = new Button { Text = "Tổng quan", Tag = "Tổng quan", Location = new Point(10, 76), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav0" }; nav0.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav0);
            nav1 = new Button { Text = "Tiếp nhận hồ sơ", Tag = "Tiếp nhận hồ sơ", Location = new Point(10, 122), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav1" }; nav1.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav1);
            nav2 = new Button { Text = "Kiểm tra hợp lệ", Tag = "Kiểm tra hợp lệ", Location = new Point(10, 168), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav2" }; nav2.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav2);
            nav3 = new Button { Text = "Thẩm định hồ sơ", Tag = "Thẩm định hồ sơ", Location = new Point(10, 214), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav3" }; nav3.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav3);
            nav4 = new Button { Text = "Ý kiến liên ngành", Tag = "Ý kiến liên ngành", Location = new Point(10, 260), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav4" }; nav4.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav4);
            nav5 = new Button { Text = "Tổng hợp / ngoại lệ", Tag = "Tổng hợp / ngoại lệ", Location = new Point(10, 306), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav5" }; nav5.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav5);
            statsPanel = new TableLayoutPanel { Location = new Point(26, 108), Size = new Size(1080, 112), ColumnCount = 4, RowCount = 1, BackColor = ColorTranslator.FromHtml("#F5F8FC"), Name = "statsPanel" };
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); statCard0 = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0,0,12,0), Name = "statCard0" };
            statLabel0 = new Label { Text = "Chờ tiếp nhận", Location = new Point(14,12), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; statValue0 = new Label { Text = "?", Location = new Point(14,36), AutoSize = true, Font = new Font("Times New Roman",22,FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; statNote0 = new Label { Text = "Chờ dữ liệu SQL", Location = new Point(14,78), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; statCard0.Controls.AddRange(new Control[] { statLabel0,statValue0,statNote0 }); statsPanel.Controls.Add(statCard0,0,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); statCard1 = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0,0,12,0), Name = "statCard1" };
            statLabel1 = new Label { Text = "Đang kiểm tra", Location = new Point(14,12), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; statValue1 = new Label { Text = "?", Location = new Point(14,36), AutoSize = true, Font = new Font("Times New Roman",22,FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; statNote1 = new Label { Text = "Chờ dữ liệu SQL", Location = new Point(14,78), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; statCard1.Controls.AddRange(new Control[] { statLabel1,statValue1,statNote1 }); statsPanel.Controls.Add(statCard1,1,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); statCard2 = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0,0,12,0), Name = "statCard2" };
            statLabel2 = new Label { Text = "Chờ liên ngành", Location = new Point(14,12), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; statValue2 = new Label { Text = "?", Location = new Point(14,36), AutoSize = true, Font = new Font("Times New Roman",22,FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; statNote2 = new Label { Text = "Chờ dữ liệu SQL", Location = new Point(14,78), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; statCard2.Controls.AddRange(new Control[] { statLabel2,statValue2,statNote2 }); statsPanel.Controls.Add(statCard2,2,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); statCard3 = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0,0,12,0), Name = "statCard3" };
            statLabel3 = new Label { Text = "Chờ tổng hợp", Location = new Point(14,12), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; statValue3 = new Label { Text = "?", Location = new Point(14,36), AutoSize = true, Font = new Font("Times New Roman",22,FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; statNote3 = new Label { Text = "Chờ dữ liệu SQL", Location = new Point(14,78), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; statCard3.Controls.AddRange(new Control[] { statLabel3,statValue3,statNote3 }); statsPanel.Controls.Add(statCard3,3,0);
            content.Controls.Add(statsPanel);
            section = new GroupBox { Text = "Danh sách cần xử lý hôm nay", Location = new Point(26,242), Size = new Size(1080, 430), BackColor = Color.White, ForeColor = ColorTranslator.FromHtml("#142C41"), Font = new Font("Times New Roman",11,FontStyle.Bold), Name = "section" };
            grid = new DataGridView { Location = new Point(14,48), Size = new Size(1044, 350), Anchor = AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 36, Name = "grid" }; grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#EEF3F7"); grid.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#142C41");
            grid.Columns.Add("column0","Mã hồ sơ");
            grid.Columns.Add("column1","Tên sự kiện");
            grid.Columns.Add("column2","Người tổ chức");
            grid.Columns.Add("column3","Trạng thái");
            grid.Columns.Add("column4","Hành động");
            section.Controls.Add(grid); content.Controls.Add(section);
            btnIntake = new Button { Text = "Mở tiếp nhận hồ sơ", Location = new Point(26,690), Size = new Size(178,40), BackColor = ColorTranslator.FromHtml("#137A78"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Name = "btnIntake" }; content.Controls.Add(btnIntake);
            btnValidate = new Button { Text = "Kiểm tra hợp lệ", Location = new Point(216,690), Size = new Size(178,40), BackColor = ColorTranslator.FromHtml("#137A78"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Name = "btnValidate" }; content.Controls.Add(btnValidate);
            btnAppraise = new Button { Text = "Thẩm định hồ sơ", Location = new Point(406,690), Size = new Size(178,40), BackColor = ColorTranslator.FromHtml("#137A78"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Name = "btnAppraise" }; content.Controls.Add(btnAppraise);
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

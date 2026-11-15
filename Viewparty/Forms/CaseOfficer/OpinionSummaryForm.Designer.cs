using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.CaseOfficer
{
 partial class OpinionSummaryForm
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
  private DataGridView summaryGrid;
  private GroupBox conclusionPanel;
  private RadioButton rbReady;
  private RadioButton rbException;
  private RadioButton rbReject;
  private Button btnSaveSummary;
  private Button btnException;
  private Panel sumCard0;
  private Panel sumCard1;
  private Panel sumCard2;
  private Panel sumCard3;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
            SuspendLayout();
            Text = "Tổng hợp thẩm định"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(1440, 900); MinimumSize = new Size(1180, 760); BackColor = ColorTranslator.FromHtml("#F5F8FC"); Font = new Font("Times New Roman", 9.5F);
            header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White, Name = "header" };
            sidebar = new Panel { Dock = DockStyle.Left, Width = 244, BackColor = ColorTranslator.FromHtml("#142C41"), AutoScroll = true, Name = "sidebar" };
            content = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F5F8FC"), AutoScroll = true, Name = "content" };
            lblBrand = new Label { Text = "VIEWPARTY  |  EVENT PERMIT PORTAL", Location = new Point(22, 18), AutoSize = true, Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole = new Label { Text = "Người xử lý", Location = new Point(18, 20), Size = new Size(208, 28), Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle = new Label { Text = "Tổng hợp thẩm định", Location = new Point(26, 18), AutoSize = true, Font = new Font("Times New Roman", 20, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle = new Label { Text = "Tổng hợp kết quả quy tắc và phản hồi liên ngành.", Location = new Point(28, 58), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0 = new Button { Text = "Tổng quan", Tag = "Tổng quan", Location = new Point(10, 76), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav0" }; nav0.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav0);
            nav1 = new Button { Text = "Tiếp nhận hồ sơ", Tag = "Tiếp nhận hồ sơ", Location = new Point(10, 122), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav1" }; nav1.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav1);
            nav2 = new Button { Text = "Kiểm tra hợp lệ", Tag = "Kiểm tra hợp lệ", Location = new Point(10, 168), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav2" }; nav2.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav2);
            nav3 = new Button { Text = "Thẩm định hồ sơ", Tag = "Thẩm định hồ sơ", Location = new Point(10, 214), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav3" }; nav3.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav3);
            nav4 = new Button { Text = "Ý kiến liên ngành", Tag = "Ý kiến liên ngành", Location = new Point(10, 260), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav4" }; nav4.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav4);
            nav5 = new Button { Text = "Tổng hợp / ngoại lệ", Tag = "Tổng hợp / ngoại lệ", Location = new Point(10, 306), Size = new Size(224, 42), TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White, Name = "nav5" }; nav5.FlatAppearance.BorderSize = 0; sidebar.Controls.Add(nav5);
            statsPanel = new TableLayoutPanel { Location = new Point(26,112), Size = new Size(1080,100), ColumnCount=4, RowCount=1 };
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); sumCard0 = new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; sumCard0.Controls.Add(new Label { Text="Điều kiện đạt", Location=new Point(12,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }); sumCard0.Controls.Add(new Label { Text="?", Location=new Point(12,42), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }); statsPanel.Controls.Add(sumCard0,0,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); sumCard1 = new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; sumCard1.Controls.Add(new Label { Text="Điều kiện không đạt", Location=new Point(12,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }); sumCard1.Controls.Add(new Label { Text="?", Location=new Point(12,42), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }); statsPanel.Controls.Add(sumCard1,1,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); sumCard2 = new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; sumCard2.Controls.Add(new Label { Text="Ý kiến bắt buộc", Location=new Point(12,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }); sumCard2.Controls.Add(new Label { Text="?", Location=new Point(12,42), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }); statsPanel.Controls.Add(sumCard2,2,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); sumCard3 = new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; sumCard3.Controls.Add(new Label { Text="Cảnh báo", Location=new Point(12,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }); sumCard3.Controls.Add(new Label { Text="?", Location=new Point(12,42), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }); statsPanel.Controls.Add(sumCard3,3,0);
            summaryGrid = new DataGridView { Location = new Point(26,232), Size = new Size(1080,270), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None };
            summaryGrid.Columns.Add("sum0","Nhóm điều kiện");
            summaryGrid.Columns.Add("sum1","Kết quả");
            summaryGrid.Columns.Add("sum2","Kiến nghị");
            conclusionPanel = new GroupBox { Text="Kết luận nghiệp vụ", Location=new Point(26,522), Size=new Size(1080,150), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41") }; rbReady=new RadioButton { Text="Đủ điều kiện → Chờ duyệt", Location=new Point(18,38), AutoSize=true }; rbException=new RadioButton { Text="Đề xuất ngoại lệ", Location=new Point(330,38), AutoSize=true }; rbReject=new RadioButton { Text="Không đủ điều kiện", Location=new Point(600,38), AutoSize=true }; btnSaveSummary=new Button { Text="Lưu tổng hợp", Location=new Point(18,86), Size=new Size(140,38), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White }; btnException=new Button { Text="Lập đề xuất ngoại lệ", Name="btnException", Location=new Point(175,86), Size=new Size(190,38) }; conclusionPanel.Controls.AddRange(new Control[] { rbReady,rbException,rbReject,btnSaveSummary,btnException }); content.Controls.AddRange(new Control[] { statsPanel,summaryGrid,conclusionPanel });
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

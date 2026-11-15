using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Interagency
{
 partial class InteragencyDashboardForm
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
  private GroupBox requestsGroup;
  private DataGridView requestsGrid;
  private Button btnRequestList;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
            Text="Tổng quan Đơn vị liên ngành"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
            header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
            lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole=new Label { Text="Đơn vị liên ngành", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle=new Label { Text="Tổng quan Đơn vị liên ngành", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle=new Label { Text="Tại đây chỉ hiển thị hồ sơ được gửi cho đơn vị của bạn.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0=new Button { Text="Tổng quan", Tag="Tổng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
            nav1=new Button { Text="Yêu cầu cần phản hồi", Tag="Yêu cầu cần phản hồi", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
            nav2=new Button { Text="Chi tiết hồ sơ", Tag="Chi tiết hồ sơ", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
            nav3=new Button { Text="Gửi ý kiến", Tag="Gửi ý kiến", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
            nav4=new Button { Text="Văn bản phản hồi", Tag="Văn bản phản hồi", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
            statsPanel=new TableLayoutPanel { Location=new Point(26,108), Size=new Size(1080,112), ColumnCount=4, RowCount=1, BackColor=ColorTranslator.FromHtml("#F5F8FC"), Name="statsPanel" };
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard0=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0), Name="statCard0" };
            statLabel0=new Label { Text="Yêu cầu mới", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue0=new Label { Text="—", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote0=new Label { Text="Chờ dữ liệu SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard0.Controls.AddRange(new Control[] { statLabel0,statValue0,statNote0 }); statsPanel.Controls.Add(statCard0,0,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard1=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0), Name="statCard1" };
            statLabel1=new Label { Text="Sắp quá hạn", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue1=new Label { Text="—", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote1=new Label { Text="Chờ dữ liệu SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard1.Controls.AddRange(new Control[] { statLabel1,statValue1,statNote1 }); statsPanel.Controls.Add(statCard1,1,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard2=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0), Name="statCard2" };
            statLabel2=new Label { Text="Đã phản hồi", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue2=new Label { Text="—", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote2=new Label { Text="Chờ dữ liệu SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard2.Controls.AddRange(new Control[] { statLabel2,statValue2,statNote2 }); statsPanel.Controls.Add(statCard2,2,0);
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard3=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0), Name="statCard3" };
            statLabel3=new Label { Text="Đang xem xét", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue3=new Label { Text="—", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote3=new Label { Text="Chờ dữ liệu SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard3.Controls.AddRange(new Control[] { statLabel3,statValue3,statNote3 }); statsPanel.Controls.Add(statCard3,3,0);
            requestsGroup=new GroupBox { Text="Yêu cầu đang chờ ý kiến", Location=new Point(26,238), Size=new Size(1080,420), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold) };
            requestsGroup.Controls.Add(new Label { Text="Tại đây chỉ hiển thị hồ sơ được gửi cho đơn vị của bạn.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
            requestsGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,330), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="requestsGrid" };
            requestsGrid.Columns.Add("col0","Mã yêu cầu");
            requestsGrid.Columns.Add("col1","Hồ sơ");
            requestsGrid.Columns.Add("col2","Tên sự kiện");
            requestsGrid.Columns.Add("col3","Hạn phản hồi");
            requestsGrid.Columns.Add("col4","Hành động");
            requestsGroup.Controls.Add(requestsGrid);
            btnRequestList=new Button { Text="Xem yêu cầu cần phản hồi", Name="btnRequestList", Location=new Point(26,684), Size=new Size(220,42), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.AddRange(new Control[] { statsPanel,requestsGroup,btnRequestList });
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
   ResumeLayout(false);
  }
 }
}

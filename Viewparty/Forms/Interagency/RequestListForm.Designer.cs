using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Interagency
{
 partial class RequestListForm
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
  private GroupBox requestsGroup;
  private DataGridView requestsGrid;
  private Button btnViewDetails;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
            Text="Yêu cầu cần phản hồi"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
            header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
            lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole=new Label { Text="Đơn vị liên ngành", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle=new Label { Text="Yêu cầu cần phản hồi", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle=new Label { Text="Danh sách yêu cầu ý kiến đã gửi đến đơn vị đang đăng nhập.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0=new Button { Text="Tổng quan", Tag="Tổng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
            nav1=new Button { Text="Yêu cầu cần phản hồi", Tag="Yêu cầu cần phản hồi", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
            nav2=new Button { Text="Chi tiết hồ sơ", Tag="Chi tiết hồ sơ", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
            nav3=new Button { Text="Gửi ý kiến", Tag="Gửi ý kiến", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
            nav4=new Button { Text="Văn bản phản hồi", Tag="Văn bản phản hồi", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
            requestsGroup=new GroupBox { Text="Danh sách yêu cầu cần phản hồi", Location=new Point(26,112), Size=new Size(1080,560), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold) }; requestsGroup.Controls.Add(new Label { Text="Chỉ hiển thị các yêu cầu đã gửi đến đơn vị đang đăng nhập.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
            requestsGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,475), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="requestsGrid" };
            requestsGrid.Columns.Add("col0","Mã yêu cầu");
            requestsGrid.Columns.Add("col1","Hồ sơ");
            requestsGrid.Columns.Add("col2","Tên sự kiện");
            requestsGrid.Columns.Add("col3","Đơn vị gửi yêu cầu");
            requestsGrid.Columns.Add("col4","Hạn phản hồi");
            requestsGrid.Columns.Add("col5","Trạng thái");
            requestsGrid.Columns.Add("col6","Hành động");
            requestsGroup.Controls.Add(requestsGrid); btnViewDetails=new Button { Text="Xem chi tiết", Name="btnViewDetails", Location=new Point(26,692), Size=new Size(150,40), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.AddRange(new Control[] { requestsGroup,btnViewDetails });
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
   ResumeLayout(false);
  }
 }
}

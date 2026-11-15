using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Admin
{
 partial class AuditLogForm
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
  private GroupBox dataGroup;
  private DataGridView dataGrid;
  private GroupBox detailGroup;
  private TextBox beforeText;
  private TextBox afterText;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
   Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
   header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
   lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
   lblRole=new Label { Text="Qu\u1EA3n tr\u1ECB vi\u00EAn", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
   lblTitle=new Label { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
   lblSubtitle=new Label { Text="L\u1ECBch s\u1EED ch\u1EC9 \u0111\u1ECDc; m\u1ED7i l\u1EA7n thay \u0111\u1ED5i c\u1EA5u h\u00ECnh \u0111\u01B0\u1EE3c ghi th\u00E0nh b\u1EA3n ghi m\u1EDBi.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
   nav0=new Button { Text="T\u1ED5ng quan", Tag="T\u1ED5ng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
   nav1=new Button { Text="T\u00E0i kho\u1EA3n / vai tr\u00F2", Tag="T\u00E0i kho\u1EA3n / vai tr\u00F2", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
   nav2=new Button { Text="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Tag="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
   nav3=new Button { Text="\u0110\u1ECBa \u0111i\u1EC3m", Tag="\u0110\u1ECBa \u0111i\u1EC3m", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
   nav4=new Button { Text="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Tag="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
   nav5=new Button { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Tag="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Location=new Point(10,306), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav5" }; nav5.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav5);
   dataGroup=new GroupBox { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Location=new Point(26,108), Size=new Size(1080,470), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold), Name="dataGroup" };
   dataGroup.Controls.Add(new Label { Text="Dữ liệu sẽ hiển thị sau khi tích hợp Service/API và SQL Server.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
   dataGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,390), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="dataGrid" };
   dataGrid.Columns.Add("col0","Th\u1EDDi gian");
   dataGrid.Columns.Add("col1","T\u00E0i kho\u1EA3n");
   dataGrid.Columns.Add("col2","\u0110\u1ED1i t\u01B0\u1EE3ng");
   dataGrid.Columns.Add("col3","Kh\u00F3a \u0111\u1ED1i t\u01B0\u1EE3ng");
   dataGrid.Columns.Add("col4","H\u00E0nh \u0111\u1ED9ng");
   dataGrid.Columns.Add("col5","Gi\u00E1 tr\u1ECB tr\u01B0\u1EDBc");
   dataGrid.Columns.Add("col6","Gi\u00E1 tr\u1ECB sau");
   dataGroup.Controls.Add(dataGrid); content.Controls.Add(dataGroup);
   detailGroup=new GroupBox { Text="Chi tiết giá trị thay đổi", Location=new Point(26,600), Size=new Size(1080,150), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41") }; beforeText=new TextBox { Multiline=true, ReadOnly=true, Location=new Point(16,28), Size=new Size(500,104), Name="beforeText" }; afterText=new TextBox { Multiline=true, ReadOnly=true, Location=new Point(532,28), Size=new Size(530,104), Name="afterText" }; detailGroup.Controls.AddRange(new Control[]{beforeText,afterText}); content.Controls.Add(detailGroup);
   content.Controls.Add(detailGroup);
   Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

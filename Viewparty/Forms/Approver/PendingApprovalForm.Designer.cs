using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Approver
{
 partial class PendingApprovalForm
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
  private GroupBox dataGroup;
  private DataGridView dataGrid;
  private Button btnViewDetails;
  private Button btnApprove;
  private Button btnReject;
  private GroupBox decisionGroup;
  private TextBox decisionText;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
   Text="H? s? ch? duy?t"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
   header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
   lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
   lblRole=new Label { Text="Ng??i ph? duy?t", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
   lblTitle=new Label { Text="H? s? ch? duy?t", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
   lblSubtitle=new Label { Text="Xem k?t qu? ki?m tra, th?m ??nh, ? ki?n li?n ng?nh v? ?i?u ki?n c?p ph?p.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
   nav0=new Button { Text="T?ng quan", Tag="T?ng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
   nav1=new Button { Text="H? s? ch? duy?t", Tag="H? s? ch? duy?t", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
   nav2=new Button { Text="X?t ngo?i l?", Tag="X?t ngo?i l?", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
   nav3=new Button { Text="Quy?t ??nh c?p ph?p", Tag="Quy?t ??nh c?p ph?p", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
   nav4=new Button { Text="Gi?y ph?p ?? c?p", Tag="Gi?y ph?p ?? c?p", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
   dataGroup=new GroupBox { Text="Danh s?ch h? s? ch? ph? duy?t", Location=new Point(26,108), Size=new Size(1080,470), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold), Name="dataGroup" };
   dataGroup.Controls.Add(new Label { Text="D? li?u s? hi?n th? khi t?ch h?p Service v? truy v?n SQL Server.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
   dataGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,384), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="dataGrid" };
   dataGrid.Columns.Add("col0","M? h? s?");
   dataGrid.Columns.Add("col1","S? ki?n");
   dataGrid.Columns.Add("col2","Ng??i t? ch?c");
   dataGrid.Columns.Add("col3","K?t qu? th?m ??nh");
   dataGrid.Columns.Add("col4","? ki?n li?n ng?nh");
   dataGrid.Columns.Add("col5","Tr?ng th?i");
   dataGrid.Columns.Add("col6","H?nh ??ng");
   dataGroup.Controls.Add(dataGrid);
   decisionGroup=new GroupBox { Text="Th?ng tin quy?t ??nh v? ?i?u ki?n c?p ph?p", Location=new Point(26,590), Size=new Size(1080,140), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",10,FontStyle.Bold) }; decisionText=new TextBox { Multiline=true, ReadOnly=true, Text="K?t qu? ki?m tra v? th?m ??nh: ch? d? li?u SQL\r\n? ki?n li?n ng?nh: ch? d? li?u SQL\r\n?i?u ki?n c?p ph?p: ch? d? li?u SQL\r\nL? do t? ch?i: nh?p khi c? Service x? l?", Location=new Point(16,28), Size=new Size(1038,96), BorderStyle=BorderStyle.FixedSingle }; decisionGroup.Controls.Add(decisionText); content.Controls.Add(decisionGroup);
   content.Controls.Add(dataGroup);
   btnViewDetails=new Button { Text="Xem chi ti?t", Name="btnViewDetails", Location=new Point(26,750), Size=new Size(170,42), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(btnViewDetails);
   btnApprove=new Button { Text="Ph? duy?t", Name="btnApprove", Location=new Point(216,750), Size=new Size(170,42), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(btnApprove);
   btnReject=new Button { Text="T? ch?i", Name="btnReject", Location=new Point(406,750), Size=new Size(170,42), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(btnReject);
   Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

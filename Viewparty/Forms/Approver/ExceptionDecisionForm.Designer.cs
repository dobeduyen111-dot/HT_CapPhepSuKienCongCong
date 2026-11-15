using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Approver
{
 partial class ExceptionDecisionForm
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
  private Button btnExceptionApprove;
  private Button btnExceptionReject;
  private GroupBox exceptionGroup;
  private TextBox txtDecisionReason;
  private TextBox txtDecisionNote;
  private TextBox proposalText;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
   Text="X?t ngo?i l?"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
   header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
   lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
   lblRole=new Label { Text="Ng??i ph? duy?t", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
   lblTitle=new Label { Text="X?t ngo?i l?", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
   lblSubtitle=new Label { Text="Xem ?? xu?t, quy t?c kh?ng ??t, c?p th?m quy?n, minh ch?ng v? bi?n ph?p ki?m so?t.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
   nav0=new Button { Text="T?ng quan", Tag="T?ng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
   nav1=new Button { Text="H? s? ch? duy?t", Tag="H? s? ch? duy?t", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
   nav2=new Button { Text="X?t ngo?i l?", Tag="X?t ngo?i l?", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
   nav3=new Button { Text="Quy?t ??nh c?p ph?p", Tag="Quy?t ??nh c?p ph?p", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
   nav4=new Button { Text="Gi?y ph?p ?? c?p", Tag="Gi?y ph?p ?? c?p", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
   dataGroup=new GroupBox { Text="Th?ng tin ?? xu?t ngo?i l?", Location=new Point(26,108), Size=new Size(1080,470), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold), Name="dataGroup" };
   dataGroup.Controls.Add(new Label { Text="D? li?u s? hi?n th? khi t?ch h?p Service v? truy v?n SQL Server.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
   dataGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,384), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="dataGrid" };
   dataGrid.Columns.Add("col0","M? h? s?");
   dataGrid.Columns.Add("col1","Quy t?c ?? xu?t");
   dataGrid.Columns.Add("col2","C?p th?m quy?n");
   dataGrid.Columns.Add("col3","Ng??i ?? xu?t");
   dataGrid.Columns.Add("col4","Tr?ng th?i");
   dataGroup.Controls.Add(dataGrid);
   exceptionGroup=new GroupBox { Text="K?t qu? xem x?t ngo?i l?", Location=new Point(26,590), Size=new Size(1080,150), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",10,FontStyle.Bold) };
   proposalText=new TextBox { Multiline=true, ReadOnly=true, Text="Quy t?c, c?p th?m quy?n, l? do ?? xu?t, bi?n ph?p ki?m so?t v? minh ch?ng: ch? d? li?u SQL", Location=new Point(16,27), Size=new Size(1038,48), Name="proposalText" };
   exceptionGroup.Controls.Add(proposalText); exceptionGroup.Controls.Add(new Label { Text="Ý kiến và lý do quyết định", Location=new Point(16,81), AutoSize=true });
   txtDecisionReason=new TextBox { Location=new Point(16,104), Size=new Size(500,28), Name="txtDecisionReason" }; txtDecisionNote=new TextBox { Location=new Point(532,104), Size=new Size(520,28), Name="txtDecisionNote" };
   exceptionGroup.Controls.AddRange(new Control[]{txtDecisionReason,txtDecisionNote}); content.Controls.Add(exceptionGroup);
   content.Controls.Add(dataGroup);
   btnExceptionApprove=new Button { Text="Ch?p thu?n ngo?i l?", Name="btnExceptionApprove", Location=new Point(26,750), Size=new Size(220,42), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(btnExceptionApprove);
   btnExceptionReject=new Button { Text="T? ch?i ngo?i l?", Name="btnExceptionReject", Location=new Point(266,750), Size=new Size(220,42), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(btnExceptionReject);
   Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

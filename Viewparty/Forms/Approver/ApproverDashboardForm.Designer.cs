using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Approver
{
 partial class ApproverDashboardForm
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
  private GroupBox dataGroup;
  private DataGridView dataGrid;
  private Button btnPending;
  private Button btnExceptions;
  private Button btnPermits;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
   Text="T?ng quan Ng??i ph? duy?t"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
   header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
   lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
   lblRole=new Label { Text="Ng??i ph? duy?t", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
   lblTitle=new Label { Text="T?ng quan Ng??i ph? duy?t", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
   lblSubtitle=new Label { Text="Theo d?i h? s? ch? quy?t ??nh, ngo?i l? v? gi?y ph?p ?? c?p.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
   nav0=new Button { Text="T?ng quan", Tag="T?ng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
   nav1=new Button { Text="H? s? ch? duy?t", Tag="H? s? ch? duy?t", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
   nav2=new Button { Text="X?t ngo?i l?", Tag="X?t ngo?i l?", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
   nav3=new Button { Text="Quy?t ??nh c?p ph?p", Tag="Quy?t ??nh c?p ph?p", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
   nav4=new Button { Text="Gi?y ph?p ?? c?p", Tag="Gi?y ph?p ?? c?p", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
   statsPanel=new TableLayoutPanel { Location=new Point(26,108), Size=new Size(1080,112), ColumnCount=4, RowCount=1, BackColor=ColorTranslator.FromHtml("#F5F8FC"), Name="statsPanel" };
   statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard0=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; statLabel0=new Label { Text="Ch? ph? duy?t", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue0=new Label { Text="?", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote0=new Label { Text="Ch? d? li?u SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard0.Controls.AddRange(new Control[]{statLabel0,statValue0,statNote0}); statsPanel.Controls.Add(statCard0,0,0);
   statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard1=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; statLabel1=new Label { Text="?ang x?t ngo?i l?", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue1=new Label { Text="?", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote1=new Label { Text="Ch? d? li?u SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard1.Controls.AddRange(new Control[]{statLabel1,statValue1,statNote1}); statsPanel.Controls.Add(statCard1,1,0);
   statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard2=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; statLabel2=new Label { Text="?? ph? duy?t", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue2=new Label { Text="?", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote2=new Label { Text="Ch? d? li?u SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard2.Controls.AddRange(new Control[]{statLabel2,statValue2,statNote2}); statsPanel.Controls.Add(statCard2,2,0);
   statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard3=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; statLabel3=new Label { Text="?? t? ch?i", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue3=new Label { Text="?", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote3=new Label { Text="Ch? d? li?u SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard3.Controls.AddRange(new Control[]{statLabel3,statValue3,statNote3}); statsPanel.Controls.Add(statCard3,3,0);
   dataGroup=new GroupBox { Text="H? s? c?n xem x?t", Location=new Point(26,238), Size=new Size(1080,420), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold), Name="dataGroup" };
   dataGroup.Controls.Add(new Label { Text="D? li?u s? hi?n th? khi t?ch h?p Service v? truy v?n SQL Server.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
   dataGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,334), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="dataGrid" };
   dataGrid.Columns.Add("col0","M? h? s?");
   dataGrid.Columns.Add("col1","S? ki?n");
   dataGrid.Columns.Add("col2","Ng??i t? ch?c");
   dataGrid.Columns.Add("col3","K?t lu?n th?m ??nh");
   dataGrid.Columns.Add("col4","Tr?ng th?i");
   dataGrid.Columns.Add("col5","H?nh ??ng");
   dataGroup.Controls.Add(dataGrid);
   content.Controls.Add(dataGroup);
   content.Controls.Add(statsPanel);
   btnPending=new Button { Text="H? s? ch? duy?t", Name="btnPending", Location=new Point(26,678), Size=new Size(220,42), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(btnPending);
   btnExceptions=new Button { Text="X?t ngo?i l?", Name="btnExceptions", Location=new Point(266,678), Size=new Size(170,42), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(btnExceptions);
   btnPermits=new Button { Text="Gi?y ph?p ?? c?p", Name="btnPermits", Location=new Point(456,678), Size=new Size(220,42), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(btnPermits);
   Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Admin
{
 partial class AdminDashboardForm
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
  private Button shortcut0;
  private Button shortcut1;
  private Button shortcut2;
  private Button shortcut3;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
   Text="T\u1ED5ng quan Qu\u1EA3n tr\u1ECB vi\u00EAn"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
   header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
   lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
   lblRole=new Label { Text="Qu\u1EA3n tr\u1ECB vi\u00EAn", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
   lblTitle=new Label { Text="T\u1ED5ng quan Qu\u1EA3n tr\u1ECB vi\u00EAn", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
   lblSubtitle=new Label { Text="T\u1ED5ng quan c\u1EA5u h\u00ECnh h\u1EC7 th\u1ED1ng ViewParty.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
   nav0=new Button { Text="T\u1ED5ng quan", Tag="T\u1ED5ng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
   nav1=new Button { Text="T\u00E0i kho\u1EA3n / vai tr\u00F2", Tag="T\u00E0i kho\u1EA3n / vai tr\u00F2", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
   nav2=new Button { Text="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Tag="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
   nav3=new Button { Text="\u0110\u1ECBa \u0111i\u1EC3m", Tag="\u0110\u1ECBa \u0111i\u1EC3m", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
   nav4=new Button { Text="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Tag="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
   nav5=new Button { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Tag="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Location=new Point(10,306), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav5" }; nav5.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav5);
   statsPanel=new TableLayoutPanel { Location=new Point(26,108), Size=new Size(1080,112), ColumnCount=4, RowCount=1, BackColor=ColorTranslator.FromHtml("#F5F8FC"), Name="statsPanel" };
   statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard0=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; statLabel0=new Label { Text="T\u00E0i kho\u1EA3n", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue0=new Label { Text="—", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote0=new Label { Text="Chờ dữ liệu SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard0.Controls.AddRange(new Control[]{statLabel0,statValue0,statNote0}); statsPanel.Controls.Add(statCard0,0,0);
   statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard1=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; statLabel1=new Label { Text="\u0110\u1ECBa \u0111i\u1EC3m", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue1=new Label { Text="—", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote1=new Label { Text="Chờ dữ liệu SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard1.Controls.AddRange(new Control[]{statLabel1,statValue1,statNote1}); statsPanel.Controls.Add(statCard1,1,0);
   statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard2=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; statLabel2=new Label { Text="Quy t\u1EAFc hi\u1EC7u l\u1EF1c", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue2=new Label { Text="—", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote2=new Label { Text="Chờ dữ liệu SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard2.Controls.AddRange(new Control[]{statLabel2,statValue2,statNote2}); statsPanel.Controls.Add(statCard2,2,0);
   statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F)); statCard3=new Panel { Dock=DockStyle.Fill, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Margin=new Padding(0,0,12,0) }; statLabel3=new Label { Text="L\u01B0\u1EE3t thay \u0111\u1ED5i", Location=new Point(14,12), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statValue3=new Label { Text="—", Location=new Point(14,36), AutoSize=true, Font=new Font("Times New Roman",22,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; statNote3=new Label { Text="Chờ dữ liệu SQL", Location=new Point(14,78), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; statCard3.Controls.AddRange(new Control[]{statLabel3,statValue3,statNote3}); statsPanel.Controls.Add(statCard3,3,0);
   dataGroup=new GroupBox { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh g\u1EA7n \u0111\u00E2y", Location=new Point(26,238), Size=new Size(1080,390), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold), Name="dataGroup" };
   dataGroup.Controls.Add(new Label { Text="Dữ liệu sẽ hiển thị sau khi tích hợp Service/API và SQL Server.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
   dataGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,310), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="dataGrid" };
   dataGrid.Columns.Add("col0","Th\u1EDDi gian");
   dataGrid.Columns.Add("col1","Qu\u1EA3n tr\u1ECB vi\u00EAn");
   dataGrid.Columns.Add("col2","\u0110\u1ED1i t\u01B0\u1EE3ng");
   dataGrid.Columns.Add("col3","H\u00E0nh \u0111\u1ED9ng");
   dataGrid.Columns.Add("col4","Phi\u00EAn b\u1EA3n");
   dataGroup.Controls.Add(dataGrid); content.Controls.Add(dataGroup);
   content.Controls.Add(statsPanel);
   shortcut0=new Button { Text="T\u00E0i kho\u1EA3n / vai tr\u00F2", Name="shortcut0", Location=new Point(26,650), Size=new Size(210,42), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(shortcut0);
   shortcut1=new Button { Text="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Name="shortcut1", Location=new Point(256,650), Size=new Size(210,42), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(shortcut1);
   shortcut2=new Button { Text="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Name="shortcut2", Location=new Point(486,650), Size=new Size(210,42), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(shortcut2);
   shortcut3=new Button { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Name="shortcut3", Location=new Point(716,650), Size=new Size(210,42), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; content.Controls.Add(shortcut3);
   Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

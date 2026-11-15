using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Admin
{
 partial class AccountManagementForm
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
  private GroupBox editGroup;
  private TextBox input0;
  private TextBox input1;
  private TextBox input2;
  private TextBox input3;
  private ComboBox combo0;
  private ComboBox combo1;
  private ComboBox combo2;

  private NumericUpDown number0;





  private Button btnNew;
  private Button btnEdit;
  private TextBox searchBox;
  private ComboBox roleFilter;

  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
   Text="T\u00E0i kho\u1EA3n v\u00E0 ph\u00E2n quy\u1EC1n"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
   header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
   lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
   lblRole=new Label { Text="Qu\u1EA3n tr\u1ECB vi\u00EAn", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
   lblTitle=new Label { Text="T\u00E0i kho\u1EA3n v\u00E0 ph\u00E2n quy\u1EC1n", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
   lblSubtitle=new Label { Text="L\u1ECDc theo vai tr\u00F2, email v\u00E0 tr\u1EA1ng th\u00E1i t\u00E0i kho\u1EA3n. D\u1eef li\u1ec7u ch\u1edd Service/SQL.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
   nav0=new Button { Text="T\u1ED5ng quan", Tag="T\u1ED5ng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
   nav1=new Button { Text="T\u00E0i kho\u1EA3n / vai tr\u00F2", Tag="T\u00E0i kho\u1EA3n / vai tr\u00F2", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
   nav2=new Button { Text="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Tag="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
   nav3=new Button { Text="\u0110\u1ECBa \u0111i\u1EC3m", Tag="\u0110\u1ECBa \u0111i\u1EC3m", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
   nav4=new Button { Text="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Tag="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
   nav5=new Button { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Tag="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Location=new Point(10,306), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav5" }; nav5.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav5);
   dataGroup=new GroupBox { Text="Qu\u1EA3n l\u00FD t\u00E0i kho\u1EA3n", Location=new Point(26,108), Size=new Size(1080,360), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold), Name="dataGroup" };
   dataGrid=new DataGridView { Location=new Point(14,100), Size=new Size(1044,238), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="dataGrid" };
   dataGrid.Columns.Add("col0","T\u00EAn \u0111\u0103ng nh\u1EADp");
   dataGrid.Columns.Add("col1","H\u1ECD t\u00EAn");
   dataGrid.Columns.Add("col2","Vai tr\u00F2");
   dataGrid.Columns.Add("col3","\u0110\u01A1n v\u1ECB");
   dataGrid.Columns.Add("col4","Tr\u1EA1ng th\u00E1i");
   dataGrid.Columns.Add("col5","H\u00E0nh \u0111\u1ED9ng");
   dataGroup.Controls.Add(dataGrid); searchBox=new TextBox { Location=new Point(14,60), Size=new Size(430,28), Name="searchBox" }; roleFilter=new ComboBox { Location=new Point(480,60), Size=new Size(260,28), DropDownStyle=ComboBoxStyle.DropDownList, Name="roleFilter" }; roleFilter.Items.Add("Tất cả vai trò"); roleFilter.Items.AddRange(new object[]{"NGUOI_TO_CHUC","NGUOI_XU_LY","LIEN_NGANH","NGUOI_PHE_DUYET","QUAN_TRI_VIEN"}); dataGroup.Controls.Add(new Label { Text="Tìm kiếm tên hoặc email", Location=new Point(14,38), AutoSize=true }); dataGroup.Controls.Add(searchBox); dataGroup.Controls.Add(new Label { Text="Vai trò", Location=new Point(480,38), AutoSize=true }); dataGroup.Controls.Add(roleFilter); content.Controls.Add(dataGroup);
   editGroup=new GroupBox { Text="Thông tin tài khoản", Location=new Point(26,490), Size=new Size(1080,230), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41") };
   editGroup.Controls.Add(new Label { Text="T\u00EAn \u0111\u0103ng nh\u1EADp", Location=new Point(16,28), AutoSize=true }); input0=new TextBox { Location=new Point(16,50), Size=new Size(480,28), Name="input0" }; editGroup.Controls.Add(input0);
   editGroup.Controls.Add(new Label { Text="H\u1ECD t\u00EAn", Location=new Point(536,28), AutoSize=true }); input1=new TextBox { Location=new Point(536,50), Size=new Size(480,28), Name="input1" }; editGroup.Controls.Add(input1);
   editGroup.Controls.Add(new Label { Text="Email", Location=new Point(16,94), AutoSize=true }); input2=new TextBox { Location=new Point(16,116), Size=new Size(480,28), Name="input2" }; editGroup.Controls.Add(input2);
   editGroup.Controls.Add(new Label { Text="S\u1ED1 \u0111i\u1EC7n tho\u1EA1i", Location=new Point(536,94), AutoSize=true }); input3=new TextBox { Location=new Point(536,116), Size=new Size(480,28), Name="input3" }; editGroup.Controls.Add(input3);
   combo0=new ComboBox { Location=new Point(16,160), Size=new Size(320,28), DropDownStyle=ComboBoxStyle.DropDownList, Name="combo0" }; combo0.Items.AddRange(new object[]{"NGUOI_TO_CHUC","NGUOI_XU_LY","LIEN_NGANH","NGUOI_PHE_DUYET","QUAN_TRI_VIEN"}); combo1=new ComboBox { Location=new Point(360,160), Size=new Size(320,28), DropDownStyle=ComboBoxStyle.DropDownList, Name="combo1" }; combo2=new ComboBox { Location=new Point(16,194), Size=new Size(320,26), DropDownStyle=ComboBoxStyle.DropDownList, Name="combo2" }; combo2.Items.AddRange(new object[]{"HOAT_DONG","TAM_KHOA","NGUNG_HOAT_DONG","CHO_XAC_MINH"}); number0=new NumericUpDown { Location=new Point(360,194), Maximum=100, Name="number0" }; editGroup.Controls.Add(new Label { Text="Vai trò", Location=new Point(16,140), AutoSize=true }); editGroup.Controls.Add(new Label { Text="Đơn vị", Location=new Point(360,140), AutoSize=true }); editGroup.Controls.Add(new Label { Text="Trạng thái", Location=new Point(16,174), AutoSize=true }); editGroup.Controls.Add(new Label { Text="Cấp thẩm quyền", Location=new Point(360,174), AutoSize=true }); editGroup.Controls.AddRange(new Control[]{combo0,combo1,combo2,number0}); content.Controls.Add(editGroup);
   btnNew=new Button { Text="+ Thêm tài khoản", Name="btnNew", Location=new Point(820,56), Size=new Size(190,34), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; dataGroup.Controls.Add(btnNew); btnEdit=new Button { Text="Sửa / Khóa", Name="btnEdit", Location=new Point(820,174), Size=new Size(190,34), BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; editGroup.Controls.Add(btnEdit);
   Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

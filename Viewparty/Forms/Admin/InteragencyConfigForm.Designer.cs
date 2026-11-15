using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Admin
{
 partial class InteragencyConfigForm
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




  private ComboBox combo0;
  private ComboBox combo1;
  private ComboBox combo2;
  private CheckBox check0;
  private NumericUpDown number0;
  private DateTimePicker date0;
  private DateTimePicker date1;


  private Button btnSave;



  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
   Text="C\u1EA5u h\u00ECnh l\u1EA5y \u00FD ki\u1EBFn li\u00EAn ng\u00E0nh"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
   header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
   lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
   lblRole=new Label { Text="Qu\u1EA3n tr\u1ECB vi\u00EAn", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
   lblTitle=new Label { Text="C\u1EA5u h\u00ECnh l\u1EA5y \u00FD ki\u1EBFn li\u00EAn ng\u00E0nh", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
   lblSubtitle=new Label { Text="M\u1ED7i c\u1EA5u h\u00ECnh g\u1ED3m lo\u1EA1i s\u1EF1 ki\u1EC7n, \u0111\u01A1n v\u1ECB, t\u00EDnh b\u1EAFt bu\u1ED9c v\u00E0 th\u1EDDi h\u1EA1n.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
   nav0=new Button { Text="T\u1ED5ng quan", Tag="T\u1ED5ng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
   nav1=new Button { Text="T\u00E0i kho\u1EA3n / vai tr\u00F2", Tag="T\u00E0i kho\u1EA3n / vai tr\u00F2", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
   nav2=new Button { Text="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Tag="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
   nav3=new Button { Text="\u0110\u1ECBa \u0111i\u1EC3m", Tag="\u0110\u1ECBa \u0111i\u1EC3m", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
   nav4=new Button { Text="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Tag="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
   nav5=new Button { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Tag="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Location=new Point(10,306), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav5" }; nav5.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav5);
   dataGroup=new GroupBox { Text="C\u1EA5u h\u00ECnh li\u00EAn ng\u00E0nh", Location=new Point(26,108), Size=new Size(1080,300), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold), Name="dataGroup" };
   dataGroup.Controls.Add(new Label { Text="Dữ liệu sẽ hiển thị sau khi tích hợp Service/API và SQL Server.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
   dataGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,220), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="dataGrid" };
   dataGrid.Columns.Add("col0","Lo\u1EA1i s\u1EF1 ki\u1EC7n");
   dataGrid.Columns.Add("col1","\u0110\u01A1n v\u1ECB");
   dataGrid.Columns.Add("col2","Quy t\u1EAFc");
   dataGrid.Columns.Add("col3","B\u1EAFt bu\u1ED9c");
   dataGrid.Columns.Add("col4","H\u1EA1n ph\u1EA3n h\u1ED3i");
   dataGrid.Columns.Add("col5","Hi\u1EC7u l\u1EF1c");
   dataGrid.Columns.Add("col6","Tr\u1EA1ng th\u00E1i");
   dataGroup.Controls.Add(dataGrid); content.Controls.Add(dataGroup);
   editGroup=new GroupBox { Text="Cấu hình liên ngành", Location=new Point(26,430), Size=new Size(1080,310), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41") };
   combo0=new ComboBox { Location=new Point(16,54), Size=new Size(300,30), DropDownStyle=ComboBoxStyle.DropDownList, Name="combo0" }; combo1=new ComboBox { Location=new Point(340,54), Size=new Size(300,30), DropDownStyle=ComboBoxStyle.DropDownList, Name="combo1" }; editGroup.Controls.AddRange(new Control[]{combo0,combo1}); editGroup.Controls.Add(new Label { Text="Loại sự kiện", Location=new Point(16,28), AutoSize=true }); editGroup.Controls.Add(new Label { Text="Đơn vị liên ngành", Location=new Point(340,28), AutoSize=true });
   combo2=new ComboBox { Location=new Point(16,126), Size=new Size(300,30), DropDownStyle=ComboBoxStyle.DropDownList, Name="combo2" }; editGroup.Controls.Add(new Label { Text="Quy tắc liên quan (tùy chọn)", Location=new Point(16,100), AutoSize=true }); editGroup.Controls.Add(combo2); check0=new CheckBox { Text="Bắt buộc lấy ý kiến", Location=new Point(680,54), AutoSize=true, Name="check0" }; number0=new NumericUpDown { Location=new Point(340,126), Maximum=365, Minimum=1, Value=7, Name="number0" }; date0=new DateTimePicker { Location=new Point(680,126), Width=300, Name="date0" }; date1=new DateTimePicker { Location=new Point(16,206), Width=300, Name="date1", ShowCheckBox=true }; editGroup.Controls.AddRange(new Control[]{check0,number0,date0,date1}); editGroup.Controls.Add(new Label { Text="Số ngày phản hồi", Location=new Point(340,100), AutoSize=true }); editGroup.Controls.Add(new Label { Text="Ngày hiệu lực từ", Location=new Point(680,100), AutoSize=true }); editGroup.Controls.Add(new Label { Text="Ngày hiệu lực đến", Location=new Point(16,180), AutoSize=true }); btnSave=new Button { Text="Lưu cấu hình", Name="btnSave", Location=new Point(820,250), Size=new Size(180,38), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; editGroup.Controls.Add(btnSave); content.Controls.Add(editGroup);
   Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

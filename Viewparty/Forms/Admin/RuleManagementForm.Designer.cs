using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Admin
{
 partial class RuleManagementForm
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


  private ComboBox combo0;
  private ComboBox combo1;
  private CheckBox check0;
  private NumericUpDown number0;
  private DateTimePicker date0;

  private TextBox json0;
  private TextBox json1;
  private Button btnSave;



  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
   Text="Quy t\u1EAFc v\u00E0 phi\u00EAn b\u1EA3n"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
   header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
   lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
   lblRole=new Label { Text="Qu\u1EA3n tr\u1ECB vi\u00EAn", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
   lblTitle=new Label { Text="Quy t\u1EAFc v\u00E0 phi\u00EAn b\u1EA3n", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
   lblSubtitle=new Label { Text="Danh s\u00E1ch quy t\u1EAFc \u0111ang c\u00F3 hi\u1EC7u l\u1EF1c; m\u1ED7i thay \u0111\u1ED5i t\u1EA1o phi\u00EAn b\u1EA3n m\u1EDBi.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
   nav0=new Button { Text="T\u1ED5ng quan", Tag="T\u1ED5ng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
   nav1=new Button { Text="T\u00E0i kho\u1EA3n / vai tr\u00F2", Tag="T\u00E0i kho\u1EA3n / vai tr\u00F2", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
   nav2=new Button { Text="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Tag="Lo\u1EA1i s\u1EF1 ki\u1EC7n", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
   nav3=new Button { Text="\u0110\u1ECBa \u0111i\u1EC3m", Tag="\u0110\u1ECBa \u0111i\u1EC3m", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
   nav4=new Button { Text="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Tag="Quy t\u1EAFc / li\u00EAn ng\u00E0nh", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
   nav5=new Button { Text="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Tag="Nh\u1EADt k\u00FD c\u1EA5u h\u00ECnh", Location=new Point(10,306), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav5" }; nav5.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav5);
   dataGroup=new GroupBox { Text="Danh s\u00E1ch quy t\u1EAFc", Location=new Point(26,108), Size=new Size(1080,300), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",11,FontStyle.Bold), Name="dataGroup" };
   dataGroup.Controls.Add(new Label { Text="Dữ liệu sẽ hiển thị sau khi tích hợp Service/API và SQL Server.", Location=new Point(16,27), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397"), Font=new Font("Times New Roman",9.5F) });
   dataGrid=new DataGridView { Location=new Point(14,58), Size=new Size(1044,220), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, ColumnHeadersHeight=36, Name="dataGrid" };
   dataGrid.Columns.Add("col0","M\u00E3 quy t\u1EAFc");
   dataGrid.Columns.Add("col1","Lo\u1EA1i s\u1EF1 ki\u1EC7n");
   dataGrid.Columns.Add("col2","Phi\u00EAn b\u1EA3n");
   dataGrid.Columns.Add("col3","M\u1EE9c \u0111\u1ED9");
   dataGrid.Columns.Add("col4","Hi\u1EC7u l\u1EF1c t\u1EEB");
   dataGrid.Columns.Add("col5","Tr\u1EA1ng th\u00E1i");
   dataGroup.Controls.Add(dataGrid); content.Controls.Add(dataGroup);
   editGroup=new GroupBox { Text="Tạo phiên bản quy tắc", Location=new Point(26,430), Size=new Size(1080,310), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41") };
   input0=new TextBox { Location=new Point(16,42), Size=new Size(240,28), Name="input0" }; combo0=new ComboBox { Location=new Point(276,42), Size=new Size(280,28), DropDownStyle=ComboBoxStyle.DropDownList, Name="combo0" }; input1=new TextBox { Location=new Point(576,42), Size=new Size(180,28), Name="input1" }; combo1=new ComboBox { Location=new Point(776,42), Size=new Size(270,28), DropDownStyle=ComboBoxStyle.DropDownList, Name="combo1" };
   editGroup.Controls.AddRange(new Control[]{input0,combo0,input1,combo1}); editGroup.Controls.Add(new Label { Text="Mã quy tắc      Loại sự kiện          Nhóm      Mức độ", Location=new Point(16,18), AutoSize=true });
   json0=new TextBox { Multiline=true, Location=new Point(16,100), Size=new Size(500,90), Name="json0" }; json1=new TextBox { Multiline=true, Location=new Point(536,100), Size=new Size(510,90), Name="json1" }; editGroup.Controls.AddRange(new Control[]{json0,json1}); editGroup.Controls.Add(new Label { Text="Điều kiện áp dụng (JSON)", Location=new Point(16,76), AutoSize=true }); editGroup.Controls.Add(new Label { Text="Điều kiện đạt (JSON)", Location=new Point(536,76), AutoSize=true });
   check0=new CheckBox { Text="Cho phép ngoại lệ", Location=new Point(16,212), AutoSize=true, Name="check0" }; number0=new NumericUpDown { Location=new Point(230,210), Maximum=100, Name="number0" }; date0=new DateTimePicker { Location=new Point(440,210), Width=220, Name="date0" }; editGroup.Controls.AddRange(new Control[]{check0,number0,date0}); editGroup.Controls.Add(new Label { Text="Cấp thẩm quyền", Location=new Point(230,186), AutoSize=true }); editGroup.Controls.Add(new Label { Text="Hiệu lực từ", Location=new Point(440,186), AutoSize=true }); btnSave=new Button { Text="Tạo phiên bản mới", Name="btnSave", Location=new Point(820,245), Size=new Size(220,38), BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White, FlatStyle=FlatStyle.Flat }; editGroup.Controls.Add(btnSave); content.Controls.Add(editGroup);
   Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
  }
 }
}

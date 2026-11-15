using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Interagency
{
 partial class OpinionResponseForm
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
  private GroupBox caseGroup;
  private Label lblRequestContent;
  private TextBox txtRequestContent;
  private GroupBox documentsGroup;
  private DataGridView documentsGrid;
  private GroupBox responseGroup;
  private Label lblConclusion;
  private ComboBox cmbConclusion;
  private Label lblComment;
  private TextBox txtComment;
  private Label lblRecommendation;
  private TextBox txtRecommendation;
  private Button btnSaveDraft;
  private Button btnSendResponse;
  private Button btnExportResponse;
  private Label caseLabel0;
  private TextBox caseValue0;
  private Label caseLabel1;
  private TextBox caseValue1;
  private Label caseLabel2;
  private TextBox caseValue2;
  private Label caseLabel3;
  private TextBox caseValue3;
  private Label caseLabel4;
  private TextBox caseValue4;
  private Label caseLabel5;
  private TextBox caseValue5;
  protected override void Dispose(bool disposing) { if(disposing && components!=null) components.Dispose(); base.Dispose(disposing); }
  private void InitializeComponent()
  {
   SuspendLayout();
            Text="Gửi ý kiến liên ngành"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1440,900); MinimumSize=new Size(1180,760); BackColor=ColorTranslator.FromHtml("#F5F8FC"); Font=new Font("Times New Roman",9.5F);
            header=new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White, Name="header" }; sidebar=new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true, Name="sidebar" }; content=new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), AutoScroll=true, Name="content" };
            lblBrand=new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,18), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; header.Controls.Add(lblBrand);
            lblRole=new Label { Text="Đơn vị liên ngành", Location=new Point(18,20), Size=new Size(208,30), Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White }; sidebar.Controls.Add(lblRole);
            lblTitle=new Label { Text="Gửi ý kiến liên ngành", Location=new Point(26,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") }; content.Controls.Add(lblTitle);
            lblSubtitle=new Label { Text="Xem hồ sơ, tài liệu liên quan và gửi kết luận chuyên môn.", Location=new Point(28,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") }; content.Controls.Add(lblSubtitle);
            nav0=new Button { Text="Tổng quan", Tag="Tổng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav0" }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
            nav1=new Button { Text="Yêu cầu cần phản hồi", Tag="Yêu cầu cần phản hồi", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav1" }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
            nav2=new Button { Text="Chi tiết hồ sơ", Tag="Chi tiết hồ sơ", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav2" }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
            nav3=new Button { Text="Gửi ý kiến", Tag="Gửi ý kiến", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav3" }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
            nav4=new Button { Text="Văn bản phản hồi", Tag="Văn bản phản hồi", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White, Name="nav4" }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
            caseGroup=new GroupBox { Text="Hồ sơ cần góp ý", Location=new Point(26,112), Size=new Size(1080,210), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",10,FontStyle.Bold) };
            caseLabel0=new Label { Text="Mã yêu cầu / Lĩnh vực", Location=new Point(18,40), AutoSize=true }; caseValue0=new TextBox { Location=new Point(168,36), Size=new Size(340,28), ReadOnly=true, Name="caseValue0" }; caseGroup.Controls.AddRange(new Control[] { caseLabel0,caseValue0 });
            caseLabel1=new Label { Text="Tên sự kiện", Location=new Point(538,40), AutoSize=true }; caseValue1=new TextBox { Location=new Point(688,36), Size=new Size(340,28), ReadOnly=true, Name="caseValue1" }; caseGroup.Controls.AddRange(new Control[] { caseLabel1,caseValue1 });
            caseLabel2=new Label { Text="Đơn vị tổ chức", Location=new Point(18,92), AutoSize=true }; caseValue2=new TextBox { Location=new Point(168,88), Size=new Size(340,28), ReadOnly=true, Name="caseValue2" }; caseGroup.Controls.AddRange(new Control[] { caseLabel2,caseValue2 });
            caseLabel3=new Label { Text="Thời gian", Location=new Point(538,92), AutoSize=true }; caseValue3=new TextBox { Location=new Point(688,88), Size=new Size(340,28), ReadOnly=true, Name="caseValue3" }; caseGroup.Controls.AddRange(new Control[] { caseLabel3,caseValue3 });
            caseLabel4=new Label { Text="Địa điểm", Location=new Point(18,144), AutoSize=true }; caseValue4=new TextBox { Location=new Point(168,140), Size=new Size(340,28), ReadOnly=true, Name="caseValue4" }; caseGroup.Controls.AddRange(new Control[] { caseLabel4,caseValue4 });
            caseLabel5=new Label { Text="Quy mô", Location=new Point(538,144), AutoSize=true }; caseValue5=new TextBox { Location=new Point(688,140), Size=new Size(340,28), ReadOnly=true, Name="caseValue5" }; caseGroup.Controls.AddRange(new Control[] { caseLabel5,caseValue5 });
            lblRequestContent=new Label { Text="N\u1ed9i dung y\u00eau c\u1ea7u", Location=new Point(18,178), AutoSize=true };
            txtRequestContent=new TextBox { Location=new Point(168,174), Size=new Size(865,28), ReadOnly=true, Name="txtRequestContent" };
            caseGroup.Controls.AddRange(new Control[] { lblRequestContent,txtRequestContent });
            documentsGroup=new GroupBox { Text="Tài liệu liên quan", Location=new Point(26,330), Size=new Size(1080,125), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",10,FontStyle.Bold) };
            documentsGrid=new DataGridView { Location=new Point(14,28), Size=new Size(1044,83), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false, Name="documentsGrid" };
            documentsGrid.Columns.Add("doc0","Tài liệu");
            documentsGrid.Columns.Add("doc1","Loại");
            documentsGrid.Columns.Add("doc2","Thao tác");
            documentsGroup.Controls.Add(documentsGrid);
            responseGroup=new GroupBox { Text="Kết luận chuyên môn", Location=new Point(26,463), Size=new Size(1080,250), BackColor=Color.White, ForeColor=ColorTranslator.FromHtml("#142C41"), Font=new Font("Times New Roman",10,FontStyle.Bold) };
            lblConclusion=new Label { Text="Kết luận", Location=new Point(18,38), AutoSize=true }; cmbConclusion=new ComboBox { Location=new Point(285,34), Size=new Size(700,30), DropDownStyle=ComboBoxStyle.DropDownList, Name="cmbConclusion" }; cmbConclusion.Items.AddRange(new object[] { "Chọn kết luận","Phù hợp","Phù hợp có điều kiện","Không phù hợp" }); cmbConclusion.SelectedIndex=0;
            lblComment=new Label { Text="Nội dung nhận xét", Location=new Point(18,88), AutoSize=true }; txtComment=new TextBox { Location=new Point(285,84), Size=new Size(700,64), Multiline=true, ScrollBars=ScrollBars.Vertical, Name="txtComment" };
            lblRecommendation=new Label { Text="Kiến nghị / điều kiện kèm theo", Location=new Point(18,166), AutoSize=true }; txtRecommendation=new TextBox { Location=new Point(285,162), Size=new Size(700,64), Multiline=true, ScrollBars=ScrollBars.Vertical, Name="txtRecommendation" }; responseGroup.Controls.AddRange(new Control[] { lblConclusion,cmbConclusion,lblComment,txtComment,lblRecommendation,txtRecommendation });
            btnSaveDraft=new Button { Text="Lưu nháp", Name="btnSaveDraft", Location=new Point(26,730), Size=new Size(140,40), FlatStyle=FlatStyle.Flat, BackColor=Color.White }; btnSendResponse=new Button { Text="Gửi phản hồi", Name="btnSendResponse", Location=new Point(180,730), Size=new Size(160,40), FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White }; btnExportResponse=new Button { Text="Xuất VB04", Name="btnExportResponse", Location=new Point(354,730), Size=new Size(140,40), FlatStyle=FlatStyle.Flat, BackColor=Color.White }; content.Controls.AddRange(new Control[] { caseGroup,documentsGroup,responseGroup,btnSaveDraft,btnSendResponse,btnExportResponse });
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header); ResumeLayout(false);
   ResumeLayout(false);
  }
 }
}

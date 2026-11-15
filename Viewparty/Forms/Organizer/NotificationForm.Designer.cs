using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Organizer
{
    partial class NotificationForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header, sidebar, content, section;
        private Label brand, role, titleLabel, subtitle;
        private Button nav0,nav1,nav2,nav3,nav4,nav5;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Thông báo g?n ??y";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1440, 900);
            MinimumSize = new Size(1100, 720);
            BackColor = ColorTranslator.FromHtml("#F5F8FC");
            Font = new Font("Times New Roman", 9.5F);
            header = new Panel { Dock=DockStyle.Top, Height=64, BackColor=Color.White };
            sidebar = new Panel { Dock=DockStyle.Left, Width=244, BackColor=ColorTranslator.FromHtml("#142C41"), AutoScroll=true };
            content = new Panel { Dock=DockStyle.Fill, BackColor=ColorTranslator.FromHtml("#F5F8FC"), Padding=new Padding(24,20,24,20), AutoScroll=true };
            brand = new Label { Text="VIEWPARTY  |  EVENT PERMIT PORTAL", Location=new Point(22,19), AutoSize=true, Font=new Font("Times New Roman",15,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") };
            role = new Label { Text="Người tổ chức", Location=new Point(18,20), AutoSize=true, Font=new Font("Times New Roman",11,FontStyle.Bold), ForeColor=Color.White };
            header.Controls.Add(brand); sidebar.Controls.Add(role);
            titleLabel = new Label { Text="Thông báo g?n ??y", Location=new Point(24,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") };
            subtitle = new Label { Text="Theo d?i v? x? l? h? s? s? ki?n theo quy tr?nh ViewParty", Location=new Point(26,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") };
            content.Controls.Add(titleLabel); content.Controls.Add(subtitle);
            nav0 = new Button { Text="Tổng quan", Tag="Tổng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
            nav1 = new Button { Text="Hồ sơ của tôi", Tag="Hồ sơ của tôi", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
            nav2 = new Button { Text="Tạo hồ sơ", Tag="Tạo hồ sơ", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
            nav3 = new Button { Text="Tra cứu địa điểm", Tag="Tra cứu địa điểm", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
            nav4 = new Button { Text="Thông báo", Tag="Thông báo", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
            nav5 = new Button { Text="Biểu mẫu Word", Tag="Biểu mẫu Word", Location=new Point(10,306), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav5.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav5);
            section = new Panel { Location=new Point(24,118), Size=new Size(1060,360), BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle };
            var grid = new DataGridView { Location=new Point(14,48), Size=new Size(1028,290), ReadOnly=true, AllowUserToAddRows=false, RowHeadersVisible=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, EnableHeadersVisualStyles=false };
            grid.Columns.Add("Th?i?i?m","Th?i ?i?m");
            grid.Columns.Add("N?idung","N?i dung");
            grid.Columns.Add("M?h?s?","Mã hồ sơ");
            grid.Columns.Add("Tr?ngth?i","Trạng thái");
            section.Controls.Add(grid);
            content.Controls.Add(section);
            var actions = new FlowLayoutPanel { Location=new Point(24,700), Size=new Size(1040,54), AutoSize=false, BackColor=Color.Transparent };
            content.Controls.Add(actions);
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header);
            ResumeLayout(false);
            
        }
    }
}

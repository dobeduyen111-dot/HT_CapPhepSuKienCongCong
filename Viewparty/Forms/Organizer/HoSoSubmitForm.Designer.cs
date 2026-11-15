using System;
using System.Drawing;
using System.Windows.Forms;
namespace Viewparty.Forms.Organizer
{
    partial class HoSoSubmitForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header, sidebar, content, section;
        private Label brand, role, titleLabel, subtitle;
        private Button nav0,nav1,nav2,nav3,nav4,nav5;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Xem lại và nộp hồ sơ";
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
            titleLabel = new Label { Text="Xem lại và nộp hồ sơ", Location=new Point(24,18), AutoSize=true, Font=new Font("Times New Roman",20,FontStyle.Bold), ForeColor=ColorTranslator.FromHtml("#142C41") };
            subtitle = new Label { Text="Theo d?i v? x? l? h? s? s? ki?n theo quy tr?nh ViewParty", Location=new Point(26,58), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#718397") };
            content.Controls.Add(titleLabel); content.Controls.Add(subtitle);
            nav0 = new Button { Text="Tổng quan", Tag="Tổng quan", Location=new Point(10,76), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav0.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav0);
            nav1 = new Button { Text="Hồ sơ của tôi", Tag="Hồ sơ của tôi", Location=new Point(10,122), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav1.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav1);
            nav2 = new Button { Text="Tạo hồ sơ", Tag="Tạo hồ sơ", Location=new Point(10,168), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav2.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav2);
            nav3 = new Button { Text="Tra cứu địa điểm", Tag="Tra cứu địa điểm", Location=new Point(10,214), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav3.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav3);
            nav4 = new Button { Text="Thông báo", Tag="Thông báo", Location=new Point(10,260), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav4.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav4);
            nav5 = new Button { Text="Biểu mẫu Word", Tag="Biểu mẫu Word", Location=new Point(10,306), Size=new Size(224,42), TextAlign=ContentAlignment.MiddleLeft, FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#142C41"), ForeColor=Color.White }; nav5.FlatAppearance.BorderSize=0; sidebar.Controls.Add(nav5);
            section = new Panel { Location=new Point(24,118), Size=new Size(1060,620), BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle };
            section.Controls.Add(new Label { Text="Tên sự kiện", Location=new Point(18,52), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#203548") });
            section.Controls.Add(new TextBox { Location=new Point(285,48), Size=new Size(700,32), Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right, BorderStyle=BorderStyle.FixedSingle });
            section.Controls.Add(new Label { Text="Quy m?", Location=new Point(18,110), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#203548") });
            section.Controls.Add(new TextBox { Location=new Point(285,106), Size=new Size(700,32), Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right, BorderStyle=BorderStyle.FixedSingle });
            section.Controls.Add(new Label { Text="Th?i gian", Location=new Point(18,168), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#203548") });
            section.Controls.Add(new TextBox { Location=new Point(285,164), Size=new Size(700,32), Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right, BorderStyle=BorderStyle.FixedSingle });
            section.Controls.Add(new Label { Text="Địa điểm", Location=new Point(18,226), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#203548") });
            section.Controls.Add(new TextBox { Location=new Point(285,222), Size=new Size(700,32), Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right, BorderStyle=BorderStyle.FixedSingle });
            section.Controls.Add(new Label { Text="Lo?i v?", Location=new Point(18,284), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#203548") });
            section.Controls.Add(new TextBox { Location=new Point(285,280), Size=new Size(700,32), Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right, BorderStyle=BorderStyle.FixedSingle });
            section.Controls.Add(new Label { Text="Quy?n tr?nh chi?u", Location=new Point(18,342), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#203548") });
            section.Controls.Add(new TextBox { Location=new Point(285,338), Size=new Size(700,32), Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right, BorderStyle=BorderStyle.FixedSingle });
            section.Controls.Add(new Label { Text="An to?n / t?i li?u", Location=new Point(18,400), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#203548") });
            section.Controls.Add(new TextBox { Location=new Point(285,396), Size=new Size(700,32), Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right, BorderStyle=BorderStyle.FixedSingle });
            section.Controls.Add(new Label { Text="Ki?m tra ?i?u ki?n n?p", Location=new Point(18,458), AutoSize=true, ForeColor=ColorTranslator.FromHtml("#203548") });
            section.Controls.Add(new TextBox { Location=new Point(285,454), Size=new Size(700,32), Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right, BorderStyle=BorderStyle.FixedSingle });
            section.Controls.Add(new CheckBox { Text="Tên sự kiện", Location=new Point(18,58), AutoSize=true });
            section.Controls.Add(new CheckBox { Text="Quy m?", Location=new Point(18,92), AutoSize=true });
            section.Controls.Add(new CheckBox { Text="Th?i gian", Location=new Point(18,126), AutoSize=true });
            section.Controls.Add(new CheckBox { Text="Địa điểm", Location=new Point(18,160), AutoSize=true });
            section.Controls.Add(new CheckBox { Text="Lo?i v?", Location=new Point(18,194), AutoSize=true });
            section.Controls.Add(new CheckBox { Text="Quy?n tr?nh chi?u", Location=new Point(18,228), AutoSize=true });
            section.Controls.Add(new CheckBox { Text="An to?n / t?i li?u", Location=new Point(18,262), AutoSize=true });
            section.Controls.Add(new CheckBox { Text="Ki?m tra ?i?u ki?n n?p", Location=new Point(18,296), AutoSize=true });
            content.Controls.Add(section);
            var actions = new FlowLayoutPanel { Location=new Point(24,700), Size=new Size(1040,54), AutoSize=false, BackColor=Color.Transparent };
            var action0=new Button { Text="L?u Nh?p", AutoSize=true, Height=38, Margin=new Padding(0,0,10,0), FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White }; action0.Click += PendingAction; actions.Controls.Add(action0);
            var action1=new Button { Text="N?p h? s?", AutoSize=true, Height=38, Margin=new Padding(0,0,10,0), FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White }; action1.Click += PendingAction; actions.Controls.Add(action1);
            var action2=new Button { Text="Quay l?i", AutoSize=true, Height=38, Margin=new Padding(0,0,10,0), FlatStyle=FlatStyle.Flat, BackColor=ColorTranslator.FromHtml("#137A78"), ForeColor=Color.White }; action2.Click += PendingAction; actions.Controls.Add(action2);
            content.Controls.Add(actions);
            Controls.Add(content); Controls.Add(sidebar); Controls.Add(header);
            ResumeLayout(false);
            
        }
    }
}

using System.Drawing;
using System.Windows.Forms;

namespace Viewparty.Forms.Organizer
{
    partial class OrganizerDashboardForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header, sidebar, content, section;
        private Label brand, role, titleLabel, subtitle;
        private Button nav0, nav1, nav2, nav3, nav4, nav5;
        private TableLayoutPanel metricGrid;
        private DataGridView recentGrid;
        private FlowLayoutPanel actions;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Tổng quan Người tổ chức";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1440, 900);
            MinimumSize = new Size(900, 600);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScaleDimensions = new SizeF(9F, 9F);
            BackColor = ColorTranslator.FromHtml("#F5F8FC");
            Font = new Font("Times New Roman", 9.5F);

            header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White, Name = "header" };
            brand = new Label { Text = "VIEWPARTY  |  EVENT PERMIT PORTAL", Dock = DockStyle.Left, Width = 410, Padding = new Padding(22, 18, 0, 0), Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41"), Name = "brand" };
            header.Controls.Add(brand);

            sidebar = new Panel { Dock = DockStyle.Left, Width = 244, BackColor = ColorTranslator.FromHtml("#142C41"), AutoScroll = true, Name = "sidebar" };
            role = new Label { Text = "Người tổ chức", Location = new Point(18, 20), AutoSize = true, Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.White, Name = "role" };
            sidebar.Controls.Add(role);
            nav0 = NavButton("Tổng quan", "Tổng quan", 76);
            nav1 = NavButton("Hồ sơ của tôi", "Hồ sơ của tôi", 122);
            nav2 = NavButton("Tạo hồ sơ", "Tạo hồ sơ", 168);
            nav3 = NavButton("Tra cứu địa điểm", "Tra cứu địa điểm", 214);
            nav4 = NavButton("Thông báo", "Thông báo", 260);
            nav5 = NavButton("Biểu mẫu Word", "Biểu mẫu Word", 306);
            sidebar.Controls.AddRange(new Control[] { nav0, nav1, nav2, nav3, nav4, nav5 });

            content = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F5F8FC"), Padding = new Padding(24, 20, 24, 24), AutoScroll = true, Name = "content" };
            titleLabel = new Label { Text = "Tổng quan Người tổ chức", Dock = DockStyle.Top, Height = 40, Font = new Font("Times New Roman", 20, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41"), Name = "titleLabel" };
            subtitle = new Label { Text = "Theo dõi và xử lý hồ sơ sự kiện theo quy trình ViewParty", Dock = DockStyle.Top, Height = 32, ForeColor = ColorTranslator.FromHtml("#718397"), Name = "subtitle" };
            content.Controls.Add(subtitle);
            content.Controls.Add(titleLabel);

            actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 54, AutoScroll = true, WrapContents = false, BackColor = Color.Transparent, Padding = new Padding(0, 6, 0, 0), Name = "actions" };
            AddAction("Tạo hồ sơ mới");
            AddAction("Chọn địa điểm");
            AddAction("Xuất biểu mẫu");
            content.Controls.Add(actions);

            section = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(14, 42, 14, 14), Name = "section" };
            recentGrid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false, AutoGenerateColumns = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 36, Name = "recentGrid" };
            recentGrid.Columns.Add("MaHoSo", "Mã hồ sơ");
            recentGrid.Columns.Add("TenSuKien", "Tên sự kiện");
            recentGrid.Columns.Add("TrangThai", "Trạng thái");
            section.Controls.Add(recentGrid);
            section.Controls.Add(new Label { Text = "Hồ sơ gần đây", Dock = DockStyle.Top, Height = 28, Font = new Font("Times New Roman", 12, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") });

            metricGrid = new TableLayoutPanel { Dock = DockStyle.Top, Height = 116, ColumnCount = 4, RowCount = 1, Padding = new Padding(0, 0, 0, 12), BackColor = Color.Transparent, Name = "metricGrid" };
            for (int i = 0; i < 4; i++) metricGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            metricGrid.Controls.Add(MetricCard("Tổng hồ sơ", "Chờ dữ liệu"), 0, 0);
            metricGrid.Controls.Add(MetricCard("Đang xử lý", "Chờ dữ liệu"), 1, 0);
            metricGrid.Controls.Add(MetricCard("Yêu cầu bổ sung", "Chờ dữ liệu"), 2, 0);
            metricGrid.Controls.Add(MetricCard("Đã cấp phép", "Chờ dữ liệu"), 3, 0);
            content.Controls.Add(section);
            content.Controls.Add(metricGrid);

            Controls.Add(content);
            Controls.Add(sidebar);
            Controls.Add(header);
            ResumeLayout(false);
        }

        private Button NavButton(string text, string tag, int top)
        {
            var button = new Button { Text = text, Tag = tag, Location = new Point(10, top), Size = new Size(224, 42), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#142C41"), ForeColor = Color.White };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void AddAction(string text)
        {
            var button = new Button { Text = text, AutoSize = true, Height = 38, Margin = new Padding(0, 0, 10, 0), FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#137A78"), ForeColor = Color.White };
            button.FlatAppearance.BorderSize = 0;
            button.Click += PendingAction;
            actions.Controls.Add(button);
        }

        private Panel MetricCard(string title, string value)
        {
            var card = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 12, 12), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            card.Controls.Add(new Label { Text = value, Location = new Point(14, 46), AutoSize = true, Font = new Font("Times New Roman", 18, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#142C41") });
            card.Controls.Add(new Label { Text = title, Location = new Point(14, 14), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#718397") });
            return card;
        }
    }
}

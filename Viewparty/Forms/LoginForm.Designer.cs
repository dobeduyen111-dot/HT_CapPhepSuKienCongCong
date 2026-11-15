using System.Drawing;
using System.Windows.Forms;

namespace Viewparty.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlMain;
        private Panel pnlLeft;
        private Panel pnlLogoBadge;
        private Label lblLogoBadge;
        private Label lblBrand;
        private Label lblLeftDesc;
        private Panel pnlFeatures;
        private Panel card;
        private Label lblWelcome;
        private Label lblSubtitle;
        private Label lblUsername;
        private Panel pnlUsernameBox;
        private TextBox username;
        private Label lblPassword;
        private Panel pnlPasswordBox;
        private TextBox password;
        private CheckBox rememberMe;
        private Button forgotPassword;
        private Button signIn;
        private Button register;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Đăng nhập - ViewParty Portal";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1440, 920);
            MinimumSize = new Size(460, 560);
            BackColor = Color.FromArgb(245, 248, 252); // #F5F8FC
            Font = new Font("Arial", 9.5F);

            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 252),
                AutoScroll = true,
                Name = "pnlMain"
            };

            // Left branding panel (0, 0, 670, 920) #142C41
            pnlLeft = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(670, 920),
                BackColor = Color.FromArgb(20, 44, 65), // #142C41
                Name = "pnlLeft"
            };

            pnlLogoBadge = new Panel
            {
                Location = new Point(63, 263),
                Size = new Size(66, 66),
                BackColor = Color.FromArgb(19, 122, 120), // #137A78
                Name = "pnlLogoBadge"
            };
            lblLogoBadge = new Label
            {
                Text = "VP",
                Location = new Point(8, 12),
                Size = new Size(50, 42),
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Name = "lblLogoBadge"
            };
            pnlLogoBadge.Controls.Add(lblLogoBadge);

            lblBrand = new Label
            {
                Text = "VIEWPARTY PORTAL",
                Location = new Point(63, 360),
                Size = new Size(545, 54),
                Font = new Font("Arial", 25F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "lblBrand"
            };

            lblLeftDesc = new Label
            {
                Text = "Cổng tiếp nhận, thẩm định và cấp phép sự kiện tập trung",
                Location = new Point(63, 438),
                Size = new Size(540, 116),
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(199, 216, 230), // #C7D8E6
                TextAlign = ContentAlignment.TopLeft,
                Name = "lblLeftDesc"
            };

            pnlFeatures = new Panel
            {
                Location = new Point(63, 580),
                Size = new Size(540, 220),
                BackColor = Color.Transparent,
                Name = "pnlFeatures"
            };

            int featY = 0;
            string[] featureItems = new[]
            {
                "•  Quy trình cấp phép trực tuyến 04 bước minh bạch và tiện lợi",
                "•  Tra cứu sức chứa và kiểm tra lịch trống địa điểm tức thì",
                "•  Tiếp nhận, thẩm định chuyên môn và phối hợp liên ngành tập trung",
                "•  Trích xuất văn bản hành chính điện tử chuẩn mẫu Nghị định 30"
            };
            foreach (var item in featureItems)
            {
                var lblFeat = new Label
                {
                    Text = item,
                    Location = new Point(0, featY),
                    Size = new Size(540, 42),
                    Font = new Font("Arial", 10.5F),
                    ForeColor = Color.FromArgb(199, 216, 230),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                pnlFeatures.Controls.Add(lblFeat);
                featY += 46;
            }

            pnlLeft.Controls.AddRange(new Control[]
            {
                pnlLogoBadge, lblBrand, lblLeftDesc, pnlFeatures
            });

            // Right Login Card (764, 130, 575, 670) #FFFFFF border #DBE4ED
            card = new Panel
            {
                Location = new Point(764, 130),
                Size = new Size(575, 670),
                BackColor = Color.White,
                Name = "card"
            };

            lblWelcome = new Label
            {
                Text = "Chào mừng trở lại",
                Location = new Point(48, 44),
                Size = new Size(475, 48),
                Font = new Font("Arial", 23F, FontStyle.Bold),
                ForeColor = Color.FromArgb(29, 51, 72), // #1D3348
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "lblWelcome"
            };

            lblSubtitle = new Label
            {
                Text = "Đăng nhập bằng tài khoản đã được cấp quyền.",
                Location = new Point(50, 94),
                Size = new Size(462, 28),
                Font = new Font("Arial", 11F),
                ForeColor = Color.FromArgb(107, 124, 141), // #6B7C8D
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "lblSubtitle"
            };

            lblUsername = new Label
            {
                Text = "Tên đăng nhập / Email",
                Location = new Point(48, 156),
                Size = new Size(475, 24),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 124, 141), // #6B7C8D
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "lblUsername"
            };

            pnlUsernameBox = new Panel
            {
                Location = new Point(48, 183),
                Size = new Size(475, 52),
                BackColor = Color.White,
                Name = "pnlUsernameBox"
            };
            username = new TextBox
            {
                Location = new Point(14, 15),
                Size = new Size(447, 24),
                MaxLength = 80,
                BorderStyle = BorderStyle.None,
                Font = new Font("Arial", 11.5F),
                ForeColor = Color.FromArgb(29, 51, 72),
                Name = "username",
                AccessibleDescription = "Tên đăng nhập trong dbo.TAI_KHOAN"
            };
            pnlUsernameBox.Controls.Add(username);

            lblPassword = new Label
            {
                Text = "Mật khẩu",
                Location = new Point(48, 263),
                Size = new Size(475, 24),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 124, 141), // #6B7C8D
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "lblPassword"
            };

            pnlPasswordBox = new Panel
            {
                Location = new Point(48, 290),
                Size = new Size(475, 52),
                BackColor = Color.White,
                Name = "pnlPasswordBox"
            };
            password = new TextBox
            {
                Location = new Point(14, 15),
                Size = new Size(447, 24),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.None,
                Font = new Font("Arial", 11.5F),
                ForeColor = Color.FromArgb(29, 51, 72),
                Name = "password"
            };
            pnlPasswordBox.Controls.Add(password);

            rememberMe = new CheckBox
            {
                Text = "Ghi nhớ đăng nhập",
                Location = new Point(52, 359),
                Size = new Size(250, 30),
                Font = new Font("Arial", 10F),
                ForeColor = Color.FromArgb(107, 124, 141),
                Name = "rememberMe"
            };

            forgotPassword = new Button
            {
                Text = "Quên mật khẩu?",
                Location = new Point(392, 359),
                Size = new Size(131, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(19, 122, 120), // #137A78
                Font = new Font("Arial", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleRight,
                Name = "forgotPassword"
            };
            forgotPassword.FlatAppearance.BorderSize = 0;

            signIn = new Button
            {
                Text = "ĐĂNG NHẬP",
                Location = new Point(50, 422),
                Size = new Size(472, 54),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(19, 122, 120), // #137A78
                ForeColor = Color.White,
                Font = new Font("Arial", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Name = "signIn"
            };
            signIn.FlatAppearance.BorderSize = 0;

            register = new Button
            {
                Text = "Chưa có tài khoản Người tổ chức? Đăng ký ngay",
                Location = new Point(48, 501),
                Size = new Size(475, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(12, 97, 95), // #0C615F
                Font = new Font("Arial", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "register"
            };
            register.FlatAppearance.BorderSize = 0;

            card.Controls.AddRange(new Control[]
            {
                lblWelcome, lblSubtitle,
                lblUsername, pnlUsernameBox,
                lblPassword, pnlPasswordBox,
                rememberMe, forgotPassword,
                signIn, register
            });

            pnlMain.Controls.Add(card);
            pnlMain.Controls.Add(pnlLeft);
            Controls.Add(pnlMain);
            AcceptButton = signIn;
            ResumeLayout(false);
        }
    }
}

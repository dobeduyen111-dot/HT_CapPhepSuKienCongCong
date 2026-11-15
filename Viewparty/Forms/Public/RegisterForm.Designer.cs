using System.Drawing;
using System.Windows.Forms;

namespace Viewparty.Forms.Public
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlMain;
        private Panel pnlTopHeader;
        private Label lblBrand;
        private Panel card;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblSchemaNote;
        private Label lblRepresentative;
        private Label lblEmail;
        private Label lblPhone;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblConfirmPassword;
        private Label lblOrganization;
        private Label lblAddress;
        private Panel pnlRepresentativeBox;
        private Panel pnlEmailBox;
        private Panel pnlPhoneBox;
        private Panel pnlUsernameBox;
        private Panel pnlPasswordBox;
        private Panel pnlConfirmPasswordBox;
        private Panel pnlOrganizationBox;
        private Panel pnlAddressBox;
        private TextBox txtRepresentative;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtOrganization;
        private TextBox txtAddress;
        private CheckBox chkConsent;
        private Button btnCreateAccount;
        private Button btnReturnLogin;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Đăng ký người tổ chức - ViewParty Portal";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1440, 920);
            MinimumSize = new Size(1024, 720);
            BackColor = Color.FromArgb(245, 248, 252); // #F5F8FC
            Font = new Font("Arial", 9.5F);

            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 252),
                AutoScroll = true,
                Name = "pnlMain"
            };

            // Top dark navy header (0, 0, 1440, 106) #142C41
            pnlTopHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1440, 106),
                BackColor = Color.FromArgb(20, 44, 65), // #142C41
                Name = "pnlTopHeader"
            };
            lblBrand = new Label
            {
                Text = "VIEWPARTY / ĐĂNG KÝ NGƯỜI TỔ CHỨC",
                Location = new Point(66, 27),
                Size = new Size(1200, 52),
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "lblBrand"
            };
            pnlTopHeader.Controls.Add(lblBrand);

            // Main registration card (120, 148, 1200, 696) #FFFFFF border #DBE4ED
            card = new Panel
            {
                Location = new Point(120, 148),
                Size = new Size(1200, 696),
                BackColor = Color.White,
                Name = "card"
            };

            lblTitle = new Label
            {
                Text = "Tạo tài khoản tổ chức",
                Location = new Point(24, 18),
                Size = new Size(1152, 28),
                Font = new Font("Arial", 14.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 57, 75), // #26394B
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "lblTitle"
            };
            lblSubtitle = new Label
            {
                Text = "Thông tin tài khoản và thông tin liên hệ",
                Location = new Point(24, 48),
                Size = new Size(1152, 21),
                Font = new Font("Arial", 9.5F),
                ForeColor = Color.FromArgb(107, 124, 141), // #6B7C8D
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "lblSubtitle"
            };

            // Row 1: y_label = 107 (255 - 148), y_box = 134 (282 - 148)
            lblRepresentative = FieldLabel("Họ tên / Người đại diện", 38, 107, 340);
            pnlRepresentativeBox = FieldBox(38, 134, 340, 42, false, "txtRepresentative", 150, out txtRepresentative);

            lblEmail = FieldLabel("Email", 430, 107, 340);
            pnlEmailBox = FieldBox(430, 134, 340, 42, false, "txtEmail", 150, out txtEmail);

            lblPhone = FieldLabel("Số điện thoại", 822, 107, 340);
            pnlPhoneBox = FieldBox(822, 134, 340, 42, false, "txtPhone", 30, out txtPhone);

            // Row 2: y_label = 215 (363 - 148), y_box = 242 (390 - 148)
            lblUsername = FieldLabel("Tên đăng nhập", 38, 215, 340);
            pnlUsernameBox = FieldBox(38, 242, 340, 42, false, "txtUsername", 80, out txtUsername);

            lblPassword = FieldLabel("Mật khẩu", 430, 215, 340);
            pnlPasswordBox = FieldBox(430, 242, 340, 42, true, "txtPassword", 72, out txtPassword);

            lblConfirmPassword = FieldLabel("Nhập lại mật khẩu", 822, 215, 340);
            pnlConfirmPasswordBox = FieldBox(822, 242, 340, 42, true, "txtConfirmPassword", 72, out txtConfirmPassword);

            // Row 3: y_label = 323 (471 - 148), y_box = 350 (498 - 148)
            lblOrganization = FieldLabel("Tên tổ chức (nếu có)", 38, 323, 536);
            pnlOrganizationBox = FieldBox(38, 350, 536, 42, false, "txtOrganization", 200, out txtOrganization);

            lblAddress = FieldLabel("Địa chỉ liên hệ", 622, 323, 540);
            pnlAddressBox = FieldBox(622, 350, 540, 42, false, "txtAddress", 300, out txtAddress);

            // Consent checkbox at (160 - 120 = 40, 593 - 148 = 445)
            chkConsent = new CheckBox
            {
                Text = "  Tôi đồng ý với chính sách sử dụng và cam kết thông tin chính xác.",
                Location = new Point(40, 445),
                Size = new Size(920, 28),
                Font = new Font("Arial", 10.5F),
                ForeColor = Color.FromArgb(107, 124, 141), // #6B7C8D
                Name = "chkConsent"
            };

            lblSchemaNote = new Label
            {
                Text = "",
                Location = new Point(40, 480),
                Size = new Size(920, 22),
                Font = new Font("Arial", 9F),
                ForeColor = Color.FromArgb(107, 124, 141),
                Visible = false,
                Name = "lblSchemaNote"
            };

            // Buttons at y = 694 - 148 = 546
            btnReturnLogin = new Button
            {
                Text = "Quay về đăng nhập",
                Location = new Point(615, 546),
                Size = new Size(236, 52),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(12, 97, 95), // #0C615F
                Font = new Font("Arial", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Name = "btnReturnLogin"
            };
            btnReturnLogin.FlatAppearance.BorderColor = Color.FromArgb(219, 228, 237); // #DBE4ED

            btnCreateAccount = new Button
            {
                Text = "Tạo tài khoản",
                Location = new Point(880, 546),
                Size = new Size(280, 52),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(19, 122, 120), // #137A78
                ForeColor = Color.White,
                Font = new Font("Arial", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Name = "btnCreateAccount"
            };
            btnCreateAccount.FlatAppearance.BorderSize = 0;

            card.Controls.AddRange(new Control[]
            {
                lblTitle, lblSubtitle,
                lblRepresentative, pnlRepresentativeBox,
                lblEmail, pnlEmailBox,
                lblPhone, pnlPhoneBox,
                lblUsername, pnlUsernameBox,
                lblPassword, pnlPasswordBox,
                lblConfirmPassword, pnlConfirmPasswordBox,
                lblOrganization, pnlOrganizationBox,
                lblAddress, pnlAddressBox,
                chkConsent, lblSchemaNote,
                btnReturnLogin, btnCreateAccount
            });

            pnlMain.Controls.Add(card);
            pnlMain.Controls.Add(pnlTopHeader);
            Controls.Add(pnlMain);
            AcceptButton = btnCreateAccount;
            ResumeLayout(false);
        }

        private Label FieldLabel(string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 24),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 124, 141), // #6B7C8D
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private Panel FieldBox(int x, int y, int width, int height, bool password, string name, int maxLen, out TextBox textBox)
        {
            var box = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White
            };
            textBox = new TextBox
            {
                Location = new Point(12, 11),
                Size = new Size(width - 24, 22),
                Font = new Font("Arial", 10.5F),
                ForeColor = Color.FromArgb(29, 51, 72),
                UseSystemPasswordChar = password,
                BorderStyle = BorderStyle.None,
                MaxLength = maxLen,
                Name = name
            };
            box.Controls.Add(textBox);
            return box;
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Viewparty.Data;
using Viewparty.Models;
using Viewparty.Services;


namespace Viewparty.Forms
{
    public sealed partial class LoginForm : Form
    {
        private static readonly Color BorderColor = Color.FromArgb(219, 228, 237); // #DBE4ED
        private static readonly Color FocusBorderColor = Color.FromArgb(19, 122, 120); // #137A78
        private readonly AuthenticationService authentication;
        private Button btnBackHome;
        private Panel pnlResetOverlay;

        public AuthenticatedUser User { get; private set; }
        public event Action<AuthenticatedUser> SignInSucceeded;
        public event Action RegisterRequested;
        public event Action BackToHomeRequested;

        public LoginForm() : this(new AuthenticationService()) { }

        public LoginForm(AuthenticationService authentication) : this(authentication, null) { }

        public LoginForm(AuthenticationService authentication, string initialUsername)
        {
            this.authentication = authentication ?? throw new ArgumentNullException("authentication");
            InitializeComponent();
            DoubleBuffered = true;
            MinimumSize = new Size(460, 560);

            username.Text = initialUsername ?? string.Empty;
            signIn.Click += SignIn_Click;
            register.Click += (s, e) =>
            {
                if (RegisterRequested != null)
                {
                    RegisterRequested();
                    return;
                }
                DialogResult = DialogResult.Retry;
                Close();
            };
            forgotPassword.Click += (s, e) => ShowForgotPasswordOverlay();

            btnBackHome = new Button
            {
                Text = "← Quay về Trang chủ",
                Location = new Point(48, 548),
                Size = new Size(475, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(245, 248, 252),
                ForeColor = Color.FromArgb(29, 51, 72),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackHome.FlatAppearance.BorderColor = BorderColor;
            btnBackHome.Click += (s, e) =>
            {
                if (BackToHomeRequested != null)
                {
                    BackToHomeRequested();
                    return;
                }
                DialogResult = DialogResult.Cancel;
                Close();
            };
            card.Controls.Add(btnBackHome);

            card.Paint += (s, e) => DrawRectBorder(e.Graphics, card.ClientRectangle, BorderColor);
            pnlUsernameBox.Paint += (s, e) => DrawRectBorder(e.Graphics, pnlUsernameBox.ClientRectangle,
                username.Focused ? FocusBorderColor : BorderColor);
            pnlPasswordBox.Paint += (s, e) => DrawRectBorder(e.Graphics, pnlPasswordBox.ClientRectangle,
                password.Focused ? FocusBorderColor : BorderColor);
            username.GotFocus += (s, e) => pnlUsernameBox.Invalidate();
            username.LostFocus += (s, e) => pnlUsernameBox.Invalidate();
            password.GotFocus += (s, e) => pnlPasswordBox.Invalidate();
            password.LostFocus += (s, e) => pnlPasswordBox.Invalidate();

            ApplyRoundedRegion(pnlLogoBadge, 12);
            ApplyRoundedRegion(signIn, 10);

            Resize += (s, e) => LayoutCard();
            pnlMain.Resize += (s, e) => LayoutCard();
            LayoutCard();
        }

        private void ShowForgotPasswordOverlay()
        {
            if (pnlResetOverlay != null && !pnlResetOverlay.IsDisposed)
            {
                pnlResetOverlay.BringToFront();
                pnlResetOverlay.Visible = true;
                return;
            }

            pnlResetOverlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(48, 36, 48, 32)
            };
            pnlResetOverlay.Paint += (s, e) => DrawRectBorder(e.Graphics, pnlResetOverlay.ClientRectangle, BorderColor);

            int w = Math.Max(320, card.ClientSize.Width - 96);

            var lblTitle = new Label
            {
                Text = "Khôi phục mật khẩu",
                Location = new Point(48, 38),
                Size = new Size(w, 40),
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(29, 51, 72)
            };
            var lblDesc = new Label
            {
                Text = "Xác nhận tên đăng nhập và địa chỉ email đã đăng ký để đặt lại mật khẩu mới.",
                Location = new Point(48, 82),
                Size = new Size(w, 36),
                Font = new Font("Arial", 10F),
                ForeColor = Color.FromArgb(107, 124, 141)
            };

            var lblResetUser = new Label
            {
                Text = "Tên đăng nhập cần khôi phục",
                Location = new Point(48, 132),
                Size = new Size(w, 22),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 124, 141)
            };
            var txtResetUser = new TextBox
            {
                Location = new Point(48, 158),
                Size = new Size(w, 32),
                Font = new Font("Arial", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                Text = username.Text
            };

            var lblResetEmail = new Label
            {
                Text = "Địa chỉ Email đã đăng ký",
                Location = new Point(48, 206),
                Size = new Size(w, 22),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 124, 141)
            };
            var txtResetEmail = new TextBox
            {
                Location = new Point(48, 232),
                Size = new Size(w, 32),
                Font = new Font("Arial", 11F),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblNewPass = new Label
            {
                Text = "Mật khẩu mới (tối thiểu 6 ký tự)",
                Location = new Point(48, 280),
                Size = new Size(w, 22),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 124, 141)
            };
            var txtNewPass = new TextBox
            {
                Location = new Point(48, 306),
                Size = new Size(w, 32),
                Font = new Font("Arial", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            var lblConfirmPass = new Label
            {
                Text = "Nhập lại mật khẩu mới",
                Location = new Point(48, 354),
                Size = new Size(w, 22),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 124, 141)
            };
            var txtConfirmPass = new TextBox
            {
                Location = new Point(48, 380),
                Size = new Size(w, 32),
                Font = new Font("Arial", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            var btnSubmitReset = new Button
            {
                Text = "XÁC NHẬN ĐẶT LẠI MẬT KHẨU",
                Location = new Point(48, 434),
                Size = new Size(w, 48),
                FlatStyle = FlatStyle.Flat,
                BackColor = FocusBorderColor,
                ForeColor = Color.White,
                Font = new Font("Arial", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSubmitReset.FlatAppearance.BorderSize = 0;

            var btnBackToLogin = new Button
            {
                Text = "← Quay lại màn hình đăng nhập",
                Location = new Point(48, 496),
                Size = new Size(w, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(245, 248, 252),
                ForeColor = Color.FromArgb(29, 51, 72),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackToLogin.FlatAppearance.BorderColor = BorderColor;

            btnSubmitReset.Click += (s, e) =>
            {
                if (!string.Equals(txtNewPass.Text, txtConfirmPass.Text, StringComparison.Ordinal))
                {
                    MessageBox.Show(this, "Mật khẩu nhập lại không khớp với mật khẩu mới.", "Khôi phục mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    new AccountRepository().ResetPasswordByUsernameAndEmail(txtResetUser.Text, txtResetEmail.Text, txtNewPass.Text);
                    username.Text = txtResetUser.Text.Trim();
                    password.Text = txtNewPass.Text.Trim();
                    pnlResetOverlay.Visible = false;
                    MessageBox.Show(this, "Đã đặt lại mật khẩu mới thành công! Bạn có thể đăng nhập ngay.", "Khôi phục mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Không thể đặt lại mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnBackToLogin.Click += (s, e) =>
            {
                pnlResetOverlay.Visible = false;
            };

            pnlResetOverlay.Resize += (s, e) =>
            {
                int rw = Math.Max(280, pnlResetOverlay.ClientSize.Width - 96);
                lblTitle.Width = rw;
                lblDesc.Width = rw;
                txtResetUser.Width = rw;
                txtResetEmail.Width = rw;
                txtNewPass.Width = rw;
                txtConfirmPass.Width = rw;
                btnSubmitReset.Width = rw;
                btnBackToLogin.Width = rw;
            };

            pnlResetOverlay.Controls.AddRange(new Control[]
            {
                lblTitle, lblDesc,
                lblResetUser, txtResetUser,
                lblResetEmail, txtResetEmail,
                lblNewPass, txtNewPass,
                lblConfirmPass, txtConfirmPass,
                btnSubmitReset, btnBackToLogin
            });

            card.Controls.Add(pnlResetOverlay);
            pnlResetOverlay.BringToFront();
        }

        private static void DrawRectBorder(Graphics g, Rectangle rect, Color color)
        {
            if (rect.Width <= 1 || rect.Height <= 1) return;
            using (var pen = new Pen(color, 1))
            {
                g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
            }
        }

        private static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null || control.Width <= 0 || control.Height <= 0) return;
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(control.Width - d, 0, d, d, 270, 90);
            path.AddArc(control.Width - d, control.Height - d, d, d, 0, 90);
            path.AddArc(0, control.Height - d, d, d, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private static string Ui(string key, string fallback)
        {
            return new ComponentResourceManager(typeof(LoginForm)).GetString(key) ?? fallback;
        }

        private void SignIn_Click(object sender, EventArgs e)
        {
            signIn.Enabled = false;
            UseWaitCursor = true;
            var enteredUsername = username.Text;
            var enteredPassword = password.Text;
            try
            {
                User = authentication.Login(enteredUsername, enteredPassword);
                if (SignInSucceeded != null)
                {
                    SignInSucceeded(User);
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (AuthenticationFailureException ex)
            {
                if (ex.FailureKind == AuthenticationFailureKind.BCryptError || ex.FailureKind == AuthenticationFailureKind.SchemaError)
                    LocalExceptionLogger.Write(ex, enteredUsername, enteredPassword);
                var key = "Auth." + ex.FailureKind;
                var message = Ui(key, ex.Message ?? "Không thể đăng nhập lúc này. Vui lòng kiểm tra thông tin và thử lại.");
                if (ex.FailureKind == AuthenticationFailureKind.BCryptError || ex.FailureKind == AuthenticationFailureKind.SchemaError)
                    message = "Không thể đăng nhập lúc này. Vui lòng liên hệ quản trị viên.";
                MessageBox.Show(this, message, Ui("Auth.FailedTitle", "Không thể đăng nhập"), MessageBoxButtons.OK,
                    ex.FailureKind == AuthenticationFailureKind.InvalidPassword || ex.FailureKind == AuthenticationFailureKind.AccountNotFound
                        ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
                password.Clear(); password.Focus();
            }
            catch (DatabaseConnectionException ex)
            {
                LocalExceptionLogger.Write(ex, enteredUsername, enteredPassword);
                MessageBox.Show(this, ex.Message,
                    Ui("Auth.FailedTitle", "Không thể đăng nhập"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                password.Clear(); password.Focus();
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex, enteredUsername, enteredPassword);
                MessageBox.Show(this,
                    "Không thể hoàn tất đăng nhập. Vui lòng thử lại hoặc liên hệ quản trị viên.",
                    Ui("Auth.FailedTitle", "Không thể đăng nhập"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                password.Clear(); password.Focus();
            }
            finally
            {
                UseWaitCursor = false;
                if (!IsDisposed) signIn.Enabled = true;
                enteredPassword = null;
            }
        }

        private void LayoutCard()
        {
            if (pnlMain == null || pnlMain.IsDisposed || card == null || pnlLeft == null) return;

            int actualW = pnlMain.ClientSize.Width;
            int actualH = pnlMain.ClientSize.Height;

            // Chế độ co giãn linh hoạt: Khi thu nhỏ cửa sổ (< 1080px), ưu tiên tuyệt đối màn hình/card đăng nhập.
            // Ẩn hoàn toàn panel đồ họa bên trái để form đăng nhập luôn hiển thị đầy đủ, không bị cắt mép hay co rúm.
            if (actualW < 1080)
            {
                pnlLeft.Visible = false;

                int cardWidth = Math.Min(520, Math.Max(380, actualW - 32));
                card.Size = new Size(cardWidth, 616);

                int innerWidth = Math.Max(280, cardWidth - 80);
                int innerX = (cardWidth - innerWidth) / 2;

                lblWelcome.Location = new Point(innerX, 44);
                lblWelcome.Width = innerWidth;

                lblSubtitle.Location = new Point(innerX, 94);
                lblSubtitle.Width = innerWidth;

                lblUsername.Location = new Point(innerX, 156);
                lblUsername.Width = innerWidth;

                pnlUsernameBox.Location = new Point(innerX, 183);
                pnlUsernameBox.Width = innerWidth;
                username.Width = Math.Max(100, innerWidth - 28);

                lblPassword.Location = new Point(innerX, 263);
                lblPassword.Width = innerWidth;

                pnlPasswordBox.Location = new Point(innerX, 290);
                pnlPasswordBox.Width = innerWidth;
                password.Width = Math.Max(100, innerWidth - 28);

                rememberMe.Location = new Point(innerX, 359);
                forgotPassword.Location = new Point(innerX + innerWidth - forgotPassword.Width, 359);

                signIn.Location = new Point(innerX, 422);
                signIn.Width = innerWidth;
                ApplyRoundedRegion(signIn, 10);

                register.Location = new Point(innerX, 501);
                register.Width = innerWidth;

                if (btnBackHome != null)
                {
                    btnBackHome.Location = new Point(innerX, 548);
                    btnBackHome.Width = innerWidth;
                }

                int cardX = Math.Max(12, (actualW - cardWidth) / 2);
                int cardY = Math.Max(16, (actualH - card.Height) / 2);
                card.Location = new Point(cardX, cardY);

                pnlMain.AutoScrollPosition = new Point(0, 0);
                pnlMain.AutoScrollMinSize = new Size(0, Math.Max(actualH, card.Bottom + 20));
            }
            else
            {
                pnlLeft.Visible = true;

                int leftWidth = Math.Max(440, Math.Min(580, (int)(actualW * 0.42)));
                pnlLeft.Location = new Point(0, 0);
                pnlLeft.Size = new Size(leftWidth, Math.Max(actualH, 760));

                int leftPad = Math.Max(24, (leftWidth - 520) / 2);
                int topShift = Math.Max(36, (actualH - 620) / 2);
                pnlLogoBadge.Location = new Point(leftPad, topShift);
                lblBrand.Location = new Point(leftPad, topShift + 82);
                lblLeftDesc.Location = new Point(leftPad, topShift + 146);
                if (pnlFeatures != null) pnlFeatures.Location = new Point(leftPad, topShift + 266);

                int rightAreaWidth = actualW - leftWidth;
                int cardWidth = Math.Min(520, Math.Max(420, rightAreaWidth - 48));
                card.Size = new Size(cardWidth, 616);

                int innerWidth = Math.Max(280, cardWidth - 96);
                int innerX = 48;

                lblWelcome.Location = new Point(innerX, 44);
                lblWelcome.Width = innerWidth;

                lblSubtitle.Location = new Point(innerX, 94);
                lblSubtitle.Width = innerWidth;

                lblUsername.Location = new Point(innerX, 156);
                lblUsername.Width = innerWidth;

                pnlUsernameBox.Location = new Point(innerX, 183);
                pnlUsernameBox.Width = innerWidth;
                username.Width = Math.Max(100, innerWidth - 28);

                lblPassword.Location = new Point(innerX, 263);
                lblPassword.Width = innerWidth;

                pnlPasswordBox.Location = new Point(innerX, 290);
                pnlPasswordBox.Width = innerWidth;
                password.Width = Math.Max(100, innerWidth - 28);

                rememberMe.Location = new Point(innerX, 359);
                forgotPassword.Location = new Point(innerX + innerWidth - forgotPassword.Width, 359);

                signIn.Location = new Point(innerX, 422);
                signIn.Width = innerWidth;
                ApplyRoundedRegion(signIn, 10);

                register.Location = new Point(innerX, 501);
                register.Width = innerWidth;

                if (btnBackHome != null)
                {
                    btnBackHome.Location = new Point(innerX, 548);
                    btnBackHome.Width = innerWidth;
                }

                int cardX = leftWidth + Math.Max(20, (rightAreaWidth - cardWidth) / 2);
                int cardY = Math.Max(20, (actualH - card.Height) / 2);
                card.Location = new Point(cardX, cardY);

                pnlMain.AutoScrollPosition = new Point(0, 0);
                pnlMain.AutoScrollMinSize = new Size(leftWidth + cardWidth + 40, Math.Max(660, card.Bottom + 24));
            }
        }
    }
}

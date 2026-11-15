using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Viewparty.Data;
using Viewparty.Services;

namespace Viewparty.Forms.Public
{
    public partial class RegisterForm : Form
    {
        private static readonly Color BorderColor = Color.FromArgb(219, 228, 237); // #DBE4ED
        private static readonly Color FocusBorderColor = Color.FromArgb(19, 122, 120); // #137A78
        private readonly RegistrationService registration;

        public string RegisteredUsername { get; private set; }
        public event Action<string> RegisterSucceeded;
        public event Action BackToLoginRequested;

        public RegisterForm()
            : this(new RegistrationService())
        {
        }

        public RegisterForm(RegistrationService registration)
        {
            this.registration = registration ?? throw new ArgumentNullException("registration");
            InitializeComponent();
            DoubleBuffered = true;
            MinimumSize = new Size(920, 640);

            btnCreateAccount.Click += CreateAccount_Click;
            btnReturnLogin.Click += (s, e) =>
            {
                if (BackToLoginRequested != null)
                {
                    BackToLoginRequested();
                    return;
                }
                DialogResult = DialogResult.Retry;
                Close();
            };

            card.Paint += (s, e) => DrawRectBorder(e.Graphics, card.ClientRectangle, BorderColor);

            BindBoxBorder(pnlRepresentativeBox, txtRepresentative);
            BindBoxBorder(pnlEmailBox, txtEmail);
            BindBoxBorder(pnlPhoneBox, txtPhone);
            BindBoxBorder(pnlUsernameBox, txtUsername);
            BindBoxBorder(pnlPasswordBox, txtPassword);
            BindBoxBorder(pnlConfirmPasswordBox, txtConfirmPassword);
            BindBoxBorder(pnlOrganizationBox, txtOrganization);
            BindBoxBorder(pnlAddressBox, txtAddress);

            ApplyRoundedRegion(btnCreateAccount, 10);
            ApplyRoundedRegion(btnReturnLogin, 10);

            ConfigureConsentPolicyBehavior();

            pnlMain.Resize += (s, e) => LayoutForm();
            LayoutForm();
        }

        private void ConfigureConsentPolicyBehavior()
        {
            chkConsent.AutoCheck = false;
            chkConsent.Cursor = Cursors.Hand;
            chkConsent.Text = "      Tôi đồng ý với chính sách sử dụng và cam kết thông tin chính xác (Nhấn để đọc chính sách)";
            chkConsent.Click += (s, e) =>
            {
                bool accepted = ShowPolicyTermsDialog();
                chkConsent.Checked = accepted;
                chkConsent.ForeColor = accepted
                    ? Color.FromArgb(19, 122, 120)
                    : Color.FromArgb(107, 124, 141);
                chkConsent.Font = new Font("Arial", 10.5F, accepted ? FontStyle.Bold : FontStyle.Regular);
                chkConsent.Text = accepted
                    ? "      Tôi đồng ý với chính sách sử dụng và cam kết thông tin chính xác (Đã đọc và đồng ý)"
                    : "      Tôi đồng ý với chính sách sử dụng và cam kết thông tin chính xác (Nhấn để đọc chính sách)";
                chkConsent.Invalidate();
            };

            chkConsent.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var coverRect = new Rectangle(0, 0, 26, chkConsent.Height);
                using (var bgBrush = new SolidBrush(card.BackColor))
                {
                    e.Graphics.FillRectangle(bgBrush, coverRect);
                }

                var boxRect = new Rectangle(2, (chkConsent.Height - 20) / 2, 20, 20);
                using (var path = new GraphicsPath())
                {
                    int d = 8;
                    path.AddArc(boxRect.X, boxRect.Y, d, d, 180, 90);
                    path.AddArc(boxRect.Right - d, boxRect.Y, d, d, 270, 90);
                    path.AddArc(boxRect.Right - d, boxRect.Bottom - d, d, d, 0, 90);
                    path.AddArc(boxRect.X, boxRect.Bottom - d, d, d, 90, 90);
                    path.CloseFigure();

                    if (chkConsent.Checked)
                    {
                        using (var fill = new SolidBrush(Color.FromArgb(22, 163, 74))) // Green #16A34A
                        {
                            e.Graphics.FillPath(fill, path);
                        }
                        using (var penCheck = new Pen(Color.White, 2.4F)
                        {
                            StartCap = LineCap.Round,
                            EndCap = LineCap.Round,
                            LineJoin = LineJoin.Round
                        })
                        {
                            e.Graphics.DrawLines(penCheck, new[]
                            {
                                new Point(boxRect.X + 5, boxRect.Y + 10),
                                new Point(boxRect.X + 9, boxRect.Y + 14),
                                new Point(boxRect.X + 15, boxRect.Y + 6)
                            });
                        }
                    }
                    else
                    {
                        using (var fill = new SolidBrush(Color.White))
                        {
                            e.Graphics.FillPath(fill, path);
                        }
                        using (var borderPen = new Pen(Color.FromArgb(148, 163, 184), 1.5F))
                        {
                            e.Graphics.DrawPath(borderPen, path);
                        }
                    }
                }
            };
        }

        private bool ShowPolicyTermsDialog()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "Chính sách sử dụng & Cam kết của Người tổ chức — ViewParty Portal";
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.ClientSize = new Size(760, 560);
                dlg.BackColor = Color.FromArgb(245, 248, 252);
                dlg.Font = new Font("Arial", 9.5F);

                var headerPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 68,
                    BackColor = Color.FromArgb(20, 44, 65),
                    Padding = new Padding(24, 12, 24, 10)
                };
                headerPanel.Controls.Add(new Label
                {
                    Text = "CHÍNH SÁCH SỬ DỤNG & CAM KẾT TRÁCH NHIỆM",
                    Location = new Point(24, 12),
                    AutoSize = true,
                    Font = new Font("Arial", 13.5F, FontStyle.Bold),
                    ForeColor = Color.White
                });
                headerPanel.Controls.Add(new Label
                {
                    Text = "Vui lòng đọc kỹ các quy định dưới đây trước khi xác nhận đồng ý đăng ký tài khoản.",
                    Location = new Point(24, 40),
                    AutoSize = true,
                    Font = new Font("Arial", 9F),
                    ForeColor = Color.FromArgb(188, 228, 224)
                });

                var bodyCard = new Panel
                {
                    Location = new Point(24, 84),
                    Size = new Size(712, 392),
                    BackColor = Color.White,
                    Padding = new Padding(18)
                };
                bodyCard.Paint += (s, e) => DrawRectBorder(e.Graphics, bodyCard.ClientRectangle, BorderColor);

                var txtPolicy = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    BorderStyle = BorderStyle.None,
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(29, 51, 72),
                    Font = new Font("Arial", 10F),
                    Text =
                        "QUY ĐỊNH SỬ DỤNG CỔNG THÔNG TIN CẤP PHÉP SỰ KIỆN VIEWPARTY PORTAL\r\n" +
                        "======================================================================\r\n\r\n" +
                        "1. Cam kết về tính chính xác của thông tin đăng ký\r\n" +
                        "   • Cá nhân/Tổ chức đăng ký tài khoản cam kết toàn bộ thông tin khai báo (Họ tên người đại diện, Tên đơn vị tổ chức, Email, Số điện thoại và Địa chỉ liên hệ) là chính xác, trung thực và hợp pháp.\r\n" +
                        "   • Người đăng ký chịu trách nhiệm bảo mật thông tin tài khoản và mật khẩu đăng nhập trên hệ thống.\r\n\r\n" +
                        "2. Quy định về lập và nộp hồ sơ xin cấp phép sự kiện View Party\r\n" +
                        "   • Thời hạn nộp hồ sơ: Hồ sơ đề nghị cấp phép phải được nộp trực tuyến trước ngày dự kiến tổ chức sự kiện tối thiểu 30 ngày.\r\n" +
                        "   • Quy mô sự kiện: Áp dụng cho sự kiện trình chiếu công cộng có quy mô từ 1.000 đến 10.000 người tham dự tại các địa điểm đủ sức chứa và không trùng lịch.\r\n\r\n" +
                        "3. Trách nhiệm về Bản quyền trình chiếu và Phương án đảm bảo an toàn\r\n" +
                        "   • Đơn vị tổ chức phải có văn bản chứng minh quyền trình chiếu công cộng hợp pháp, còn hiệu lực bao phủ thời gian và phạm vi địa điểm tổ chức.\r\n" +
                        "   • Đơn vị tổ chức bắt buộc xây dựng và thực hiện nghiêm túc phương án đảm bảo an ninh trật tự, y tế cấp cứu, phòng cháy chữa cháy (PCCC), lối thoát hiểm và phương án xử lý sự cố khẩn cấp.\r\n\r\n" +
                        "4. Phối hợp thẩm định và chấp hành quyết định của cơ quan quản lý\r\n" +
                        "   • Đơn vị tổ chức có trách nhiệm giải trình, bổ sung hồ sơ đúng thời hạn khi có thông báo yêu cầu bổ sung từ Cán bộ thẩm định hoặc Đơn vị chuyên môn liên ngành.\r\n" +
                        "   • Chỉ được phép tổ chức sự kiện sau khi đã nhận được Giấy phép tổ chức sự kiện chính thức từ Người phê duyệt có thẩm quyền.\r\n\r\n" +
                        "5. Chính sách bảo mật dữ liệu\r\n" +
                        "   • Mọi thông tin cá nhân và tài liệu hồ sơ đính kèm trên hệ thống chỉ được sử dụng cho mục đích thẩm định, phối hợp liên ngành và quản lý cấp phép theo đúng quy định."
                };
                txtPolicy.SelectionStart = 0;
                txtPolicy.SelectionLength = 0;
                bodyCard.Controls.Add(txtPolicy);

                var btnDecline = new Button
                {
                    Text = "Không đồng ý",
                    Size = new Size(160, 42),
                    Location = new Point(392, 494),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(107, 124, 141),
                    Font = new Font("Arial", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnDecline.FlatAppearance.BorderColor = BorderColor;
                btnDecline.Click += (s, e) =>
                {
                    dlg.DialogResult = DialogResult.No;
                    dlg.Close();
                };

                var btnAgree = new Button
                {
                    Text = "✓  Đồng ý chính sách",
                    Size = new Size(172, 42),
                    Location = new Point(564, 494),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(22, 163, 74), // Green #16A34A
                    ForeColor = Color.White,
                    Font = new Font("Arial", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnAgree.FlatAppearance.BorderSize = 0;
                btnAgree.Click += (s, e) =>
                {
                    dlg.DialogResult = DialogResult.Yes;
                    dlg.Close();
                };

                ApplyRoundedRegion(btnDecline, 8);
                ApplyRoundedRegion(btnAgree, 8);

                dlg.Controls.AddRange(new Control[] { headerPanel, bodyCard, btnDecline, btnAgree });
                dlg.AcceptButton = btnAgree;
                dlg.CancelButton = btnDecline;

                return dlg.ShowDialog(this) == DialogResult.Yes;
            }
        }

        private static void BindBoxBorder(Panel box, TextBox input)
        {
            box.Paint += (s, e) => DrawRectBorder(
                e.Graphics,
                box.ClientRectangle,
                input.Focused ? FocusBorderColor : BorderColor);
            input.GotFocus += (s, e) => box.Invalidate();
            input.LostFocus += (s, e) => box.Invalidate();
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

        private void LayoutForm()
        {
            if (pnlMain == null || pnlMain.IsDisposed || card == null || pnlTopHeader == null) return;

            int viewWidth = Math.Max(880, pnlMain.ClientSize.Width);
            int viewHeight = Math.Max(560, pnlMain.ClientSize.Height);

            int cardWidth = Math.Min(1160, Math.Max(800, viewWidth - 64));
            int cardX = Math.Max(24, (viewWidth - cardWidth) / 2);

            pnlTopHeader.Height = 72;
            pnlTopHeader.Width = viewWidth;
            lblBrand.Location = new Point(cardX, 16);

            int cardHeight = 440;
            card.Size = new Size(cardWidth, cardHeight);
            int cardY = Math.Max(88, 72 + Math.Max(12, (viewHeight - 72 - cardHeight) / 2));
            card.Location = new Point(cardX, cardY);

            int pad = 36;
            int gap = 24;
            int col3Width = (cardWidth - pad * 2 - gap * 2) / 3;
            int col2Width = (cardWidth - pad * 2 - gap) / 2;

            SetFieldBounds(lblRepresentative, pnlRepresentativeBox, txtRepresentative, pad, 72, col3Width);
            SetFieldBounds(lblEmail, pnlEmailBox, txtEmail, pad + col3Width + gap, 72, col3Width);
            SetFieldBounds(lblPhone, pnlPhoneBox, txtPhone, pad + (col3Width + gap) * 2, 72, col3Width);

            SetFieldBounds(lblUsername, pnlUsernameBox, txtUsername, pad, 148, col3Width);
            SetFieldBounds(lblPassword, pnlPasswordBox, txtPassword, pad + col3Width + gap, 148, col3Width);
            SetFieldBounds(lblConfirmPassword, pnlConfirmPasswordBox, txtConfirmPassword, pad + (col3Width + gap) * 2, 148, col3Width);

            SetFieldBounds(lblOrganization, pnlOrganizationBox, txtOrganization, pad, 224, col2Width);
            SetFieldBounds(lblAddress, pnlAddressBox, txtAddress, pad + col2Width + gap, 224, col2Width);

            chkConsent.Location = new Point(pad + 2, 302);
            chkConsent.Width = cardWidth - pad * 2;
            btnCreateAccount.Location = new Point(cardWidth - pad - btnCreateAccount.Width, 366);
            btnReturnLogin.Location = new Point(btnCreateAccount.Left - btnReturnLogin.Width - 14, 366);

            pnlMain.AutoScrollMinSize = new Size(0, card.Bottom + 24);
        }

        private static void SetFieldBounds(Label lbl, Panel box, TextBox txt, int x, int y, int width)
        {
            lbl.Location = new Point(x, y);
            lbl.Width = width;
            box.Location = new Point(x, y + 26);
            box.Width = width;
            txt.Width = Math.Max(100, width - 24);
        }

        private void CreateAccount_Click(object sender, EventArgs e)
        {
            btnCreateAccount.Enabled = false;
            UseWaitCursor = true;
            string password = txtPassword.Text;
            string confirmation = txtConfirmPassword.Text;

            try
            {
                var receipt = registration.Register(new RegistrationRequest
                {
                    FullName = txtRepresentative.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    Username = txtUsername.Text,
                    Password = password,
                    ConfirmPassword = confirmation,
                    ConsentAccepted = chkConsent.Checked
                });

                RegisteredUsername = receipt.Username;
                MessageBox.Show(this,
                    "Tạo tài khoản thành công. Bạn có thể đăng nhập ngay bằng tên đăng nhập vừa đăng ký.",
                    "Đăng ký thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (RegisterSucceeded != null)
                {
                    RegisterSucceeded(RegisteredUsername);
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (RegistrationValidationException ex)
            {
                MessageBox.Show(this, ex.Message, "Thông tin đăng ký chưa hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (RegistrationConflictException ex)
            {
                MessageBox.Show(this, ex.Message, "Thông tin đã được sử dụng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (DatabaseConnectionException ex)
            {
                LocalExceptionLogger.Write(ex, txtUsername.Text, txtEmail.Text, txtPhone.Text, password, confirmation);
                MessageBox.Show(this,
                    ex.Message,
                    "Không thể tạo tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex, txtUsername.Text, txtEmail.Text, txtPhone.Text, password, confirmation);
                MessageBox.Show(this,
                    "Không thể hoàn tất đăng ký. Vui lòng thử lại hoặc liên hệ quản trị viên.",
                    "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                if (!IsDisposed) btnCreateAccount.Enabled = true;
                password = null;
                confirmation = null;
                txtPassword.Clear();
                txtConfirmPassword.Clear();
            }
        }
    }
}

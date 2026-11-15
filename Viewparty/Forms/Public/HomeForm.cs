using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Viewparty.Forms.Organizer;
using Viewparty.Services;

namespace Viewparty.Forms.Public
{
    public partial class HomeForm : Form
    {
        private static readonly Color BorderColor = Color.FromArgb(219, 228, 237); // #DBE4ED
        private static readonly Color TealColor = Color.FromArgb(19, 122, 120);   // #137A78
        private static readonly Color DarkTeal = Color.FromArgb(12, 97, 95);       // #0C615F
        private static readonly Color NavyInk = Color.FromArgb(20, 44, 65);        // #142C41
        private static readonly Color MutedInk = Color.FromArgb(107, 124, 141);    // #6B7C8D
        private static readonly Color MintBg = Color.FromArgb(233, 247, 243);      // #E9F7F3
        private static readonly Color SoftBlueBg = Color.FromArgb(240, 246, 251);  // #F0F6FB
        private Button btnDangKy;
        private Form activeChildView;
        private Panel pnlSubPageHost;
        private string activeTopTab = "HOME";

        public HomeForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            MinimumSize = new Size(880, 620);

            // Move pnlHeader to top level of Form for fixed, non-scrolling, rock-solid responsive header
            pnlMainContainer.Controls.Remove(pnlHeader);
            Controls.Add(pnlHeader);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 76;
            pnlMainContainer.Dock = DockStyle.Fill;
            pnlMainContainer.BringToFront();
            pnlHeader.BringToFront();

            // Set UseMnemonic = false for all labels to preserve '&' character and avoid unexpected access keys
            lblLogoText.UseMnemonic = false;
            lblNavQuyTrinh.UseMnemonic = false;
            lblNavTraCuu.UseMnemonic = false;
            lblNavHuongDan.UseMnemonic = false;
            lblBadgeSystem.UseMnemonic = false;
            lblHeroTitle.UseMnemonic = false;
            lblHeroSubtitle.UseMnemonic = false;
            lblStepsTitle.UseMnemonic = false;
            lblStep1.UseMnemonic = false;
            lblStep2.UseMnemonic = false;
            lblStep3.UseMnemonic = false;
            lblStep4.UseMnemonic = false;
            lblTitleCard1.UseMnemonic = false;
            lblDescCard1.UseMnemonic = false;
            lblTitleCard2.UseMnemonic = false;
            lblDescCard2.UseMnemonic = false;
            lblTitleCard3.UseMnemonic = false;
            lblDescCard3.UseMnemonic = false;

            btnDangKy = new Button
            {
                Text = "Đăng ký",
                Size = new Size(116, 40),
                BackColor = Color.White,
                ForeColor = DarkTeal,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDangKy.FlatAppearance.BorderColor = BorderColor;
            btnDangKy.Click += OpenRegister;
            pnlHeader.Controls.Add(btnDangKy);

            StyleTopNavButton(lblNavQuyTrinh, "Quy trình", 112);
            StyleTopNavButton(lblNavTraCuu, "Tra cứu thông tin", 148);
            StyleTopNavButton(lblNavHuongDan, "Hướng dẫn hồ sơ", 152);

            EventHandler goHomeHandler = (s, e) => ShowHomeLanding();
            pnlLogoIcon.Cursor = Cursors.Hand;
            lblLogoIcon.Cursor = Cursors.Hand;
            lblLogoText.Cursor = Cursors.Hand;
            pnlLogoIcon.Click += goHomeHandler;
            lblLogoIcon.Click += goHomeHandler;
            lblLogoText.Click += goHomeHandler;

            // Wire up header navigation and buttons
            btnDangNhap.Click += OpenLogin;
            btnTaoHoSoNgay.Click += OpenCreateOrRegister;
            btnXemQuyTrinh.Click += ScrollToProcess;
            lblNavQuyTrinh.Click += ScrollToProcess;
            lblNavTraCuu.Click += OpenPublicVenueSearch;
            lblNavHuongDan.Click += ShowGuide;

            // Wire up feature cards click
            WireCardClick(pnlFeatureCard1, OpenRegister);
            WireCardClick(pnlFeatureCard2, OpenPublicVenueSearch);
            WireCardClick(pnlFeatureCard3, OpenLogin);

            pnlHeader.Paint += (s, e) =>
            {
                using (var pen = new Pen(BorderColor, 1))
                    e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };
            pnlHeroBanner.Paint += (s, e) =>
            {
                using (var pen = new Pen(BorderColor, 1))
                    e.Graphics.DrawLine(pen, 0, pnlHeroBanner.Height - 1, pnlHeroBanner.Width, pnlHeroBanner.Height - 1);
            };

            foreach (Panel card in new[] { pnlCardSteps, pnlFeatureCard1, pnlFeatureCard2, pnlFeatureCard3 })
            {
                Panel target = card;
                target.Paint += (s, e) => DrawCardBorder(e.Graphics, target.ClientRectangle, BorderColor);
            }

            ApplyRoundedRegion(pnlLogoIcon, 10);
            ApplyRoundedRegion(btnDangNhap, 10);
            ApplyRoundedRegion(btnDangKy, 10);
            ApplyRoundedRegion(pnlBadgeSystem, 13);
            ApplyRoundedRegion(btnTaoHoSoNgay, 10);
            ApplyRoundedRegion(btnXemQuyTrinh, 10);
            ApplyRoundedRegion(pnlBadgeCard1, 12);
            ApplyRoundedRegion(pnlBadgeCard2, 12);
            ApplyRoundedRegion(pnlBadgeCard3, 12);

            UpdateTopNavHighlight();
            pnlHeader.Resize += (s, e) => LayoutHeaderControls();
            pnlMainContainer.Resize += (s, e) => LayoutResponsive();
            Resize += (s, e) =>
            {
                LayoutHeaderControls();
                LayoutResponsive();
            };
            LayoutHeaderControls();
            LayoutResponsive();
        }

        private void StyleTopNavButton(Label navLabel, string text, int width)
        {
            navLabel.Text = text;
            navLabel.AutoSize = false;
            navLabel.Size = new Size(width, 36);
            navLabel.TextAlign = ContentAlignment.MiddleCenter;
            navLabel.Font = new Font("Arial", 9.5F, FontStyle.Bold);
            navLabel.Cursor = Cursors.Hand;
            navLabel.Paint += (s, e) =>
            {
                bool isActive = (navLabel == lblNavQuyTrinh && activeTopTab == "PROCESS") ||
                                (navLabel == lblNavTraCuu && activeTopTab == "SEARCH") ||
                                (navLabel == lblNavHuongDan && activeTopTab == "GUIDE");
                Color border = isActive ? TealColor : Color.FromArgb(226, 234, 242);
                using (var pen = new Pen(border, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, navLabel.Width - 1, navLabel.Height - 1);
                }
            };
            navLabel.MouseEnter += (s, e) =>
            {
                if ((navLabel == lblNavQuyTrinh && activeTopTab == "PROCESS") ||
                    (navLabel == lblNavTraCuu && activeTopTab == "SEARCH") ||
                    (navLabel == lblNavHuongDan && activeTopTab == "GUIDE"))
                    return;
                navLabel.BackColor = MintBg;
                navLabel.ForeColor = DarkTeal;
            };
            navLabel.MouseLeave += (s, e) => UpdateTopNavHighlight();
        }

        private void UpdateTopNavHighlight()
        {
            foreach (var item in new[]
            {
                Tuple.Create(lblNavQuyTrinh, "PROCESS"),
                Tuple.Create(lblNavTraCuu, "SEARCH"),
                Tuple.Create(lblNavHuongDan, "GUIDE")
            })
            {
                bool active = activeTopTab == item.Item2;
                item.Item1.BackColor = active ? MintBg : SoftBlueBg;
                item.Item1.ForeColor = active ? DarkTeal : Color.FromArgb(38, 57, 75);
                item.Item1.Invalidate();
            }
        }

        private void ShowHomeLanding()
        {
            activeTopTab = "HOME";
            UpdateTopNavHighlight();
            if (pnlSubPageHost != null)
            {
                pnlSubPageHost.Visible = false;
            }
            pnlHeroBanner.Visible = true;
            pnlFeatureCard1.Visible = true;
            pnlFeatureCard2.Visible = true;
            pnlFeatureCard3.Visible = true;
            LayoutResponsive();
        }

        private void ShowEmbeddedForm(Form child, string windowTitle)
        {
            SuspendLayout();
            if (activeChildView != null)
            {
                Controls.Remove(activeChildView);
                activeChildView.Dispose();
                activeChildView = null;
            }
            if (child == null)
            {
                Text = "ViewParty Portal - Hệ Thống Quản Lý Sự Kiện";
                if (pnlHeader != null) pnlHeader.Visible = true;
                pnlMainContainer.Visible = true;
                pnlMainContainer.BringToFront();
                LayoutResponsive();
                ResumeLayout(true);
                return;
            }

            Text = windowTitle ?? child.Text;
            if (pnlHeader != null) pnlHeader.Visible = false;
            pnlMainContainer.Visible = false;
            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.WindowState = FormWindowState.Normal;
            child.Dock = DockStyle.Fill;
            activeChildView = child;
            Controls.Add(child);
            child.BringToFront();
            child.Show();
            ResumeLayout(true);
        }

        private static void WireCardClick(Control root, EventHandler handler)
        {
            root.Click += handler;
            foreach (Control child in root.Controls)
            {
                child.Cursor = Cursors.Hand;
                WireCardClick(child, handler);
            }
        }

        private static void DrawCardBorder(Graphics g, Rectangle rect, Color borderColor)
        {
            if (rect.Width <= 1 || rect.Height <= 1) return;
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
            }
        }

        private static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null || control.Width <= 0 || control.Height <= 0) return;
            using (var path = CreateRoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius))
            {
                control.Region = new Region(path);
            }
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void LayoutHeaderControls()
        {
            if (pnlHeader == null || pnlHeader.IsDisposed) return;
            int headerW = Math.Max(760, pnlHeader.ClientSize.Width);
            int safeContentW = Math.Min(1200, Math.Max(700, headerW - 48));
            int headerStartX = Math.Max(24, (headerW - safeContentW) / 2);
            int headerRightEdge = headerStartX + safeContentW;

            pnlLogoIcon.Location = new Point(headerStartX, 18);
            lblLogoText.Location = new Point(pnlLogoIcon.Right + 12, 24);
            lblLogoText.UseMnemonic = false;

            btnDangNhap.Size = new Size(headerW < 960 ? 116 : 136, 40);
            ApplyRoundedRegion(btnDangNhap, 10);
            btnDangNhap.Location = new Point(headerRightEdge - btnDangNhap.Width, 18);

            if (btnDangKy != null)
            {
                btnDangKy.Size = new Size(headerW < 960 ? 104 : 118, 40);
                ApplyRoundedRegion(btnDangKy, 10);
                btnDangKy.Location = new Point(btnDangNhap.Left - btnDangKy.Width - 10, 18);
            }

            int leftLimit = lblLogoText.Right + 18;
            int rightLimit = (btnDangKy != null ? btnDangKy.Left : btnDangNhap.Left) - 18;
            int availNav = rightLimit - leftLimit;

            int w1 = 96, w2 = 138, w3 = 138;
            int navGap = 10;
            int totalNav = w1 + w2 + w3 + navGap * 2;

            if (availNav < totalNav)
            {
                navGap = 6;
                int shrinkW = Math.Max(240, availNav - navGap * 2);
                w1 = Math.Max(76, (int)(shrinkW * 0.25));
                w2 = Math.Max(104, (int)(shrinkW * 0.375));
                w3 = Math.Max(104, (int)(shrinkW * 0.375));
                totalNav = w1 + w2 + w3 + navGap * 2;
            }

            int navStart = Math.Max(leftLimit, leftLimit + (availNav - totalNav) / 2);

            lblNavQuyTrinh.Size = new Size(w1, 38);
            lblNavTraCuu.Size = new Size(w2, 38);
            lblNavHuongDan.Size = new Size(w3, 38);

            lblNavQuyTrinh.Location = new Point(navStart, 19);
            lblNavTraCuu.Location = new Point(lblNavQuyTrinh.Right + navGap, 19);
            lblNavHuongDan.Location = new Point(lblNavTraCuu.Right + navGap, 19);
        }

        private void LayoutResponsive()
        {
            LayoutHeaderControls();

            if (pnlMainContainer == null || pnlMainContainer.IsDisposed || !pnlMainContainer.Visible) return;

            // 2. MAIN SCROLLABLE BODY RESPONSIVE LAYOUT
            int actualWidth = Math.Max(760, pnlMainContainer.ClientSize.Width);
            int padX = actualWidth < 960 ? 20 : 28;
            int contentWidth = Math.Min(1200, Math.Max(680, actualWidth - padX * 2));
            int startX = Math.Max(padX, (actualWidth - contentWidth) / 2);
            int rightEdge = startX + contentWidth;

            if (pnlSubPageHost != null && pnlSubPageHost.Visible)
            {
                pnlSubPageHost.Location = new Point(0, 0);
                pnlSubPageHost.Width = actualWidth;
                LayoutSubPageContent(actualWidth, startX, contentWidth);
                pnlMainContainer.AutoScrollMinSize = new Size(0, pnlSubPageHost.Bottom + 32);
                return;
            }

            pnlHeroBanner.Location = new Point(0, 0);
            pnlHeroBanner.Width = actualWidth;
            pnlBadgeSystem.Location = new Point(startX, 45);
            lblHeroTitle.Location = new Point(startX, 104);
            lblHeroSubtitle.Location = new Point(startX, 239);
            btnTaoHoSoNgay.Location = new Point(startX, 340);
            btnXemQuyTrinh.Location = new Point(startX + 212, 340);

            int stepsWidth = Math.Min(426, Math.Max(300, (int)(contentWidth * 0.34)));
            pnlCardSteps.Size = new Size(stepsWidth, 278);
            pnlCardSteps.Location = new Point(rightEdge - stepsWidth, 66);

            int leftMax = Math.Max(340, pnlCardSteps.Left - startX - 24);
            lblHeroTitle.Width = leftMax;
            lblHeroSubtitle.Width = leftMax;

            int gap = 20;
            int cardWidth = (contentWidth - gap * 2) / 3;
            pnlFeatureCard1.Size = new Size(cardWidth, 225);
            pnlFeatureCard2.Size = new Size(cardWidth, 225);
            pnlFeatureCard3.Size = new Size(cardWidth, 225);

            pnlFeatureCard1.Location = new Point(startX, 556);
            pnlFeatureCard2.Location = new Point(startX + cardWidth + gap, 556);
            pnlFeatureCard3.Location = new Point(startX + (cardWidth + gap) * 2, 556);

            foreach (var tuple in new[]
            {
                Tuple.Create(pnlFeatureCard1, lblTitleCard1, lblDescCard1),
                Tuple.Create(pnlFeatureCard2, lblTitleCard2, lblDescCard2),
                Tuple.Create(pnlFeatureCard3, lblTitleCard3, lblDescCard3)
            })
            {
                tuple.Item2.Width = Math.Max(160, cardWidth - 48);
                tuple.Item3.Width = Math.Max(160, cardWidth - 48);
            }

            pnlMainContainer.AutoScrollMinSize = new Size(0, 810);
        }

        private void OpenLogin(object sender, EventArgs e)
        {
            OpenLogin(null);
        }

        private void OpenLogin(string initialUsername)
        {
            var login = new LoginForm(new AuthenticationService(), initialUsername);
            login.SignInSucceeded += user =>
            {
                OpenWorkspace(user);
            };
            login.RegisterRequested += () =>
            {
                OpenRegister(this, EventArgs.Empty);
            };
            login.BackToHomeRequested += () =>
            {
                ShowEmbeddedForm(null, null);
            };
            ShowEmbeddedForm(login, "Đăng nhập - ViewParty Portal");
        }

        private void OpenWorkspace(Models.AuthenticatedUser user)
        {
            var workspace = new WorkspaceForm(user);
            workspace.LogoutRequested += () =>
            {
                OpenLogin(user.Username);
            };
            ShowEmbeddedForm(workspace, "ViewParty Portal - " + user.DisplayRole);
        }

        private void OpenCreateOrRegister(object sender, EventArgs e)
        {
            OpenLogin(null);
        }

        private void OpenRegister(object sender, EventArgs e)
        {
            var register = new RegisterForm();
            register.RegisterSucceeded += username =>
            {
                OpenLogin(username);
            };
            register.BackToLoginRequested += () =>
            {
                OpenLogin(null);
            };
            ShowEmbeddedForm(register, "Đăng ký Người tổ chức - ViewParty Portal");
        }

        private Panel EnsureSubPageHost()
        {
            if (pnlSubPageHost == null || pnlSubPageHost.IsDisposed)
            {
                pnlSubPageHost = new Panel
                {
                    Location = new Point(0, 0),
                    Width = pnlMainContainer.ClientSize.Width,
                    Height = 760,
                    BackColor = Color.FromArgb(245, 248, 252)
                };
                pnlMainContainer.Controls.Add(pnlSubPageHost);
            }
            pnlHeroBanner.Visible = false;
            pnlFeatureCard1.Visible = false;
            pnlFeatureCard2.Visible = false;
            pnlFeatureCard3.Visible = false;
            pnlSubPageHost.Controls.Clear();
            pnlSubPageHost.Visible = true;
            pnlSubPageHost.BringToFront();
            return pnlSubPageHost;
        }

        private void LayoutSubPageContent(int actualWidth, int startX, int contentWidth)
        {
            if (pnlSubPageHost == null || !pnlSubPageHost.Visible) return;
            int y = 20;
            foreach (Control child in pnlSubPageHost.Controls)
            {
                child.Location = new Point(startX, y);
                child.Width = contentWidth;
                if (child is TableLayoutPanel grid && grid.Tag as string == "TwoColumnCards")
                {
                    if (contentWidth < 820)
                    {
                        if (grid.ColumnCount != 1)
                        {
                            grid.SuspendLayout();
                            grid.ColumnCount = 1;
                            grid.RowCount = 4;
                            grid.ColumnStyles.Clear();
                            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                            grid.RowStyles.Clear();
                            for (int r = 0; r < 4; r++) grid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                            int idx = 0;
                            foreach (Control c in grid.Controls)
                            {
                                grid.SetCellPosition(c, new TableLayoutPanelCellPosition(0, idx++));
                                c.Dock = DockStyle.Fill;
                            }
                            grid.ResumeLayout(true);
                        }
                        grid.Height = 1120;
                    }
                    else
                    {
                        if (grid.ColumnCount != 2)
                        {
                            grid.SuspendLayout();
                            grid.ColumnCount = 2;
                            grid.RowCount = 2;
                            grid.ColumnStyles.Clear();
                            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                            grid.RowStyles.Clear();
                            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                            int idx = 0;
                            foreach (Control c in grid.Controls)
                            {
                                grid.SetCellPosition(c, new TableLayoutPanelCellPosition(idx % 2, idx / 2));
                                c.Dock = DockStyle.Fill;
                                idx++;
                            }
                            grid.ResumeLayout(true);
                        }
                        grid.Height = 620;
                    }
                    foreach (Control card in grid.Controls)
                    {
                        card.Dock = DockStyle.Fill;
                    }
                }
                y = child.Bottom + 20;
            }
            pnlSubPageHost.Height = Math.Max(720, y + 24);
        }

        private Panel CreateSubBanner(
            string badgeText,
            string title,
            string subtitle,
            string primaryButtonText,
            EventHandler primaryAction)
        {
            var banner = new Panel
            {
                Height = 148,
                BackColor = Color.FromArgb(233, 242, 244),
                Padding = new Padding(28, 20, 28, 20)
            };
            banner.Paint += (s, e) => DrawCardBorder(e.Graphics, banner.ClientRectangle, BorderColor);

            var badge = new Label
            {
                Text = badgeText,
                Location = new Point(28, 18),
                Size = new Size(260, 24),
                BackColor = Color.White,
                ForeColor = DarkTeal,
                Font = new Font("Arial", 8.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                UseMnemonic = false
            };
            ApplyRoundedRegion(badge, 12);

            var btnBackHome = new Button
            {
                Text = "← Trang chủ",
                Size = new Size(128, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = NavyInk,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackHome.FlatAppearance.BorderColor = BorderColor;
            btnBackHome.Click += (s, e) => ShowHomeLanding();

            var btnAction = new Button
            {
                Text = primaryButtonText,
                Size = new Size(188, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = TealColor,
                ForeColor = Color.White,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAction.FlatAppearance.BorderSize = 0;
            btnAction.Click += primaryAction;

            ApplyRoundedRegion(btnBackHome, 8);
            ApplyRoundedRegion(btnAction, 8);

            var lblTitle = new Label
            {
                UseMnemonic = false,
                Text = title,
                Location = new Point(28, 52),
                AutoSize = true,
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = NavyInk
            };
            var lblSub = new Label
            {
                UseMnemonic = false,
                Text = subtitle,
                Location = new Point(28, 94),
                AutoSize = true,
                Font = new Font("Arial", 10F),
                ForeColor = MutedInk
            };

            Action repositionBanner = () =>
            {
                btnAction.Location = new Point(banner.ClientSize.Width - btnAction.Width - 24, 20);
                btnBackHome.Location = new Point(btnAction.Left - btnBackHome.Width - 10, 20);
                int textMax = Math.Max(240, btnBackHome.Left - 40);
                lblTitle.MaximumSize = new Size(textMax, 0);
                lblSub.MaximumSize = new Size(textMax, 0);
            };

            banner.Resize += (s, e) => repositionBanner();
            banner.Controls.AddRange(new Control[] { badge, lblTitle, lblSub, btnBackHome, btnAction });
            repositionBanner();
            return banner;
        }

        private Panel CreateInfoCard(string stepBadge, string title, string metaLine, string description)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Margin = new Padding(8),
                Padding = new Padding(24, 18, 24, 18)
            };
            card.Paint += (s, e) => DrawCardBorder(e.Graphics, card.ClientRectangle, BorderColor);

            var badge = new Label
            {
                UseMnemonic = false,
                Text = stepBadge,
                Location = new Point(22, 18),
                Size = new Size(92, 26),
                BackColor = MintBg,
                ForeColor = DarkTeal,
                Font = new Font("Arial", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ApplyRoundedRegion(badge, 12);

            var lblMeta = new Label
            {
                UseMnemonic = false,
                Text = metaLine,
                Location = new Point(122, 20),
                Size = new Size(380, 24),
                Font = new Font("Arial", 8.5F, FontStyle.Bold),
                ForeColor = MutedInk,
                AutoEllipsis = true
            };

            var lblTitle = new Label
            {
                UseMnemonic = false,
                Text = title,
                Location = new Point(22, 50),
                Size = new Size(460, 52),
                Font = new Font("Arial", 11.5F, FontStyle.Bold),
                ForeColor = NavyInk
            };

            var lblDesc = new Label
            {
                UseMnemonic = false,
                Text = description,
                Location = new Point(22, 106),
                Size = new Size(460, 140),
                Font = new Font("Arial", 9.5F),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            card.Resize += (s, e) =>
            {
                int w = Math.Max(200, card.ClientSize.Width - 44);
                lblMeta.Width = Math.Max(120, card.ClientSize.Width - badge.Right - 20);
                lblTitle.Width = w;
                lblDesc.Top = lblTitle.Bottom + 6;
                lblDesc.Width = w;
                lblDesc.Height = Math.Max(70, card.ClientSize.Height - lblDesc.Top - 12);
            };
            card.Controls.AddRange(new Control[] { badge, lblMeta, lblTitle, lblDesc });
            return card;
        }

        private void ScrollToProcess(object sender, EventArgs e)
        {
            activeTopTab = "PROCESS";
            UpdateTopNavHighlight();
            var host = EnsureSubPageHost();

            var banner = CreateSubBanner(
                "QUY TRÌNH CẤP PHÉP 04 BƯỚC",
                "Quy trình thẩm định và cấp phép sự kiện View Party",
                "Toàn bộ tiến trình từ khi đăng ký hồ sơ trực tuyến đến khi nhận Giấy phép chính thức.",
                "Tạo hồ sơ sự kiện ngay",
                OpenCreateOrRegister);

            var grid = new TableLayoutPanel
            {
                Tag = "TwoColumnCards",
                Height = 620,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.FromArgb(245, 248, 252)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            grid.Controls.Add(CreateInfoCard(
                "BƯỚC 01",
                "Đăng ký tài khoản & Lập hồ sơ trực tuyến",
                "Thực hiện: Người tổ chức · Tối thiểu trước 30 ngày",
                "• Khai báo thông tin sự kiện, quy mô khán giả dự kiến (từ 1.000 đến 10.000 người) và chọn địa điểm đủ sức chứa, không trùng lịch.\r\n" +
                "• Kê khai phương án vé / mã QR kiểm soát, đính kèm văn bản chứng minh bản quyền trình chiếu và phương án đảm bảo an ninh, y tế, PCCC."), 0, 0);

            grid.Controls.Add(CreateInfoCard(
                "BƯỚC 02",
                "Tiếp nhận & Kiểm tra tính hợp lệ của hồ sơ",
                "Thực hiện: Cán bộ xử lý · Thời hạn: 01 – 02 ngày làm việc",
                "• Cán bộ một cửa tiếp nhận hồ sơ, cấp Phiếu tiếp nhận và hẹn trả kết quả (VB09).\r\n" +
                "• Kiểm tra đầy đủ thành phần hồ sơ, hiệu lực bản quyền và điều kiện địa điểm. Trường hợp thiếu sót sẽ gửi Thông báo yêu cầu bổ sung (VB02)."), 1, 0);

            grid.Controls.Add(CreateInfoCard(
                "BƯỚC 03",
                "Thẩm định chuyên môn & Phối hợp liên ngành",
                "Thực hiện: Cán bộ thẩm định, Đơn vị An ninh & Đơn vị PCCC",
                "• Đánh giá hồ sơ theo bộ tiêu chí thẩm định chuyên môn và lập Phiếu thẩm định (VB10).\r\n" +
                "• Gửi Công văn lấy ý kiến liên ngành (VB03) đến Đơn vị An ninh trật tự và Đơn vị PCCC; tiếp nhận Văn bản phản hồi chuyên môn (VB04)."), 0, 1);

            grid.Controls.Add(CreateInfoCard(
                "BƯỚC 04",
                "Phê duyệt quyết định & Cấp giấy phép chính thức",
                "Thực hiện: Lãnh đạo phê duyệt · Thời hạn: 01 – 02 ngày làm việc",
                "• Xem xét báo cáo tổng hợp thẩm định và Tờ trình đề xuất ngoại lệ (VB05 - nếu có).\r\n" +
                "• Ban hành Văn bản chấp thuận (VB06) kèm Giấy phép tổ chức sự kiện chính thức (VB08), hoặc Thông báo không chấp thuận (VB07) nêu rõ lý do."), 1, 1);

            host.Controls.Add(banner);
            host.Controls.Add(grid);
            LayoutResponsive();
        }

        private void OpenPublicVenueSearch(object sender, EventArgs e)
        {
            activeTopTab = "SEARCH";
            UpdateTopNavHighlight();
            var host = EnsureSubPageHost();

            var banner = CreateSubBanner(
                "TRA CỨU ĐỊA ĐIỂM & LỊCH TRỐNG",
                "Tra cứu địa điểm tổ chức sự kiện và kiểm tra lịch trống",
                "Lọc nhanh địa điểm theo khu vực, sức chứa khán giả và kiểm tra xung đột lịch trình.",
                "Đăng nhập để gán địa điểm",
                OpenLogin);

            var filterCard = new Panel
            {
                Height = 152,
                BackColor = Color.White,
                Padding = new Padding(24, 16, 24, 16)
            };
            filterCard.Paint += (s, ev) => DrawCardBorder(ev.Graphics, filterCard.ClientRectangle, BorderColor);

            var diaDiemService = new DiaDiemService();
            System.Collections.Generic.IList<Models.DiaDiem> allVenues;
            try { allVenues = diaDiemService.GetAllActive(); }
            catch { allVenues = new System.Collections.Generic.List<Models.DiaDiem>(); }

            var lblVenuePick = new Label { Text = "Chọn địa điểm / Khu vực", Location = new Point(24, 16), AutoSize = true, Font = new Font("Arial", 9F, FontStyle.Bold), ForeColor = MutedInk, UseMnemonic = false };
            var cmbVenue = new ComboBox
            {
                Location = new Point(24, 40),
                Size = new Size(340, 32),
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                Font = new Font("Arial", 10F)
            };
            cmbVenue.Items.Add("-- Tất cả địa điểm đang hoạt động --");
            foreach (var v in allVenues)
            {
                cmbVenue.Items.Add(v.ten_dia_diem + " — " + v.dia_chi + " (" + v.suc_chua.ToString("N0") + " chỗ)");
            }
            cmbVenue.SelectedIndex = 0;

            var lblGuests = new Label { Text = "Quy mô khách dự kiến", Location = new Point(384, 16), AutoSize = true, Font = new Font("Arial", 9F, FontStyle.Bold), ForeColor = MutedInk, UseMnemonic = false };
            var numGuests = new NumericUpDown
            {
                Location = new Point(384, 40),
                Size = new Size(150, 32),
                Minimum = 100,
                Maximum = 10000,
                Increment = 100,
                ThousandsSeparator = true,
                Value = 1500,
                Font = new Font("Arial", 10F)
            };

            var lblStart = new Label { Text = "Bắt đầu dự kiến", Location = new Point(554, 16), AutoSize = true, Font = new Font("Arial", 9F, FontStyle.Bold), ForeColor = MutedInk, UseMnemonic = false };
            var dtpStart = new DateTimePicker
            {
                Location = new Point(554, 40),
                Size = new Size(185, 32),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                ShowUpDown = true,
                Value = DateTime.Now.Date.AddDays(45).AddHours(18),
                Font = new Font("Arial", 10F)
            };

            var lblEnd = new Label { Text = "Kết thúc dự kiến", Location = new Point(755, 16), AutoSize = true, Font = new Font("Arial", 9F, FontStyle.Bold), ForeColor = MutedInk, UseMnemonic = false };
            var dtpEnd = new DateTimePicker
            {
                Location = new Point(755, 40),
                Size = new Size(185, 32),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                ShowUpDown = true,
                Value = DateTime.Now.Date.AddDays(45).AddHours(22),
                Font = new Font("Arial", 10F)
            };

            var btnRunSearch = new Button
            {
                Text = "Kiểm tra khả dụng",
                Location = new Point(24, 90),
                Size = new Size(176, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = TealColor,
                ForeColor = Color.White,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRunSearch.FlatAppearance.BorderSize = 0;

            var btnResetFilter = new Button
            {
                Text = "Đặt lại bộ lọc",
                Location = new Point(212, 90),
                Size = new Size(140, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = SoftBlueBg,
                ForeColor = NavyInk,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnResetFilter.FlatAppearance.BorderColor = BorderColor;

            var lblSummaryCount = new Label
            {
                Text = "",
                Location = new Point(368, 99),
                AutoSize = true,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = DarkTeal,
                UseMnemonic = false
            };

            Action layoutFilterCard = () =>
            {
                int totalW = Math.Max(500, filterCard.ClientSize.Width - 48);
                int gap = 12;
                int col1 = Math.Max(180, (int)(totalW * 0.36));
                int col2 = Math.Max(110, (int)(totalW * 0.18));
                int col3 = Math.Max(130, (int)(totalW * 0.23));
                int col4 = Math.Max(130, totalW - col1 - col2 - col3 - gap * 3);

                lblVenuePick.Location = new Point(24, 16);
                cmbVenue.Location = new Point(24, 40);
                cmbVenue.Width = col1;

                lblGuests.Location = new Point(cmbVenue.Right + gap, 16);
                numGuests.Location = new Point(cmbVenue.Right + gap, 40);
                numGuests.Width = col2;

                lblStart.Location = new Point(numGuests.Right + gap, 16);
                dtpStart.Location = new Point(numGuests.Right + gap, 40);
                dtpStart.Width = col3;

                lblEnd.Location = new Point(dtpStart.Right + gap, 16);
                dtpEnd.Location = new Point(dtpStart.Right + gap, 40);
                dtpEnd.Width = col4;

                btnRunSearch.Location = new Point(24, 90);
                btnResetFilter.Location = new Point(btnRunSearch.Right + gap, 90);
                lblSummaryCount.Location = new Point(btnResetFilter.Right + 16, 99);
            };

            filterCard.Resize += (s, ev) => layoutFilterCard();

            filterCard.Controls.AddRange(new Control[]
            {
                lblVenuePick, cmbVenue, lblGuests, numGuests,
                lblStart, dtpStart, lblEnd, dtpEnd,
                btnRunSearch, btnResetFilter, lblSummaryCount
            });
            layoutFilterCard();

            var resultCard = new Panel
            {
                Height = 380,
                BackColor = Color.White,
                Padding = new Padding(24, 18, 24, 20)
            };
            resultCard.Paint += (s, ev) => DrawCardBorder(ev.Graphics, resultCard.ClientRectangle, BorderColor);

            var lblResultTitle = new Label
            {
                Text = "Danh sách địa điểm phù hợp điều kiện tra cứu",
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Arial", 12.5F, FontStyle.Bold),
                ForeColor = NavyInk
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = BorderColor,
                ColumnHeadersHeight = 40,
                EnableHeadersVisualStyles = false
            };
            grid.RowTemplate.Height = 38;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(239, 244, 249);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = MutedInk;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Arial", 9.5F);
            grid.DefaultCellStyle.ForeColor = NavyInk;
            grid.DefaultCellStyle.SelectionBackColor = MintBg;
            grid.DefaultCellStyle.SelectionForeColor = NavyInk;
            grid.Columns.Add("Code", "Mã ĐĐ");
            grid.Columns.Add("Name", "Tên địa điểm tổ chức");
            grid.Columns.Add("Address", "Khu vực / Địa chỉ chi tiết");
            grid.Columns.Add("Capacity", "Sức chứa tối đa");
            grid.Columns.Add("Fit", "Đáp ứng quy mô");
            grid.Columns.Add("Schedule", "Trạng thái lịch");
            grid.Columns[0].FillWeight = 10;
            grid.Columns[1].FillWeight = 24;
            grid.Columns[2].FillWeight = 28;
            grid.Columns[3].FillWeight = 13;
            grid.Columns[4].FillWeight = 13;
            grid.Columns[5].FillWeight = 12;

            resultCard.Controls.Add(grid);
            resultCard.Controls.Add(lblResultTitle);

            Action executeSearch = () =>
            {
                try
                {
                    string kw = string.Empty;
                    if (cmbVenue.SelectedIndex > 0 && cmbVenue.SelectedIndex - 1 < allVenues.Count)
                        kw = allVenues[cmbVenue.SelectedIndex - 1].ten_dia_diem;
                    else if (!string.IsNullOrWhiteSpace(cmbVenue.Text) && !cmbVenue.Text.StartsWith("--"))
                        kw = cmbVenue.Text.Trim();

                    int guests = Decimal.ToInt32(numGuests.Value);
                    var list = diaDiemService.Search(kw, new DateTimeOffset(dtpStart.Value), new DateTimeOffset(dtpEnd.Value), guests);
                    grid.Rows.Clear();
                    foreach (var v in list)
                    {
                        grid.Rows.Add(
                            "DD-" + v.ma_dia_diem.ToString("00"),
                            v.ten_dia_diem,
                            v.dia_chi,
                            v.suc_chua.ToString("N0") + " người",
                            v.suc_chua >= guests ? "Đủ sức chứa" : "Vượt sức chứa",
                            v.co_canh_bao_trung_lich ? "Có lịch trùng" : "Trống lịch");
                    }
                    lblSummaryCount.Text = "Tìm thấy " + list.Count + " địa điểm phù hợp.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Thông báo tra cứu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnRunSearch.Click += (s, ev) => executeSearch();
            cmbVenue.SelectedIndexChanged += (s, ev) => executeSearch();
            btnResetFilter.Click += (s, ev) =>
            {
                cmbVenue.SelectedIndex = 0;
                numGuests.Value = 1500;
                executeSearch();
            };

            host.Controls.Add(banner);
            host.Controls.Add(filterCard);
            host.Controls.Add(resultCard);
            executeSearch();
            LayoutResponsive();
        }

        private void ShowGuide(object sender, EventArgs e)
        {
            activeTopTab = "GUIDE";
            UpdateTopNavHighlight();
            var host = EnsureSubPageHost();

            var banner = CreateSubBanner(
                "HƯỚNG DẪN CHUẨN BỊ HỒ SƠ",
                "Hướng dẫn lập hồ sơ và điều kiện cấp phép sự kiện View Party",
                "Các tiêu chuẩn về quy mô, thời hạn nộp hồ sơ, bản quyền trình chiếu và phương án đảm bảo an toàn.",
                "Đăng ký tài khoản tổ chức",
                OpenRegister);

            var grid = new TableLayoutPanel
            {
                Tag = "TwoColumnCards",
                Height = 620,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.FromArgb(245, 248, 252)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            grid.Controls.Add(CreateInfoCard(
                "MỤC 01",
                "Điều kiện về Quy mô & Thời hạn nộp hồ sơ",
                "Quy định bắt buộc khi đăng ký sự kiện",
                "• Quy mô khán giả tham dự: Từ 1.000 đến 10.000 người.\r\n" +
                "• Thời gian nộp hồ sơ: Phải gửi hồ sơ trực tuyến trước ngày bắt đầu sự kiện ít nhất 30 ngày.\r\n" +
                "• Địa điểm tổ chức: Phải có sức chứa tối đa lớn hơn hoặc bằng quy mô khách dự kiến và không bị trùng lịch."), 0, 0);

            grid.Controls.Add(CreateInfoCard(
                "MỤC 02",
                "Hồ sơ Bản quyền trình chiếu công cộng",
                "Minh chứng pháp lý về nội dung tiếp sóng",
                "• Có văn bản xác nhận quyền trình chiếu từ đơn vị nắm giữ bản quyền hợp pháp.\r\n" +
                "• Thời hạn hiệu lực của bản quyền phải bao phủ toàn bộ thời gian diễn ra sự kiện.\r\n" +
                "• Phạm vi địa điểm được cấp quyền phải khớp với địa điểm tổ chức hoặc có phạm vi Toàn quốc / Thành phố."), 1, 0);

            grid.Controls.Add(CreateInfoCard(
                "MỤC 03",
                "Phương án Phát hành vé & Kiểm soát khán giả",
                "Đảm bảo trật tự và kiểm soát lưu lượng người tham dự",
                "• Khai báo rõ hình thức tổ chức (Phi thương mại hoặc Thương mại) và thông tin đơn vị tài trợ.\r\n" +
                "• Tổng số lượng vé phát hành (Miễn phí, Thư mời hoặc Bán vé) không được vượt quá quy mô khách đăng ký.\r\n" +
                "• Có giải pháp kiểm soát vé / quét mã QR định danh tại các cổng ra vào."), 0, 1);

            grid.Controls.Add(CreateInfoCard(
                "MỤC 04",
                "Phương án An ninh trật tự, Y tế & PCCC",
                "Phối hợp thẩm định cùng Đơn vị An ninh và PCCC",
                "• Phương án y tế: Bố trí tổ y tế trực cấp cứu tại chỗ và phương tiện chuyển viện khi cần thiết.\r\n" +
                "• Phương án an ninh: Bố trí lực lượng bảo vệ phân luồng giao thông, kiểm soát khu vực khán đài.\r\n" +
                "• Phương án PCCC & thoát hiểm: Đảm bảo lối thoát hiểm thông suốt, trang bị bình chữa cháy và kịch bản xử lý sự cố."), 1, 1);

            host.Controls.Add(banner);
            host.Controls.Add(grid);
            LayoutResponsive();
        }
    }
}

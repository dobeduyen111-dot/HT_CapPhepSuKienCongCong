using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Services;

namespace Viewparty.Forms
{
    /// <summary>Authenticated navigation shell matching SDBC.drawio 1440x920 portal layout.</summary>
    internal sealed class WorkspaceForm : Form
    {
        private static readonly Color Navy = Color.FromArgb(20, 44, 65);          // #142C41
        private static readonly Color ActiveNav = Color.FromArgb(36, 83, 106);    // #24536A
        private static readonly Color RoleBoxBg = Color.FromArgb(35, 65, 88);     // #234158
        private static readonly Color Teal = Color.FromArgb(19, 122, 120);        // #137A78
        private static readonly Color DarkTeal = Color.FromArgb(12, 97, 95);      // #0C615F
        private static readonly Color MintBg = Color.FromArgb(233, 247, 243);     // #E9F7F3
        private static readonly Color SoftBlueBg = Color.FromArgb(236, 244, 250); // #ECF4FA
        private static readonly Color Canvas = Color.FromArgb(245, 248, 252);     // #F5F8FC
        private static readonly Color Muted = Color.FromArgb(107, 124, 141);      // #6B7C8D
        private static readonly Color BorderColor = Color.FromArgb(219, 228, 237);// #DBE4ED
        private static readonly string[] NavIcons = { "◈", "▣", "⊞", "⌕", "☷", "▤", "⚙" };

        private readonly AuthenticatedUser user;
        private readonly AuthorizationService authorization = new AuthorizationService();
        private readonly Panel navigation;
        private readonly Panel header;
        private readonly Panel content;
        private readonly Label lblBreadcrumb;
        private readonly Button btnToggleSidebar;
        private readonly Panel pnlRoleCard;
        private Label lblAccountNameCard;
        private Label lblAccountMetaCard;
        private Label lblHeaderUserName;
        private readonly List<Panel> navItems = new List<Panel>();
        private readonly RoleWorkspaceControl workspaceControl;
        private bool userToggledSidebar = false;
        private Panel pnlAvatarRef;
        private Button logoutRef;

        public event Action LogoutRequested;

        public WorkspaceForm(AuthenticatedUser authenticatedUser)
        {
            if (!authorization.IsAuthenticatedRole(authenticatedUser))
                throw new UnauthorizedAccessException("Phiên đăng nhập không hợp lệ hoặc không có vai trò được cấp quyền.");

            user = authenticatedUser;
            DoubleBuffered = true;
            Text = "ViewParty Portal - " + user.DisplayRole;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1440, 920);
            MinimumSize = new Size(960, 640);
            Font = new Font("Arial", 9.5F);
            BackColor = Canvas;

            // Left Sidebar: (0, 0, 235, 920) #142C41
            navigation = new Panel
            {
                Dock = DockStyle.Left,
                Width = 235,
                BackColor = Navy
            };

            // Top Header Bar: (235, 0, 1205, 78) #FFFFFF
            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 78,
                BackColor = Color.White
            };
            header.Paint += (s, e) =>
            {
                using (var pen = new Pen(BorderColor, 1))
                    e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            // Sidebar toggle button (hamburger ☰)
            btnToggleSidebar = new Button
            {
                Text = "☰",
                Size = new Size(38, 38),
                Location = new Point(14, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = SoftBlueBg,
                ForeColor = Navy,
                Font = new Font("Arial", 12.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnToggleSidebar.FlatAppearance.BorderColor = BorderColor;
            ApplyRoundedRegion(btnToggleSidebar, 8);
            var tt = new ToolTip();
            tt.SetToolTip(btnToggleSidebar, "Ẩn / Hiện thanh điều hướng bên trái");
            btnToggleSidebar.Click += (s, e) =>
            {
                userToggledSidebar = true;
                ToggleSidebar();
            };

            lblBreadcrumb = new Label
            {
                Text = "VIEWPARTY / " + user.DisplayRole,
                Location = new Point(btnToggleSidebar.Right + 14, 24),
                Size = new Size(420, 30),
                AutoEllipsis = true,
                ForeColor = Muted,
                Font = new Font("Arial", 10.5F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var pnlAvatar = new Panel
            {
                Size = new Size(42, 42),
                BackColor = MintBg,
                Cursor = Cursors.Hand
            };
            var lblAvatar = new Label
            {
                Text = "VP",
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10.5F, FontStyle.Bold),
                ForeColor = DarkTeal,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            pnlAvatar.Controls.Add(lblAvatar);
            ApplyRoundedRegion(pnlAvatar, 21);

            EventHandler openAccountHandler = (s, e) => OpenAccountPage();
            pnlAvatar.Click += openAccountHandler;
            lblAvatar.Click += openAccountHandler;

            lblHeaderUserName = new Label
            {
                UseMnemonic = false,
                Text = user.FullName + " (@" + user.Username + ")",
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(29, 51, 72),
                TextAlign = ContentAlignment.MiddleRight,
                AutoEllipsis = true,
                Cursor = Cursors.Hand
            };
            lblHeaderUserName.Click += openAccountHandler;

            var logout = new Button
            {
                Text = "Đăng xuất",
                Size = new Size(98, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = SoftBlueBg,
                ForeColor = Color.FromArgb(38, 57, 75),
                Font = new Font("Arial", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            logout.FlatAppearance.BorderColor = BorderColor;
            logout.Click += (s, e) =>
            {
                if (LogoutRequested != null)
                {
                    LogoutRequested();
                    return;
                }
                DialogResult = DialogResult.Abort;
                Close();
            };

            pnlAvatarRef = pnlAvatar;
            logoutRef = logout;

            header.Controls.AddRange(new Control[] { btnToggleSidebar, lblBreadcrumb, lblHeaderUserName, pnlAvatar, logout });
            header.Resize += (s, e) => LayoutHeader(pnlAvatar, logout);

            // Responsive auto-collapse/expand on window resize
            Resize += (s, e) =>
            {
                if (WindowState == FormWindowState.Minimized) return;
                if (ClientSize.Width < 960)
                {
                    if (navigation.Visible && !userToggledSidebar)
                    {
                        navigation.Visible = false;
                        btnToggleSidebar.BackColor = MintBg;
                        btnToggleSidebar.ForeColor = DarkTeal;
                        LayoutHeader(pnlAvatarRef, logoutRef);
                    }
                }
                else if (ClientSize.Width >= 1060)
                {
                    if (!navigation.Visible && !userToggledSidebar)
                    {
                        navigation.Visible = true;
                        btnToggleSidebar.BackColor = SoftBlueBg;
                        btnToggleSidebar.ForeColor = Navy;
                        LayoutHeader(pnlAvatarRef, logoutRef);
                    }
                }
            };

            // Main Content Host: (235, 78, 1205, 842) #F5F8FC
            content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Canvas,
                Padding = new Padding(0)
            };

            pnlRoleCard = new Panel
            {
                Location = new Point(14, 774),
                Size = new Size(209, 86),
                BackColor = RoleBoxBg,
                Cursor = Cursors.Hand
            };
            ApplyRoundedRegion(pnlRoleCard, 8);

            workspaceControl = RoleWorkspaceControl.CreateFor(user);
            workspaceControl.Dock = DockStyle.Fill;
            workspaceControl.ModuleChanged += moduleTitle =>
            {
                if (string.Equals(moduleTitle, "Tài khoản cá nhân", StringComparison.OrdinalIgnoreCase))
                {
                    lblBreadcrumb.Text = "VIEWPARTY / " + user.DisplayRole + " / Tài khoản cá nhân";
                    HighlightNavModule(null, true);
                }
                else
                {
                    lblBreadcrumb.Text = "VIEWPARTY / " + user.DisplayRole + (string.IsNullOrWhiteSpace(moduleTitle) ? "" : " / " + moduleTitle);
                    HighlightNavModule(moduleTitle, false);
                }
            };
            content.Controls.Add(workspaceControl);

            // Add in exact Z-order so navigation docks full-height Left (0..920)
            // and header docks Top of remaining Right area (235..1440)
            Controls.Add(content);
            Controls.Add(header);
            Controls.Add(navigation);

            BuildNavigation();
            navigation.Resize += (s, e) => LayoutNavigation();

            var modules = authorization.GetModules(user);
            if (modules.Length > 0)
            {
                HighlightNavModule(modules[0], false);
                workspaceControl.OpenModule(modules[0]);
            }

            LayoutHeader(pnlAvatar, logout);
            LayoutNavigation();
        }

        private void ToggleSidebar()
        {
            navigation.Visible = !navigation.Visible;
            btnToggleSidebar.BackColor = navigation.Visible ? SoftBlueBg : MintBg;
            btnToggleSidebar.ForeColor = navigation.Visible ? Navy : DarkTeal;
            LayoutHeader(pnlAvatarRef, logoutRef);
        }

        private void OpenAccountPage()
        {
            if (workspaceControl == null) return;
            workspaceControl.ShowAccountProfilePage(user, updatedFullName =>
            {
                if (lblHeaderUserName != null)
                    lblHeaderUserName.Text = user.FullName + " (@" + user.Username + ")";
                if (lblAccountNameCard != null)
                    lblAccountNameCard.Text = user.FullName;
                if (lblAccountMetaCard != null)
                    lblAccountMetaCard.Text = "@" + user.Username + " · " + user.DisplayRole;
            });
        }

        private void LayoutHeader(Panel pnlAvatar, Button logout)
        {
            if (header == null || pnlAvatar == null || logout == null || btnToggleSidebar == null) return;
            int rightEdge = Math.Max(320, header.ClientSize.Width - 24);
            logout.Location = new Point(rightEdge - logout.Width, 21);
            pnlAvatar.Location = new Point(logout.Left - pnlAvatar.Width - 12, 18);

            if (lblHeaderUserName != null)
            {
                int maxUserW = Math.Min(360, Math.Max(120, pnlAvatar.Left - btnToggleSidebar.Right - 220));
                lblHeaderUserName.Width = maxUserW;
                lblHeaderUserName.Location = new Point(pnlAvatar.Left - lblHeaderUserName.Width - 12, 26);
                lblHeaderUserName.Visible = header.ClientSize.Width >= 640;
            }

            lblBreadcrumb.Location = new Point(btnToggleSidebar.Right + 14, 24);
            int breadcrumbRight = (lblHeaderUserName != null && lblHeaderUserName.Visible)
                ? lblHeaderUserName.Left - 14
                : pnlAvatar.Left - 14;
            lblBreadcrumb.Width = Math.Max(120, breadcrumbRight - lblBreadcrumb.Left);

            btnToggleSidebar.BringToFront();
            if (lblHeaderUserName != null) lblHeaderUserName.BringToFront();
            pnlAvatar.BringToFront();
            logout.BringToFront();
        }

        private void LayoutNavigation()
        {
            if (navigation == null || pnlRoleCard == null) return;
            int bottomY = Math.Max(520, navigation.ClientSize.Height - 104);
            pnlRoleCard.Location = new Point(14, bottomY);
        }

        private void BuildNavigation()
        {
            navigation.Controls.Clear();
            navItems.Clear();

            // Logo badge (24, 26, 43, 43) #137A78
            var pnlLogo = new Panel
            {
                Location = new Point(24, 26),
                Size = new Size(43, 43),
                BackColor = Teal
            };
            var lblLogo = new Label
            {
                Text = "VP",
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 13.5F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlLogo.Controls.Add(lblLogo);
            ApplyRoundedRegion(pnlLogo, 10);

            var lblBrandName = new Label
            {
                Text = "VIEWPARTY",
                Location = new Point(79, 26),
                Size = new Size(147, 26),
                Font = new Font("Arial", 14.5F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblBrandSub = new Label
            {
                Text = "EVENT PERMIT PORTAL",
                Location = new Point(79, 53),
                Size = new Size(148, 18),
                Font = new Font("Arial", 7.5F),
                ForeColor = Color.FromArgb(180, 198, 213), // #B4C6D5
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblSection = new Label
            {
                Text = "KHÔNG GIAN LÀM VIỆC",
                Location = new Point(25, 99),
                Size = new Size(183, 23),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(167, 185, 202), // #A7B9CA
                TextAlign = ContentAlignment.MiddleLeft
            };

            navigation.Controls.AddRange(new Control[] { pnlLogo, lblBrandName, lblBrandSub, lblSection });

            string[] modules = authorization.GetModules(user);
            int y = 137;
            for (int i = 0; i < modules.Length; i++)
            {
                string module = modules[i];
                string icon = NavIcons[i % NavIcons.Length];

                var itemPanel = new Panel
                {
                    Location = new Point(14, y),
                    Size = new Size(209, 44),
                    BackColor = Navy,
                    Tag = module,
                    Cursor = Cursors.Hand
                };
                ApplyRoundedRegion(itemPanel, 8);

                var lblIcon = new Label
                {
                    Text = icon,
                    Location = new Point(14, 7),
                    Size = new Size(24, 28),
                    Font = new Font("Arial", 14F),
                    ForeColor = Color.FromArgb(188, 228, 224), // #BCE4E0
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };

                var lblText = new Label
                {
                    Text = module,
                    Location = new Point(44, 6),
                    Size = new Size(158, 31),
                    Font = new Font("Arial", 10F),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoEllipsis = true,
                    Cursor = Cursors.Hand
                };

                EventHandler clickHandler = (s, e) => OnModuleSelected(module);
                itemPanel.Click += clickHandler;
                lblIcon.Click += clickHandler;
                lblText.Click += clickHandler;

                itemPanel.Controls.Add(lblIcon);
                itemPanel.Controls.Add(lblText);
                navigation.Controls.Add(itemPanel);
                navItems.Add(itemPanel);

                y += 51;
            }

            // Account & Role footer card (clickable -> opens Account Profile Page)
            pnlRoleCard.Controls.Clear();
            int roleCardW = Math.Max(190, navigation.ClientSize.Width - 24);
            pnlRoleCard.Width = roleCardW;
            pnlRoleCard.Height = 88;
            var lblCardHeader = new Label
            {
                Text = "TÀI KHOẢN ĐĂNG NHẬP  ⚙",
                Location = new Point(13, 8),
                Size = new Size(roleCardW - 26, 16),
                Font = new Font("Arial", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(188, 228, 224),
                Cursor = Cursors.Hand,
                UseMnemonic = false
            };
            lblAccountNameCard = new Label
            {
                Text = user.FullName,
                Location = new Point(13, 26),
                Size = new Size(roleCardW - 26, 22),
                Font = new Font("Arial", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoEllipsis = true,
                Cursor = Cursors.Hand,
                UseMnemonic = false
            };
            lblAccountMetaCard = new Label
            {
                Text = "@" + user.Username + " · " + user.DisplayRole,
                Location = new Point(13, 49),
                Size = new Size(roleCardW - 26, 32),
                Font = new Font("Arial", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(180, 198, 213),
                AutoEllipsis = true,
                Cursor = Cursors.Hand,
                UseMnemonic = false
            };

            EventHandler accountClick = (s, e) => OpenAccountPage();
            pnlRoleCard.Click += accountClick;
            lblCardHeader.Click += accountClick;
            lblAccountNameCard.Click += accountClick;
            lblAccountMetaCard.Click += accountClick;

            pnlRoleCard.Controls.Add(lblCardHeader);
            pnlRoleCard.Controls.Add(lblAccountNameCard);
            pnlRoleCard.Controls.Add(lblAccountMetaCard);
            navigation.Controls.Add(pnlRoleCard);
        }

        private void OnModuleSelected(string module)
        {
            if (module == null || !authorization.CanOpen(user, module))
            {
                MessageBox.Show(this, "Vai trò hiện tại không được phép mở chức năng này.", "Không có quyền",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            HighlightNavModule(module, false);
            workspaceControl.OpenModule(module);
        }

        private void HighlightNavModule(string activeModule, bool accountCardActive)
        {
            foreach (Panel item in navItems)
            {
                bool isActive = !accountCardActive &&
                                string.Equals(item.Tag as string, activeModule, StringComparison.OrdinalIgnoreCase);
                item.BackColor = isActive ? ActiveNav : Navy;
                foreach (Control child in item.Controls)
                {
                    if (child is Label && child.Left > 30)
                    {
                        child.Font = new Font("Arial", 10F, isActive ? FontStyle.Bold : FontStyle.Regular);
                    }
                }
            }
            if (pnlRoleCard != null)
            {
                pnlRoleCard.BackColor = accountCardActive ? Teal : RoleBoxBg;
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
    }
}

using System;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Services;
using Viewparty.Forms.Organizer;
using Viewparty.Forms.CaseOfficer;
using Viewparty.Forms.Interagency;
using Viewparty.Forms.Approver;
using Viewparty.Forms.Admin;

namespace Viewparty.Forms.Common
{
    internal static class FormNavigation
    {
        private static readonly AuthorizationService Authorization = new AuthorizationService();

        public static void AttachRoleMenu(Form form, AuthenticatedUser user)
        {
            if (user != null) AttachChildren(form, user);
        }

        private static void AttachChildren(Control parent, AuthenticatedUser user)
        {
            foreach (Control control in parent.Controls)
            {
                Button button = control as Button;
                if (button != null && button.Tag is string)
                {
                    string module = (string)button.Tag;
                    if (Authorization.CanOpen(user, module))
                        button.Click += (s, e) => OpenModule(button.FindForm(), user, module);
                    else button.Enabled = false;
                }
                AttachChildren(control, user);
            }
        }

        public static bool OpenModule(Form owner, AuthenticatedUser user, string module)
        {
            if (!Authorization.CanOpen(user, module))
            {
                MessageBox.Show(owner, "Tài khoản hiện tại không được phép mở chức năng này.", "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string[] modules = Authorization.GetModules(user);
            int index = Array.IndexOf(modules, module);
            Form target = null;
            if (user.RoleCode == "NGUOI_TO_CHUC")
            {
                if (index == 0) target = new OrganizerDashboardForm(user);
                else if (index == 1) target = new HoSoListForm(user);
                else if (index == 2) target = new HoSoCreateForm(user);
                else if (index == 3) target = new VenueSearchForm(user);
                else if (index == 4) target = new NotificationForm(user);
                else if (index == 5) target = new WordFormsForm(user);
            }
            else if (user.RoleCode == "NGUOI_XU_LY")
            {
                if (index == 0) target = new CaseOfficerDashboardForm(user);
                else if (index == 1) target = new HoSoIntakeForm(user);
                else if (index == 2) target = new HoSoValidationForm(user);
                else if (index == 3) target = new HoSoAppraisalForm(user);
                else if (index == 4) target = new InteragencyRequestForm(user);
                else if (index == 5) target = new OpinionSummaryForm(user);
            }
            else if (user.RoleCode == "LIEN_NGANH")
            {
                if (index == 0) target = new InteragencyDashboardForm(user);
                else if (index == 1) target = new RequestListForm(user);
                else if (index >= 2 && index <= 4) target = new OpinionResponseForm(user);
            }
            else if (user.RoleCode == "NGUOI_PHE_DUYET")
            {
                if (index == 0) target = new ApproverDashboardForm(user);
                else if (index == 1 || index == 3) target = new PendingApprovalForm(user);
                else if (index == 2) target = new ExceptionDecisionForm(user);
                else if (index == 4) target = new PermitManagementForm(user);
            }
            else if (user.RoleCode == "QUAN_TRI_VIEN")
            {
                if (index == 0) target = new AdminDashboardForm(user);
                else if (index == 1) target = new AccountManagementForm(user);
                else if (index == 2) target = new EventTypeManagementForm(user);
                else if (index == 3) target = new VenueManagementForm(user);
                else if (index == 4) target = new RuleManagementForm(user);
                else if (index == 5) target = new AuditLogForm(user);
            }
            if (target == null)
            {
                MessageBox.Show(owner, "Chức năng này hiện chưa khả dụng.", "Chưa khả dụng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            ShowEmbedded(owner, target);
            return true;
        }

        public static bool OpenOrganizerAction(Form owner, AuthenticatedUser user, string action)
        {
            if (user == null || user.RoleCode != "NGUOI_TO_CHUC") return false;
            Form target = null;
            string current = owner.GetType().Name;
            if (action.Contains("TẠO HỒ SƠ") || action.Contains("Tạo hồ sơ") || action.Contains("Tiep tuc")) target = new HoSoCreateForm(user);
            if (current == "HoSoCreateForm" && (action.Contains("Tiếp tục") || action.Contains("Tiếp tục →"))) target = new ScreeningRightsForm(user);
            if (current == "ScreeningRightsForm" && (action.Contains("Tiếp tục") || action.Contains("Tiếp tục →"))) target = new SafetyDocumentsForm(user);
            if (current == "SafetyDocumentsForm" && (action.Contains("Xem lại") || action.Contains("Xem lại →"))) target = new HoSoSubmitForm(user);
            if (current == "ScreeningRightsForm" && action.Contains("Quay lại")) target = new HoSoCreateForm(user);
            if (current == "SafetyDocumentsForm" && action.Contains("Quay lại")) target = new ScreeningRightsForm(user);
            if (current == "HoSoSubmitForm" && action.Contains("Quay lại")) target = new SafetyDocumentsForm(user);
            if (action.Contains("ĐỊA ĐIỂM") || action.Contains("địa điểm")) target = new VenueSearchForm(user);
            if (action.Contains("BIỂU MẪU") || action.Contains("biểu mẫu")) target = new WordFormsForm(user);
            if (target == null) return false;
            ShowEmbedded(owner, target);
            return true;
        }

        public static bool OpenCaseOfficerAction(Form owner, AuthenticatedUser user, string action)
        {
            if (user == null || user.RoleCode != "NGUOI_XU_LY") return false;
            Form target = null;
            if (action == "btnIntake") target = new HoSoIntakeForm(user);
            else if (action == "btnValidate") target = new HoSoValidationForm(user);
            else if (action == "btnAppraise") target = new HoSoAppraisalForm(user);
            else if (action == "btnException") target = new ExceptionProposalForm(user);
            if (target == null) return false;
            ShowEmbedded(owner, target);
            return true;
        }

        public static bool OpenInteragencyAction(Form owner, AuthenticatedUser user, string action)
        {
            if (user == null || user.RoleCode != "LIEN_NGANH") return false;
            Form target = action == "btnRequestList" ? (Form)new RequestListForm(user) :
                action == "btnViewDetails" ? new OpinionResponseForm(user) : null;
            if (target == null) return false;
            ShowEmbedded(owner, target);
            return true;
        }

        public static bool OpenApproverAction(Form owner, AuthenticatedUser user, string action)
        {
            if (user == null || user.RoleCode != "NGUOI_PHE_DUYET") return false;
            Form target = null;
            if (action == "btnPending") target = new PendingApprovalForm(user);
            else if (action == "btnExceptions") target = new ExceptionDecisionForm(user);
            else if (action == "btnPermits") target = new PermitManagementForm(user);
            if (target == null) return false;
            ShowEmbedded(owner, target);
            return true;
        }

        public static bool OpenAdminAction(Form owner, AuthenticatedUser user, string action)
        {
            if (user == null || user.RoleCode != "QUAN_TRI_VIEN") return false;
            Form target = null;
            if (action == "shortcut0") target = new AccountManagementForm(user);
            else if (action == "shortcut1") target = new EventTypeManagementForm(user);
            else if (action == "shortcut2") target = new RuleManagementForm(user);
            else if (action == "shortcut3") target = new AuditLogForm(user);
            if (target == null) return false;
            ShowEmbedded(owner, target);
            return true;
        }

        private static void ShowEmbedded(Form owner, Form child)
        {
            if (owner == null) throw new ArgumentNullException("owner");
            if (child == null) throw new ArgumentNullException("child");

            Panel host = FindContentPanel(owner);
            if (host == null)
            {
                child.Dispose();
                throw new InvalidOperationException("Không tìm thấy panel nội dung để nhúng màn hình.");
            }

            foreach (Control control in host.Controls)
            {
                Form previous = control as Form;
                if (previous != null)
                {
                    host.Controls.Remove(previous);
                    previous.Close();
                    previous.Dispose();
                }
            }

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;
            host.Controls.Add(child);
            child.Show();
            child.BringToFront();
        }

        private static Panel FindContentPanel(Control root)
        {
            foreach (Control control in root.Controls)
            {
                Panel panel = control as Panel;
                if (panel != null && (panel.Name == "content" || panel.Name == "contentPanel" || panel.Dock == DockStyle.Fill))
                    return panel;

                Panel nested = FindContentPanel(control);
                if (nested != null) return nested;
            }
            return null;
        }
    }
}

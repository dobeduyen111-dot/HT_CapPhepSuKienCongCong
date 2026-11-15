using System;
using System.ComponentModel;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Forms.Common;
namespace Viewparty.Forms.Interagency
{
 public partial class InteragencyDashboardForm : Form
 {
  internal AuthenticatedUser CurrentUser { get; private set; }
  public InteragencyDashboardForm() { InitializeComponent(); WireActions(this); }
  internal InteragencyDashboardForm(AuthenticatedUser user) : this() { if(user==null || user.RoleCode!="LIEN_NGANH") throw new UnauthorizedAccessException("Account does not have interagency role."); CurrentUser=user; FormNavigation.AttachRoleMenu(this,user); }
  protected override void OnShown(EventArgs e) { base.OnShown(e); if(CurrentUser==null && LicenseManager.UsageMode!=LicenseUsageMode.Designtime) Close(); }
  private void WireActions(Control parent) { foreach(Control child in parent.Controls) { var button=child as Button; if(button!=null && button.Tag==null) button.Click+=PendingAction; WireActions(child); } }
  private void PendingAction(object sender, EventArgs e) { var button=sender as Button; if(button!=null && FormNavigation.OpenInteragencyAction(this,CurrentUser,button.Name)) return; MessageBox.Show(this,"Ch\u1ee9c n\u0103ng ch\u01b0a k\u1ebft n\u1ed1i Service/API ho\u1eb7c SQL; ch\u01b0a c\u00f3 ph\u1ea3n h\u1ed3i n\u00e0o đ\u01b0\u1ee3c l\u01b0u.","Ch\u01b0a ho\u00e0n thi\u1ec7n",MessageBoxButtons.OK,MessageBoxIcon.Information); }
 }
}

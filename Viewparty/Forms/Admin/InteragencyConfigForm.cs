using System;
using System.ComponentModel;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Forms.Common;
namespace Viewparty.Forms.Admin
{
 public partial class InteragencyConfigForm : Form
 {
  internal AuthenticatedUser CurrentUser { get; private set; }
  public InteragencyConfigForm() { InitializeComponent(); WireActions(this); }
  internal InteragencyConfigForm(AuthenticatedUser user) : this() { if(user==null || user.RoleCode!="QUAN_TRI_VIEN") throw new UnauthorizedAccessException("Account does not have administrator role."); CurrentUser=user; FormNavigation.AttachRoleMenu(this,user); }
  protected override void OnShown(EventArgs e) { base.OnShown(e); if(CurrentUser==null && LicenseManager.UsageMode!=LicenseUsageMode.Designtime) Close(); }
  private void WireActions(Control parent) { foreach(Control child in parent.Controls) { Button b=child as Button; if(b!=null && b.Tag==null) b.Click+=PendingAction; WireActions(child); } }
  private void PendingAction(object sender,EventArgs e) { Button b=sender as Button; if(b!=null && FormNavigation.OpenAdminAction(this,CurrentUser,b.Name)) return; MessageBox.Show(this,"Chức năng này chưa kết nối Service/Repository hoặc SQL Server; chưa có dữ liệu nào được thay đổi.","Chưa hoàn thiện",MessageBoxButtons.OK,MessageBoxIcon.Information); }
 }
}

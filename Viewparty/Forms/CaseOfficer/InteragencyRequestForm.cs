using System;
using System.ComponentModel;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Forms.Common;
namespace Viewparty.Forms.CaseOfficer
{
 public partial class InteragencyRequestForm : Form
 {
  internal AuthenticatedUser CurrentUser { get; private set; }
  public InteragencyRequestForm() { InitializeComponent(); WireActions(this); }
  internal InteragencyRequestForm(AuthenticatedUser user) : this() { if(user==null || user.RoleCode!="NGUOI_XU_LY") throw new UnauthorizedAccessException("Forbidden role."); CurrentUser=user; FormNavigation.AttachRoleMenu(this,user); }
  protected override void OnShown(EventArgs e) { base.OnShown(e); if(CurrentUser==null && LicenseManager.UsageMode!=LicenseUsageMode.Designtime) Close(); }
  private void WireActions(Control parent) { foreach(Control child in parent.Controls) { var button=child as Button; if(button!=null && button.Tag==null) button.Click += PendingAction; WireActions(child); } }
  private void PendingAction(object sender, EventArgs e) { var button=sender as Button; if(button!=null && FormNavigation.OpenCaseOfficerAction(this,CurrentUser,button.Name)) return; MessageBox.Show(this,"Nghiệp vụ chưa kết nối Service/API và SQL Server; chưa thực hiện thay đổi hồ sơ.","Chưa hoàn thiện",MessageBoxButtons.OK,MessageBoxIcon.Information); }
 }
}

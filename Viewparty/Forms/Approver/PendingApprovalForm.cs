using System;
using System.ComponentModel;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Forms.Common;
namespace Viewparty.Forms.Approver
{
 public partial class PendingApprovalForm : Form
 {
  internal AuthenticatedUser CurrentUser { get; private set; }
  public PendingApprovalForm() { InitializeComponent(); ApplyCaptions(); WireActions(this); }
  internal PendingApprovalForm(AuthenticatedUser user) : this() { if(user==null || user.RoleCode!="NGUOI_PHE_DUYET") throw new UnauthorizedAccessException("Account does not have approver role."); CurrentUser=user; FormNavigation.AttachRoleMenu(this,user); }
  protected override void OnShown(EventArgs e) { base.OnShown(e); if(CurrentUser==null && LicenseManager.UsageMode!=LicenseUsageMode.Designtime) Close(); }
  private void ApplyCaptions() { Text="H\u1ed3 s\u01a1 ch\u1edd duy\u1ec7t"; lblRole.Text="Ng\u01b0\u1eddi ph\u00ea duy\u1ec7t"; lblTitle.Text=Text; lblSubtitle.Text="Xem k\u1ebft qu\u1ea3 ki\u1ec3m tra, th\u1ea9m \u0111\u1ecbnh, \u00fd ki\u1ebfn li\u00ean ng\u00e0nh v\u00e0 \u0111i\u1ec1u ki\u1ec7n c\u1ea5p ph\u00e9p."; SetMenu(); dataGroup.Text="Danh s\u00e1ch h\u1ed3 s\u01a1 ch\u1edd ph\u00ea duy\u1ec7t"; string[] h={"M\u00e3 h\u1ed3 s\u01a1","S\u1ef1 ki\u1ec7n","Ng\u01b0\u1eddi t\u1ed5 ch\u1ee9c","K\u1ebft qu\u1ea3 th\u1ea9m \u0111\u1ecbnh","\u00dd ki\u1ebfn li\u00ean ng\u00e0nh","Tr\u1ea1ng th\u00e1i","H\u00e0nh \u0111\u1ed9ng"}; SetHeaders(h); decisionGroup.Text="Th\u00f4ng tin quy\u1ebft \u0111\u1ecbnh v\u00e0 \u0111i\u1ec1u ki\u1ec7n c\u1ea5p ph\u00e9p"; decisionText.Text="K\u1ebft qu\u1ea3 ki\u1ec3m tra v\u00e0 th\u1ea9m \u0111\u1ecbnh: ch\u1edd d\u1eef li\u1ec7u SQL\\r\\n\u00dd ki\u1ebfn li\u00ean ng\u00e0nh: ch\u1edd d\u1eef li\u1ec7u SQL\\r\\n\u0110i\u1ec1u ki\u1ec7n c\u1ea5p ph\u00e9p: ch\u1edd d\u1eef li\u1ec7u SQL\\r\\nL\u00fd do t\u1eeb ch\u1ed1i: Service ch\u01b0a k\u1ebft n\u1ed1i"; btnViewDetails.Text="Xem chi ti\u1ebft";btnApprove.Text="Ph\u00ea duy\u1ec7t";btnReject.Text="T\u1eeb ch\u1ed1i"; }
  private void SetMenu() { string[] t={"T\u1ed5ng quan","H\u1ed3 s\u01a1 ch\u1edd duy\u1ec7t","X\u00e9t ngo\u1ea1i l\u1ec7","Quy\u1ebft \u0111\u1ecbnh c\u1ea5p ph\u00e9p","Gi\u1ea5y ph\u00e9p \u0111\u00e3 c\u1ea5p"}; Button[] b={nav0,nav1,nav2,nav3,nav4}; for(int i=0;i<b.Length;i++){b[i].Text=t[i];b[i].Tag=t[i];} }
  private void SetHeaders(string[] h) { for(int i=0;i<h.Length;i++) dataGrid.Columns[i].HeaderText=h[i]; }
  private void WireActions(Control parent) { foreach(Control child in parent.Controls) { var button=child as Button; if(button!=null && button.Tag==null) button.Click+=PendingAction; WireActions(child); } }
  private void PendingAction(object sender, EventArgs e) { var button=sender as Button; if(button!=null && FormNavigation.OpenApproverAction(this,CurrentUser,button.Name)) return; MessageBox.Show(this,"Ch?c n?ng n?y ch?a k?t n?i Service/API ho?c SQL Server; kh?ng c? quy?t ??nh hay thay ??i tr?ng th?i n?o ???c l?u.","Ch?a ho?n thi?n",MessageBoxButtons.OK,MessageBoxIcon.Information); }
 }
}

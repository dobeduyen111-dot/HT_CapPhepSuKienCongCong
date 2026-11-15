using System;
using System.ComponentModel;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Forms.Common;
namespace Viewparty.Forms.Approver
{
 public partial class ExceptionDecisionForm : Form
 {
  internal AuthenticatedUser CurrentUser { get; private set; }
  public ExceptionDecisionForm() { InitializeComponent(); ApplyCaptions(); WireActions(this); }
  internal ExceptionDecisionForm(AuthenticatedUser user) : this() { if(user==null || user.RoleCode!="NGUOI_PHE_DUYET") throw new UnauthorizedAccessException("Account does not have approver role."); CurrentUser=user; FormNavigation.AttachRoleMenu(this,user); }
  protected override void OnShown(EventArgs e) { base.OnShown(e); if(CurrentUser==null && LicenseManager.UsageMode!=LicenseUsageMode.Designtime) Close(); }
  private void ApplyCaptions() { Text="X\u00e9t ngo\u1ea1i l\u1ec7";lblRole.Text="Ng\u01b0\u1eddi ph\u00ea duy\u1ec7t";lblTitle.Text=Text;lblSubtitle.Text="Xem \u0111\u1ec1 xu\u1ea5t, quy t\u1eafc kh\u00f4ng \u0111\u1ea1t, c\u1ea5p th\u1ea9m quy\u1ec1n, minh ch\u1ee9ng v\u00e0 bi\u1ec7n ph\u00e1p ki\u1ec3m so\u00e1t.";SetMenu();dataGroup.Text="Th\u00f4ng tin \u0111\u1ec1 xu\u1ea5t ngo\u1ea1i l\u1ec7";string[] h={"M\u00e3 h\u1ed3 s\u01a1","Quy t\u1eafc \u0111\u1ec1 xu\u1ea5t","C\u1ea5p th\u1ea9m quy\u1ec1n","Ng\u01b0\u1eddi \u0111\u1ec1 xu\u1ea5t","Tr\u1ea1ng th\u00e1i"};for(int i=0;i<h.Length;i++)dataGrid.Columns[i].HeaderText=h[i];exceptionGroup.Text="K\u1ebft qu\u1ea3 xem x\u00e9t ngo\u1ea1i l\u1ec7";proposalText.Text="Quy t\u1eafc, c\u1ea5p th\u1ea9m quy\u1ec1n, l\u00fd do \u0111\u1ec1 xu\u1ea5t, bi\u1ec7n ph\u00e1p ki\u1ec3m so\u00e1t v\u00e0 minh ch\u1ee9ng: ch\u1edd d\u1eef li\u1ec7u SQL";btnExceptionApprove.Text="Ch\u1ea5p thu\u1eadn ngo\u1ea1i l\u1ec7";btnExceptionReject.Text="T\u1eeb ch\u1ed1i ngo\u1ea1i l\u1ec7"; }
  private void SetMenu() { string[] t={"T\u1ed5ng quan","H\u1ed3 s\u01a1 ch\u1edd duy\u1ec7t","X\u00e9t ngo\u1ea1i l\u1ec7","Quy\u1ebft \u0111\u1ecbnh c\u1ea5p ph\u00e9p","Gi\u1ea5y ph\u00e9p \u0111\u00e3 c\u1ea5p"};Button[] b={nav0,nav1,nav2,nav3,nav4};for(int i=0;i<b.Length;i++){b[i].Text=t[i];b[i].Tag=t[i];} }
  private void WireActions(Control parent) { foreach(Control child in parent.Controls) { var button=child as Button; if(button!=null && button.Tag==null) button.Click+=PendingAction; WireActions(child); } }
  private void PendingAction(object sender, EventArgs e) { var button=sender as Button; if(button!=null && FormNavigation.OpenApproverAction(this,CurrentUser,button.Name)) return; MessageBox.Show(this,"Ch?c n?ng n?y ch?a k?t n?i Service/API ho?c SQL Server; kh?ng c? quy?t ??nh hay thay ??i tr?ng th?i n?o ???c l?u.","Ch?a ho?n thi?n",MessageBoxButtons.OK,MessageBoxIcon.Information); }
 }
}

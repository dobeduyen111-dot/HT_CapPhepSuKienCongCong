using System;
using System.ComponentModel;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Forms.Common;
namespace Viewparty.Forms.Approver
{
 public partial class PermitManagementForm : Form
 {
  internal AuthenticatedUser CurrentUser { get; private set; }
  public PermitManagementForm() { InitializeComponent(); ApplyCaptions(); WireActions(this); }
  internal PermitManagementForm(AuthenticatedUser user) : this() { if(user==null || user.RoleCode!="NGUOI_PHE_DUYET") throw new UnauthorizedAccessException("Account does not have approver role."); CurrentUser=user; FormNavigation.AttachRoleMenu(this,user); }
  protected override void OnShown(EventArgs e) { base.OnShown(e); if(CurrentUser==null && LicenseManager.UsageMode!=LicenseUsageMode.Designtime) Close(); }
  private void ApplyCaptions() { Text="Gi\u1ea5y ph\u00e9p \u0111\u00e3 c\u1ea5p";lblRole.Text="Ng\u01b0\u1eddi ph\u00ea duy\u1ec7t";lblTitle.Text=Text;lblSubtitle.Text="Danh s\u00e1ch gi\u1ea5y ph\u00e9p v\u00e0 th\u00f4ng tin hi\u1ec7u l\u1ef1c theo d\u1eef li\u1ec7u h\u1ed3 s\u01a1.";SetMenu();dataGroup.Text="Danh s\u00e1ch gi\u1ea5y ph\u00e9p";string[] h={"M\u00e3 h\u1ed3 s\u01a1","S\u1ef1 ki\u1ec7n","S\u1ed1 gi\u1ea5y ph\u00e9p","Ng\u00e0y c\u1ea5p","Hi\u1ec7u l\u1ef1c t\u1eeb","Hi\u1ec7u l\u1ef1c \u0111\u1ebfn","\u0110i\u1ec1u ki\u1ec7n k\u00e8m theo"};for(int i=0;i<h.Length;i++)dataGrid.Columns[i].HeaderText=h[i];permitGroup.Text="Th\u00f4ng tin gi\u1ea5y ph\u00e9p";permitNote.Text="Th\u1eddi h\u1ea1n v\u00e0 \u0111i\u1ec1u ki\u1ec7n k\u00e8m theo \u0111\u01b0\u1ee3c \u0111\u1ecdc t\u1eeb b\u1ea3n ghi gi\u1ea5y ph\u00e9p khi c\u00f3 truy v\u1ea5n SQL.";btnPermitDetails.Text="Xem chi ti\u1ebft";btnExportPermit.Text="Xu\u1ea5t gi\u1ea5y ph\u00e9p VB08"; }
  private void SetMenu() { string[] t={"T\u1ed5ng quan","H\u1ed3 s\u01a1 ch\u1edd duy\u1ec7t","X\u00e9t ngo\u1ea1i l\u1ec7","Quy\u1ebft \u0111\u1ecbnh c\u1ea5p ph\u00e9p","Gi\u1ea5y ph\u00e9p \u0111\u00e3 c\u1ea5p"};Button[] b={nav0,nav1,nav2,nav3,nav4};for(int i=0;i<b.Length;i++){b[i].Text=t[i];b[i].Tag=t[i];} }
  private void WireActions(Control parent) { foreach(Control child in parent.Controls) { var button=child as Button; if(button!=null && button.Tag==null) button.Click+=PendingAction; WireActions(child); } }
  private void PendingAction(object sender, EventArgs e) { var button=sender as Button; if(button!=null && FormNavigation.OpenApproverAction(this,CurrentUser,button.Name)) return; MessageBox.Show(this,"Ch?c n?ng n?y ch?a k?t n?i Service/API ho?c SQL Server; kh?ng c? quy?t ??nh hay thay ??i tr?ng th?i n?o ???c l?u.","Ch?a ho?n thi?n",MessageBoxButtons.OK,MessageBoxIcon.Information); }
 }
}

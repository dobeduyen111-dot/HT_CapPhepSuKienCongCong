using System;
using System.ComponentModel;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Forms.Common;
namespace Viewparty.Forms.Approver
{
 public partial class ApproverDashboardForm : Form
 {
  internal AuthenticatedUser CurrentUser { get; private set; }
  public ApproverDashboardForm() { InitializeComponent(); ApplyCaptions(); WireActions(this); }
  internal ApproverDashboardForm(AuthenticatedUser user) : this() { if(user==null || user.RoleCode!="NGUOI_PHE_DUYET") throw new UnauthorizedAccessException("Account does not have approver role."); CurrentUser=user; FormNavigation.AttachRoleMenu(this,user); }
  protected override void OnShown(EventArgs e) { base.OnShown(e); if(CurrentUser==null && LicenseManager.UsageMode!=LicenseUsageMode.Designtime) Close(); }
  private void ApplyCaptions() { Text="T\u1ed5ng quan Ng\u01b0\u1eddi ph\u00ea duy\u1ec7t";lblRole.Text="Ng\u01b0\u1eddi ph\u00ea duy\u1ec7t";lblTitle.Text=Text;lblSubtitle.Text="Theo d\u00f5i h\u1ed3 s\u01a1 ch\u1edd quy\u1ebft \u0111\u1ecbnh, ngo\u1ea1i l\u1ec7 v\u00e0 gi\u1ea5y ph\u00e9p \u0111\u00e3 c\u1ea5p.";string[] menu={"T\u1ed5ng quan","H\u1ed3 s\u01a1 ch\u1edd duy\u1ec7t","X\u00e9t ngo\u1ea1i l\u1ec7","Quy\u1ebft \u0111\u1ecbnh c\u1ea5p ph\u00e9p","Gi\u1ea5y ph\u00e9p \u0111\u00e3 c\u1ea5p"};Button[] nav={nav0,nav1,nav2,nav3,nav4};for(int i=0;i<nav.Length;i++){nav[i].Text=menu[i];nav[i].Tag=menu[i];}string[] stats={"Ch\u1edd ph\u00ea duy\u1ec7t","\u0110ang x\u00e9t ngo\u1ea1i l\u1ec7","\u0110\u00e3 ph\u00ea duy\u1ec7t","\u0110\u00e3 t\u1eeb ch\u1ed1i"};Label[] labels={statLabel0,statLabel1,statLabel2,statLabel3};Label[] values={statValue0,statValue1,statValue2,statValue3};Label[] notes={statNote0,statNote1,statNote2,statNote3};for(int i=0;i<4;i++){labels[i].Text=stats[i];values[i].Text="\u2014";notes[i].Text="Ch\u1edd d\u1eef li\u1ec7u SQL";}dataGroup.Text="H\u1ed3 s\u01a1 c\u1ea7n xem x\u00e9t";string[] headers={"M\u00e3 h\u1ed3 s\u01a1","S\u1ef1 ki\u1ec7n","Ng\u01b0\u1eddi t\u1ed5 ch\u1ee9c","K\u1ebft lu\u1eadn th\u1ea9m \u0111\u1ecbnh","Tr\u1ea1ng th\u00e1i","H\u00e0nh \u0111\u1ed9ng"};for(int i=0;i<headers.Length;i++)dataGrid.Columns[i].HeaderText=headers[i];btnPending.Text=menu[1];btnExceptions.Text=menu[2];btnPermits.Text=menu[4]; }
  private void WireActions(Control parent) { foreach(Control child in parent.Controls) { var button=child as Button; if(button!=null && button.Tag==null) button.Click+=PendingAction; WireActions(child); } }
  private void PendingAction(object sender, EventArgs e) { var button=sender as Button; if(button!=null && FormNavigation.OpenApproverAction(this,CurrentUser,button.Name)) return; MessageBox.Show(this,"Ch?c n?ng n?y ch?a k?t n?i Service/API ho?c SQL Server; kh?ng c? quy?t ??nh hay thay ??i tr?ng th?i n?o ???c l?u.","Ch?a ho?n thi?n",MessageBoxButtons.OK,MessageBoxIcon.Information); }
 }
}

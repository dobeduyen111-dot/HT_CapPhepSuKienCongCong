using System;
using System.Collections.Generic;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class AdminWorkflowService
    {
        private readonly AdminWorkflowRepository repository = new AdminWorkflowRepository();
        private static void Validate(AuthenticatedUser user) { if (user == null || user.AccountId <= 0 || user.RoleCode != "QUAN_TRI_VIEN") throw new AdminValidationException("Chức năng chỉ dành cho Quản trị viên."); }
        public IList<AdminAccountItem> Accounts(AuthenticatedUser u) { Validate(u); return repository.Accounts(u.AccountId); }
        public void SaveAccount(AuthenticatedUser u, long id, string role, string status) { Validate(u); if (id <= 0) throw new AdminValidationException("Mã tài khoản không hợp lệ."); repository.SaveAccount(u.AccountId,id,role,status); }
        public void SaveApprovalAuthority(AuthenticatedUser u, long id, short level) { Validate(u); if (id <= 0 || level < 1 || level > 5) throw new AdminValidationException("Mã tài khoản và cấp thẩm quyền (1 - 3) phải hợp lệ."); repository.SaveApprovalAuthority(u.AccountId, id, level); }
        public IList<AdminEventTypeItem> EventTypes(AuthenticatedUser u) { Validate(u); return repository.EventTypes(u.AccountId); }
        public void SaveEventType(AuthenticatedUser u,string name,string description) { Validate(u); if(string.IsNullOrWhiteSpace(name)) throw new AdminValidationException("Tên loại sự kiện không được để trống."); repository.SaveEventType(u.AccountId,name.Trim(),description); }
        public IList<AdminVenueItem> Venues(AuthenticatedUser u) { Validate(u); return repository.Venues(u.AccountId); }
        public void SaveVenue(AuthenticatedUser u, long? id, string name, string address, int capacity, bool active) { Validate(u); if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address) || capacity < 100) throw new AdminValidationException("Tên địa điểm, địa chỉ và sức chứa (≥ 100) phải hợp lệ."); repository.SaveVenue(u.AccountId, id, name.Trim(), address.Trim(), capacity, active); }
        public IList<AdminRuleItem> Rules(AuthenticatedUser u) { Validate(u); return repository.Rules(u.AccountId); }
        public void SaveRule(AuthenticatedUser u,string code,long eventType,string group,string severity,string apply,string pass,bool allow,DateTime from,short? level) { Validate(u); if(string.IsNullOrWhiteSpace(code)||string.IsNullOrWhiteSpace(group)||string.IsNullOrWhiteSpace(severity)) throw new AdminValidationException("Mã, nhóm và mức độ quy tắc không được để trống."); repository.SaveRule(u.AccountId,code.Trim(),eventType,group.Trim(),severity.Trim(),apply,pass,allow,from,level); }
        public IList<AdminInteragencyConfigItem> Configs(AuthenticatedUser u) { Validate(u); return repository.Configs(u.AccountId); }
        public void SaveConfig(AuthenticatedUser u,long eventType,long unit,bool required,int days,DateTime from) { Validate(u); if(eventType<=0||unit<=0||days<=0) throw new AdminValidationException("Loại sự kiện, đơn vị và thời hạn phải hợp lệ."); repository.SaveConfig(u.AccountId,eventType,unit,required,days,from); }
        public AdminAuditData Audit(AuthenticatedUser u) { Validate(u); return repository.Audit(u.AccountId); }
    }
    public sealed class AdminValidationException : Exception { public AdminValidationException(string message):base(message){} }
}

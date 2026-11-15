using System;
using System.Collections.Generic;

namespace Viewparty.Models
{
    public sealed class AdminAccountItem { public long Id; public string Username; public string FullName; public string Email; public string Role; public string Status; public int? ApprovalLevel; public long? UnitId; }
    public sealed class AdminEventTypeItem { public string Name; public string Description; public bool Active; }
    public sealed class AdminVenueItem { public long Id; public string Name; public string Address; public int Capacity; public bool Active; }
    public sealed class AdminRuleItem { public long Id; public string Code; public int Version; public string EventType; public string Group; public string Severity; public bool ExceptionAllowed; public DateTime EffectiveFrom; }
    public sealed class AdminInteragencyConfigItem { public long Id; public string EventType; public string Unit; public bool Required; public int Days; public DateTime EffectiveFrom; }
    public sealed class AdminAuditItem { public DateTimeOffset At; public long? AccountId; public string ObjectName; public string Key; public string Action; public string Result; }
    public sealed class AdminAuditData { public IList<AdminAuditItem> History = new List<AdminAuditItem>(); public IList<AdminAuditItem> Configuration = new List<AdminAuditItem>(); }
}

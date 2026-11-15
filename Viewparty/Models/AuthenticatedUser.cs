namespace Viewparty.Models
{
    /// <summary>Minimal authenticated session data; never contains password material.</summary>
    public sealed class AuthenticatedUser
    {
        internal AuthenticatedUser(long accountId, string username, string fullName, string roleCode,
            string displayRole, short? approvalLevel, long? unitId)
        {
            AccountId = accountId;
            Username = username;
            FullName = fullName;
            RoleCode = roleCode;
            DisplayRole = displayRole;
            ApprovalLevel = approvalLevel;
            UnitId = unitId;
        }

        public long AccountId { get; private set; }
        public string Username { get; private set; }
        public string FullName { get; private set; }
        public string RoleCode { get; private set; }
        public string DisplayRole { get; private set; }
        public short? ApprovalLevel { get; private set; }
        public long? UnitId { get; private set; }

        internal void UpdateFullName(string newFullName)
        {
            if (!string.IsNullOrWhiteSpace(newFullName))
            {
                FullName = newFullName.Trim();
            }
        }
    }
}

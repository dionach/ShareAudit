using Dionach.ShareAudit.Model;
using Dionach.ShareAudit.Modules.Services.Interop;

namespace Dionach.ShareAudit.Modules.Services
{
    public class ShareInfo1
    {
        internal ShareInfo1(SHARE_INFO_1 shi1)
        {
            NetName = shi1.shi1_netname;
            Type = (ShareTypes)shi1.shi1_type;
            Remark = shi1.shi1_remark;
        }

        public string NetName { get; }

        public string Remark { get; }

        public ShareTypes Type { get; }
    }
}

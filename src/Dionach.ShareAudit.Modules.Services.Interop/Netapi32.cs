using System;
using System.Runtime.InteropServices;

namespace Dionach.ShareAudit.Modules.Services.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHARE_INFO_1
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string shi1_netname;

        internal uint shi1_type;

        [MarshalAs(UnmanagedType.LPWStr)]
        internal string shi1_remark;
    }

    internal partial class NativeMethods
    {
        [DllImport("Netapi32.dll")]
        internal static extern int NetApiBufferFree(IntPtr Buffer);

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int NetShareEnum(
            string ServerName,
            int level,
            ref IntPtr bufPtr,
            uint prefmaxlen,
            ref int entriesread,
            ref int totalentries,
            ref int resume_handle);
    }
}

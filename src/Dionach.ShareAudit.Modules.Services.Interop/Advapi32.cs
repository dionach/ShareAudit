using System;
using System.Runtime.InteropServices;

namespace Dionach.ShareAudit.Modules.Services.Interop
{
    internal enum SID_NAME_USE
    {
        SidTypeUser = 1,
        SidTypeGroup,
        SidTypeDomain,
        SidTypeAlias,
        SidTypeWellKnownGroup,
        SidTypeDeletedAccount,
        SidTypeInvalid,
        SidTypeUnknown,
        SidTypeComputer
    }

    internal partial class NativeMethods
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true, SetLastError = true)]
        internal static extern bool ConvertStringSidToSid(string StringSid, out IntPtr ptrSid);

        [DllImport("advapi32.dll", EntryPoint = "GetLengthSid", CharSet = CharSet.Auto)]
        internal static extern int GetLengthSid(IntPtr pSID);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true, SetLastError = true)]
        internal static extern bool LookupAccountSid(
          string lpSystemName,
          [MarshalAs(UnmanagedType.LPArray)] byte[] Sid,
          System.Text.StringBuilder lpName,
          ref uint cchName,
          System.Text.StringBuilder ReferencedDomainName,
          ref uint cchReferencedDomainName,
          out SID_NAME_USE peUse);
    }
}

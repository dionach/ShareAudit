using Dionach.ShareAudit.Modules.Services.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Dionach.ShareAudit.Modules.Services
{
    public class SmbUtilitiesService : ISmbUtilitiesService
    {
        public NetUseConnection CreateNetUseConnection(string host, string username, string domain, string password)
        {
            return new NetUseConnection(host, username, domain, password);
        }

        public IEnumerable<ShareInfo1> NetShareEnum(string host)
        {
            const int ERROR_SUCCESS = 0;
            const int ERROR_MORE_DATA = 234;
            const int ERROR_NETWORK_PATH_NOT_FOUND = 53;
            const int ERROR_ACCESS_DENIED = 0x5;

            var maxPreferredLength = 0xFFFFFFFF;
            var level = 1;
            var entriesRead = 0;
            var totalEntries = 0;
            var resumeHandle = 0;
            var offset = Marshal.SizeOf(typeof(SHARE_INFO_1));
            var bufPtr = IntPtr.Zero;
            var result = ERROR_SUCCESS;

            do
            {
                result = NativeMethods.NetShareEnum(host, level, ref bufPtr, maxPreferredLength, ref entriesRead, ref totalEntries, ref resumeHandle);

                if (result == ERROR_SUCCESS || result == ERROR_MORE_DATA || result == ERROR_NETWORK_PATH_NOT_FOUND)
                {
                    for (int i = 0, lpItem = bufPtr.ToInt32(); i < entriesRead; i++, lpItem += offset)
                    {
                        var pItem = new IntPtr(lpItem);

                        yield return new ShareInfo1((SHARE_INFO_1)Marshal.PtrToStructure(pItem, typeof(SHARE_INFO_1)));
                    }
                }
                else if (result != ERROR_ACCESS_DENIED)
                {
                    throw new Win32Exception(result);
                }

                if (bufPtr != IntPtr.Zero)
                {
                    NativeMethods.NetApiBufferFree(bufPtr);
                }
            }
            while (result == ERROR_MORE_DATA);
        }
    }
}

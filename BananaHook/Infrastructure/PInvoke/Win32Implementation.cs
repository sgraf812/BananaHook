using System;
using System.Runtime.InteropServices;

namespace BananaHook.Infrastructure.PInvoke
{
    public class Win32Implementation : IMemoryProtection
    {
        #region IMemoryProtection members

        bool IMemoryProtection.VirtualProtect(IntPtr lpAddress, IntPtr dwSize, MemoryProtectionConstraints flNewProtect,
            out MemoryProtectionConstraints pflOldProtect)
        {
            return VirtualProtect(lpAddress, dwSize, flNewProtect, out pflOldProtect);
        }

        #region P/Invoke

        [DllImport("kernel32", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VirtualProtect(
            IntPtr lpAddress,
            IntPtr dwSize,
            MemoryProtectionConstraints flNewProtect,
            out MemoryProtectionConstraints pflOldProtect
            );

        #endregion

        #endregion
    }
}
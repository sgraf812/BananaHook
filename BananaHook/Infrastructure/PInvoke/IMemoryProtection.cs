using System;

namespace BananaHook.Infrastructure.PInvoke
{
    public interface IMemoryProtection
    {
        bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize,
            MemoryProtectionConstraints flNewProtect, out MemoryProtectionConstraints pflOldProtect);
    }
}
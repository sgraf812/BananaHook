using System;
using BananaHook.Infrastructure.PInvoke;

namespace BananaHook.Specs
{
    public class StubMemoryProtection : IMemoryProtection
    {
        public MemoryProtectionConstraints CurrentProtectection { get; set; }
        public bool HasChanged { get; private set; }

        #region Implementation of IMemoryProtection

        public bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, MemoryProtectionConstraints flNewProtect, out MemoryProtectionConstraints pflOldProtect)
        {
            pflOldProtect = CurrentProtectection;
            HasChanged = CurrentProtectection != flNewProtect;
            CurrentProtectection = flNewProtect;
            return true;
        }

        #endregion
    }
}
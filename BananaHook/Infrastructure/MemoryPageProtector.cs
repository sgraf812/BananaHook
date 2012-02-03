using System;
using BananaHook.Infrastructure.PInvoke;

namespace BananaHook.Infrastructure
{
    public class MemoryPageProtector
    {
        private readonly IMemoryProtection _protection;
        private readonly IntPtr _address;
        private readonly IntPtr _length;

        public MemoryPageProtector(IMemoryProtection protection, IntPtr address, IntPtr length)
        {
            _protection = protection;
            _address = address;
            _length = length;
        }

        public void ExecuteWithProtection(MemoryProtectionConstraints requestedProtection, Action action)
        {
            MemoryProtectionConstraints oldProtection = MemoryProtectionConstraints.None;

            Helper.ThrowWin32ExceptionIfFalse(() =>
                _protection.VirtualProtect(_address, _length, requestedProtection, out oldProtection));

            try
            {
                action();
            }
            finally
            {
                Helper.ThrowWin32ExceptionIfFalse(() =>
                    _protection.VirtualProtect(_address, _length, oldProtection, out oldProtection));
            }
        }
    }
}
using System;
using System.Runtime.InteropServices;

namespace BananaHook.Infrastructure.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_BASIC_INFORMATION
    {
        // ReSharper disable MemberCanBePrivate.Global
        public static readonly int Size = Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
        public readonly IntPtr BaseAddress;
        public readonly IntPtr AllocationBase;
        public readonly MemoryProtectionConstraints AllocationProtect;
        public readonly IntPtr RegionSize;
        public readonly uint State;
        public readonly MemoryProtectionConstraints Protect;
        public readonly uint Type;
        // ReSharper restore MemberCanBePrivate.Global
    }
}
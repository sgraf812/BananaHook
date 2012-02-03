using System;
using System.Runtime.InteropServices;
using BananaHook.Infrastructure;

namespace BananaHook.Specs
{
    class InProcessMemory : IMemory
    {
        #region Implementation of IMemory

        public byte[] ReadBytes(IntPtr address, int count)
        {
            var buffer = new byte[count];
            Marshal.Copy(address, buffer, 0, count);
            return buffer;
        }

        public unsafe void WriteBytes(IntPtr address, byte[] bytes)
        {
            var cur = (byte*)address;
            foreach (var b in bytes)
                *cur++ = b;
        }

        #endregion
    }
}
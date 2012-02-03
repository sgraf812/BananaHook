using System;
using System.Collections.Generic;

namespace BananaHook.Infrastructure
{
    public interface IMemory
    {
        byte[] ReadBytes(IntPtr address, int count);
        void WriteBytes(IntPtr address, byte[] bytes);
    }
}
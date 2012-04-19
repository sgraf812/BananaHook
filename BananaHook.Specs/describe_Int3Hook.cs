using System;
using BananaHook.Infrastructure;

namespace BananaHook.Specs
{
    class describe_Int3Hook : describe_IHook
    {
        protected override IHook CreateHook(IMemory memory, IntPtr targetAddress, IntPtr hookAddress)
        {
            return new Int3Hook(memory, targetAddress, hookAddress);
        }
    }
}
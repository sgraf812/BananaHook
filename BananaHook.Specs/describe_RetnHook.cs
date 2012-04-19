using System;
using BananaHook.Infrastructure;

namespace BananaHook.Specs
{
    class describe_RetnHook : describe_IHook
    {
        protected override IHook CreateHook(IMemory memory, IntPtr targetAddress, IntPtr hookAddress)
        {
            return new RetnHook(memory, targetAddress, hookAddress);
        }
    }
}
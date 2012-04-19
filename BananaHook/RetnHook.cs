using System;
using BananaHook.Asm;
using BananaHook.Infrastructure;

namespace BananaHook
{
    public class RetnHook : PatchingHook
    {
        public RetnHook(IMemory memory, IntPtr targetAddress, IntPtr hookAddress)
            : base(memory, targetAddress, GenerateDetourOpCodes(hookAddress))
        {
        }

        private static byte[] GenerateDetourOpCodes(IntPtr hookAddress)
        {
            // retn sets eip to the the top of the stack -> hook function is being called.
            var assembler = new X86Assembler();
            assembler.Push(hookAddress);
            assembler.Retn();
            return assembler.GetBytes();
        }
    }
}